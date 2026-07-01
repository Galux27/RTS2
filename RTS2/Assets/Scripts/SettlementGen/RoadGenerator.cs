using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

public static class RoadGenerator
{
    public static List<RoadData> AllRoads = new List<RoadData>();
    const int maxIterations = 500;
    static HashSet<Vector2Int> Edges=new HashSet<Vector2Int>();
    static Vector2Int Batch, Chunk, Coords;

    public static bool IsIntersectingAnything(List<RoadIntersection> AllRoadIntersections,Vector2 pos)
    {
        for(int x=0;x<AllRoadIntersections.Count;x++)
        {
            if (AllRoadIntersections[x].IsPointIntersectintPoints(pos))
            {
                return true;
            }
        }
        return false;
    }

    public static void PopulateRoadTileData(RoadData data)
    {
        switch (data.Type)
        {
            case RoadType.None:
                break;
            case RoadType.MajorRoad:
                data.HasEdge = true;
                data.EdgeTile = "Tiled";
                data.RoadTile = "MajorRoad";

                break;
            case RoadType.MinorRoad:
                data.HasEdge = true;
                data.EdgeTile = "Tiled";
                data.RoadTile = "MinorRoad";

                break;
            case RoadType.Backroad:
                data.RoadTile = "BackRoad";
                data.HasEdge = false;
                break;
            default:
                break;
        }
    }

    static bool IsPositionNearStartOrEndOfPositions(Vector2 pos,List<RoadData> roads)
    {
        for(int x = 0; x < roads.Count; x++)
        {
            if (Vector2.Distance(pos, roads[x].StartPos) < roads[x].HalfWidth
                || Vector2.Distance(pos, roads[x].EndPos) < roads[x].HalfWidth)
            {
                return true;
            }
        }
        return false;
    }

