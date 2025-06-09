using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WorldChunkManager : MonoBehaviour
{
    public const int ChunkSize = 16;
    public const int ChunksPerBatch = 16;
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
        ChunkBatches.Add(new Vector2Int(), WorldChunkBatch.CreateWorldChunkBatch(new Vector2Int()));
        Debug.Log("Init world chunk manager");
        foreach (KeyValuePair<Vector2Int,WorldChunkBatch> kvp in ChunkBatches)
        {
            kvp.Value.InitWorldChunks();
        }
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
    public WorldChunkBatch GetWorldChunkBatchFromPosition(Vector2Int pos)
    {
        return ChunkBatches[new Vector2Int()];
    }

    public WorldChunkBatch GetWorldChunkBatchFromPosition(Vector3 pos)
    {
        return ChunkBatches[new Vector2Int()];
    }


    public void OnBuildableFinished(BuildableStructure bs)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).OnBuildableFinished(bs);
       
    }
    
    public WorldChunk GetWorldChunkFromPos(Vector3 pos)
    {
        Vector2Int chunkCoords = GetChunkCoordsFromWorldPos(pos);
        return GetWorldChunkBatchFromPosition(pos).Chunks[chunkCoords.x, chunkCoords.y];
    }

    public Vector2Int GetChunkCoordsFromWorldPos(Vector3 worldPos)
    {
        return GetWorldChunkBatchFromPosition(worldPos).GetChunkCoordsFromWorldPos(worldPos);
    }

    public WorldChunk GetWorldChunkFromTileCoords(Vector2Int coords)
    {
        Vector2Int chunkCoords = GetWorldChunkBatchFromPosition(coords).GetChunkCoordsFromTileCoords(coords);
        return GetWorldChunkBatchFromPosition(coords).Chunks[chunkCoords.x, chunkCoords.y];
    }


    public Vector2Int GetChunkCoordsFromTileCoords(Vector2Int coords)
    {
        return GetWorldChunkBatchFromPosition(coords).GetChunkCoordsFromTileCoords(coords);
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
    public void RemoveConstructable(Constructable bs)
    {
        GetWorldChunkBatchFromPosition(bs.GetPosition()).RemoveConstructable(bs);
    }
    private void Update()
    {
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
