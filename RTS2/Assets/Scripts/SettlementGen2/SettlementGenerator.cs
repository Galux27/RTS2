using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public  static class SettlementGenerator 
{
    static GeneratedSettlement CurrentlyGenerating;
    public static void GenerateSettlement(GeneratedSettlement settlement, Settlement_Settings settings)
    {
        CurrentlyGenerating = settlement;
        settlement.SetRiver(RiverGenerator.GenerateRiver(settings));
        validRoads.Clear();
        workingCopy.Clear();
        highways.Clear();
        avenues.Clear();
        roads.Clear();
        DoneRoads = false;
        DoneHighways = false;
        DoneAvenues = false;
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
    }


    public static void DebugDrawSettlementRoads(GeneratedSettlement settlement,float duration)
    {
        if (settlement != null && settlement.areas != null)
        {
            for (int q = 0; q < settlement.areas.GetLength(0); q++)
            {
                for (int r = 0; r < settlement.areas.GetLength(1); r++)
                {

                    for (int x = 0; x < settlement.areas[q, r].roads.Count; x++)
                    {
                        Debug.DrawLine(settlement.areas[q, r].roads[x].StartPos, settlement.areas[q, r].roads[x].EndPos, settlement.areas[q, r].DebugColour, duration);
                    }


                    for (int x = 0; x < settlement.areas[q, r].avenues.Count; x++)
                    {
                        Debug.DrawLine(settlement.areas[q, r].avenues[x].StartPos, settlement.areas[q, r].avenues[x].EndPos, settlement.areas[q, r].DebugColour, duration);
                    }

                    for (int x = 0; x < settlement.areas[q, r].highways.Count; x++)
                    {
                        Debug.DrawLine(settlement.areas[q, r].highways[x].StartPos, settlement.areas[q, r].highways[x].EndPos, Color.cyan, duration);
                    }


                }
            }

            for (int x = 0; x < settlement.River.RiverSections.Count; x++)
            {
                Debug.DrawLine(settlement.River.RiverSections[x].StartPos, settlement.River.RiverSections[x].EndPos, Color.blue, duration);
                Debug.DrawLine(settlement.River.RiverSections[x].PosSideStart, settlement.River.RiverSections[x].PosSideEnd, Color.blue, duration);
                Debug.DrawLine(settlement.River.RiverSections[x].NegSideStart, settlement.River.RiverSections[x].NegSideEnd, Color.blue, duration);

            }
        }
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

        if (!foundEnd &&!IsPositionNearEdge(newEndPoint,settings)&&!IsRoadInInvalidChunk(original,CurrentlyGenerating))
        {
            newStart = newEndPoint;
            workingCopy.Add(new Settlement_Road(newStart, (original.Direction + new Vector2(Random.Range(-.1f,.1f)*settings.HighwayDirOffsetScale,Random.Range(-.1f,.1f) * settings.HighwayDirOffsetScale)).normalized, settings.HighwayLength, Settlement_RoadType.Highway));
        }

    }

    static bool IsRoadInInvalidChunk(Settlement_Road road,GeneratedSettlement settlement)
    {
        return !settlement.GetAreaFromPosition(road.EndPos).CanUse;
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
    static Vector2 RiverIntersect;
    static void GenerateAvenue(Settlement_Road original, Settlement_Settings settings)
    {
        bool foundEnd = false, add = true ;
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

       if(RiverGenerator.CheckForIntersection(original.StartPos,original.endPos,CurrentlyGenerating.River.RiverSections,out RiverIntersect))
        {
            foundEnd = true;
            newEndPoint = RiverIntersect;
            add = false;
        }
        if (!add)
        {
            return;
        }
        original.UpdateEndPosition( newEndPoint);
        original.EndedByLink = foundEnd;
        avenues.Add(original);

        


        if (!foundEnd && !IsPositionNearEdge(newEndPoint, settings) 
            && Vector2.Distance(settings.Center, original.EndPos) < settings.Size.magnitude/2f && !IsRoadInInvalidChunk(original, CurrentlyGenerating))
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
        bool foundEnd = false, add = true ;
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
        if (RiverGenerator.CheckForIntersection(original.StartPos, original.endPos, CurrentlyGenerating.River.RiverSections, out RiverIntersect))
        {
            foundEnd = true;
            newEndPoint = RiverIntersect;
            add = false;
        }

        if (!add)
        {
            return;
        }
        original.UpdateEndPosition(newEndPoint);
        original.EndedByLink = foundEnd;
        roads.Add(original);




        if (!foundEnd && !IsPositionNearEdge(newEndPoint, settings) && !IsRoadInInvalidChunk(original, CurrentlyGenerating))
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
    public Settlement_River River;
    public List<Settlement_Road> highways=new List<Settlement_Road>(),
        avenues = new List<Settlement_Road>(),
        roads = new List<Settlement_Road>();
    public GeneratedSettlementArea[,] areas;
    int width;
    int height;
    Vector2 low;
    Vector2 high;
    Vector2Int Batch, Chunk, Coords;

    public List<Settlement_Road> GetRoadsInWorldBatch(WorldChunkBatch batch,List<Settlement_Road> toGetFrom)
    {
        Vector2Int min = batch.coords;
        Vector2Int max = batch.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);
        List<Settlement_Road> roads = new List<Settlement_Road>();

        


        for(int x = 0; x < toGetFrom.Count; x++)
        {
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(toGetFrom[x].StartPos.x, toGetFrom[x].StartPos.y, out Batch, out Chunk, out Coords);
            if (Batch == batch.coords)
            {
                if (IsPointInBounds(min, max, toGetFrom[x].StartPos))
                {
                    roads.Add(toGetFrom[x]);
                }
            }
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(toGetFrom[x].EndPos.x, toGetFrom[x].EndPos.y, out Batch, out Chunk, out Coords);
            if (Batch == batch.coords)
            {
                if ( IsPointInBounds(min, max, toGetFrom[x].EndPos))
                {
                    roads.Add(toGetFrom[x]);
                }
            }
        }

        return roads;
    }
    bool IsPointInBounds(Vector2Int min,Vector2Int max,Vector2 pos)
    {
        return pos.x >= min.x && pos.x < max.x && pos.y >= min.y && pos.y< max.y;
    }

    public GeneratedSettlementArea GetAreaFromOverworld(Vector2Int overworld,Vector2Int toGenerateInBatch)
    {
        float xLerp = Mathf.InverseLerp(low.x, high.x, toGenerateInBatch.x);
        float yLerp = Mathf.InverseLerp(low.y, high.y, toGenerateInBatch.y);
        int xInd = Mathf.FloorToInt( Mathf.Lerp(0, areas.GetLength(0), xLerp));
        int yInd = Mathf.FloorToInt(Mathf.Lerp(0, areas.GetLength(1), yLerp));
        return areas[xInd, yInd];
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(high.x, high.y, out Batch, out Chunk, out Coords);

        Vector2 difference = Batch - toGenerateInBatch;


        Vector2Int arrayCoords = new Vector2Int(Mathf.FloorToInt( high.x/WorldChunkManager.ChunkBatchSize) - overworld.x, Mathf.FloorToInt(high.y / WorldChunkManager.ChunkBatchSize) - overworld.y);
        Debug.LogError("Overworld to array " + overworld + "->" + arrayCoords+","+low+","+high+","+Batch+" to gen batch " + toGenerateInBatch);
        
        return areas[arrayCoords.x, arrayCoords.y];
    }

    public void GenerateSettlementAreas(Settlement_Settings settings,int areaSize)
    {
        width = Mathf.RoundToInt( settings.Size.x / areaSize);
        height = Mathf.RoundToInt(settings.Size.y / areaSize);
        low = settings.Center - (settings.Size * .5f);
        high = settings.Center + (settings.Size * .5f);

        areas = new GeneratedSettlementArea[width, height];
        for(int x=0;x<width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                areas[x, y] = new GeneratedSettlementArea(new Vector2(Mathf.Lerp(low.x, high.x, Mathf.InverseLerp(0, width, x)), Mathf.Lerp(low.y, high.y, Mathf.InverseLerp(0, height, y))));
            }
        }
        

    }

    public void SetRiver(Settlement_River river)
    {
        River=river;
    }

    public GeneratedSettlementArea GetAreaFromPosition(Vector2 pos)
    {
        int xc = 0, yc = 0;

        xc = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, pos.x)));
        yc = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, pos.y)));
        return areas[xc, yc];
    }

    public void PopulateAreas(Settlement_Settings settings, int areaSize)
    {
        
       
        int xc = 0, yc = 0, xc2 = 0, yc2 = 0;
        for (int q = 0; q < highways.Count; q++)
        {
            xc = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, highways[q].StartPos.x)));
            yc = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, highways[q].StartPos.y)));
            areas[xc, yc].AddHighway(highways[q]);

            xc2 = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, highways[q].EndPos.x)));
            yc2 = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, highways[q].EndPos.y)));
            if (xc != xc2 || yc != yc2)
            {
                areas[xc, yc].AddHighway(highways[q]);
            }
        }
        xc = 0;
        yc = 0;
        for (int q = 0; q < avenues.Count; q++)
        {
            xc = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, avenues[q].StartPos.x)));
            yc = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, avenues[q].StartPos.y)));
            areas[xc, yc].AddAvenue(avenues[q]);

            xc2 = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, avenues[q].EndPos.x)));
            yc2 = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, avenues[q].EndPos.y)));
            if (xc != xc2 || yc != yc2)
            {
                areas[xc, yc].AddHighway(avenues[q]);
            }
        }

        xc = 0;
        yc = 0;
        for (int q = 0; q < roads.Count; q++)
        {
            xc = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, roads[q].StartPos.x)));
            yc = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, roads[q].StartPos.y)));
            areas[xc, yc].AddRoad(roads[q]);

            xc2 = Mathf.FloorToInt(Mathf.Lerp(0, width - 1, Mathf.InverseLerp(low.x, high.x, roads[q].EndPos.x)));
            yc2 = Mathf.FloorToInt(Mathf.Lerp(0, height - 1, Mathf.InverseLerp(low.y, high.y, roads[q].EndPos.y)));
            if (xc != xc2 || yc != yc2)
            {
                areas[xc, yc].AddRoad(roads[q]);
            }
        }
    }



}

public class GeneratedSettlementArea
{
    public GeneratedSettlementArea(Vector2 bottomLeft)
    {
        Point = bottomLeft;
        DebugColour = new Color(Random.value, Random.value, Random.value);
        //int r = Random.Range(0, 100);
        //if (r < 15)
        //{
        //    CanUse = false;
        //}
    }
    public Color DebugColour;
    public Vector2 Point;
    public List<Settlement_Road> highways = new List<Settlement_Road>(), avenues = new List<Settlement_Road>(), roads = new List<Settlement_Road>();
    public bool CanUse = true;
    public void AddHighway(Settlement_Road highway)
    {
        Debug.Log("Added road at " + highway.StartPos + " to " + Point);

        highways.Add(highway);
    }
    public void AddAvenue(Settlement_Road highway)
    {
        Debug.Log("Added road at " + highway.StartPos + " to " + Point);
        avenues.Add(highway);
    }
    public void AddRoad(Settlement_Road highway)
    {
        Debug.Log("Added road at " + highway.StartPos + " to " + Point);

        roads.Add(highway);
    }
   


    public bool IsPointInArea(Vector2 point)
    {
        return false;
    }

}