   public static void GenerateRoad(RoadData data, ref List<RoadData> existingRoads,WorldChunkBatch curBatch)
   {
        List<RoadData> intersectingRoads = new List<RoadData>();
        List<RoadIntersection> AllRoadIntersections = new List<RoadIntersection>();
        RoadIntersection toAdd = null;
        //for (int x = 0; x < existingRoads.Count; x++)
        //{
        //    toAdd = existingRoads[x].DoesRoadIntersect(data);
        //    if (toAdd != null && toAdd.RoadPoints.Count > 0)
        //    {
        //        AllRoadIntersections.Add(toAdd);
        //        intersectingRoads.Add(existingRoads[x]);
        //        Debug.DrawLine(toAdd.GetFirstPoint(), toAdd.GetLastPoint(), Color.yellow, 99f);
        //    }
        //    else
        //    {
        //        toAdd = existingRoads[x].ReverseDoesRoadIntersect(data);
        //        if (toAdd != null && toAdd.RoadPoints.Count > 0)
        //        {
        //            AllRoadIntersections.Add(toAdd);
        //            intersectingRoads.Add(existingRoads[x]);
        //            Debug.DrawLine(toAdd.GetFirstPoint(), toAdd.GetLastPoint(), Color.cyan, 99f);
        //        }
        //    }
        //    toAdd = null;

        //}

        Edges = new HashSet<Vector2Int>();
        Vector2 Direction = data.EndPos - data.StartPos;
        Direction = Direction.normalized;
        Vector2 PerpDirection=Vector2.Perpendicular(Direction);
        PerpDirection = PerpDirection.normalized;
        Vector2 currentCenterPosition = data.StartPos;
        Vector2Int roundedCurrentCenterPosition = data.StartPos;
        Vector2Int newPosition = Vector2Int.zero;
        bool hitEnd = false;
        int count = 0;
        int updatedTilesCount = 0;
        int updateCount = 0;
        float dist = Vector2.Distance(data.StartPos, data.EndPos);
        while (!hitEnd)
        {
            newPosition = new Vector2Int(Mathf.RoundToInt(currentCenterPosition.x), Mathf.RoundToInt(currentCenterPosition.y));
            //if (!IsIntersectingAnything(AllRoadIntersections, newPosition))
            {
                GenerateRoadSegmentInterior(roundedCurrentCenterPosition, data, PerpDirection,out updateCount,curBatch.coords);
                updatedTilesCount += updateCount;
                roundedCurrentCenterPosition = newPosition;
            }
            if (Vector2.Distance(currentCenterPosition, data.StartPos) > dist)
            {
                hitEnd = true;
            }
            currentCenterPosition += Direction;
            count++;
            if (count > maxIterations)
            {
                hitEnd = true;
            }
        }
        Debug.LogError("Generating road from " + data.StartPos + " to " + data.EndPos+" count "+ count+" "+newPosition+","
            +Direction+","+currentCenterPosition+","+data.Width+" tiles updated " + updatedTilesCount);

        currentCenterPosition = data.StartPos + Direction;
        hitEnd = false;
        count = 0;
        newPosition = Vector2Int.zero;
        roundedCurrentCenterPosition = data.StartPos;
        while (!hitEnd)
        {
            newPosition = new Vector2Int(Mathf.RoundToInt(currentCenterPosition.x), Mathf.RoundToInt(currentCenterPosition.y));
            //if (Vector2.Distance(newPosition, data.StartPos) > data.Width && Vector2.Distance(newPosition, data.EndPos) > data.Width)
            {
                if (!IsIntersectingAnything(AllRoadIntersections, newPosition) && !IsPositionNearStartOrEndOfPositions(newPosition, intersectingRoads))
                {
                    GenerateRoadSegmentEdges(newPosition, data, PerpDirection);
                    roundedCurrentCenterPosition = newPosition;

                }
            }
            if (Vector2.Distance(currentCenterPosition, data.EndPos) < Vector2.Distance(currentCenterPosition + Direction, data.EndPos))
            {
                hitEnd = true;
            }
            currentCenterPosition += Direction;
            count++;
            if (count > maxIterations)
            {
                hitEnd = true;
            }
        }
    }
    static WorldTile currentTile;
    static HashSet<Vector2Int> updatedTiles = new HashSet<Vector2Int>();
    static void GenerateRoadSegmentEdges(Vector2Int startCoords,RoadData data,Vector2 direction)
    {
        if (data.EdgeTile == null)
        {
            return;
        }
        Vector2 startPos = startCoords-direction*(data.Width/2);
        Vector2 endPos = startCoords + direction* (data.Width / 2);
        uint edgeID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.EdgeTile);
        uint roadID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.RoadTile);
        updatedTiles = new HashSet<Vector2Int>();
        Vector2Int globalCoords = new Vector2Int();
        WorldChunkBatch batch = null;
        currentTile = null;

        if (data.HasEdge)
        {

            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {

                currentTile = WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                UpdateTile(data.EdgeTile, edgeID, currentTile, Vector2Int.zero);
              
            }
           
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(endPos.x, endPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {  
                currentTile = WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                UpdateTile(data.EdgeTile, edgeID, currentTile, Vector2Int.zero);
         
            }
        }


   
   
    }
    static void GenerateRoadSegmentInterior(Vector2Int startCoords, RoadData data, Vector2 direction,out int updatedTilesCount,Vector2Int currentBatch)
    {
        updatedTilesCount = 0;
        Vector2 startPos = startCoords - direction * (data.Width / 2);
        Vector2 endPos = startCoords + direction * (data.Width / 2);

        uint edgeID = 0;
        if (data.HasEdge)
        {
            WorldRenderer.Instance.WorldTilesManager.GetTileID(data.EdgeTile);

        }
        uint roadID =  WorldRenderer.Instance.WorldTilesManager.GetTileID(data.RoadTile);
        updatedTiles = new HashSet<Vector2Int>();
        Vector2Int globalCoords = new Vector2Int();
        WorldChunkBatch batch = null;
        currentTile = null;
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
        Vector2 testPos = startPos;
        bool FoundSuccessfulStart = false;
        if (!WorldChunkManager.Instance.DoesBatchExist(Batch))
        {
            for (float f = 0f; f < 1f; f += .01f)
            {
                testPos = Vector2.Lerp(startPos, endPos, f);
                WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(testPos.x, testPos.y, out Batch, out Chunk, out Coords);
                if (WorldChunkManager.Instance.DoesBatchExist(Batch))
                {
                    FoundSuccessfulStart = true;
                    startPos = testPos;
                    break;
                }
            }
        }
       

        startPos += direction;
        while (Vector2.Distance(startPos, endPos) > Vector2.Distance(startPos + direction, endPos))
        {
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            //batch is null generating settlement roads, fuck knows why
            if (batch != null)
            {
                currentTile = batch.Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                globalCoords.x = currentTile.x;
                globalCoords.y = currentTile.y;

                updatedTilesCount +=UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.zero, false);
                updatedTilesCount += UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.up, false);
                updatedTilesCount += UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.down, false);
                updatedTilesCount += UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.left, false);
                updatedTilesCount += UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.right, false);
                updatedTilesCount++;

            }else
            {
                Debug.LogError("Batch: batch is null " + startCoords + "," + direction + "," + Batch+","+startCoords);
            }
            startPos += direction;

        }

    }

    static int UpdateTile(string type, uint id, WorldTile tile, Vector2Int offset, bool canSetTile = true)
    {
        Vector2Int globalCoords = new Vector2Int(tile.x, tile.y)+offset;
        if (updatedTiles.Contains(globalCoords))
        {
            return -1;
        }
        Vector2Int myCoords = Coords + offset;
        if(!WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].CoordsValid(myCoords.x, myCoords.y))
        {
            return 0;
        }
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].UpdateTile(myCoords.x, myCoords.y, type,id);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[myCoords.x, myCoords.y].SetElevation(OverworldGenerator.Instance.SeaLevel+1);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y]
            .ChunkTiles[myCoords.x, myCoords.y].UpdateWaterLevel(WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[myCoords.x, myCoords.y].WaterData.WaterLevel * -1f);

        EnvironmentObjectInstance obj = WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].GetEnvObjectNearPoint(new Vector2(currentTile.x,currentTile.y), 2f);
        if (obj != null)
        {
            obj.DestroyInstance();
        }
        if (canSetTile)
        {
            updatedTiles.Add(globalCoords);
        }
        return 1;
    }
}

