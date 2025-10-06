using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class BatchRoad : ISerialize
{
    public Vector2 RoadStart, RoadEnd;
    public List<RoadSegment> Segments;
    public int Width;
    public string Key;
    public RoadType type;
    public string GetRoadTileType()
    {
        switch (type)
        {
            case RoadType.MajorRoad:
                return "MajorRoad";
                break;
            case RoadType.MinorRoad:
                return "MinorRoad";
                break;
            case RoadType.Backroad:
                return "Mud";
                break;
            default:
                break;
        }
        return "Error";
    }


    protected int HalfWidth()
    {
        return Mathf.Max(1, Width / 2);
    }
    public BatchRoad(RoadType type,Vector2 start,Vector2 end,int width)
    {
        this.type = type;
        RoadStart = start;
        RoadEnd = end;
        this.Width= width;
        Segments = new List<RoadSegment>();
        Segments.Add(new RoadSegment(start, end));
    }


    public virtual void GenerateRoad()
    {
        Segments = new List<RoadSegment>();
        Segments.Add(new RoadSegment(RoadStart, RoadEnd));
    }

    public virtual void RenderRoad(WorldChunkBatch batch)
    {
        for(int x=0;x<Segments.Count; x++)
        {
            Vector2 pos = Segments[x].Start;
            Vector2 target = Segments[x].End;
            float dist = Vector2.Distance(pos, new Vector2(target.x, target.y));
            Vector2 dir = target - pos;
            dir = dir.normalized;
            Vector2 perpDir = Vector2.Perpendicular(target - pos).normalized * HalfWidth();
            float inc = 1f / dist;
            inc /= 2f;
            float widthInc = 1f / Width;
            widthInc /= 2f;
            Vector2 leftEdge = Vector2.zero;
            Vector2 rightEdge = Vector2.zero;
            Vector2 curPos = new Vector2();
            Vector2 finalPos = new Vector2();
            for (float f = 0f; f < 1f; f += inc)
            {
                curPos = Vector2.Lerp(pos, target, f);
                leftEdge = curPos + perpDir;
                rightEdge = curPos + (perpDir * -1f);
               
                for (float a = 0f; a < 1f; a += widthInc)
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    UpdateTile(batch, finalPos, GetRoadTileType());
                }
            }
        }
    }
    protected bool UpdateTile(WorldChunkBatch toGenerateIn, Vector2 pos, string type)
    {
        WorldTile toEdit = toGenerateIn.GetTileFromPosition(pos);
        if (toEdit != null)
        {
            EnvironmentObjectInstance OnTile = null;
            if(WorldChunkManager.Instance.ChunkBatches[WorldChunkBatch.chunkBatch].Chunks[WorldChunkBatch.chunk.x, WorldChunkBatch.chunk.y].DoesAnyObjectExistAtCoords(toEdit.Coords(), out OnTile))
            {
                OnTile.DestroyInstance();
            }
            OverworldTile tile= OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);
            toEdit.Elevation.SetTileToWalkable(tile.Elevation);
            toEdit.UpdateWaterLevel(toEdit.WaterData.WaterLevel * -1f);
            toEdit.UpdateTileType(type);
            toEdit.CanPutDecorationsOn = false;
            return true;
        }
        return false;
    }

    protected bool IsNextIncrementGoingOverPoint(float val,float inc,float target)
    {
        if (val <= target && val + inc > target)
        {
            Debug.Log("Half way point " + val + "+" + inc);
            return true;
        }
        return false;
    }


    protected bool IsFirstIncrementOrLast(float val, float inc)
    {
        if (val <= inc || val + inc >= 1f-inc)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected string GetEdgeTile()
    {
        return "Tiled";
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.RoadType, type);
        retVal.AddDataToSerialize(DataKeys.RoadWidth, Width);
        retVal.AddDataToSerialize(DataKeys.RoadElement, Segments);
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
}

public class Backroad : BatchRoad
{
    public Backroad(RoadType type, Vector2 start,Vector2 end,int width): base(type, start, end, width)
    {
        
    }

    public override void GenerateRoad()
    {
        Segments = new List<RoadSegment>();
        List<PathfindingNode> path = Pathfinding.FindPath(RoadStart, RoadEnd);
        if (path != null && path.Count > 0)
        {
            for (int x = 0; x < path.Count - 1; x++)
            {
                Segments.Add(new RoadSegment(path[x].worldPos, path[x + 1].worldPos));
            }
        }
        else
        {
            base.GenerateRoad();
        }
    }

}

