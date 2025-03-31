using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkManager : MonoBehaviour
{
    public const int ChunkSize = 16;

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
                Chunks[x,y] = new WorldChunk(x,y);
            }
        }
        Height = Chunks.GetLength(1);
        Width = Chunks.GetLength(0);
    }

    public void OnBuildableFinished(BuildableStructure bs)
    {
        Vector2Int coords = GetChunkCoordsFromWorldPos(bs.GetPosition());
        if (!CoordsValid(coords.x, coords.y))
        {
            return;
        }
        Chunks[coords.x, coords.y].ToBuild.Remove(bs);
    }
    

    Vector2Int getCoordsCache=new Vector2Int();
    public Vector2Int GetChunkCoordsFromWorldPos(Vector3 worldPos)
    {
        getCoordsCache.x = Mathf.Min(Mathf.FloorToInt(worldPos.x/ChunkSize), Chunks.GetLength(0) - 1);
        getCoordsCache.y = Mathf.Min( Mathf.FloorToInt(worldPos.y / ChunkSize),Chunks.GetLength(1)-1);

        return getCoordsCache;
    }

    public Vector2Int GetChunkCoordsFromTileCoords(Vector2Int coords)
    {
        getCoordsCache.x = Mathf.Min(coords.x / ChunkSize, Chunks.GetLength(0) - 1);
        getCoordsCache.y = Mathf.Min(coords.y / ChunkSize, Chunks.GetLength(1) - 1);

        return getCoordsCache;
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

    const bool DrawNodeWalkable = false, DrawNodeNeighbours = false;
    void DebugDrawChunks()
    {
    
        Vector3 tl = new Vector3(0 , ChunkSize , 0f);
        Vector3 tr = new Vector3(ChunkSize , ChunkSize , 0f);
        Vector3 bl = new Vector3(0 , 0 , 0f);
        Vector3 br = new Vector3(ChunkSize , 0 , 0f);
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Vector3 Center = new Vector3(x * ChunkSize, y * ChunkSize, 0);
                //for(int z = 0; z < Chunks[x,y].UnitsInChunk.Count; z++)
                //{
                //    try
                //    {
                //        Debug.DrawLine(Center, Chunks[x, y].UnitsInChunk[z].transform.position, Chunks[x, y].DebugColor);
                //    }
                //    catch
                //    {
                //        Debug.LogError("Error drawing chunk units in chunk " + x + "," + y);
                //    }


                //}

                Debug.DrawLine(Center+tl, Center+tr, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + tr, Center + br, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + br, Center + bl, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + tl, Center + bl, Chunks[x, y].DebugColor);


                Vector3 pos = Vector3.zero;
                for (int x1 = 0; x1 < Chunks[x,y].PathfindingNodes.GetLength(0); x1++)
                {
                    for (int y1 = 0; y1 < Chunks[x, y].PathfindingNodes.GetLength(1); y1++)
                    {
                        if (DrawNodeWalkable)
                        {
                            pos = Chunks[x, y].PathfindingNodes[x1, y1].worldPos;
                            if (Chunks[x, y].PathfindingNodes[x1, y1].IsPassable)
                            {
                                Debug.DrawLine(pos, pos + (Vector3.up * (x1+y1)/32f), Color.green);
                            }
                            else
                            {
                                Debug.DrawLine(pos, pos + (Vector3.up * (x1 + y1) / 32f), Color.red);

                            }
                        }

                        if (DrawNodeNeighbours)
                        {
                            pos = Chunks[x, y].PathfindingNodes[x1, y1].worldPos;
                            for (int i=0;i< Chunks[x, y].PathfindingNodes[x1, y1].neighbours.Count; i++) {
                                Debug.DrawLine(pos, Chunks[x, y].PathfindingNodes[x1, y1].neighbours[i].worldPos);
                            }
                        }
                        Debug.DrawLine(Chunks[x, y].PathfindingNodes[0, 0].worldPos, Chunks[x, y].PathfindingNodes[1, 1].worldPos, Color.magenta);
                    }
                }

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
