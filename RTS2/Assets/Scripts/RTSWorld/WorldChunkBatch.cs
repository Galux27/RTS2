using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public class WorldChunkBatch : MonoBehaviour
{

    public Vector2Int coords;
    public WorldChunk[,] Chunks;
    public bool IsActive = false;
    Vector2Int UpperBound = new Vector2Int();
    public bool NeedsGeneration = true;
    public Vector2Int OverworldCoords = new Vector2Int();
    public List<WorldTileBlend> BlendList = new List<WorldTileBlend>();
    public List<RoadData> Roads = new List<RoadData>();
    public List<RiverData> Rivers = new List<RiverData>();
    public WorldChunkBatchUnits UnitsInBatch;


    

    public static WorldChunkBatch CreateWorldChunkBatch(Vector2Int coords, Vector2Int overworld)
    {

        WorldChunkBatch wcb = WorldChunkBatchPool.GetChunkBatch();
        wcb.SetCoords(coords, overworld);

        return wcb;

    }

    public Vector2Int Center()
    {
        return coords + new Vector2Int(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2);
    }

    public void OnBatchCreated()
    {
        Chunks = new WorldChunk[WorldChunkManager.ChunksPerBatch, WorldChunkManager.ChunksPerBatch];
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y] = new WorldChunk(0, 0, 0, 0, Vector2Int.zero);  
            }
        }
    }

    public void ApplyOverworldHeight(float height)
    {
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                for(int x1=0;x1< Chunks[x, y].ChunkTiles.GetLength(0); x1++)
                {
                    for (int y1 = 0; y1 < Chunks[x, y].ChunkTiles.GetLength(1); y1++)
                    {
                        Chunks[x, y].ChunkTiles[x1, y1].SetElevation(height,false);
                    }
                }
                 
            }
        }
        UpdateElevations();
    }

    public void UpdateElevations()
    {
        BlendEdgeElevations();

        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].UpdateElevationType(this);
            }
        }

        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                for (int x1 = 0; x1 < Chunks[x, y].ChunkTiles.GetLength(0); x1++)
                {
                    for (int y1 = 0; y1 < Chunks[x, y].ChunkTiles.GetLength(1); y1++)
                    {
                        Chunks[x, y].PathfindingNodes[x1, y1].UpdatePassable(Chunks[x, y].ChunkTiles[x1, y1].TileTraversable());
                    }
                }
            }
        }
    }

    public void GenerateWorldTileBlends()
    {
        WorldTileBlending.OnWorldChunkBatchGenerated(this);
    }

    public void AddRoad(RoadData road)
    {
        Roads.Add(road);
        //road.GenerateRoad();
    }

    public void AddRiver(RiverData river)
    {
        Rivers.Add(river);
    }

    public void GenerateRoadBlends(RoadType toGen)
    {
        //List<BatchRoad> toGenerateBlendFor = new List<BatchRoad>();
        //for (int x = 0; x < Roads.Count; x++)
        //{
        //    if (Roads[x].Type == toGen)
        //    {
        //        toGenerateBlendFor.Add(Roads[x]);
        //    }
        //}
        //if (toGenerateBlendFor.Count > 1)
        //{
        //    AddRoad(new BatchRoadBlend(toGen, Center(), toGenerateBlendFor[0].RoadEnd, toGenerateBlendFor[0].Width,toGenerateBlendFor));
        //}
    }

    public RoadIntersection GetFirstRoadIntersection(RoadData checking,RoadData toIgnore)
    {
        RoadIntersection intersection = null,currentInt=null;
        float dist = 9999999f,dist2=0f;
        for (int x = 0; x < Roads.Count; x++)
        {
            if (Roads[x] == toIgnore)
            {
                continue;
            }
            currentInt = Roads[x].DoesRoadIntersect(checking);
            if (currentInt!=null&&currentInt.RoadPoints.Count>0)
            {
                if (intersection == null)
                {
                    intersection = currentInt;
                    dist = Vector2.Distance(currentInt.GetFirstPoint(), checking.StartPos);
                }
                else
                {
                    dist2 = Vector2.Distance(currentInt.GetFirstPoint(), checking.StartPos);
                    if (dist2 < dist)
                    {
                        dist = dist2;
                        intersection = currentInt;
                        
                    }
                    currentInt = null;
                    dist2 = 0;
                }
            }
        }

        return intersection;
    }
    public List<BuildingZone> Zones = new List<BuildingZone>();
    public void GenerateBuildings()
    {
        for(int x = 0; x < Zones.Count; x++)
        {
            for(int y = 0; y < Zones[x].Buildings.Count; y++)
            {
                BuildingGenerator.Instance.GenerateBuilding(Zones[x].Buildings[y]);
            }
        }
    }

    public void GenerateRoads(RoadType toGen)
    {
        //for(int x = 0; x < Roads.Count; x++)
        //{
        //    if (Roads[x].type == toGen)
        //    {
        //        Roads[x].GenerateRoad();
        //    }
        //}

        for (int x = 0; x < Roads.Count; x++)
        {
            if (Roads[x].Type == toGen)
            {
                RoadGenerator.GenerateRoad(Roads[x],ref Roads);
               // Roads[x].RenderRoad(this);
               // Roads[x].LogCount();
            }
        }
          //  RefreshElevationTiles();
        
    }

    public void AddWorldBlend(WorldTileBlend blend)
    {
        BlendList.Add(blend);
    }

    public void SetChunksLoaded()
    {
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].HasChunkFinishedLoading = true;
            }
        }

    }

    public void SetCoords(Vector2Int coords,Vector2Int overworld)
    {
        this.OverworldCoords = overworld;
        this.coords = coords;
        UpperBound = coords + new Vector2Int(WorldChunkManager.ChunksPerBatch * WorldChunkManager.ChunkSize, WorldChunkManager.ChunksPerBatch * WorldChunkManager.ChunkSize);
        gameObject.name = "World Chunk Batch" + coords.ToString();
        this.transform.position = new Vector3(coords.x, coords.y);
    }





    public bool IsPointInChunk(int x,int y)
    {
        if (x >= coords.x && y >= coords.y && x < UpperBound.x && y < UpperBound.y) { return true; }
        return false;
    }


    public void InitWorldChunks()
    {
        if (Chunks == null)
        {
            Chunks = new WorldChunk[WorldChunkManager.ChunksPerBatch, WorldChunkManager.ChunksPerBatch];
        }
        if (WorldChunkManager.Instance.ExistingChunkData == null)
        {
            WorldChunkManager.Instance.ExistingChunkData = new Dictionary<Vector2Int, string>();
        }
        Debug.Log("World Load: loading chunk " + coords + " working copy " + WorldChunkManager.Instance.DoesChunkExistInWorkingCopy(coords) + " in save " + WorldChunkManager.Instance.DoesChunkExist(coords) + " count " + WorldChunkManager.Instance.ExistingChunkData.Count);
        if (WorldChunkManager.Instance.DoesChunkExistInWorkingCopy(coords))
        {
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    if (Chunks[x, y] == null)
                    {
                        Chunks[x, y]= new WorldChunk(coords.x + (x * WorldChunkManager.ChunkSize), coords.y + (y * WorldChunkManager.ChunkSize), x, y, coords);

                    }
                    else
                    {
                        Chunks[x, y].Init(coords.x + (x * WorldChunkManager.ChunkSize), coords.y + (y * WorldChunkManager.ChunkSize), x, y, coords);

                    }

                }
            }

                for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].InitPathfindingNodes();
                }
            }
            
            LoadFromWorkingCopy();
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].LinkNodesToAdjacentChunksInBatch(this);
                }
            }
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].UpdateTileWalkable() ;
                }
            }
        }
        else if (WorldChunkManager.Instance.DoesChunkExist(coords))
        {
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    if (Chunks[x, y] == null)
                    {
                        Chunks[x, y] = new WorldChunk(coords.x + (x * WorldChunkManager.ChunkSize), coords.y + (y * WorldChunkManager.ChunkSize), x, y, coords);
                    }
                    else
                    {
                        Chunks[x, y].Init(coords.x + (x * WorldChunkManager.ChunkSize), coords.y + (y * WorldChunkManager.ChunkSize), x, y, coords);

                    }
                    }
                }

            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].InitPathfindingNodes();
                }
            }
           
            LoadChunksFromFile(SaveLoadHelpers.SaveToLoad);
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].LinkNodesToAdjacentChunksInBatch(this);
                }
            }
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].UpdateTileWalkable();
                }
            }
        }
        else
        {
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    if (Chunks[x, y] == null)
                    {
                        Chunks[x, y] = new WorldChunk(coords.x + (x * WorldChunkManager.ChunkSize), coords.y + (y * WorldChunkManager.ChunkSize), x, y, this.coords);
                    }
                    else
                    {
                        Chunks[x, y].Init(coords.x + (x * WorldChunkManager.ChunkSize), coords.y + (y * WorldChunkManager.ChunkSize), x, y, this.coords);

                    }
                }
                }

            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].InitPathfindingNodes();
                }
            }
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    Chunks[x, y].LinkNodesToAdjacentChunksInBatch(this);
                }
            }
        }



        //  WorldChunkManager.Instance.LoadChunkBatchUnits(coords);
        GameLifeManager.Instance.OnChunkBatchGenerated(this);
        LinkBatchToOtherBatches();
    }

    public void GeneratePathfindingGroups()
    {

        for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                Chunks[x, y].GeneratePathfindingGroups();
            }
        }


        HashSet<PathfindingNode> allChunk = new HashSet<PathfindingNode>();
        for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                for (int x1 = 0; x1 < WorldChunkManager.ChunkSize; x1++)
                {
                    for (int y1 = 0; y1 < WorldChunkManager.ChunkSize; y1++)
                    {
                        allChunk.Add(Chunks[x, y].PathfindingNodes[x1, y1]);
                    }
                }
            }
        }
       
        for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                Chunks[x, y].MergeIds(allChunk) ;
            }
        }
        BuildPathfindingIDMap();
    }

    public void BuildPathfindingIDMap()
    {
        WorldChunk myChunk = null;
        WorldChunk opposingChunk = null;
        PathfindingNode myNode = null, opposingNode = null;
        Vector2Int neighbouringChunkCoords = this.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
        
        if (WorldChunkManager.Instance.DoesBatchExist(neighbouringChunkCoords) && WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].NeedsGeneration==false)
        {
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                myChunk = Chunks[WorldChunkManager.ChunksPerBatch - 1, y];
                opposingChunk = WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].Chunks[0, y];

                for(int y1 = 0; y1 < WorldChunkManager.ChunkSize; y1++)
                {
                    myNode = myChunk.PathfindingNodes[WorldChunkManager.ChunkSize - 1, y1];
                    opposingNode = opposingChunk.PathfindingNodes[0, y1];
                    NodeIDPathing.AddPathfindingIDLink(myNode.PathNodeGroupID, 
                        opposingNode.PathNodeGroupID, 
                        this.coords, neighbouringChunkCoords,
                        new Vector2Int(myNode.X, myNode.Y),
                        new Vector2Int(opposingNode.X, opposingNode.Y));
                }
            }
        }
        neighbouringChunkCoords = this.coords - new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
        if (WorldChunkManager.Instance.DoesBatchExist(neighbouringChunkCoords) && WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].NeedsGeneration == false)
        {
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                myChunk = Chunks[0, y];
                opposingChunk = WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].Chunks[WorldChunkManager.ChunksPerBatch - 1, y];

                for (int y1 = 0; y1 < WorldChunkManager.ChunkSize; y1++)
                {
                    myNode = myChunk.PathfindingNodes[0, y1];
                    opposingNode = opposingChunk.PathfindingNodes[WorldChunkManager.ChunkSize - 1, y1];
                    NodeIDPathing.AddPathfindingIDLink(myNode.PathNodeGroupID,
                        opposingNode.PathNodeGroupID,
                        this.coords, 
                        neighbouringChunkCoords,
                        new Vector2Int(myNode.X,myNode.Y),
                        new Vector2Int(opposingNode.X,opposingNode.Y));
                }
            }
        }

        neighbouringChunkCoords = this.coords + new Vector2Int( 0, WorldChunkManager.ChunkBatchSize);
        if (WorldChunkManager.Instance.DoesBatchExist(neighbouringChunkCoords) && WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].NeedsGeneration == false)
        {
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                myChunk = Chunks[x,WorldChunkManager.ChunksPerBatch - 1];
                opposingChunk = WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].Chunks[x, 0];

                for (int x1 = 0; x1 < WorldChunkManager.ChunkSize; x1++)
                {
                    myNode = myChunk.PathfindingNodes[ x1,WorldChunkManager.ChunkSize - 1];
                    opposingNode = opposingChunk.PathfindingNodes[x1, 0];
                    NodeIDPathing.AddPathfindingIDLink(myNode.PathNodeGroupID, 
                        opposingNode.PathNodeGroupID, 
                        this.coords, neighbouringChunkCoords,
                        new Vector2Int(myNode.X, myNode.Y),
                        new Vector2Int(opposingNode.X, opposingNode.Y));
                }
            }
        }
        neighbouringChunkCoords = this.coords - new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
        if (WorldChunkManager.Instance.DoesBatchExist(neighbouringChunkCoords) && WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].NeedsGeneration == false)
        {
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                myChunk = Chunks[x, 0];
                opposingChunk = WorldChunkManager.Instance.ChunkBatches[neighbouringChunkCoords].Chunks[x, WorldChunkManager.ChunksPerBatch - 1];

                for (int x1 = 0; x1 < WorldChunkManager.ChunkSize; x1++)
                {
                    myNode = myChunk.PathfindingNodes[x1, 0];
                    opposingNode = opposingChunk.PathfindingNodes[x1, WorldChunkManager.ChunkSize - 1];
                    NodeIDPathing.AddPathfindingIDLink(myNode.PathNodeGroupID, 
                        opposingNode.PathNodeGroupID, 
                        this.coords, neighbouringChunkCoords
                        ,
                        new Vector2Int(myNode.X, myNode.Y),
                        new Vector2Int(opposingNode.X, opposingNode.Y));
                }
            }
        }
    }

    public bool IsRendered = false;
    public int RenderCount = 0;
    public bool RenderChunk()
    {
        if (IsRendered)
        {
            return false;
        }
        RenderCount = 0;
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                if (Chunks[x, y].CheckIfChunkNeedsToRender() 
                    || Chunks[x,y].DoesChunkNeedRefresh())
                {

                    WorldRenderer.Instance.RenderChunk(Chunks[x, y].ChunkTiles);
                    Chunks[x, y].RenderEnvironmentObjects();
                    Chunks[x, y].NeedsToRender = false;
                    Chunks[x, y].RefreshWalls();
                    Chunks[x, y].IsRendered = true;
                    Chunks[x, y].NeedsUpdate = false;
                    RenderCount++;
                }else if (Chunks[x, y].IsRendered)
                {
                    RenderCount++;
                }
            }
        }
        IsRendered = (RenderCount==Chunks.GetLength(0)*Chunks.GetLength(1));
        return true;
    }

    void BlendEdgeElevations()
    {
        OverworldTile leftTile = OverworldGenerator.Instance.GetOverworldTile(OverworldCoords - new Vector2Int(1, 0));
        OverworldTile rightTile = OverworldGenerator.Instance.GetOverworldTile(OverworldCoords + new Vector2Int(1, 0));
        OverworldTile aboveTile = OverworldGenerator.Instance.GetOverworldTile(OverworldCoords + new Vector2Int(0, 1));
        OverworldTile belowTile = OverworldGenerator.Instance.GetOverworldTile(OverworldCoords - new Vector2Int(0, 1));
        OverworldTile myTile = OverworldGenerator.Instance.GetOverworldTile(OverworldCoords);
        float leftElevation = leftTile.Elevation;
        float rightElevation = rightTile.Elevation;
        float aboveElevation = aboveTile.Elevation;
        float belowElevation = belowTile.Elevation;


        int r = Random.Range(0, 100);
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            if (r < 80)
            {
                // Chunks[x, 0].BlendHeight(belowElevation, Vector2Int.down);
                Chunks[x, Chunks.GetLength(1) - 1].BlendHeight(aboveElevation, Vector2Int.up);
            }
            r = Random.Range(0, 100);
        }

         for (int y = 0; y < Chunks.GetLength(1); y++)
        {
            if (r <80)
            {
                // Chunks[0, y].BlendHeight(leftElevation, Vector2Int.left);
                Chunks[Chunks.GetLength(0) - 1, y].BlendHeight(rightElevation, Vector2Int.right);
            }
            r = Random.Range(0, 100);
        }
    }


    public void RefreshElevationTiles()
    {
        return;
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].ClearElevationMarkers();
            }
        }

        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].UpdateElevationType(this);
            }
        }
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                WorldRenderer.Instance.RefreshElevation(Chunks[x, y].ChunkTiles);
            }
        }

    }

    public void RefreshGroundTiles()
    {
        if (!IsRendered)
        {
            return;
        }
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                WorldRenderer.Instance.RenderChunk(Chunks[x, y].ChunkTiles);
            }
        }

     }


    public void CheckForCleanup(bool forceCleaup=false)
    {
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                if (  Chunks[x,y].CanWeCleanupChunk()||forceCleaup)
                {
                    WorldRenderer.Instance.UnrenderChunk(Chunks[x, y].ChunkTiles);
                    Chunks[x, y].CleanupEnvironmentObjects();
                   Chunks[x, y].UnRenderChunk();
                    Chunks[x, y].IsRendered = false;
                    IsRendered = false;
                }
            }
        }
    }
    const float DistToUnloadChunkBatch = 750f;
    public float distToCam = 0f;
    bool InProcessOfUnloading = false;
   
    public bool IsFarEnoughAwayToUnload(Vector2Int CamPos)
    {
        if (InProcessOfUnloading)
        {
            return false;
        }
        distToCam = Vector2Int.Distance(CamPos, coords);
        return distToCam > DistToUnloadChunkBatch;
    }

    public void UnloadChunkData()
    {
        if (InProcessOfUnloading)
        {
            return;
        }
        InProcessOfUnloading = true;
        IsRendered = false;
        bool DoWeNeedToUpdateData = false;
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                if (Chunks[x, y].HasChunkBeenModified())
                {
                    DoWeNeedToUpdateData = true;
                    break;
                }
            }
        }
      //  WorldChunkManager.Instance.CanPerformCreateNewChunksCheck++;

        GameLifeManager.Instance.OnChunkBatchUnloaded(this);
        //Write chunk data to some live save place as its changed from the savegame
        if (DoWeNeedToUpdateData)
        {

            MultiThreadedManager.Instance.AddDataWritingAction(() => { SerializationHelpers.SaveChunkBatchToWorkingCopy(this); UnloadChunk(); }, () => WorldChunkManager.Instance.ChunkBatches.Remove(this.coords));
        }
        else
        {
            UnloadChunk();
        }

    }

  

    void UnloadChunk()
    {
        UnloadChunks();
        //go through chunks on the edge and remove pathfinding neighbours that 
        UnlinkBatchFromOtherBatches();
        //reset all environment objects and remove UIDs
        WorldChunkBatchPool.ReturnChunkBatch(this);
        InProcessOfUnloading = false;
       // WorldChunkManager.Instance.CanPerformCreateNewChunksCheck--;

    }

    void UnloadChunks()
    {
        for(int x = 0; x < Chunks.GetLength(0); x++)
        {
            for(int y=0;y<Chunks.GetLength(1); y++)
            {
                Chunks[x, y].UnloadChunk();
            }
        }
    }


    public void LoadFromWorkingCopy()
    {
        EasyStopwatch.StartStopwatch();
        string path = SerializationHelpers.GetWorldChunkBatchFilePathFromWorkingCopy(coords);
        Debug.Log("World Load: Loading from working copy " + path);
        List<string> dataFromFile = SerializationHelpers.ReadFile(path);
        for (int q = 0; q < dataFromFile.Count; q++)
        {
            try
            {
                WorldChunk wc = DataReaders.ParseWorldChunk(dataFromFile[q]);
                int x = wc.LocalXCoord;
                int y = wc.LocalYCoord;
                wc.SetAllChunkBatches(this.coords);

                Chunks[x, y] = wc;
            }
            catch(System.Exception e)
            {
                Debug.LogError("error parsing chunk " + dataFromFile[q]);
                Debug.LogError(e.ToString());
            }

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
                        if (Chunks[x, y].WallSegments[x1, y1].HasDoor)
                        {
                            Vector2Int coords = new Vector2Int(Chunks[x, y].WallSegments[x1, y1].x, Chunks[x, y].WallSegments[x1, y1].y);
                            Chunks[x, y].WallSegments[x1, y1].DestroyWall();
                            WallHelpers.CreateDoorObject(coords.x, coords.y,
                                WorldController.Instance.BuildingTilemap, Chunks[x, y].WallSegments[x1, y1].baseWallType);
                        }else if (Chunks[x, y].WallSegments[x1, y1].HasWall)
                        {
                            Vector2Int coords = new Vector2Int(Chunks[x, y].WallSegments[x1, y1].x, Chunks[x, y].WallSegments[x1, y1].y);
                            Chunks[x, y].WallSegments[x1, y1].DestroyWall();
                            WallHelpers.CreateWallObject(coords.x, coords.y,
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
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].UpdateElevationType(this);
            }
        }
        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].HasChunkFinishedLoading = true;
            }
        }
    }


    public void LoadChunksFromFile(string name)
    {
        EasyStopwatch.StartStopwatch();
        string path = SerializationHelpers.GetWorldChunkBatchFilePath(coords, name);
        Debug.Log("World Load: loading from path " + path);
        List<string> dataFromFile = SerializationHelpers.ReadFile(path);
        int endPointForChunkData = WorldChunkManager.ChunksPerBatch * WorldChunkManager.ChunksPerBatch;
        for (int q = 0; q < endPointForChunkData; q++)
        {
            WorldChunk wc = DataReaders.ParseWorldChunk(dataFromFile[q]);
            int x = wc.LocalXCoord; 
            int y = wc.LocalYCoord;
            wc.SetAllChunkBatches(this.coords);
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
                            Chunks[x, y].WallSegments[x1, y1].DestroyWall();
                            WallHelpers.CreateDoorObject(coords.x, coords.y,
                                WorldController.Instance.BuildingTilemap, Chunks[x, y].WallSegments[x1, y1].baseWallType);
                        }
                    }
                }


                for (int q = 0; q < Chunks[x, y].EnvironmentObjectsInChunk.Count; q++)
                {
                    WorldController.Instance.SetTilesAroundEnvrionmentObjectTraversable(Chunks[x, y].EnvironmentObjectsInChunk[q],
                        !EnvironmentObjectHelpers.GetEnvironmentObject(Chunks[x, y].EnvironmentObjectsInChunk[q].Name()).BlocksTile);
                }

            }
        }

        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].UpdateElevationType(this);
            }
        }


        for (int x = 0; x < Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < Chunks.GetLength(1); y++)
            {
                Chunks[x, y].HasChunkFinishedLoading = true;
            }
        }

        string[] RoadPoints = null;
        string[] RoadSplit = null;
        string type="";
        for(int q = endPointForChunkData; q < dataFromFile.Count; q++)
        {
            Debug.Log("Road Data Found in " + name + " was " + dataFromFile[q]);
            RoadSplit = dataFromFile[q].Split(SerializeDataHelpers.DATA_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
            type = RoadSplit[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries)[1];
            RoadType rtype = (RoadType)int.Parse(type);
            int width = int.Parse(RoadSplit[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries)[1]);
            List<RoadSegment> segments = new List<RoadSegment>();
          
                RoadPoints = RoadSplit[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(SerializeDataHelpers.DATA_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < RoadPoints.Length; x += 4)
                {
                    RoadSegment segment = new RoadSegment(new Vector2Int(int.Parse(RoadPoints[0]), int.Parse(RoadPoints[1]))
                        , new Vector2Int(int.Parse(RoadPoints[2]), int.Parse(RoadPoints[3])));
                    segments.Add(segment);
                }
            AddSerializedRoad(rtype, width, segments);

                //RDT; 1 ^ RDE; 128,384,128,384,^
            }


        }

    void AddSerializedRoad(RoadType type,int width,List<RoadSegment> segments)
    {
        switch (type)
        {
            case RoadType.None:
                break;
            case RoadType.MajorRoad:
                AddRoad(new RoadData(segments[0].Start, segments[segments.Count - 1].End, width, type));                break;
            case RoadType.MinorRoad:
                AddRoad(new RoadData(segments[0].Start, segments[segments.Count - 1].End, width, type));

                break;
            case RoadType.Backroad:
                AddRoad(new RoadData(segments[0].Start, segments[segments.Count - 1].End, width, type));

                break;
            default:
                break;
        }
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
    public WorldTile GetWorldTileFromVec2Int(Vector2Int pos)
    {
        return GetTileFromPosition(new Vector3(pos.x, pos.y));
    }

    public static Vector2Int chunkBatch, chunk, local;
    public string GetDebugOut()
    {
        return chunkBatch.ToString() + "::" + chunk.ToString() + "::" + local.ToString();
    }


    public WorldTile GetTileFromPosition(Vector3 pos)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out chunkBatch, out chunk, out local);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(chunkBatch))
        {
            return WorldChunkManager.Instance.ChunkBatches[chunkBatch].Chunks[chunk.x, chunk.y].ChunkTiles[local.x, local.y];
        }
        else
        {
            return null;
        }
        }

    Vector2Int batchCache = new Vector2Int(), chunkCache = new Vector2Int(), localCache = new Vector2Int();
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
      
            ValidateCoordsCache();
        return getCoordsCache;
    }

    void UnlinkFromOtherBatch(Vector2Int coordsToCheck)
    {
        WorldChunkBatch neighbour = WorldChunkManager.Instance.ChunkBatches[coordsToCheck];
        if (coords.x < coordsToCheck.x)
        {
            //link left of mine to right of theres
            int myX = WorldChunkManager.ChunksPerBatch - 1;
            int theirX = 0;
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                Chunks[myX, y].ManuallyRemoveNeighboursFromOtherBatches(coords, coordsToCheck, myX, y);
            }
        }
        else if (coords.x > coordsToCheck.x)
        {
            //link right of mine to left of theres
            int myX = 0;
            int theirX = WorldChunkManager.ChunksPerBatch - 1;
            for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
            {
                Chunks[myX, y].ManuallyRemoveNeighboursFromOtherBatches(coords, coordsToCheck, myX, y);
            }
        }

        if (coords.y < coordsToCheck.y)
        {
            //top of mine to bottom of theres
            int myY = WorldChunkManager.ChunksPerBatch - 1;
            int theirY = 0;
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                Chunks[x, myY].ManuallyRemoveNeighboursFromOtherBatches(coords, coordsToCheck, x, myY);

            }
        }
        else if (coords.y > coordsToCheck.y)
        {
            //bottom of mine to top of theres,
            int myY = 0;
            int theirY = WorldChunkManager.ChunksPerBatch - 1;
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                Chunks[x, myY].ManuallyRemoveNeighboursFromOtherBatches(coords, coordsToCheck, x, myY);
            }
        }
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
    public void UnlinkBatchFromOtherBatches()
    {
        Vector2Int coordsToCheck = coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            UnlinkFromOtherBatch(coordsToCheck);
        }

        coordsToCheck = coords + new Vector2Int(-WorldChunkManager.ChunkBatchSize, 0);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            UnlinkFromOtherBatch(coordsToCheck);
        }

        coordsToCheck = coords + new Vector2Int(0, -WorldChunkManager.ChunkBatchSize);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            UnlinkFromOtherBatch(coordsToCheck);
        }

        coordsToCheck = coords + new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coordsToCheck))
        {
            UnlinkFromOtherBatch(coordsToCheck);
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

    static List<WorldChunk> GetChunksCache = new List<WorldChunk>();
    public List<WorldChunk> GetChunksInRadius(float radius, Vector3 searchCenter)
    {
        GetChunksCache.Clear();
        GetChunkCoordsFromWorldPos(searchCenter);
        int chunkRadius = Mathf.Max(Mathf.RoundToInt(radius / WorldChunkManager.ChunkSize), 1);

        for (int x = getCoordsCache.x - chunkRadius; x < getCoordsCache.x + chunkRadius; x++)
        {
            for (int y = getCoordsCache.y - chunkRadius; y < getCoordsCache.y + chunkRadius; y++)
            {
                if (CoordsValid(x, y))
                {
                    GetChunksCache.Add(Chunks[x, y]);
                }
            }
        }

        return GetChunksCache;
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
                                Debug.DrawLine(pos, Chunks[x, y].PathfindingNodes[x1, y1].neighbours[i].Node.worldPos);
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
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(u.transform.position.x, u.transform.position.y, out batchCache, out chunkCache, out localCache);
        Chunks[chunkCache.x, chunkCache.y].AddUnitToChunk(u);
        u.UpdateChunk(Chunks[chunkCache.x, chunkCache.y]);
    }

    public void OnUnitMove(Unit u)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(u.transform.position.x, u.transform.position.y, out batchCache, out chunkCache, out localCache);
        u.UpdateChunk(Chunks[chunkCache.x, chunkCache.y]);
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


public class WorldChunkBatchUnits
{
    public List<Unit> UnitsInBatch;
    public WorldChunkBatchUnits()
    {
        UnitsInBatch = new List<Unit>();
    }

    public void AddUnitToBatch(Unit unit)
    {
        if(!UnitsInBatch.Contains(unit))
        {
            UnitsInBatch.Add(unit);
        }
    }
}