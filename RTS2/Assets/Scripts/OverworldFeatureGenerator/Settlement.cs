using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Settlement : OverworldFeatureToWorldConverter
{

    const int MaxRoads = 25;


    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        List<SettlementArea> areas = new List<SettlementArea>();
        areas.Add(new SettlementArea(toGenerateIn.coords, new Vector2(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize),toGenerateIn.coords));
        int count = 0;
        int index = 0;
        SettlementArea[] split = null;
        while (count < 100)
        {
            index = Random.Range(0, areas.Count);
            split = areas[index].Split();
            bool valid = true;
            for(int x=0;x< split.Length; x++)
            {
                if (!IsSplitValid(split[x])) {
                    valid = false;
                    break;
                }

            }
            if (valid)
            {
                areas.RemoveAt(index);
                areas.Add(split[0]);
                areas.Add(split[1]);
            }
            count++;
        }
        List<RoadData> data = new List<RoadData>();
        for(int x = 0; x < areas.Count; x++)
        {
            areas[x].CreateRoadsFromSplit(ref data);
        }
        for(int x = 0; x < data.Count; x++)
        {
           
            toGenerateIn.AddRoad(data[x]);
        }
        Debug.Log("Generated settlement, final road count " + toGenerateIn.Roads.Count+" in " + toGenerateIn.coords);
    }

    bool IsSplitValid(SettlementArea area)
    {
        if (area.size.x > 20 && area.size.y > 20)
        {
            return true;
        }
        return false;
    }

    int GetMaxLengthRoadCouldBe(Vector2 startPos, Vector2Int coords,Vector2 dir)
    {
        Vector2Int max = coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);
        Vector2 intersection = Vector2.zero;
        if (LineIntersection(startPos, startPos + dir * WorldChunkManager.ChunkBatchSize, coords, coords + Vector2.up*WorldChunkManager.ChunkBatchSize,ref intersection))
        {
            return Mathf.FloorToInt( (intersection - startPos).magnitude);
        }
        if (LineIntersection(startPos, startPos + dir * WorldChunkManager.ChunkBatchSize, coords, coords + Vector2.right*WorldChunkManager.ChunkBatchSize, ref intersection))
        {
            return Mathf.FloorToInt((intersection - startPos).magnitude);
        }
        if (LineIntersection(startPos, startPos + dir * WorldChunkManager.ChunkBatchSize, coords + Vector2.up * WorldChunkManager.ChunkBatchSize, coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize), ref intersection))
        {
            return Mathf.FloorToInt((intersection - startPos).magnitude);
        }
        if (LineIntersection(startPos, startPos + dir * WorldChunkManager.ChunkBatchSize, coords + Vector2.right * WorldChunkManager.ChunkBatchSize, coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize), ref intersection))
        {
            return Mathf.FloorToInt((intersection - startPos).magnitude);
        }
        return 0;
    }

    bool DoesRoadStartTouchMyEdge(RoadData data,Vector2Int coords)
    {
        Vector2Int max = coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);

        if (Mathf.Abs(data.StartPos.x-coords.x)<=2|| Mathf.Abs(data.StartPos.x - max.x) <= 2
            || Mathf.Abs(data.StartPos.y - coords.y) <= 2 || Mathf.Abs(data.StartPos.y - max.y) <= 2)
        {
            return true;
        }

        return false;
    }

    bool DoesRoadEndTouchMyEdge(RoadData data, Vector2Int coords)
    {
        Vector2Int max = coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize);

        if (Mathf.Abs(data.EndPos.x - coords.x) <= 2 || Mathf.Abs(data.EndPos.x - max.x) <= 2
            || Mathf.Abs(data.EndPos.y - coords.y) <= 2 || Mathf.Abs(data.EndPos.y - max.y) <= 2)
        {
            return true;
        }

        return false;
    }

    public override OverworldFeature GetFeatureIGenerate()
    {
        return OverworldFeature.Settlement;
    }
    
    void AddRoad(WorldChunkBatch toAddTo, RoadType type, Vector2Int start, Vector2Int end, int width,RoadData comingOff)
    {
        RoadData road = new RoadData(start, end, width, type);
        
        //RoadIntersection roadIntersection = null;
        //for (int x = 0; x < toAddTo.Roads.Count; x++)
        //{
        //    int minDist = Mathf.Max(width, toAddTo.Roads[x].Width);
        //    if (Vector2Int.Distance(start, toAddTo.Roads[x].StartPos) <minDist
        //        || Vector2Int.Distance(end, toAddTo.Roads[x].StartPos) < minDist
        //        || Vector2Int.Distance(start, toAddTo.Roads[x].EndPos) < minDist
        //        || Vector2Int.Distance(end, toAddTo.Roads[x].EndPos) < minDist)
        //    {
        //        return;
        //    }
        //}
       
        switch (type)
        {
            case RoadType.None:
                break;
            case RoadType.MajorRoad:
                toAddTo.AddRoad(road);
                break;
            case RoadType.MinorRoad:
                toAddTo.AddRoad(road);
                break;
            case RoadType.Backroad:
                toAddTo.AddRoad(road);
                break;
            default:
                break;
        }
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

//break up areas into random grids then make roads and buildings based off this (only use right and top edges for roads
public class SettlementArea
{
    public Vector2 position, size;
    public Vector2Int parentChunkBatch;
    public SettlementArea(Vector2 pos, Vector2 s, Vector2Int parent)
    {
        this.position = pos;
        this.size = s;
        parentChunkBatch = parent;
    }

    public void CreateRoadsFromSplit(ref List<RoadData> toAddTo)
    {
        Vector2Int start = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y + size.y));
        if(start.y>(parentChunkBatch.y+ WorldChunkManager.ChunkBatchSize - 7))
        {
            start.y = (parentChunkBatch.y + WorldChunkManager.ChunkBatchSize - 7);
        }
        Vector2Int start2 = new Vector2Int(Mathf.RoundToInt(position.x+size.x), Mathf.RoundToInt(position.y));
        if (start2.x > (parentChunkBatch.x + WorldChunkManager.ChunkBatchSize - 7))
        {
            start2.x = (parentChunkBatch.x + WorldChunkManager.ChunkBatchSize - 7);
        }
        Vector2Int end = new Vector2Int(start2.x, start.y);
        toAddTo.Add(new RoadData(start, end+Vector2Int.right, 7, RoadType.MinorRoad));
        toAddTo.Add(new RoadData(start2,end+Vector2Int.up,7, RoadType.MinorRoad));
    }

    public SettlementArea[] Split()
    {
        SettlementArea[] retVal = new SettlementArea[2];
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            float x = Random.Range(position.x, position.x + size.x);
            float firstSize = x - position.x;
            float secondSize = (position.x + size.x) - x;
            retVal[0] = new SettlementArea(position, new Vector2(firstSize, size.y), parentChunkBatch);
            retVal[1]=new SettlementArea(new Vector2(x,position.y),new Vector2(secondSize, size.y), parentChunkBatch);

        }
        else
        {
            float y = Random.Range(position.y, position.y + size.y);
            float firstSize = y - position.y;
            float secondSize = (position.y + size.y) - y;
            retVal[0] = new SettlementArea(position, new Vector2(size.x,firstSize), parentChunkBatch);
            retVal[1] = new SettlementArea(new Vector2(position.x,y), new Vector2(size.x,secondSize), parentChunkBatch);
        }

        return retVal;
    }
}

