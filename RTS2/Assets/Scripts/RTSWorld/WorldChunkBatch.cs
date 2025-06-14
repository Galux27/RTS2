using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunkBatch : MonoBehaviour
{
    
    public Vector2Int coords;
    public WorldChunk[,] Chunks;
    public bool IsActive = false;
    Vector2Int UpperBound = new Vector2Int();


    public static WorldChunkBatch CreateWorldChunkBatch(Vector2Int coords)
    {
        GameObject g = new GameObject();
        g.name = "World Chunk Batch" + coords.ToString();
        WorldChunkBatch wcb = g.AddComponent<WorldChunkBatch>();
        wcb.SetCoords( coords);
        return wcb;

    }

    public void SetCoords(Vector2Int coords)
    {
        this.coords = coords;
        UpperBound = coords + new Vector2Int(WorldChunkManager.ChunksPerBatch * WorldChunkManager.ChunkSize, WorldChunkManager.ChunksPerBatch * WorldChunkManager.ChunkSize);
    }


    public bool IsPointInChunk(int x,int y)
    {
        if (x >= coords.x && y >= coords.y && x < UpperBound.x && y < UpperBound.y) { return true; }
        return false;
    }


    public void InitWorldChunks()
    {
        Chunks = new WorldChunk[ WorldChunkManager.ChunksPerBatch, WorldChunkManager.ChunksPerBatch];

        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y] = new WorldChunk(coords.x+(x* WorldChunkManager.ChunkSize),coords.y+ (y *WorldChunkManager.ChunkSize));
            }
        }

        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].InitPathfindingNodes();
            }
        }

        LinkBatchToOtherBatches();
    }
    public bool IsRendered = false;
    public void RenderChunk()
    {
        if (IsRendered)
        {
            return;
        }
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                WorldRenderer.Instance.RenderWorld(Chunks[x, y].ChunkTiles);
            }
        }
        IsRendered = true;
        
    }

    public void LoadChunksFromFile(string name)
    {
        EasyStopwatch.StartStopwatch();
        List<string> dataFromFile = SerializationHelpers.ReadFile(SerializationHelpers.GetWorldFilePath(name));
        //  string[] splitObjects = null;
        for (int q = 0; q < dataFromFile.Count; q++)
        {
            WorldChunk wc = DataReaders.ParseWorldChunk(dataFromFile[q]);
            int x = wc.WorldCoords.x / WorldChunkManager.ChunkSize; int y = wc.WorldCoords.y / WorldChunkManager.ChunkSize;
            Chunks[x, y] = wc;

        }
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].InitPathfindingNodes();

                for (int x1 = 0; x1 < Chunks[x, y].ChunkTiles.GetLength(0); x1++)
                {
                    for (int y1 = 0; y1 < Chunks[x, y].ChunkTiles.GetLength(0); y1++)
                    {
                        Chunks[x, y].ChunkTiles[x1, y1].UpdateWaterLevel(Chunks[x, y].ChunkTiles[x1, y1].WaterData.WaterLevel);
                        if (Chunks[x, y].WallSegments[x1, y1].WallType == WallType.Door)
                        {
                            Vector2Int coords = new Vector2Int(Chunks[x, y].WallSegments[x1, y1].x, Chunks[x, y].WallSegments[x1, y1].y);
                            //WorldController.Instance.WallManager.SetDoor(coords.x, coords.y,WorldController.Instance.BuildingTilemap, Chunks[x, y].WallSegments[x1, y1].baseWallType);
                            Chunks[x, y].WallSegments[x1, y1].DestroyWall();
                            WallHelpers.CreateDoorObject(coords.x, coords.y,
                                WorldController.Instance.BuildingTilemap, Chunks[x, y].WallSegments[x1, y1].baseWallType);
                        }
                    }
                }


                for (int q = 0; q < Chunks[x, y].EnvironmentObjectsInChunk.Count; q++)
                {
                    WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(Chunks[x, y].EnvironmentObjectsInChunk[q], !EnvironmentObjectHelpers.GetEnvironmentObject(Chunks[x, y].EnvironmentObjectsInChunk[q].Name()).BlocksTile);
                }

            }
        }

        Debug.Log("reading took " + EasyStopwatch.GetStopwatchElapsedTime() + "s");
    }


    public void OnBuildableFinished(BuildableStructure bs)
    {
        Vector2Int coords = GetChunkCoordsFromWorldPos(bs.GetPosition());
        if (!IsPointInChunk(coords.x, coords.y))
        {
            return;
        }
        Chunks[coords.x, coords.y].RemoveConstructable(bs);
    }


    Vector2Int getCoordsCache = new Vector2Int();
    public Vector2Int GetChunkCoordsFromWorldPos(Vector3 worldPos)
    {
        return GetChunkCoordsFromTileCoords(new Vector2Int((int)worldPos.x,(int) worldPos.y));
    }

    public Vector2Int GetChunkCoordsFromTileCoords(Vector2Int coords,bool debug=false)
    {
        int topRightX = Chunks[Chunks.GetLength(0) - 1, Chunks.GetLength(1) - 1].X+WorldChunkManager.ChunkSize;
        int topRightY = Chunks[Chunks.GetLength(0) - 1, Chunks.GetLength(1) - 1].Y + WorldChunkManager.ChunkSize;
        float xLerp = Mathf.InverseLerp(this.coords.x, topRightX, coords.x);
        float yLerp = Mathf.InverseLerp(this.coords.y, topRightY, coords.y);
        //


        getCoordsCache.x = Mathf.FloorToInt (Mathf.Lerp(0,Chunks.GetLength(0),xLerp));//Mathf.Min(coords.x / WorldChunkManager.ChunkSize, Chunks.GetLength(0) - 1);
        getCoordsCache.y = Mathf.FloorToInt(Mathf.Lerp(0, Chunks.GetLength(1), yLerp)); //Mathf.Min(coords.y / WorldChunkManager.ChunkSize, Chunks.GetLength(1) - 1);
        if (debug)
        {
            Debug.Log("Getting chunk coords from " + coords + " returning " + getCoordsCache+"TR" + topRightX+","+topRightY+" my coords "+   this.coords);
        }
            ValidateCoordsCache();
        return getCoordsCache;
    }

    void LinkToOtherBatch(Vector2Int coordsToCheck)
    {
        WorldChunkBatch neighbour = WorldChunkManager.Instance.ChunkBatches[coordsToCheck];
        if (coords.x < coordsToCheck.x)
        {
            //link left of mine to right of theres
            int myX = WorldChunkManager.ChunksPerBatch - 1;
            int theirX = 0;
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                Chunks[myX, y].GenerateOtherChunkLinks(coords, coordsToCheck, myX, y);
            }
        }
        else if (coords.x > coordsToCheck.x)
        {
            //link right of mine to left of theres
            int myX = 0;
            int theirX = WorldChunkManager.ChunksPerBatch - 1;
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                Chunks[myX, y].GenerateOtherChunkLinks(coords, coordsToCheck, myX, y);
            }
        }

        if (coords.y < coordsToCheck.y)
        {
            //top of mine to bottom of theres
            int myY = WorldChunkManager.ChunksPerBatch - 1;
            int theirY = 0;
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                Chunks[x, myY].GenerateOtherChunkLinks(coords, coordsToCheck, x, myY);

            }
        }
        else if (coords.y > coordsToCheck.y)
        {
            //bottom of mine to top of theres,
            int myY = 0;
            int theirY = WorldChunkManager.ChunksPerBatch - 1;
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                Chunks[x, myY].GenerateOtherChunkLinks(coords, coordsToCheck, x, myY);
            }
        }
    }


    public void LinkBatchToOtherBatches()
    {
        Vector2Int coordsToCheck = coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
        if(WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            LinkToOtherBatch(coordsToCheck);
        }

        coordsToCheck = coords + new Vector2Int(-WorldChunkManager.ChunkBatchSize, 0);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            LinkToOtherBatch(coordsToCheck);
        }

        coordsToCheck = coords + new Vector2Int(0, -WorldChunkManager.ChunkBatchSize);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            LinkToOtherBatch(coordsToCheck);
        }

        coordsToCheck = coords + new Vector2Int( 0, WorldChunkManager.ChunkBatchSize);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            LinkToOtherBatch(coordsToCheck);
        }

    }


    void ValidateCoordsCache()
    {
        if (getCoordsCache.x < 0)
        {
            getCoordsCache.x = 0;
        }
        else if (getCoordsCache.x > Chunks.GetLength(0) - 1)
        {
            getCoordsCache.x = Chunks.GetLength(0) - 1;
        }

        if (getCoordsCache.y < 0)
        {
            getCoordsCache.y = 0;
        }
        else if (getCoordsCache.y > Chunks.GetLength(1) - 1)
        {
            getCoordsCache.y = Chunks.GetLength(1) - 1;
        }
    }

    public List<WorldChunk> GetChunksInRadius(float radius, Vector3 searchCenter)
    {
        List<WorldChunk> retVal = new List<WorldChunk>();
        GetChunkCoordsFromWorldPos(searchCenter);
        int chunkRadius = Mathf.Max(Mathf.RoundToInt(radius / WorldChunkManager.ChunkSize), 1);

        for (int x = getCoordsCache.x - chunkRadius; x < getCoordsCache.x + chunkRadius; x++)
        {
            for (int y = getCoordsCache.y - chunkRadius; y < getCoordsCache.y + chunkRadius; y++)
            {
                if (CoordsValid(x, y))
                {
                    retVal.Add(Chunks[x, y]);
                }
            }
        }

        return retVal;
    }

    const bool DrawNodeWalkable = false, DrawNodeNeighbours = false;
    public void DebugDrawChunks()
    {

        Vector3 tl = new Vector3(0, WorldChunkManager.ChunkSize, 0f);
        Vector3 tr = new Vector3(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize, 0f);
        Vector3 bl = new Vector3(0, 0, 0f);
        Vector3 br = new Vector3(WorldChunkManager.ChunkSize, 0, 0f);
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Vector3 Center = new Vector3(x * WorldChunkManager.ChunkSize, y * WorldChunkManager.ChunkSize, 0);
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

                Debug.DrawLine(Center + tl, Center + tr, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + tr, Center + br, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + br, Center + bl, Chunks[x, y].DebugColor);
                Debug.DrawLine(Center + tl, Center + bl, Chunks[x, y].DebugColor);


                Vector3 pos = Vector3.zero;
                for (int x1 = 0; x1 < Chunks[x, y].PathfindingNodes.GetLength(0); x1++)
                {
                    for (int y1 = 0; y1 < Chunks[x, y].PathfindingNodes.GetLength(1); y1++)
                    {
                        if (DrawNodeWalkable)
                        {
                            pos = Chunks[x, y].PathfindingNodes[x1, y1].worldPos;
                            if (Chunks[x, y].PathfindingNodes[x1, y1].IsPassable)
                            {
                                Debug.DrawLine(pos, pos + (Vector3.up * (x1 + y1) / 32f), Color.green);
                            }
                            else
                            {
                                Debug.DrawLine(pos, pos + (Vector3.up * (x1 + y1) / 32f), Color.red);

                            }
                        }

                        if (DrawNodeNeighbours)
                        {
                            pos = Chunks[x, y].PathfindingNodes[x1, y1].worldPos;
                            for (int i = 0; i < Chunks[x, y].PathfindingNodes[x1, y1].neighbours.Count; i++)
                            {
                                Debug.DrawLine(pos, Chunks[x, y].PathfindingNodes[x1, y1].neighbours[i].worldPos);
                            }
                        }
                        Debug.DrawLine(Chunks[x, y].PathfindingNodes[0, 0].worldPos, Chunks[x, y].PathfindingNodes[1, 1].worldPos, Color.magenta);
                    }
                }

            }
        }

    }

    public bool CoordsValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Chunks.GetLength(0) && y < Chunks.GetLength(1);
    }
    public void OnUnitCreated(Unit u)
    {
        GetChunkCoordsFromWorldPos(u.transform.position);
        Chunks[getCoordsCache.x, getCoordsCache.y].AddUnitToChunk(u);
        u.UpdateChunk(getCoordsCache);
    }

    public void OnUnitMove(Unit u)
    {
        GetChunkCoordsFromWorldPos(u.transform.position);

        if (u.MyCurrentChunk != getCoordsCache)
        {
            try
            {
                Chunks[u.MyCurrentChunk.x, u.MyCurrentChunk.y].RemoveUnitFromChunk(u);
                Chunks[getCoordsCache.x, getCoordsCache.y].AddUnitToChunk(u);
                u.UpdateChunk(getCoordsCache);
            }
            catch
            {
                Debug.LogError("Issue moving between chunks " + u.MyCurrentChunk.ToString() + " to " + getCoordsCache.ToString());
            }
        }


    }

    public void OnUnitDeath(Unit u)
    {
        Chunks[u.MyCurrentChunk.x, u.MyCurrentChunk.y].RemoveUnitFromChunk(u);
    }

    public void AddEnvironmentObject(EnvironmentObjectInstance instance,Vector3 pos)
    {
        Vector2Int chunk = GetChunkCoordsFromTileCoords(instance.coords);

        Chunks[chunk.x, chunk.y].AddEnvironmentObject(instance);

    }

    public void AddContainerObject(Inventory toAdd)
    {
        Vector2Int chunk = GetChunkCoordsFromWorldPos(toAdd.transform.position);
        Chunks[chunk.x, chunk.y].AddContainerObject(toAdd);
    }
    public void RemoveContainerObject(Inventory toRemove)
    {
        Vector2Int chunk = GetChunkCoordsFromWorldPos(toRemove.transform.position);
        Chunks[chunk.x, chunk.y].RemoveContainerObject(toRemove);
    }

    public void AddResourceObject(ResourceInstance res)
    {
        Vector2Int chunk = GetChunkCoordsFromWorldPos(res.transform.position);
        Chunks[chunk.x, chunk.y].AddResourceObject(res);
    }

    public void RemoveResourceObject(ResourceInstance res)
    {
        Vector2Int chunk = GetChunkCoordsFromWorldPos(res.transform.position);
        Chunks[chunk.x, chunk.y].RemoveResourceObject(res);
    }

    public void AddConstructable(Constructable bs)
    {
        Vector2Int coords = GetChunkCoordsFromWorldPos(bs.GetPosition());
        Chunks[coords.x, coords.y].AddConstructable(bs);
    }


    public void RemoveConstructable(Constructable bs, bool needsCleanup = true)
    {
        Vector2Int coords = GetChunkCoordsFromWorldPos(bs.GetPosition());
        Chunks[coords.x, coords.y].RemoveConstructable(bs,needsCleanup);
    }

}