public class RoadData : ISerialize
{
    public Vector2Int StartPos, EndPos,LeftStart,LeftEnd,RightStart,RightEnd;



    public Vector2 perp,dir;
    public int Width,HalfWidth;
    public bool HasEdge = false,IsGenerated=false;
    public string RoadTile, EdgeTile;
    public RoadType Type;
    public RoadData(Vector2Int start,Vector2Int end,int width,RoadType type)
    {
        StartPos=start; EndPos=end; Width=width;
        perp = Vector2.Perpendicular((end - start)).normalized*(width/2);
        dir = end - start;
        dir = dir.normalized;
        RightStart = Vec2ToInt(StartPos + perp);
        RightEnd = Vec2ToInt(EndPos + perp);
        LeftStart = Vec2ToInt(StartPos - perp);
        LeftEnd=Vec2ToInt(EndPos - perp);
        Type = type;
        RoadGenerator.PopulateRoadTileData(this);
    }

    public RoadData(Vector2 start, Vector2 end, int width, RoadType type)
    {
        StartPos =Vec2ToInt( start);
        EndPos = Vec2ToInt(end);
        Width = width;
        perp = Vector2.Perpendicular((end - start)).normalized * (width / 2);
        dir = end - start;
        dir = dir.normalized;
        RightStart = Vec2ToInt(StartPos + perp);
        RightEnd = Vec2ToInt(EndPos + perp);
        LeftStart = Vec2ToInt(StartPos - perp);
        LeftEnd = Vec2ToInt(EndPos - perp);
        Type = type;
        RoadGenerator.PopulateRoadTileData(this);
    }

    Vector2Int Vec2ToInt(Vector2 val)
    {
        return new Vector2Int(Mathf.RoundToInt(val.x),Mathf.RoundToInt(val.y));
    }


    public Vector3 DebugStart()
    {
        return new Vector3(StartPos.x, StartPos.y);
    }
    public Vector3 DebugEnd()
    {
        return new Vector3(EndPos.x, EndPos.y);
    }
    public Vector3 DebugPerp()
    {
        return new Vector3(perp.x, perp.y);
    }
    public bool IntersectsLeftEdge(Vector2 start,Vector2 end,ref Vector2 pos)
    {
        return LineIntersection(start, end, (StartPos-dir) - (perp), (EndPos + dir) - (perp), ref pos);
    }
    public bool IntersectsRightEdge(Vector2 start, Vector2 end, ref Vector2 pos)
    {
        return LineIntersection(start, end, (StartPos-dir) + (perp), (EndPos+dir )+ (perp), ref pos);
    }
    public bool IntersectsCenterLine(Vector2 start, Vector2 end, ref Vector2 pos)
    {
        return LineIntersection(start, end, StartPos-dir, EndPos+dir, ref pos);
    }