public class MinorRoad : BatchRoad
{
    public MinorRoad(RoadType type, Vector2 start, Vector2 end, int width) : base(type, start, end, width)
    {

    }

    public override void GenerateRoad()
    {
        Segments = new List<RoadSegment>();
        List<PathfindingNode> path = Pathfinding.FindPath(RoadStart, RoadEnd);

        if (path != null && path.Count > 0)
        {
            for (float f = 0f; f < 1f; f += .1f)
            {
                int startIndex = (int)Mathf.Lerp(0, path.Count - 1, f);
                int endIndex = (int)Mathf.Lerp(0, path.Count - 1, f + .1f);
                Segments.Add(new RoadSegment(path[startIndex].worldPos, path[endIndex].worldPos));
            }
        }
        else
        {
            base.GenerateRoad();
        }
    }

   


    public override void RenderRoad(WorldChunkBatch batch)
    {
        for (int x = 0; x < Segments.Count; x++)
        {
            Vector2 pos = Segments[x].Start;
            Vector2 target = Segments[x].End;
            float dist = Vector2.Distance(pos, new Vector2(target.x, target.y));
            Vector2 dir = target - pos;
            dir = dir.normalized;
            Vector2 perpDir = Vector2.Perpendicular(target - pos).normalized * HalfWidth();
            float inc = 1f / dist;
            inc /= 2f;
            float widthInc = 1f / Width;
            widthInc /= 2f;
            Vector2 leftEdge = Vector2.zero;
            Vector2 rightEdge = Vector2.zero;
            Vector2 curPos = new Vector2();
            Vector2 finalPos = new Vector2();
            for (float f = 0f; f < 1f; f += inc)
            {
                curPos = Vector2.Lerp(pos, target, f);
                leftEdge = curPos + perpDir;
                rightEdge = curPos + (perpDir * -1f);

                for (float a = 0f; a < 1f; a += widthInc)
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    if (IsFirstIncrementOrLast(a, widthInc))
                    {
                        UpdateTile(batch, finalPos, GetEdgeTile());

                    }
                    else
                    {
                        UpdateTile(batch, finalPos, GetRoadTileType());

                    }
                }
            }
        }
    }
}

public class MajorRoad : BatchRoad
{
    public MajorRoad(RoadType type, Vector2 start, Vector2 end, int width) : base(type, start, end, width)
    {

    }

    public override void GenerateRoad()
    {
        base.GenerateRoad();
    }

    public override void RenderRoad(WorldChunkBatch batch)
    {
        for (int x = 0; x < Segments.Count; x++)
        {
            Vector2 pos = Segments[x].Start;
            Vector2 target = Segments[x].End;
            float dist = Vector2.Distance(pos, new Vector2(target.x, target.y));
            Vector2 dir = target - pos;
            dir = dir.normalized;
            Vector2 perpDir = Vector2.Perpendicular(target - pos).normalized * HalfWidth();
            float inc = 1f / dist;
            inc /= 2f;
            float widthInc = 1f / Width;
            widthInc /= 2f;
            Vector2 leftEdge = Vector2.zero;
            Vector2 rightEdge = Vector2.zero;
            Vector2 curPos = new Vector2();
            Vector2 finalPos = new Vector2();
            for (float f = 0f; f < 1f; f += inc)
            {
                curPos = Vector2.Lerp(pos, target, f);
                leftEdge = curPos + perpDir;
                rightEdge = curPos + (perpDir * -1f);

                //base road
                for (float a = 0f; a < 1f; a += widthInc)
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    if (IsFirstIncrementOrLast(a, widthInc))
                    {
                        UpdateTile(batch, finalPos, GetEdgeTile());

                    }
                    else
                    {
                        UpdateTile(batch, finalPos, GetRoadTileType());

                    }
                }
                // ||
                //edges
                for (float a = 0f; a < 1f; a += widthInc)
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    if ( IsNextIncrementGoingOverPoint(a, widthInc, .5f))
                    {
                        UpdateTile(batch, finalPos, GetEdgeTile());

                    }
                    
                }
            }
        }
    }
}


