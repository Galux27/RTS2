using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkManager : MonoBehaviour
{
    const int ChunkSize = 5;

    static WorldChunkManager instance;


    public static WorldChunkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorldChunkManager>();
            }
            return instance;
        }
    }

    public WorldChunk[,] Chunks;
    private void Awake()
    {
        InitWorldChunks();
    }
    int Width, Height;
    public void InitWorldChunks()
    {
        Chunks=new WorldChunk[WorldController.Instance.WorldWidth/ChunkSize, WorldController.Instance.WorldHeight / ChunkSize];

        for(int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y< Chunks.GetLength(1); y++)
            {
                Chunks[x,y] = new WorldChunk();
            }
        }
        Height = Chunks.GetLength(1);
        Width = Chunks.GetLength(0);
    }


    

    Vector2Int getCoordsCache=new Vector2Int();
    public Vector2Int GetChunkCoordsFromWorldPos(Vector3 worldPos)
    {
        getCoordsCache.x = Mathf.RoundToInt(worldPos.x/ChunkSize);
        getCoordsCache.y = Mathf.RoundToInt(worldPos.y / ChunkSize);

        return getCoordsCache;
    }

    private void Update()
    {
        DebugDrawChunks();
    }


    bool CoordsValid(int x,int y)
    {
        return x>=0&&y>=0&&x<Width&&y<Height;
    }

    public List<WorldChunk> GetChunksInRadius(float radius,Vector3 searchCenter)
    {
        List<WorldChunk> retVal = new List<WorldChunk>();
        GetChunkCoordsFromWorldPos(searchCenter);
        int chunkRadius = Mathf.Max(Mathf.RoundToInt(radius/ChunkSize), 1);

        for (int x = getCoordsCache.x-chunkRadius; x < getCoordsCache.x + chunkRadius; x++)
        {
            for (int y= getCoordsCache.y - chunkRadius;y < getCoordsCache.y + chunkRadius; y++)
            {
                if (CoordsValid(x, y))
                {
                    retVal.Add(Chunks[x,y]);
                }
            }
        }

        return retVal;
    }


    void DebugDrawChunks()
    {
        Vector3 tl = new Vector3(-ChunkSize / 2f, ChunkSize / 2f, 0f);
        Vector3 tr = new Vector3(ChunkSize / 2f, ChunkSize / 2f, 0f);
        Vector3 bl = new Vector3(-ChunkSize / 2f, -ChunkSize / 2f, 0f);
        Vector3 br = new Vector3(ChunkSize / 2f, -ChunkSize / 2f, 0f);
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Vector3 Center = new Vector3(x * ChunkSize, y * ChunkSize, 0);
                for(int z = 0; z < Chunks[x,y].UnitsInChunk.Count; z++)
                {
                    Debug.DrawLine(Center, Chunks[x, y].UnitsInChunk[z].transform.position, Chunks[x, y].DebugColor);



                }

                Debug.DrawLine(Center+tl, Center+tr, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + tr, Center + br, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + br, Center + bl, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + tl, Center + bl, Chunks[x, y].DebugColor);

            }
        }
    }


    public void OnUnitCreated(Unit u)
    {
        GetChunkCoordsFromWorldPos(u.transform.position);
        Chunks[getCoordsCache.x, getCoordsCache.y].AddUnitToChunk(u) ;
        u.UpdateChunk(getCoordsCache);
    }

    public void OnUnitMove(Unit u)
    {
        GetChunkCoordsFromWorldPos(u.transform.position);

        if(u.MyCurrentChunk != getCoordsCache)
        {
            Chunks[u.MyCurrentChunk.x, u.MyCurrentChunk.y].RemoveUnitFromChunk(u);
            Chunks[getCoordsCache.x, getCoordsCache.y].AddUnitToChunk(u);
            u.UpdateChunk( getCoordsCache);
        }

    
    }

    public void OnUnitDeath(Unit u)
    {
        Chunks[u.MyCurrentChunk.x, u.MyCurrentChunk.y].RemoveUnitFromChunk(u);
    }

}
