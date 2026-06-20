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
                validRoads.Add(new Settlement_Road(startPos, dir.normalized, settings.HighwayLength));
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

        //original checks for if the new end positions are close to the start/end positions of the existing roads
        //
        original.UpdateEndPosition(newEndPoint);
        original.EndedByLink = foundEnd;
        highways.Add(original);

        if (!foundEnd &&!IsPositionNearEdge(newEndPoint,settings))
        {
            newStart = newEndPoint;
            workingCopy.Add(new Settlement_Road(newStart, (original.Direction + new Vector2(Random.Range(-.1f,.1f)*settings.HighwayDirOffsetScale,Random.Range(-.1f,.1f) * settings.HighwayDirOffsetScale)).normalized, settings.HighwayLength));
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
            validRoads.Add(new Settlement_Road(pos, highways[x].Perp(false).normalized, settings.AvenueLength));
            validRoads.Add(new Settlement_Road(pos, highways[x].Perp(true).normalized, settings.AvenueLength));

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


        original.UpdateEndPosition( newEndPoint);
        original.EndedByLink = foundEnd;
        avenues.Add(original);

        


        if (!foundEnd && !IsPositionNearEdge(newEndPoint, settings) && Vector2.Distance(settings.Center, original.EndPos) < settings.Size.magnitude/2f)
        {
            newStart = newEndPoint;
            workingCopy.Add(new Settlement_Road(newStart, (original.Direction).normalized, settings.AvenueLength));
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
                Vector2 pos = avenues[x].GetPositionOnRoad(.5f);
                avenues[x].AddPointToSplit(.5f);
                validRoads.Add(new Settlement_Road(pos, avenues[x].Perp(false).normalized, settings.RoadLength));
                validRoads.Add(new Settlement_Road(pos, avenues[x].Perp(true).normalized, settings.RoadLength));
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


        original.UpdateEndPosition(newEndPoint);
        original.EndedByLink = foundEnd;
        roads.Add(original);




        if (!foundEnd && !IsPositionNearEdge(newEndPoint, settings))
        {
            newStart = newEndPoint;
            if (Random.Range(0, 100) < 20)
            {
                workingCopy.Add(new Settlement_Road(newStart, (original.Perp(false)).normalized, settings.RoadLength));

            }
            else
            {
                workingCopy.Add(new Settlement_Road(newStart, (original.Direction).normalized, settings.RoadLength));

            }
        }

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

    static bool IsPointFarEnoughAway(List<Settlement_Road> toCheck,float minDist,Vector2 position)
    {
        for(int x=0;x < toCheck.Count; x++)
        {
            if (toCheck[x].IsPointCloseToPositions(position, minDist))
            {
                return false;
            }
        }

        return true;
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
    public void AddHighway(Settlement_Road road)
    {
        highways.Add(road);
    }
}