public class BatchRoadBlend : BatchRoad
{
    List<BatchRoad> toLink;
    public BatchRoadBlend(RoadType type, Vector2 start, Vector2 end, int width,List<BatchRoad> roadsToLink) : base(type, start, end, width)
    {
        toLink = roadsToLink;
    }

    public override void GenerateRoad()
    {
        Segments = new List<RoadSegment>();
        Vector2 startPoint = Vector2.zero;
        Vector2 endPoint = Vector2.zero;
        Vector2 finalStartPoint = Vector2.zero;
        Vector2 finalEndPoint = Vector2.zero;
        
        for (int x = 0; x < toLink.Count; x++)
        {



           // Segments.Add(new RoadSegment(RoadStart, toLink[x].RoadStart));
            if (x < toLink.Count - 1)
            {
                startPoint = toLink[x].RoadStart;
                endPoint =  toLink[x + 1].RoadStart;
                
                for(float f = 0f; f < 1f; f += .05f)
                {


                    finalEndPoint = Vector2.Lerp(RoadStart, endPoint, f);
                    finalStartPoint =Vector2.Lerp(startPoint, RoadStart, f);

                    Vector2 p1 = Vector2.Lerp(finalStartPoint, finalEndPoint, f);
                    Vector2 p2 = Vector2.Lerp(finalStartPoint, finalEndPoint, f + .1f);

                   // finalStartPoint = Vector2.Lerp(startPoint, endPoint, f);
                    Segments.Add(new RoadSegment(p1,p2));

                }


            }
            else
            {
                startPoint =  toLink[x].RoadStart;
                endPoint = toLink[0].RoadStart;

                for (float f = 0f; f < 1f; f += .05f)
                {


                    finalEndPoint = Vector2.Lerp(RoadStart, endPoint, f);
                    finalStartPoint = Vector2.Lerp(startPoint, RoadStart, f);

                    Vector2 p1 = Vector2.Lerp(finalStartPoint, finalEndPoint, f);
                    Vector2 p2 = Vector2.Lerp(finalStartPoint, finalEndPoint, f + .1f);

                    // finalStartPoint = Vector2.Lerp(startPoint, endPoint, f);
                    Segments.Add(new RoadSegment(p1, p2));

                }
            }
        }
    }



    public override void RenderRoad(WorldChunkBatch batch)
    {
        for (int x = 0; x < Segments.Count; x++)
        {
            Vector2 pos = Segments[x].Start;
            Vector2 target = Segments[x].End;
            float dist = Vector2.Distance(pos, new Vector2(target.x, target.y));
            Vector2 dir = target - pos;
            dir = dir.normalized;
            Vector2 perpDir = Vector2.Perpendicular(target - pos).normalized * HalfWidth();
            float inc = 1f / dist;
            inc /= 2f;
            float widthInc = 1f / Width;
            widthInc /= 2f;
            Vector2 leftEdge = Vector2.zero;
            Vector2 rightEdge = Vector2.zero;
            Vector2 curPos = new Vector2();
            Vector2 finalPos = new Vector2();
            for (float f = 0f; f < 1f; f += inc)
            {
                curPos = Vector2.Lerp(pos, target, f);
                leftEdge = curPos + perpDir;
                rightEdge = curPos + (perpDir * -1f);

                //base road
                for (float a = 0f; a < 1f; a += widthInc)
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    UpdateTile(batch, finalPos, GetRoadTileType());

                }
                // ||
                //edges
                //for (float a = 0f; a < 1f; a += widthInc)
                //{
                //    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                //    if (IsNextIncrementGoingOverPoint(a, widthInc, .5f))
                //    {
                //        UpdateTile(batch, finalPos, GetEdgeTile());

                //    }

                //}
            }
        }
    }
}


public enum RoadType
{
    None,
    MajorRoad,
    MinorRoad,
    Backroad
}

public class RoadSegment
{
    public Vector2 Start, End;
    public RoadSegment(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;  
    }

    public Vector2Int StartInt()
    {
        return new Vector2Int(Mathf.RoundToInt( Start.x),Mathf.RoundToInt( Start.y));
    }

    public Vector2Int EndInt()
    {
        return new Vector2Int(Mathf.RoundToInt(End.x), Mathf.RoundToInt(Start.y));
    }
}