    public RoadIntersection ReverseDoesRoadIntersect(RoadData road)
    {
        List<Vector2> Intersections = new List<Vector2>();
        Vector2 intersection = new Vector2();
        if (IntersectsLeftEdge(road.EndPos, road.StartPos, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsCenterLine(road.EndPos, road.StartPos, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsRightEdge(road.EndPos, road.StartPos, ref intersection))
        {
            Intersections.Add(intersection);
        }


        if (Intersections.Count == 1)
        {
            Intersections.Add(road.StartPos);
        }
        if (Intersections.Count > 0)
        {
            return new RoadIntersection(Intersections);
        }
        return null;
    }

        public RoadIntersection DoesRoadIntersect(RoadData road) 
    {
        List<Vector2> Intersections = new List<Vector2>();
        Vector2 intersection = new Vector2();

        Vector2Int dir = road.EndPos - road.StartPos;

        if (IntersectsLeftEdge(road.StartPos-dir, road.EndPos+dir, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsCenterLine(road.StartPos - dir, road.EndPos + dir, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsRightEdge(road.StartPos - dir, road.EndPos + dir, ref intersection))
        {
            Intersections.Add(intersection);
        }


        
        if (Intersections.Count > 0)
        {

            return new RoadIntersection(Intersections);
        }
        return null;
        if (IntersectsLeftEdge(road.LeftStart, road.LeftEnd, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsCenterLine(road.LeftStart, road.LeftEnd, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsRightEdge(road.LeftStart, road.LeftEnd, ref intersection))
        {
            Intersections.Add(intersection);
        }



        if (Intersections.Count == 1)
        {
            Intersections.Add(road.EndPos);
        }
        if (Intersections.Count > 0)
        {
            return new RoadIntersection(Intersections);
        }

        if (IntersectsLeftEdge(road.RightStart, road.RightEnd, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsCenterLine(road.RightStart, road.RightEnd, ref intersection))
        {
            Intersections.Add(intersection);
        }

        if (IntersectsRightEdge(road.RightStart, road.RightEnd, ref intersection))
        {
            Intersections.Add(intersection);
        }



        if (Intersections.Count == 1)
        {
            Intersections.Add(road.EndPos);
        }
        //if (Intersections.Count > 0)
        {
            return new RoadIntersection(Intersections);
        }


    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    List<RoadSegment> Segments()
    {
        List<RoadSegment> rs = new List<RoadSegment>();
        rs.Add(new RoadSegment(StartPos,EndPos));
        return rs;
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.RoadType, Type);
        retVal.AddDataToSerialize(DataKeys.RoadWidth, Width);
        retVal.AddDataToSerialize(DataKeys.RoadElement, Segments());
        return retVal;
    }

    public UID GetMyUID()
    {
        throw new System.NotImplementedException();
    }

    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void SetMyUID(ulong uid)
    {
        throw new System.NotImplementedException();
    }

    static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num, offset;
        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//

        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx;  // beta numerator//

        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }

        // check if they are parallel
        if (f == 0) return false;

        // compute intersection coordinates //
        num = d * Ax;   // numerator //
        offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;  // round direction //
        intersection.x = p1.x + (num + offset) / f;

        num = d * Ay;
        offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;
        intersection.y = p1.y + (num + offset) / f;

        return true;
    }

    private static bool same_sign(float a, float b)
    {
        return ((a * b) >= 0f);
    }
}

public class RoadIntersection
{
    public List<Vector2> RoadPoints;
    public RoadIntersection(List<Vector2> RoadPoints)
    {
        this.RoadPoints= RoadPoints;
    }
    public bool IsPointIntersectintPoints(Vector2 pos)
    {
        if (RoadPoints.Count > 1)
        {
            return BackwardsDot(pos) < 0f && ForwardsDot(pos) <0f;
        }
        return false;
    }

    float ForwardsDot(Vector2 pos)
    {
        Vector2 heading = GetFirstPoint() - pos;
        return Vector2.Dot(heading, GetForwards());
    }

    float BackwardsDot(Vector2 pos)
    {
        Vector2 heading = GetLastPoint() - pos;
        return Vector2.Dot(heading, GetForwards()*-1);
    }

    Vector2 GetForwards()
    {
        return GetLastPoint() - GetFirstPoint();
    }

    public Vector2 GetFirstPoint()
    {
        return RoadPoints[0];
    }
    public Vector2 GetLastPoint()
    {
        return RoadPoints[RoadPoints.Count - 1];
    }
}
