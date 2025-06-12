using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WorldChunkManager : MonoBehaviour
{
    public const int ChunkSize = 16;
    public const int ChunksPerBatch = 16;
    public const int ChunkBatchSize = ChunkSize * ChunksPerBatch;
    static WorldChunkManager instance;
    public Dictionary<Vector2Int, WorldChunkBatch> ChunkBatches;

    public static WorldChunkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorldChunkManager>();
                if (instance.IsInit==false)
                {
                    instance.Init();
                }
            }
            return instance;
        }
    }
    public bool IsInit = false;
    public void Init()
    {
       
        InitWorldChunks();
        IsInit = true;
    }
    int Width, Height;
    public void InitWorldChunks()
    {
        ChunkBatches = new Dictionary<Vector2Int, WorldChunkBatch>();
        CreateChunkBatch(new Vector2Int());
     
        Debug.Log("Init world chunk manager");
       
    }
    
    void CreateChunkBatch(Vector2Int coords,bool init=true)
    {
        ChunkBatches.Add(coords, WorldChunkBatch.CreateWorldChunkBatch(coords));
        Debug.Log("Creating chunk at " + coords);
        if (init)
        {
            ChunkBatches[coords].InitWorldChunks();
        }
    }
    
    public void PerformCreateNewChunksCheck()
    {
        Vector3 cameraPos = CameraController.Instance.transform.position;
        Vector2Int cameraCoords = ConvertPositionToChunkBatchCoords(cameraPos);
        Debug.Log("Coords Convert: Converted " + cameraPos + " to " + cameraCoords + " exists " + ChunkBatches.ContainsKey(cameraCoords));
        List<Vector2Int> coords = GetAdjacentBatchCoords(cameraCoords);
        bool needToRender = false;
        for(int x = 0; x < coords.Count; x++)
        {
            if (!ChunkBatches.ContainsKey(coords[x]))
            {
                CreateChunkBatch(coords[x]);
                needToRender = true;
            }
        }
        if (needToRender)
        {
            RenderWorldChunks();
        }
    }

    List<Vector2Int> GetAdjacentBatchCoords(Vector2Int coords)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();

        retVal.Add(coords + new Vector2Int(ChunkBatchSize, 0));
        retVal.Add(coords + new Vector2Int(-ChunkBatchSize, 0));
        retVal.Add(coords + new Vector2Int(0, ChunkBatchSize));
        retVal.Add(coords + new Vector2Int(0,-ChunkBatchSize));

        return retVal;
    }
    Vector2Int ConvertPositionToChunkBatchCoords(Vector2Int pos)
    {
        int x = 0;

        if (pos.x < 0)
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        else
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        int y = 0;

        if (pos.y < 0)
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        else
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        return new Vector2Int(x - ChunkBatchSize, y - ChunkBatchSize);
    }

    Vector2Int ConvertPositionToChunkBatchCoords(Vector3 pos)
    {
        int x = 0;

        if (pos.x < 0)
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        else
        {
            x = RoundToMultiple(pos.x, ChunkBatchSize);
        }
        int y = 0;

        if (pos.y < 0)
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        else
        {
            y = RoundToMultiple(pos.y, ChunkBatchSize);
        }
        return new Vector2Int(x-ChunkBatchSize, y-ChunkBatchSize);
    }
    public int RoundToMultiple(float value, int roundTo)
    {
        return  Mathf.CeilToInt(value / roundTo) * roundTo;
    }
    public void RenderWorldChunks()
    {
        foreach(KeyValuePair<Vector2Int,WorldChunkBatch> kvp in ChunkBatches)
        {
            kvp.Value.RenderChunk();
        }
    }

    public void LoadChunksFromFile(string name)
    {
        ChunkBatches[new Vector2Int()].LoadChunksFromFile(name);
    }
    public WorldChunkBatch GetWorldChunkBatchFromPosition(Vector2Int pos,bool canCreateNew=false)
    {
        Vector2Int coords = ConvertPositionToChunkBatchCoords(pos);
        
            if (!ChunkBatches.ContainsKey(coords))
            {
            if (canCreateNew)
            {
                CreateChunkBatch(coords);
            }
            else
            {
                return null;
            }
            }

            return ChunkBatches[coords];
    }

    public WorldChunkBatch GetWorldChunkBatchFromPosition(Vector3 pos,bool canCreateNew = false)
    {
        Vector2Int coords = ConvertPositionToChunkBatchCoords(pos);
        if (!ChunkBatches.ContainsKey(coords))
        {
            if (canCreateNew)
            {
                CreateChunkBatch(coords);
            }
            else
            {
                return null;
            }
        }

            return ChunkBatches[coords];
    }


    public void OnBuildableFinished(BuildableStructure bs)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).OnBuildableFinished(bs);
       
    }
    
    public WorldChunk GetWorldChunkFromPos(Vector3 pos)
    {
        Vector2Int chunkCoords = GetChunkCoordsFromWorldPos(pos);
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(pos);
        if (batch != null)
        {
            return batch.Chunks[chunkCoords.x, chunkCoords.y];
        }
        else
        {
            return null;
        }
        }

        public Vector2Int GetChunkCoordsFromWorldPos(Vector3 worldPos)
    {
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(worldPos);
        if (batch != null)
        {
            return batch.GetChunkCoordsFromWorldPos(worldPos);
        }
        else
        {
            return Vector2Int.zero;
        }
        }

        public WorldChunk GetWorldChunkFromTileCoords(Vector2Int coords,bool canCreateNew=false,bool debug=false)
    {
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(coords, canCreateNew);
        if (batch != null)
        {
            Vector2Int chunkCoords = batch.GetChunkCoordsFromTileCoords(coords,debug);
            if (debug)
            {
                Debug.Log("Furniture Click: coords " + coords + " to tile coords was " + chunkCoords + " from " + batch.coords);
            }
                return batch.Chunks[chunkCoords.x, chunkCoords.y];
        }
        else
        {
            return null;
        }
    }


    public Vector2Int GetChunkCoordsFromTileCoords(Vector2Int coords)
    {
        WorldChunkBatch batch = GetWorldChunkBatchFromPosition(coords);
        if (batch != null)
        {
            return batch.GetChunkCoordsFromTileCoords(coords);
        }
        else
        {
            return Vector2Int.zero;
        }
    }

        public void AddEnvironmentObjectInstanceToChunk(EnvironmentObjectInstance obj)
    {
        GetWorldChunkBatchFromPosition(obj.Position()).AddEnvironmentObject(obj, obj.Position());

    }

    public void AddContainerObject(Inventory toAdd)
    {
        GetWorldChunkBatchFromPosition(toAdd.transform.position).AddContainerObject(toAdd);
    }

    public void RemoveContainerObject(Inventory toRemove)
    {
        GetWorldChunkBatchFromPosition(toRemove.transform.position).RemoveContainerObject(toRemove);

    }

    public void AddResourceObject(ResourceInstance res)
    {
        GetWorldChunkBatchFromPosition(res.transform.position).AddResourceObject(res);
    }

    public void RemoveResourceObject(ResourceInstance res)
    {
        GetWorldChunkBatchFromPosition(res.transform.position).RemoveResourceObject(res);
    }

    public void AddConstructable(Constructable bs)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).AddConstructable(bs);
    }
    public void RemoveConstructable(Constructable bs, bool needsCleanup = true)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).RemoveConstructable(bs,needsCleanup);
    }
    private void Update()
    {
        PerformCreateNewChunksCheck();
        DebugDrawChunks();
    }


   public bool CoordsValid(int x,int y)
    {
        return x>=0&&y>=0&&x<Width&&y<Height;
    }

    public List<WorldChunk> GetChunksInRadius(float radius,Vector3 searchCenter)
    {
       return GetWorldChunkBatchFromPosition(searchCenter).GetChunksInRadius(radius,searchCenter);
    }

    const bool DrawNodeWalkable = false, DrawNodeNeighbours = false;
    void DebugDrawChunks()
    {
        foreach (KeyValuePair<Vector2Int, WorldChunkBatch> kvp in ChunkBatches)
        {
            kvp.Value.DebugDrawChunks();
        }
       
    }





  


    public void OnUnitCreated(Unit u)
    {
        GetWorldChunkBatchFromPosition(u.transform.position).OnUnitCreated(u);
      
    }

    public void OnUnitMove(Unit u)
    {
        GetWorldChunkBatchFromPosition(u.transform.position).OnUnitMove(u);

      
    }

    public void OnUnitDeath(Unit u)
    {
        GetWorldChunkBatchFromPosition(u.transform.position).OnUnitDeath(u);
    }

}
