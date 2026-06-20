using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public  static class SettlementGenerator 
{
    public static GeneratedSettlement GenerateSettlement(Settlement_Settings settings)
    {
        validRoads.Clear();
        workingCopy.Clear();
        highways.Clear();
        avenues.Clear();
        roads.Clear();
        DoneRoads = false;
        DoneHighways = false;
        DoneAvenues = false;
        GeneratedSettlement settlement=new GeneratedSettlement();
        GenerateInitialHighways(settings);
        Debug.Log("Settlement Gen: initial highways done ");
        HighwayGenerationPass(settings);
        settlement.highways = highways;
        Debug.Log("Settlement Gen: highway gen done");

        GenerateInitialAvenues(settings);
        Debug.Log("Settlement Gen: initial avenues done");

        AvenueGenerationPass(settings);
        settlement.avenues = avenues;
        Debug.Log("Settlement Gen: avenue gen done");
        GenerateInitialRoads(settings);
        RoadGenerationPass(settings);
        settlement.roads = roads;
        return settlement;
    }

    static void GenerateInitialHighways(Settlement_Settings settings)
    {
        Vector2 startPos = GetEdge(settings);
        Vector2 dir = settings.Center - startPos;

        for (int x = 0; x < settings.StartingHighwayCount; x++)
        {
            //if (IsPointFarEnoughAway(validRoads, settings.DistBetweenHighways, startPos))
            {
                validRoads.Add(new Settlement_Road(startPos, dir.normalized, settings.HighwayLength, Settlement_RoadType.Highway));
                //settlement.AddHighway();

            }
            startPos = GetEdge(settings);
            dir = settings.Center - startPos;
        }
    }

    static List<Settlement_Road> workingCopy = new List<Settlement_Road>(),
        validRoads = new List<Settlement_Road>(),highways=new List<Settlement_Road>(),
        avenues=new List<Settlement_Road>(),roads=new List<Settlement_Road>();
    static bool DoneHighways = false,DoneAvenues=false,DoneRoads=false;
    static void HighwayGenerationPass(Settlement_Settings settings)
    {
        while (!DoneHighways)
        {
            for (int x = 0; x < validRoads.Count; x++)
            {
                if (!validRoads[x].EndedByLink)
                {
                    GenerateHighway(validRoads[x],settings);
                }
            }
            validRoads.Clear();
            validRoads.AddRange(workingCopy);
            workingCopy.Clear();
            if (validRoads.Count == 0)
            {
                DoneHighways = true;
            }
        }
        validRoads.Clear();
        workingCopy.Clear();
     }

    static Vector2 intersection,newStart;
    static void GenerateHighway(Settlement_Road original,Settlement_Settings settings)
    {
        bool foundEnd = false;
        Vector2 newEndPoint = original.StartPos + (original.Direction * original.Length);

        if (CheckForIntersection(validRoads, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

        if (CheckForIntersection(highways, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

        if(CheckForMovingEndToOtherPoint(highways, ref newEndPoint, settings.DistBetweenHighways))
        {
            foundEnd = true;
        }

        //original checks for if the new end positions are close to the start/end positions of the existing roads
        //
        original.UpdateEndPosition(newEndPoint);
        original.EndedByLink = foundEnd;
        highways.Add(original);

        if (!foundEnd &&!IsPositionNearEdge(newEndPoint,settings))
        {
            newStart = newEndPoint;
            workingCopy.Add(new Settlement_Road(newStart, (original.Direction + new Vector2(Random.Range(-.1f,.1f)*settings.HighwayDirOffsetScale,Random.Range(-.1f,.1f) * settings.HighwayDirOffsetScale)).normalized, settings.HighwayLength, Settlement_RoadType.Highway));
        }

    }


    static void GenerateInitialAvenues(Settlement_Settings settings)
    {
        validRoads.Clear();
        workingCopy.Clear();
        int initialHighways = highways.Count;
        for(int x=0;x<initialHighways;x++)
        {
            Vector2 pos = highways[x].GetPositionOnRoad(.5f);
            highways[x].AddPointToSplit(.5f);
            validRoads.Add(new Settlement_Road(pos, highways[x].Perp(false).normalized, settings.AvenueLength, Settlement_RoadType.Avenue, highways[x]));
            validRoads.Add(new Settlement_Road(pos, highways[x].Perp(true).normalized, settings.AvenueLength, Settlement_RoadType.Avenue, highways[x]));

        }
    }
    static int totalPasses = 0;
    static void AvenueGenerationPass(Settlement_Settings settings)
    {
   
     

        totalPasses = 0;
        while (!DoneAvenues&&totalPasses< settings.MaxAvenuePasses)
        {
            for (int x = 0; x < validRoads.Count; x++)
            {
                if (!validRoads[x].EndedByLink)
                {
                    GenerateAvenue(validRoads[x], settings);
                }
            }
            validRoads.Clear();
            validRoads.AddRange(workingCopy);
            workingCopy.Clear();
            totalPasses++;
            if (validRoads.Count == 0)
            {
                DoneAvenues = true;
            }
        }

        validRoads.Clear();
        workingCopy.Clear();
    }

    static void GenerateAvenue(Settlement_Road original, Settlement_Settings settings)
    {
        bool foundEnd = false;
        Vector2 newEndPoint = original.EndPos;

        if (CheckForIntersection(validRoads, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

        if (CheckForIntersection(highways, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }


        if (CheckForIntersection(avenues, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

       if( CheckForMovingEndToOtherPoint(avenues, ref newEndPoint, settings.DistBetweenAvenues))
        {
            foundEnd = true;
        }

        original.UpdateEndPosition( newEndPoint);
        original.EndedByLink = foundEnd;
        avenues.Add(original);

        


        if (!foundEnd && !IsPositionNearEdge(newEndPoint, settings) && Vector2.Distance(settings.Center, original.EndPos) < settings.Size.magnitude/2f)
        {
            newStart = newEndPoint;
            workingCopy.Add(new Settlement_Road(newStart, (original.Direction).normalized, settings.AvenueLength, Settlement_RoadType.Avenue, original));
        }

    }


    static void GenerateInitialRoads(Settlement_Settings settings)
    {
        validRoads.Clear();
        workingCopy.Clear();
        int initialRoads = avenues.Count;
        for (int x = 0; x < initialRoads; x++)
        {
            if (avenues[x].Length > settings.MinAvenueLengthForRoad)
            {
                float split = Random.Range(.45f, .55f);
                Vector2 pos = avenues[x].GetPositionOnRoad(split);
                avenues[x].AddPointToSplit(split);
                validRoads.Add(new Settlement_Road(pos, avenues[x].Perp(false).normalized, settings.RoadLength, Settlement_RoadType.Road, avenues[x]));
                validRoads.Add(new Settlement_Road(pos, avenues[x].Perp(true).normalized, settings.RoadLength, Settlement_RoadType.Road, avenues[x]));
            }
        }
    }
    static void RoadGenerationPass(Settlement_Settings settings)
    {
        totalPasses = 0;
        while (!DoneRoads && totalPasses < settings.MaxRoadPasses)
        {
            for (int x = 0; x < validRoads.Count; x++)
            {
                if (!validRoads[x].EndedByLink)
                {
                    GenerateRoads(validRoads[x], settings);
                }
            }
            validRoads.Clear();
            validRoads.AddRange(workingCopy);
            workingCopy.Clear();
            totalPasses++;
            if (validRoads.Count == 0)
            {
                DoneRoads = true;
            }
        }

        validRoads.Clear();
        workingCopy.Clear();
    }

    static void GenerateRoads(Settlement_Road original, Settlement_Settings settings)
    {
        bool foundEnd = false;
        Vector2 newEndPoint = original.EndPos;

        if (CheckForIntersection(validRoads, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }
        if (CheckForIntersection(avenues, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

        if (CheckForIntersection(highways, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

        if (CheckForIntersection(roads, original.StartPos, ref newEndPoint, out intersection))
        {
            foundEnd = true;
        }

        if(CheckForMovingEndToOtherPoint(roads, ref newEndPoint, settings.DistBetweenRoads))
        {
            foundEnd = true;
        }

        original.UpdateEndPosition(newEndPoint);
        original.EndedByLink = foundEnd;
        roads.Add(original);




        if (!foundEnd && !IsPositionNearEdge(newEndPoint, settings))
        {
            newStart = newEndPoint;
            if (Random.Range(0, 100) < 20)
            {
                workingCopy.Add(new Settlement_Road(newStart, (original.Perp(false)).normalized, settings.RoadLength, Settlement_RoadType.Road, original));

            }
            else
            {
                workingCopy.Add(new Settlement_Road(newStart, (original.Direction).normalized, settings.RoadLength, Settlement_RoadType.Road, original));

            }
        }

    }


    static bool CheckForMovingEndToOtherPoint(List<Settlement_Road> toTestAgainst,  ref Vector2 end,float maxDist)
    {
      
        float dist = maxDist;
        Vector2 newPos = end;
        for(int x = 0; x < toTestAgainst.Count; x++)
        {
            if (toTestAgainst[x].IsPointCloseToStart(end, dist))
            {
                dist = Vector2.Distance(end, toTestAgainst[x].StartPos);

                newPos = toTestAgainst[x].StartPos;
                return true;
            }

            //if (toTestAgainst[x].IsPointCloseToEnd(end, maxDist))
            //{
            //    end = toTestAgainst[x].EndPos;
            //    return true;
            //}
        }
        end = newPos;
        return false;
    }

    static bool CheckForIntersection(List<Settlement_Road> toTestAgainst,Vector2 start,ref Vector2 end,out Vector2 intersection)
    {
        for (int x = 0; x < toTestAgainst.Count; x++)
        {
            if (LineUtil.IntersectRoadSegments2D(start, end, toTestAgainst[x].StartPos, toTestAgainst[x].EndPos, out intersection))
            {
                if (Vector2.Distance(start, intersection) > .1f)
                {
                    end = intersection;
                    return true;
                }
            }

        }
        intersection = Vector2.zero;
        return false;
    }

    static bool IsPositionNearEdge(Vector2 pos,Settlement_Settings settings)
    {
        if (pos.x < settings.Center.x - (settings.Size.x / 2f))
        {
            return true;
        }
        if (pos.x > settings.Center.x + (settings.Size.x / 2f))
        {
            return true;
        }

        if (pos.y < settings.Center.y - (settings.Size.y / 2f))
        {
            return true;
        }
        if (pos.y > settings.Center.y + (settings.Size.y / 2f))
        {
            return true;
        }
        return false;
    }

  
    static Vector2 GetEdge(Settlement_Settings settings)
    {
        Vector2 edge = new Vector2();
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            r = Random.Range(0, 100);
            if (r < 50)
            {
                edge.x = settings.Center.x + (settings.Size.x / 2);
                edge.y = Random.Range(settings.Center.y - (settings.Size.y / 2), settings.Center.y + (settings.Size.y / 2));
            }
            else
            {
                edge.x = settings.Center.x - (settings.Size.x / 2);
                edge.y = Random.Range(settings.Center.y - (settings.Size.y / 2), settings.Center.y + (settings.Size.y / 2));


            }
        }
        else
        {

            r = Random.Range(0, 100);
            if (r < 50)
            {
                edge.y = settings.Center.y + (settings.Size.y / 2);
                edge.x = Random.Range(settings.Center.x - (settings.Size.x / 2), settings.Center.x + (settings.Size.x / 2));
            }
            else
            {
                edge.y = settings.Center.y - (settings.Size.y / 2);
                edge.x = Random.Range(settings.Center.x - (settings.Size.x / 2), settings.Center.x + (settings.Size.x / 2));
            }
        }
      
        return edge;
    }
}

[System.Serializable]
public class GeneratedSettlement
{
    public List<Settlement_Road> highways=new List<Settlement_Road>(),avenues = new List<Settlement_Road>(), roads = new List<Settlement_Road>();
    public GeneratedSettlementArea[,] areas;
    public void GenerateSettlementAreas(Settlement_Settings settings,int areaSize)
    {
        int width = Mathf.RoundToInt( settings.Size.x / areaSize);
        int height = Mathf.RoundToInt(settings.Size.y / areaSize);
        Vector2 low = settings.Center - (settings.Size * .5f);
        Vector2 high = settings.Center + (settings.Size * .5f);

        areas = new GeneratedSettlementArea[width, height];
        for(int x=0;x<width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                areas[x, y] = new GeneratedSettlementArea(new Vector2(Mathf.Lerp(low.x, high.x, Mathf.InverseLerp(0, width, x)), Mathf.Lerp(low.y, high.y, Mathf.InverseLerp(0, height, y))));
            }
        }
        int xc = 0, yc = 0;
        for(int q = 0; q < highways.Count; q++)
        {
            xc = Mathf.FloorToInt( Mathf.Lerp(0, width-1, Mathf.InverseLerp(low.x, high.x, highways[q].RoadNode.position.x)));
            yc = Mathf.FloorToInt(Mathf.Lerp(0,height-1, Mathf.InverseLerp(low.y, high.y, highways[q].RoadNode.position.y)));
            areas[xc, yc].AddHighway(highways[q]);
        }
        xc = 0;
        yc = 0;
        for (int q = 0; q < avenues.Count; q++)
        {
            xc = Mathf.FloorToInt(Mathf.Lerp(0, width-1, Mathf.InverseLerp(low.x, high.x, avenues[q].RoadNode.position.x)));
            yc = Mathf.FloorToInt(Mathf.Lerp(0, height-1, Mathf.InverseLerp(low.y, high.y, avenues[q].RoadNode.position.y)));
            areas[xc, yc].AddAvenue(avenues[q]);
        }

        xc = 0;
        yc = 0;
        for (int q = 0; q <roads.Count; q++)
        {
            xc = Mathf.FloorToInt(Mathf.Lerp(0, width-1, Mathf.InverseLerp(low.x, high.x, roads[q].RoadNode.position.x)));
            yc = Mathf.FloorToInt(Mathf.Lerp(0, height-1, Mathf.InverseLerp(low.y, high.y, roads[q].RoadNode.position.y)));
            areas[xc, yc].AddHighway(roads[q]);
        }

    }



}

public class GeneratedSettlementArea
{
    public GeneratedSettlementArea(Vector2 bottomLeft)
    {
        Point = bottomLeft;
        DebugColour = new Color(Random.value, Random.value, Random.value);
    }
    public Color DebugColour;
    public Vector2 Point;
    public List<Settlement_Road> highways = new List<Settlement_Road>(), avenues = new List<Settlement_Road>(), roads = new List<Settlement_Road>();

    public void AddHighway(Settlement_Road highway)
    {
        highways.Add(highway);
    }
    public void AddAvenue(Settlement_Road highway)
    {
        avenues.Add(highway);
    }
    public void AddRoad(Settlement_Road highway)
    {
       roads.Add(highway);
    }
   


    public bool IsPointInArea(Vector2 point)
    {
        return false;
    }

}
