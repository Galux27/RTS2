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
    int HalfWidth()
    {
        return Mathf.Max(1, Width / 2);
    }
    public BatchRoad(RoadType type,Vector2 start,Vector2 end,string key,int width)
    {
        this.type = type;
        RoadStart = start;
        RoadEnd = end;
        this.Width= width;
        this.Key = key;
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
            Vector2 leftEdge = Vector2.zero;
            Vector2 rightEdge = Vector2.zero;
            Vector2 curPos = new Vector2();
            Vector2 finalPos = new Vector2();
            for (float f = 0f; f < 1f; f += inc)
            {
                curPos = Vector2.Lerp(pos, target, f);
                leftEdge = curPos + perpDir;
                rightEdge = curPos + (perpDir * -1f);
               
                for (float a = 0f; a < 1f; a += (1f / (float)Width))
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    UpdateTile(batch, finalPos, Key);
                }
            }
        }
    }
    bool UpdateTile(WorldChunkBatch toGenerateIn, Vector2 pos, string type)
    {
        WorldTile toEdit = toGenerateIn.GetTileFromPosition(pos);
        if (toEdit != null)
        {
            EnvironmentObjectInstance OnTile = null;
            if(WorldChunkManager.Instance.ChunkBatches[WorldChunkBatch.chunkBatch].Chunks[WorldChunkBatch.chunk.x, WorldChunkBatch.chunk.y].DoesAnyObjectExistAtCoords(toEdit.Coords(), out OnTile))
            {
                OnTile.DestroyInstance();
            }

            
            toEdit.UpdateWaterLevel(toEdit.WaterData.WaterLevel * -1f);

            toEdit.UpdateTileType(type);
            toEdit.CanPutDecorationsOn = false;
            return true;
        }
        return false;
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    public DataToSerialize GetDataToSerialize()
    {
        throw new System.NotImplementedException();
    }

    public UID GetMyUID()
    {
        throw new System.NotImplementedException();
    }

    public SerializedData Serialize()
    {
        throw new System.NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
        throw new System.NotImplementedException();
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
}
