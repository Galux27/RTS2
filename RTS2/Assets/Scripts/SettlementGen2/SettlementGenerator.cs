using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public  static class SettlementGenerator 
{
    static GeneratedSettlement CurrentlyGenerating;
    public static void GenerateSettlement(GeneratedSettlement settlement, Settlement_Settings settings)
    {
        HasEdges = false;
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
        AddRoadsFromOverworld(settings);
        GenerateInitialHighways(settings);
        Debug.Log("Settlement Gen: initial highways done ");
        HighwayGenerationPass(settings);
        Debug.Log("Settlement Gen: highway gen done");
        
        settlement.highways = highways;
        
        GenerateInitialAvenues(settings);
        Debug.Log("Settlement Gen: initial avenues done");

        AvenueGenerationPass(settings);
        Debug.Log("Settlement Gen: avenue gen done");
        //ValidateAllAvenues();
        settlement.avenues = avenues;

        GenerateInitialRoads(settings);
        RoadGenerationPass(settings);

        ValidateAllRoads();

        settlement.roads = roads;
    }


    static List<GeneratedSettlementArea> NeighbouringAreas(int x,int y)
    {
        List<GeneratedSettlementArea> retVal = new List<GeneratedSettlementArea>();
        if (x > 0)
        {
            retVal.Add(CurrentlyGenerating.areas[x - 1, y]);
        }

        if (x < CurrentlyGenerating.areas.GetLength(0) - 1)
        {
            retVal.Add(CurrentlyGenerating.areas[x + 1, y]);
        }
        if (y > 0)
        {
            retVal.Add(CurrentlyGenerating.areas[x , y - 1]);
        }

        if (y < CurrentlyGenerating.areas.GetLength(1) - 1)
        {
            retVal.Add(CurrentlyGenerating.areas[x , y + 1]);
        }
        return retVal;
    }

    static void AddRoadsFromOverworld(Settlement_Settings settings)
    {
        List<GeneratedSettlementArea> neighbours = new List<GeneratedSettlementArea>();
        OverworldTile tile = null,neighbour=null;
        GeneratedSettlementArea current = null;
        int count = 0;
        for (int q = 0; q < CurrentlyGenerating.areas.GetLength(0); q++)
        {
            for (int r = 0; r < CurrentlyGenerating.areas.GetLength(1); r++)
            {
                current = CurrentlyGenerating.areas[q, r];
                neighbours = NeighbouringAreas(q, r);
                tile = OverworldGenerator.Instance.OverworldTiles[current.OverworldTile.x, current.OverworldTile.y];
                for(int x = 0; x < neighbours.Count; x++)
                {
                    neighbour = OverworldGenerator.Instance.GetOverworldTile(neighbours[x].OverworldTile);
                    if (neighbour.Features.Contains(OverworldFeature.MajorRoad) && tile.Features.Contains(OverworldFeature.MajorRoad))
                    {
                        highways.Add(new Settlement_Road(current.Center(),  (neighbours[x].Center()- current.Center()).normalized, WorldChunkManager.ChunkBatchSize , Settlement_RoadType.Highway));
                        count++;
                    }
                    
                    if (neighbour.Features.Contains(OverworldFeature.MinorRoad) && tile.Features.Contains(OverworldFeature.MinorRoad))
                    {
                        avenues.Add(new Settlement_Road(current.Center(), (neighbours[x].Center() - current.Center()).normalized, WorldChunkManager.ChunkBatchSize , Settlement_RoadType.Avenue));
                        count++;

                    }
                    
                    if (neighbour.Features.Contains(OverworldFeature.Backroad) && tile.Features.Contains(OverworldFeature.Backroad))
                    {
                        roads.Add(new Settlement_Road(current.Center(), (neighbours[x].Center() - current.Center()).normalized, WorldChunkManager.ChunkBatchSize , Settlement_RoadType.Road));
                        count++;

                    }
                }

            }
        }
        Debug.Log("Total roads from overworld " + count);
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
                Debug.Log("Highway: Initial Tile at " + startPos);
                validRoads.Add(new Settlement_Road(startPos, dir.normalized, settings.HighwayLength, Settlement_RoadType.Highway));
                //settlement.AddHighway();

            }
            startPos = GetEdge(settings);
            dir = settings.Center - startPos;
        }
    }

    static bool IsARoadNearMyStart(Vector2 potentialPos, List<Settlement_Road> toCheck,float maxDist)
    {
        
        for(int x = 0; x < toCheck.Count; x++)
        {
            if (Vector2.Distance(toCheck[x].StartPos, potentialPos) < maxDist || Vector2.Distance(toCheck[x].EndPos,potentialPos) < maxDist)
            {
                return true;
            }
        }
        return false;
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
        int i = 0;
        if (CheckForIntersection(validRoads, original.StartPos, ref newEndPoint, out intersection))
        {
            i = 1;
            foundEnd = true;
        }

        if (CheckForIntersection(highways, original.StartPos, ref newEndPoint, out intersection))
        {
            i = 2;
            foundEnd = true;
        }

        if(CheckForMovingEndToOtherPoint(highways, ref newEndPoint, settings.DistBetweenHighways))
        {
            i = 3;
            foundEnd = true;
        }

        //original checks for if the new end positions are close to the start/end positions of the existing roads
        //
        original.UpdateEndPosition(newEndPoint);
        original.EndedByLink = foundEnd;
        highways.Add(original);
        Debug.Log("Highway: from " + original.StartPos + " to " + newEndPoint+","+ IsPositionNearEdge(newEndPoint, settings)+","+foundEnd+","+i);
        //
        if (!foundEnd &&!IsPositionNearEdge(newEndPoint,settings) && !IsRoadInInvalidChunk(original, CurrentlyGenerating))
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
        int count = 0;
        for(int x=0;x<initialHighways;x++)
        {
            Vector2 pos = highways[x].GetPositionOnRoad(.5f);
            if (IsARoadNearMyStart(pos, validRoads, settings.DistBetweenAvenues))
            {
                continue;
            }
            highways[x].AddPointToSplit(.5f);

            Vector2 dir = GetRandomRightAngleDirectionFromRoad(highways[x]);

            validRoads.Add(new Settlement_Road(pos, dir.normalized, settings.AvenueLength, Settlement_RoadType.Avenue, highways[x]));
            validRoads.Add(new Settlement_Road(pos, (dir*-1).normalized, settings.AvenueLength, Settlement_RoadType.Avenue, highways[x]));
            count++;
            if (count > settings.StartingAvenueCount)
            {
                break;
            }

        }
    }

    static bool IsHorizontalMoreThanVertical(Vector2 dir)
    {
        return Mathf.Abs(dir.x)> Mathf.Abs(dir.y);
    }

    static Vector2 GetRandomAngledOffsetFromRoad(Settlement_Road gettingFrom)
    {
        return Vector2.zero;
        Vector2 retVal = Vector2.zero;
        Vector2 dir = gettingFrom.endPos - gettingFrom.StartPos;
        dir = dir.normalized;
        bool isHorizontal = IsHorizontalMoreThanVertical(dir);
        int r = 0;
        if (isHorizontal)
        {
            r = Random.Range(0, 100);
            if (r < 95)
            {

            }
            else if (r <= 97)
            {
                retVal += new Vector2(0, .5f);
            }
            else
            {
                retVal -= new Vector2(0, .5f);

            }
        }
        else
        {
            r = Random.Range(0, 100);
            if (r < 95)
            {

            }
            else if (r <= 97)
            {
                retVal += new Vector2(.5f, 0);
            }
            else
            {
                retVal -= new Vector2(.5f, 0);

            }
        }
        return retVal;
    }


    static Vector2 GetRandomRightAngleDirectionFromRoad(Settlement_Road gettingFrom)
    {
        Vector2 retVal = Vector2.zero;
        Vector2 dir = gettingFrom.endPos - gettingFrom.StartPos;
        dir = dir.normalized;
        bool isHorizontal = IsHorizontalMoreThanVertical(dir);
        int r = 0;
        if (isHorizontal)
        {
            r = Random.Range(0, 100);
            if (r<50)
            {
                retVal = Vector2.left;
            }
            else
            {
                retVal = Vector2.right;
            }
        }
        else
        {
           r = Random.Range(0, 100);
            if (r < 50)
            {
                retVal = Vector2.up;
            }
            else
            {
                retVal = Vector2.down;
            }
        }



        return retVal;
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
        if (IsRoadInInvalidChunk(original, CurrentlyGenerating))
        {
            return;
        }
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
                if (IsARoadNearMyStart(pos, validRoads, settings.DistBetweenRoads))
                {
                    continue;
                }
                Vector2 dir = GetRandomRightAngleDirectionFromRoad(avenues[x]);

                avenues[x].AddPointToSplit(split);
                validRoads.Add(new Settlement_Road(pos, dir.normalized, settings.RoadLength, Settlement_RoadType.Road, avenues[x]));
                validRoads.Add(new Settlement_Road(pos, (dir*-1).normalized, settings.RoadLength, Settlement_RoadType.Road, avenues[x]));
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

        Vector2 offset = GetRandomAngledOffsetFromRoad(original);
        if (offset != Vector2.zero)
        {
            newEndPoint = original.StartPos + ((original.Direction + offset) * original.Length);
        }


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

        //if(CheckForMovingEndToOtherPoint(roads, ref newEndPoint, settings.DistBetweenRoads))
        //{
        //    foundEnd = true;
        //}
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


        if(IsRoadInInvalidChunk(original, CurrentlyGenerating))
        {
            return;
        }

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
        float dist2 = 9999999f;
        Vector2 newPos = end;
        Vector2 closestPoint = Vector2.zero;
        for(int x = 0; x < toTestAgainst.Count; x++)
        {
            closestPoint = toTestAgainst[x].NearestPointOnLine(end);
            dist2 = Vector2.Distance(closestPoint, end);
            if (dist2<dist&&dist2<maxDist)
            {
                dist = dist2;

                newPos = closestPoint;
                
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

    static List<Vector2Int> EdgeTiles = new List<Vector2Int>();
    static bool HasEdges = false;

    static void GetEdges()
    {
        int width = CurrentlyGenerating.areas.GetLength(0);
        int height = CurrentlyGenerating.areas.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CurrentlyGenerating.areas[x, y].CanUse)
                {
                    EdgeTiles.Add(CurrentlyGenerating.areas[x, y].topCorner);
                    break;
                }
            }

            for (int y = height-1; y >0; y--)
            {
                if (CurrentlyGenerating.areas[x, y].CanUse)
                {
                    EdgeTiles.Add(CurrentlyGenerating.areas[x, y].topCorner);
                    break;
                }
            }

        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CurrentlyGenerating.areas[x, y].CanUse)
                {
                    EdgeTiles.Add(CurrentlyGenerating.areas[x, y].topCorner);
                    break;
                }
            }

            for (int x = width-1; x >0; x--)
            {
                if (CurrentlyGenerating.areas[x, y].CanUse)
                {
                    EdgeTiles.Add(CurrentlyGenerating.areas[x, y].topCorner);
                    break;
                }
            }

        }
        Debug.Log("Edge tiles " + EdgeTiles.Count);
    }

    static Vector2 GetEdge(Settlement_Settings settings)
    {
        if (!HasEdges)
        {
            GetEdges();
            HasEdges = true;
        }
        return EdgeTiles[Random.Range(0,EdgeTiles.Count)];
    }


    static void ValidateAllRoads()
    {
       
        roads = ValidateRoads(roads, highways,false,15);
        roads = ValidateRoads(roads, avenues,false,10f);
       // roads = ValidateRoads(roads, roads,true,10f);
      
    }


    static void ValidateAllAvenues()
    {
       avenues = ValidateRoads(avenues, highways);
       avenues = ValidateRoads(avenues, avenues,true);

    }

    static List<Settlement_Road> ValidateRoads(List<Settlement_Road> roads,List<Settlement_Road> comp,bool isSameList=false,float dist=5f)
    {
        List<Settlement_Road> retVal = new List<Settlement_Road>();
        if (!isSameList)
        {
            for (int x = 0; x < roads.Count; x++)
            {
                bool hit = false;
                for (int q = 0; q < comp.Count; q++)
                {
                    if (Settlement_Road.IsRoadTooCloseToOtherRoad(roads[x], comp[q],dist))
                    {
                        hit = true;
                        break;
                    }
                }
                if (!hit)
                {
                    retVal.Add(roads[x]);
                }
            }
        }
        else
        {
            List<int> invalid = new List<int>();
            for (int x = 0; x < roads.Count; x++)
            {
                bool hit = false;
                for (int q = 0; q < comp.Count; q++)
                {
                    if (x!=q&& Settlement_Road.IsRoadTooCloseToOtherRoad(roads[x], comp[q],dist))
                    {
                        if (!invalid.Contains(q))
                        {
                            hit = true;
                            invalid.Add(q);
                            break;
                        }
                        }
                    }
                if (!hit)
                {
                    retVal.Add(roads[x]);
                }
            }
        }
        return retVal;
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
            bool added = false;

            Vector2 pos = Vector2.Lerp(toGetFrom[x].StartPos, toGetFrom[x].EndPos, .5f);
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out Batch, out Chunk, out Coords);
            if (Batch == batch.coords)
            {
                if (IsPointInBounds(min, max, pos))
                {
                    roads.Add(toGetFrom[x]);
                    added = true;
                }
            }

            //WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(toGetFrom[x].StartPos.x, toGetFrom[x].StartPos.y, out Batch, out Chunk, out Coords);
            //if (Batch == batch.coords)
            //{
            //    if (IsPointInBounds(min, max, toGetFrom[x].StartPos))
            //    {
            //        roads.Add(toGetFrom[x]);
            //        added = true;
            //    }
            //}
            //if (!added)
            //{
            //    WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(toGetFrom[x].EndPos.x, toGetFrom[x].EndPos.y, out Batch, out Chunk, out Coords);
            //    if (Batch == batch.coords)
            //    {
            //        if (IsPointInBounds(min, max, toGetFrom[x].EndPos))
            //        {
            //            roads.Add(toGetFrom[x]);
            //            added = true;
            //        }
            //    }
            //    if (!added)
            //    {
            //        Vector2 pos = Vector2.Lerp(toGetFrom[x].StartPos, toGetFrom[x].EndPos, .5f);
            //        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out Batch, out Chunk, out Coords);
            //        if (Batch == batch.coords)
            //        {
            //            if (IsPointInBounds(min, max, pos))
            //            {
            //                roads.Add(toGetFrom[x]);
            //                added = true;
            //            }
            //        }
            //    }
            //}
           
        }

        return roads;
    }
    bool IsPointInBounds(Vector2Int min,Vector2Int max,Vector2 pos)
    {
        return pos.x >= min.x && pos.x < max.x && pos.y >= min.y && pos.y< max.y;
    }

    public GeneratedSettlementArea AreaFromCoords(Vector2Int batchCoords)
    {
        for(int x = 0; x < areas.GetLength(0); x++)
        {
            for(int y = 0; y < areas.GetLength(1); y++)
            {
                if (areas[x, y].batchCoords == batchCoords)
                {
                    return areas[x, y];
                }
            }
        }
        return null;
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

        Vector2Int batch = new Vector2Int(), chunk = new Vector2Int(), tile = new Vector2Int();
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(low.x, low.y, out batch, out chunk, out tile);

        Vector2Int areaBatch = new Vector2Int();
        Vector2Int overworldTile = new Vector2Int();
        areas = new GeneratedSettlementArea[width, height];
        for(int x=0;x<width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                areaBatch = new Vector2Int(batch.x + (WorldChunkManager.ChunkBatchSize * x), batch.y + (WorldChunkManager.ChunkBatchSize * y));
                overworldTile = new Vector2Int(areaBatch.x / WorldChunkManager.ChunkBatchSize, areaBatch.y / WorldChunkManager.ChunkBatchSize);
                areas[x, y] = new GeneratedSettlementArea(
                    new Vector2(low.x+ (WorldChunkManager.ChunkBatchSize * x), low.y + (WorldChunkManager.ChunkBatchSize * y)),
                    areaBatch,overworldTile);
                Debug.Log("Setting overworld tile: " + overworldTile + " has settlement " + OverworldGenerator.Instance.GetOverworldTile(overworldTile).Features.Contains(OverworldFeature.Settlement));
                if (OverworldGenerator.Instance.GetOverworldTile(overworldTile).Features.Contains(OverworldFeature.Settlement)==false)
                {
                    areas[x, y].CanUse = false;
                }
                else
                {
                    areas[x, y].CanUse = true;

                }

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
        //rewrite this so it goes through all areas and checks if roads are in them
        GeneratedSettlementArea curArea=null;
        for(int x = 0; x < areas.GetLength(0); x++)
        {
            for (int y = 0; y < areas.GetLength(1); y++)
            {
                curArea = areas[x, y];

                for (int q = 0; q < highways.Count; q++)
                {
                    bool added = false;

                    if (curArea.IsPointInArea(highways[q].StartPos)
                        || curArea.IsPointInArea(Vector2.Lerp( highways[q].StartPos, highways[q].EndPos,.5f)) 
                        || curArea.IsPointInArea(highways[q].endPos))
                    {
                        curArea.AddHighway(highways[q]);
                        added = true;
                    }

                    if (!added)
                    {
                        for(float f = 0f; f < 1f; f += .1f)
                        {
                            if (curArea.IsPointInArea(Vector2.Lerp(highways[q].StartPos, highways[q].EndPos, f)))
                            {
                                curArea.AddHighway(highways[q]);
                                break;
                            }
                        }
                    }
                }

                for (int q = 0; q < avenues.Count; q++)
                {
                    bool added = false;

                    if (curArea.IsPointInArea(avenues[q].StartPos) || curArea.IsPointInArea(Vector2.Lerp(avenues[q].StartPos, avenues[q].EndPos, .5f)) || curArea.IsPointInArea(avenues[q].endPos))
                    {
                        curArea.AddAvenue(avenues[q]);
                        added = true;

                    }
                    if (!added)
                    {
                        for (float f = 0f; f < 1f; f += .1f)
                        {
                            if (curArea.IsPointInArea(Vector2.Lerp(avenues[q].StartPos, avenues[q].EndPos, f)))
                            {
                                curArea.AddHighway(avenues[q]);
                                break;
                            }
                        }
                    }
                }
                for (int q = 0; q <roads.Count; q++)
                {
                    bool added = false;

                    if (curArea.IsPointInArea(roads[q].StartPos) || curArea.IsPointInArea(Vector2.Lerp(roads[q].StartPos, roads[q].EndPos, .5f)) || curArea.IsPointInArea(roads[q].endPos))
                    {
                        curArea.AddRoad(roads[q]);
                        added = true;

                    }
                    if (!added)
                    {
                        for (float f = 0f; f < 1f; f += .1f)
                        {
                            if (curArea.IsPointInArea(Vector2.Lerp(roads[q].StartPos, roads[q].EndPos, f)))
                            {
                                curArea.AddHighway(roads[q]);
                                break;
                            }
                        }
                    }
                }
            }
        } 
    }



}

