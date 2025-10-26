using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class BatchRoad : ISerialize
{
    public Vector2 RoadStart, RoadEnd;
    public List<RoadSegment> Segments;
    public int Width;
    public string Key;
    public RoadType type;
    public bool IsGenerated = false, IsDrawn = false;
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

    public virtual bool IsBlend()
    {
        return false;
    }

    public uint GetRoadTileID()
    {
        if (!HasCached)
        {
            GetCachedTileIDs();
        }
        switch (type)
        {
            case RoadType.MajorRoad:
                return CachedMajor;
                break;
            case RoadType.MinorRoad:
                return CachedMinor;
                break;
            case RoadType.Backroad:
                return CachedBackroad;
                break;
            default:
                break;
        }
        return CachedMajor;
    }

    public bool SeperateLanes()
    {
        switch (type)
        {
            case RoadType.MajorRoad:
                return true;
                break;
            case RoadType.MinorRoad:
                return false;
                break;
            case RoadType.Backroad:
                return false;
                break;
            default:
                break;
        }
        return false;
    }

    public bool GenerateEdge()
    {
        switch (type)
        {
            case RoadType.MajorRoad:
                return true;
                break;
            case RoadType.MinorRoad:
                return true;
                break;
            case RoadType.Backroad:
                return false;
                break;
            default:
                break;
        }
        return false;
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
        IsGenerated = true;
    }

    public virtual void RenderRoad(WorldChunkBatch batch)
    {
        IsDrawn = true;
        for(int x=0;x<Segments.Count; x++)
        {
            Vector2 pos = Segments[x].Start;
            Vector2 target = Segments[x].End;
            float dist = Vector2.Distance(pos, new Vector2(target.x, target.y));
            Vector2 dir = target - pos;
            dir = dir.normalized;
            Vector2 perpDir = Vector2.Perpendicular(target - pos).normalized * HalfWidth();
            float inc = 1f / dist;
            //inc /= 2f;
            float widthInc = 1f / Width;
            //widthInc /= 2f;
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
                    if(! UpdateTile(batch, finalPos, GetRoadTileType(), GetRoadTileID()))
                    {
                    }
                }
            }
        }


    }
    public void LogCount()
    {
        Debug.Log("GetTile " + newMethod + "/" + oldMethod);

    }

    uint CachedTiled, CachedMajor, CachedMinor, CachedBackroad;
    bool HasCached = false;
    void GetCachedTileIDs()
    {
        CachedTiled = WorldRenderer.Instance.WorldTilesManager.GetTileID("Tiled");
        CachedMajor = WorldRenderer.Instance.WorldTilesManager.GetTileID("MajorRoad");
        CachedMinor = WorldRenderer.Instance.WorldTilesManager.GetTileID("MinorRoad");
        CachedBackroad = WorldRenderer.Instance.WorldTilesManager.GetTileID("Backroad");

        HasCached = true;
    }

    public bool IsAlreadyRoadTile(uint type,uint newType, RoadType Generating)
    {
        if (!HasCached)
        {
            GetCachedTileIDs();
        }

        if (newType == CachedTiled)
        {
            return type == CachedMajor || type == CachedTiled || type == CachedMinor || type == CachedBackroad;

        }
        switch (Generating)
        {
            case RoadType.None:
                return type == CachedMajor || type == CachedTiled || type == CachedMinor || type == CachedBackroad;
                break;
            case RoadType.MajorRoad:
                return type == CachedMajor;
                break;
            case RoadType.MinorRoad:
                return type == CachedMajor || type == CachedMinor;
                break;
            case RoadType.Backroad:
                return type == CachedMajor || type == CachedMinor || type == CachedBackroad;
                break;
            default:
                break;
        }
        return false;
    }

    bool InChunkRange(Vector2 coords)
    {
        if (coords.x >= 0 && coords.y >= 0 && coords.x < WorldChunkManager.ChunkSize && coords.y < WorldChunkManager.ChunkSize)
        {
            return true;
        }
        return false;
    }

    WorldTile toEdit = null;
    int newMethod = 0, oldMethod = 0;
    Vector2Int batch = new Vector2Int(),coords=new Vector2Int();
    int localChunkX=0, localChunkY = 0;
    Vector2Int lastCoords= new Vector2Int();
    bool lastExisted = true;

    bool IsPosInBatch(WorldChunkBatch toGenerateIn,Vector2 pos)
    {
        if(pos.x<toGenerateIn.coords.x || pos.x>toGenerateIn.coords.x+WorldChunkManager.ChunkBatchSize
            || pos.y < toGenerateIn.coords.y || pos.y > toGenerateIn.coords.y + WorldChunkManager.ChunkBatchSize)
        {
            return false;
        }
        return true;
    }

    protected bool UpdateTile(WorldChunkBatch toGenerateIn, Vector2 pos, string type,uint typeID, bool CareAboutOverwrite = true)
    {
        if (!IsPosInBatch(toGenerateIn, pos))
        {
            return false;
        }
       
        batch = toGenerateIn.coords;

   
        lastExisted = WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch);
         
        if (!lastExisted)
        {
            return false;
        }


        localChunkX = Mathf.Clamp( Mathf.FloorToInt( Mathf.InverseLerp(batch.x, batch.x+ WorldChunkManager.ChunkBatchSize, pos.x ) * WorldChunkManager.ChunkSize),0,15);//Mathf.FloorToInt((pos.x-batch.x) / WorldChunkManager.ChunksPerBatch);
        localChunkY = Mathf.Clamp(Mathf.FloorToInt(Mathf.InverseLerp(batch.y, batch.y + WorldChunkManager.ChunkBatchSize, pos.y ) * WorldChunkManager.ChunkSize), 0, 15);


        coords.x = Mathf.FloorToInt((pos.x % WorldChunkManager.ChunkSize));
        coords.y = Mathf.FloorToInt((pos.y % WorldChunkManager.ChunkSize));//= new Vector2Int(,);

        if (coords.x < 0)
        {
            coords.x += WorldChunkManager.ChunkSize;
        }
        if(coords.y < 0)
        {
            coords.y += WorldChunkManager.ChunkSize;
        }

        if (coords == lastCoords)
        {
            return false;
        }
        else
        {
            lastCoords = coords;
        }

        try
        {
            
            if (lastExisted==false)
            {
                return false;
            }
            toEdit = toGenerateIn.Chunks[localChunkX, localChunkY].ChunkTiles[coords.x, coords.y];//WorldTileHelpers.GetTileNearExisting(toEdit,toGenerateIn,pos);

            if (toEdit != null)
            {
                if (IsAlreadyRoadTile(toEdit.TileID, typeID, this.type) && CareAboutOverwrite)
                {
                    return false;
                }
                EnvironmentObjectInstance OnTile = null;
                if (toGenerateIn.Chunks[localChunkX, localChunkY].DoesAnyObjectExistAtCoords(toEdit.Coords(), out OnTile))
                {
                    Debug.Log("Destroyed object on road " + OnTile.Name());
                    OnTile.DestroyInstance();
                }
                OverworldTile tile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);
                toEdit.Elevation.SetTileToWalkable(tile.Elevation);
                toEdit.UpdateWaterLevel(toEdit.WaterData.WaterLevel * -1f);
                 toEdit.UpdateTileType(type,typeID);
                //toGenerateIn.Chunks[localChunkX, localChunkY].UpdateTile(toEdit.Local.x, toEdit.Local.y, type, typeID);
                toEdit.CanPutDecorationsOn = false;
                return true;
            }
        }
        catch(System.Exception e) 
        {
            Debug.LogError("error rendering road with " + pos+"batch " + batch+"Local Chunk "+  localChunkX+","+localChunkY+" coords "+coords+ " error "+ e.ToString());
        }
            return false;
    }

    protected bool IsNextIncrementGoingOverPoint(float val,float inc,float target)
    {
        if (val <= target && val + inc > target)
        {
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

    protected uint GetEdgeID()
    {
        if (!HasCached)
        {
            GetCachedTileIDs();
        }
        return CachedTiled;
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
        IsGenerated = true;
    }

}

public class MinorRoad : BatchRoad
{
    public MinorRoad(RoadType type, Vector2 start, Vector2 end, int width) : base(type, start, end, width)
    {

    }

    public override void GenerateRoad()
    {
        IsGenerated = true;
        Segments = new List<RoadSegment>();
        List<PathfindingNode> path = Pathfinding.FindPath(RoadStart, RoadEnd);
        Vector2 startPoint = Vector2.zero;
        Vector2 endPoint = Vector2.zero;
        Vector2 finalEndPoint = Vector2.zero;
        Vector2 finalStartPoint = Vector2.zero;
        if (path != null && path.Count > 0)
        {
            for (int x=0;x<path.Count-1;x++)
            {
               
                startPoint = path[x].worldPos;
                endPoint = path[x+1].worldPos;

                for (float q = 0f;q < 1f; q += .05f)
                {


                    finalEndPoint = Vector2.Lerp(RoadStart, endPoint, q);
                    finalStartPoint = Vector2.Lerp(startPoint, RoadStart, q);

                    Vector2 p1 = Vector2.Lerp(finalStartPoint, finalEndPoint, q);
                    Vector2 p2 = Vector2.Lerp(finalStartPoint, finalEndPoint, q + .1f);

                    // finalStartPoint = Vector2.Lerp(startPoint, endPoint, f);
                    Segments.Add(new RoadSegment(p1, p2));

                }
            }
            Vector3 dir = (endPoint - startPoint).normalized;
            Segments.Add(new RoadSegment(path[path.Count - 1].worldPos, path[path.Count - 1].worldPos + (dir * 4)));
        }
        else
        {
            base.GenerateRoad();
        }

       
        
    }

   


    public override void RenderRoad(WorldChunkBatch batch)
    {
        IsDrawn = true;
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
                   
                    UpdateTile(batch, finalPos, GetRoadTileType(), GetRoadTileID());

                    
                }
                finalPos = Vector2.Lerp(leftEdge, rightEdge, 0f);
                UpdateTile(batch, finalPos, GetEdgeTile(), GetEdgeID());
                finalPos = Vector2.Lerp(leftEdge, rightEdge, 1f);
                UpdateTile(batch, finalPos, GetEdgeTile(), GetEdgeID());

              
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
        IsDrawn = true;
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
                        if(!UpdateTile(batch, finalPos, GetEdgeTile(), GetEdgeID()))
                        {
                        }

                    }
                    else
                    {
                        if(!UpdateTile(batch, finalPos, GetRoadTileType(), GetRoadTileID())){
                         
                        }
                    }
                }
                // ||
                //edges
                for (float a = 0f; a < 1f; a += widthInc)
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    if ( IsNextIncrementGoingOverPoint(a, widthInc, .5f))
                    {
                        UpdateTile(batch, finalPos, GetEdgeTile(), GetEdgeID());

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
        IsGenerated = true;
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
        IsDrawn = true;
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
                    if (IsFirstIncrementOrLast(a, widthInc) && GenerateEdge())
                    {
                        UpdateTile(batch, finalPos, GetEdgeTile(), GetEdgeID());

                    }
                    else
                    {
                        UpdateTile(batch, finalPos, GetRoadTileType(), GetRoadTileID());

                    }
                }
                // ||
                //edges
                if (SeperateLanes())
                {
                    for (float a = 0f; a < 1f; a += widthInc)
                    {
                        finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                        if (IsNextIncrementGoingOverPoint(a, widthInc, .5f))
                        {
                            UpdateTile(batch, finalPos, GetEdgeTile(), GetEdgeID());

                        }

                    }
                }
                }
            }
        
    }

    public override bool IsBlend()
    {
        return true;
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