public class GeneratedSettlementArea
{
    public GeneratedSettlementArea(Vector2 bottomLeft,Vector2Int batchCoords,Vector2Int overworldCoords)
    {
        Point = bottomLeft;
        this.batchCoords = batchCoords;
        topCorner = batchCoords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);
        DebugColour = new Color(Random.value, Random.value, Random.value);
        OverworldTile = overworldCoords;
    }


    public Vector2 Center()
    {
        return Vector2.Lerp(Point, topCorner,.5f);
    }
   
    public Color DebugColour;
    public Vector2 Point;
    public Vector2Int batchCoords,topCorner,OverworldTile;
    public List<Settlement_Road> highways = new List<Settlement_Road>(), avenues = new List<Settlement_Road>(), roads = new List<Settlement_Road>();
    public bool CanUse = true;
    public void AddHighway(Settlement_Road highway)
    {
        Debug.Log("Added road at " + vecToInt(highway.StartPos) + "," + vecToInt(highway.endPos) + " to " + batchCoords);

        highways.Add(highway);
    }
    public void AddAvenue(Settlement_Road highway)
    {
        Debug.Log("Added road at " + vecToInt(highway.StartPos) + ","+ vecToInt(highway.endPos) + " to " + batchCoords);
        avenues.Add(highway);
    }
    public void AddRoad(Settlement_Road highway)
    {
        Debug.Log("Added road at " + vecToInt(highway.StartPos) + "," + vecToInt(highway.endPos) + " to " + batchCoords);

        roads.Add(highway);
    }
   Vector2Int vecToInt(Vector2 vec)
    {
        return new Vector2Int(Mathf.RoundToInt(vec.x),Mathf.RoundToInt(vec.y));
    }


    public bool IsPointInArea(Vector2 point)
    {
        return point.x>=batchCoords.x && point.y>=batchCoords.y && point.x<topCorner.x&& point.y<topCorner.y;
    }

}
