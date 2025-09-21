using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Class to store data of objects within a given area (units, props, items etc...)
/// </summary>
[System.Serializable]
public class WorldChunk:ISerialize
{
    public List<Unit> UnitsInChunk=new List<Unit>();
    public List<EnvironmentObjectInstance> EnvironmentObjectsInChunk = new List<EnvironmentObjectInstance>();
    public List<ResourceInstance> ResourceObjectsInChunk = new List<ResourceInstance>();
    public List<Inventory> StaticContainersInChunk = new List<Inventory>();
    public List<Constructable> ToBuild=new List<Constructable>();
    public Color DebugColor;
    public int X, Y,LocalXCoord,LocalYCoord;

    public WallSegment[,] WallSegments;
    public PathfindingNode[,] PathfindingNodes;
    public WorldTile[,] ChunkTiles;
    public Vector2Int WorldCoords;
    public bool NeedsToRender = false,IsRendered=false;
    const float CameraRenderDistance = 16*3;
    public Dictionary<WorldTileBlendType, WorldTileBlendCoordDataStore> TileBlends;

    public void AddTileBlends(Vector2Int dir,Vector2Int coords,WorldTileBlendType type,int val,bool isHorizontal)
    {
        if (TileBlends == null)
        {
            TileBlends = new Dictionary<WorldTileBlendType, WorldTileBlendCoordDataStore>();
        }
        if (!TileBlends.ContainsKey(type))
        {
            TileBlends.Add(type, new WorldTileBlendCoordDataStore(type));
        }
        TileBlends[type].AddBlend( dir, val,coords,isHorizontal);
    }



    public bool CheckIfChunkNeedsToRender()
    {
        Vector3 cameraPos = CameraController.Instance.transform.position;
        float dist = Vector2.Distance(new Vector2(X + (WorldChunkManager.ChunkSize / 2),
            Y + (WorldChunkManager.ChunkSize / 2)), new Vector2(cameraPos.x, cameraPos.y));
        return NeedsToRender || dist<CameraRenderDistance && IsRendered==false;
    }

    public bool CanWeCleanupChunk()
    {
        Vector3 cameraPos = CameraController.Instance.transform.position;
        float dist = Vector2.Distance(new Vector2(X + (WorldChunkManager.ChunkSize / 2), Y + (WorldChunkManager.ChunkSize / 2)), new Vector2(cameraPos.x, cameraPos.y));

        return dist > CameraRenderDistance*2f && IsRendered;
    }


    public bool CoordsValid(int x,int y)
    {
        if(x<0 || y<0) return false;
        if (x >= WorldChunkManager.ChunkSize || y >= WorldChunkManager.ChunkSize) return false;
        return true;
    }


    public WorldChunk(int x,int y,int localX,int localY)
    {
        HasChunkFinishedLoading = false;
        DebugColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f,1f),1f);
        X = x;
        Y = y;
        LocalXCoord=localX;
        LocalYCoord=localY;
        WorldCoords=new Vector2Int(x, y);
        GenerateTilesForChunk();
        GenerateWallsForChunk();
        GeneratePathfindingNodes();
    }

    public void ClearElevationMarkers()
    {
        for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                ChunkTiles[x, y].Elevation.IsCorner=false;
                ChunkTiles[x, y].Elevation.IsEdge = false;
                ChunkTiles[x, y].Elevation.DirectionsForEdge.Clear();
            }
        }
    }


    public void UpdateElevationType(WorldChunkBatch myBatch)
    {
        for(int x = 0; x < WorldChunkManager.ChunkSize; x++)
        {
            for(int y=0;y < WorldChunkManager.ChunkSize; y++)
            {
                ChunkTiles[x, y].Elevation.WorkOutStartingEdges(this,x,y,myBatch);
            }
        }

        for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                ChunkTiles[x, y].Elevation.WorkOutCorners(this, x, y, myBatch);
            }
        }

        for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                ChunkTiles[x, y].Elevation.FinalBlend(this, x, y, myBatch);
            }
        }
    }

    public void UpdateTile(int x, int y, string type)
    {
        ChunkTiles[x, y].UpdateTileType(type);
    }

    public void UpdateWaterLevel(int x, int y,float val)
    {
        ChunkTiles[x, y].UpdateWaterLevel(val);
    }

    void GenerateTilesForChunk()
    {
        ChunkTiles = new WorldTile[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        xStart = X;
        yStart = Y;
        localx = 0;
        localy = 0;


        for (int x = xStart; x < xStart + WorldChunkManager.ChunkSize; x++)
        {

            for (int y = yStart; y < yStart + WorldChunkManager.ChunkSize; y++)
            {
                ChunkTiles[localx, localy] = new WorldTile(x,y);
                localy++;
            }
            localx++;
            localy = 0;
        }
    }
    static int xStart, yStart,localx,localy;
    void GeneratePathfindingNodes()
    {
        PathfindingNodes=new PathfindingNode[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        xStart = X;
        yStart = Y;
        localx = 0;
        localy = 0;
        for (int x = xStart; x < xStart + WorldChunkManager.ChunkSize; x++)
        {

            for (int y = yStart; y < yStart + WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[localx, localy] = new PathfindingNode(x,y,true);
                localy++;
            }
            localx++;
            localy = 0;
        }
    }

    public void LinkNodesToAdjacentChunksInBatch(WorldChunkBatch batch)
    {
        Vector2Int MyCoords = new Vector2Int(LocalXCoord, LocalYCoord);
        WorldChunk checking = null;
        int myX, myY, theirX, theirY;
        if (LocalXCoord > 0)
        {
            checking = batch.Chunks[LocalXCoord-1,LocalYCoord];
            myX = 0;
            theirX = WorldChunkManager.ChunkSize - 1;
            for(int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[myX, y].ManuallyAddNeighbour(checking.PathfindingNodes[theirX, y]);
                checking.PathfindingNodes[theirX, y].ManuallyAddNeighbour(PathfindingNodes[myX, y]);
            }

        }

        if (LocalXCoord < WorldChunkManager.ChunksPerBatch - 1)
        {
            checking = batch.Chunks[LocalXCoord + 1, LocalYCoord];
            myX = WorldChunkManager.ChunkSize - 1;
            theirX = 0;
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[myX, y].ManuallyAddNeighbour(checking.PathfindingNodes[theirX, y]);
                checking.PathfindingNodes[theirX, y].ManuallyAddNeighbour(PathfindingNodes[myX, y]);
            }
        }

        if(LocalYCoord> 0)
        {
            checking = batch.Chunks[LocalXCoord , LocalYCoord - 1];
            myY = 0;
            theirY = WorldChunkManager.ChunkSize - 1;
            for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                PathfindingNodes[x, myY].ManuallyAddNeighbour(checking.PathfindingNodes[x, theirY]);
                checking.PathfindingNodes[x, theirY].ManuallyAddNeighbour(PathfindingNodes[x, myY]);
            }
        }

        if(LocalYCoord< WorldChunkManager.ChunksPerBatch - 1)
        {
            checking = batch.Chunks[LocalXCoord, LocalYCoord + 1];
            myY = WorldChunkManager.ChunkSize - 1;
            theirY = 0;
            for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                PathfindingNodes[x, myY].ManuallyAddNeighbour(checking.PathfindingNodes[x, theirY]);
                checking.PathfindingNodes[x, theirY].ManuallyAddNeighbour(PathfindingNodes[x, myY]);
            }
        }
    }


    public void ManuallyRemoveNeighboursFromOtherBatches(Vector2Int myBatchCoords,Vector2Int neighbourBatchCoords, int localX, int localY)
    {
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(neighbourBatchCoords) == false)
        {
            return;
        }
        WorldChunkBatch neighbour = WorldChunkManager.Instance.ChunkBatches[neighbourBatchCoords];
        WorldChunk editing = null;
        if (myBatchCoords.x < neighbourBatchCoords.x)
        {
            //link left of mine to right of theres
            editing = neighbour.Chunks[0, localY];
            int myX = WorldChunkManager.ChunkSize - 1;
            int theirX = 0;
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[myX, y].ManuallyRemoveNeighbour(editing.PathfindingNodes[theirX, y]);
                editing.PathfindingNodes[theirX, y].ManuallyRemoveNeighbour(PathfindingNodes[myX, y]);
            }
        }
        else if (myBatchCoords.x > neighbourBatchCoords.x)
        {
            //link right of mine to left of theres
            editing = neighbour.Chunks[WorldChunkManager.ChunkSize - 1, localY];
            int myX = 0;
            int theirX = WorldChunkManager.ChunkSize - 1;
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[myX, y].ManuallyRemoveNeighbour(editing.PathfindingNodes[theirX, y]);
                editing.PathfindingNodes[theirX, y].ManuallyRemoveNeighbour(PathfindingNodes[myX, y]);
            }
        }

        if (myBatchCoords.y < neighbourBatchCoords.y)
        {
            //top of mine to bottom of theres
            editing = neighbour.Chunks[localX, 0];
            int myY = WorldChunkManager.ChunkSize - 1;
            int theirY = 0;
            for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                PathfindingNodes[x, myY].ManuallyRemoveNeighbour(editing.PathfindingNodes[x, theirY]);
                editing.PathfindingNodes[x, theirY].ManuallyRemoveNeighbour(PathfindingNodes[x, myY]);
            }
        }
        else if (myBatchCoords.y > neighbourBatchCoords.y)
        {
            //bottom of mine to top of theres,
            editing = neighbour.Chunks[localX, WorldChunkManager.ChunksPerBatch - 1];
            int myY = 0;
            int theirY = WorldChunkManager.ChunkSize - 1;
            for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                PathfindingNodes[x, myY].ManuallyRemoveNeighbour(editing.PathfindingNodes[x, theirY]);
                editing.PathfindingNodes[x, theirY].ManuallyRemoveNeighbour(PathfindingNodes[x, myY]);
            }
        }
    }

    public void GenerateOtherChunkLinks(Vector2Int myBatchCoords, Vector2Int neighbourBatchCoords, int localX, int localY)
    {
        WorldChunkBatch neighbour = WorldChunkManager.Instance.ChunkBatches[neighbourBatchCoords];
        WorldChunk editing = null;
        if (myBatchCoords.x < neighbourBatchCoords.x)
        {
            //link left of mine to right of theres
            editing = neighbour.Chunks[0, localY];
            int myX = WorldChunkManager.ChunkSize-1;
            int theirX = 0;
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[myX, y].ManuallyAddNeighbour(editing.PathfindingNodes[theirX, y]);
                editing.PathfindingNodes[theirX, y].ManuallyAddNeighbour(PathfindingNodes[myX, y]);
            }
        }
        else if (myBatchCoords.x > neighbourBatchCoords.x)
        {
            //link right of mine to left of theres
            editing = neighbour.Chunks[WorldChunkManager.ChunkSize-1, localY];
            int myX = 0;
            int theirX = WorldChunkManager.ChunkSize - 1;
            for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[myX, y].ManuallyAddNeighbour(editing.PathfindingNodes[theirX, y]);
                editing.PathfindingNodes[theirX, y].ManuallyAddNeighbour(PathfindingNodes[myX, y]);
            }
        }

        if (myBatchCoords.y < neighbourBatchCoords.y)
        {
            //top of mine to bottom of theres
            editing = neighbour.Chunks[ localX,0];
            int myY = WorldChunkManager.ChunkSize - 1;
            int theirY = 0;
            for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                PathfindingNodes[x, myY].ManuallyAddNeighbour(editing.PathfindingNodes[x, theirY]);
                editing.PathfindingNodes[x, theirY].ManuallyAddNeighbour(PathfindingNodes[x, myY]);
            }
        }
        else if (myBatchCoords.y > neighbourBatchCoords.y)
        {
            //bottom of mine to top of theres,
            editing = neighbour.Chunks[localX, WorldChunkManager.ChunksPerBatch- 1];
            int myY = 0;
            int theirY = WorldChunkManager.ChunkSize - 1;
            for (int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                PathfindingNodes[x, myY].ManuallyAddNeighbour(editing.PathfindingNodes[x, theirY]);
                editing.PathfindingNodes[x, theirY].ManuallyAddNeighbour(PathfindingNodes[x, myY]);
            }
        }
    }
    static int xIterator=0,yIterator=0;
    public void InitPathfindingNodes()
    {
        xStart = X;
        yStart = Y;
        localx = 0;
        localy = 0;
        for (xIterator = xStart; xIterator < xStart + (WorldChunkManager.ChunkSize); xIterator++)
        {

            for (yIterator = yStart; yIterator < yStart +( WorldChunkManager.ChunkSize); yIterator++)
            {
                PathfindingNodes[localx, localy].InitData(PathfindingNodes,localx,localy);
                localy++;
            }
            
            localx++;
            localy = 0;
        }
    }

    public void UpdateTileWalkable()
    {
        for (int x = 0; x < WallSegments.GetLength(0); x++)
        {
            for (int y = 0; y < WallSegments.GetLength(1); y++)
            {
                if(WallSegments[x, y].HasWall)
                {
                    PathfindingNodes[x, y].UpdatePassable(false);
                }else if (WallSegments[x, y].HasDoor)
                {
                    PathfindingNodes[x, y].AddModifier(new PathNodeModifier_Door());
                }else if (ChunkTiles[x, y].traversable == false)
                {
                    PathfindingNodes[x, y].UpdatePassable(false);
                }
            }
        }
    }

    public void UnloadChunk()
    {
        for(int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            IDManager.CleanupUID(EnvironmentObjectsInChunk[x], EnvironmentObjectsInChunk[x].GetMyUID());
        }

        for(int x=0;x<ResourceObjectsInChunk.Count; x++)
        {
            IDManager.CleanupUID(ResourceObjectsInChunk[x], ResourceObjectsInChunk[x].GetMyUID());
        }

        for (int x = 0; x <StaticContainersInChunk.Count; x++)
        {
            IDManager.CleanupUID(StaticContainersInChunk[x], StaticContainersInChunk[x].GetMyUID());
        }

        for (int x = 0; x < ToBuild.Count; x++)
        {
            IDManager.CleanupUID(ToBuild[x], ToBuild[x].GetMyUID());
        }

        for (int x = 0; x < WallSegments.GetLength(0); x++)
        {
            for(int y = 0; y < WallSegments.GetLength(1); y++)
            {
                IDManager.CleanupUID(WallSegments[x, y], WallSegments[x, y].GetMyUID());
            }
        }
    }

    void GenerateWallsForChunk()
    {
        WallSegments = new WallSegment[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        xStart = X;
        yStart = Y;
        localx = 0;
        localy = 0;
        for(int x=xStart; x<xStart+WorldChunkManager.ChunkSize; x++)
        {
           
            for (int y = yStart; y < yStart + WorldChunkManager.ChunkSize; y++)
            {
                WallSegments[localx, localy] = new WallSegment(x, y, null,localx,localy);
                localy++;
            }
            localx++;
            localy = 0;
        }
    }

    public void RefreshWalls()
    {
        Debug.Log("Refreshing walls in chunk");
        for (int x = 0; x < WallSegments.GetLength(0); x++)
        {
            for (int y = 0; y < WallSegments.GetLength(1); y++)
            {
                if (WallSegments[x, y].HasWall)
                {
                    Debug.Log("Refreshing wall at " + x + "," + y+(WallSegments[x, y].ToDraw==null)+"|"+(WallSegments[x, y].baseWallType==null));
                    WallHelpers.CalculateTileType(ref WallSegments[x, y], WorldController.Instance.WallManager, WallSegments[x, y].baseWallType);
                    WallSegments[x, y].RenderWall();
                }
                }
            }
    }


    public void AddUnitToChunk(Unit unit)
    {
        UnitsInChunk.Add(unit);
    }

    public void RemoveUnitFromChunk(Unit unit)
    {
        UnitsInChunk.Remove(unit);
    }

    public bool HasChunkFinishedLoading = false;
    bool hasChunkBeenModified = false;
    public bool HasChunkBeenModified()
    {
        return hasChunkBeenModified;
    }

    public void AddResourceObject(ResourceInstance resourceInstance)
    {
        if (ResourceObjectsInChunk.Contains(resourceInstance))
        {
            return;
        }
        ResourceObjectsInChunk.Add(resourceInstance);
        SetModifiedIfLoaded();
    }

    void SetModifiedIfLoaded()
    {
        if (HasChunkFinishedLoading)
        {
            Debug.Log("Set chunk modifed ");
            hasChunkBeenModified = true;
        }
    }

    public void RemoveResourceObject(ResourceInstance resourceInstance)
    {
        ResourceObjectsInChunk.Remove(resourceInstance);
        SetModifiedIfLoaded();
    }


    public void AddContainerObject(Inventory container)
    {
        StaticContainersInChunk.Add(container);
        SetModifiedIfLoaded();

    }

    public void RemoveContainerObject(Inventory container)
    {
        StaticContainersInChunk.Remove(container);
        SetModifiedIfLoaded();

    }

    public void AddEnvironmentObject(EnvironmentObjectInstance environmentObject)
    {
        EnvironmentObjectsInChunk.Add(environmentObject);
        environmentObject.SetChunk(this);
        if (ShouldDrawEnvironmentObjects() && environmentObject.Drawn == false)
        {
            environmentObject.RenderInstance();
        }
        SetModifiedIfLoaded();

    }

    public void RemoveEnvironmentObject(EnvironmentObjectInstance instance)
    {
        if (instance.Drawn)
        {
            instance.CleanupInstance();
        }
        
        EnvironmentObjectsInChunk.Remove(instance);
        SetModifiedIfLoaded();

    }

    List<EnvironmentObjectInstance> GetAllObjectsAtCoords(Vector2Int coords)
    {
        List<EnvironmentObjectInstance> retVal = new List<EnvironmentObjectInstance>();

        for (int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            if (EnvironmentObjectsInChunk[x].PosX == coords.x && EnvironmentObjectsInChunk[x].PosY == coords.y)
            {
                retVal.Add( EnvironmentObjectsInChunk[x]);
            }
        }
        return retVal;
    }


    EnvironmentObjectInstance GetObjectAtCoords(Vector2Int coords)
    {
        EnvironmentObjectInstance retVal = null;
        int count = 0;
        for(int x=0;x<EnvironmentObjectsInChunk.Count;x++)
        {
            if (EnvironmentObjectsInChunk[x].PosX==coords.x && EnvironmentObjectsInChunk[x].PosY== coords.y)
            {
                count++;
               retVal= EnvironmentObjectsInChunk[x];
            }
        }
        Debug.Log("Room: found " + count + " objects at " + coords);
        return retVal;
    }

    public bool DoesAnyObjectExistAtCoords(Vector2Int coords, out EnvironmentObjectInstance objFound)
    {
        List<EnvironmentObjectInstance> objects = GetAllObjectsAtCoords(coords);
        if (objects.Count == 0)
        {
            objFound = null;
            return false;
        }
        else
        {
            objFound = objects[0];
            return true;
        }
    }


    public bool DoesObjectExistAtCoords(Vector2Int coords,string toCheckFor, out EnvironmentObjectInstance objFound)
    {
        List<EnvironmentObjectInstance> objects = GetAllObjectsAtCoords(coords);
        if (objects.Count == 0)
        {
            objFound = null;
            return false;
        }
     

        for(int x = 0; x < objects.Count; x++)
        { 
            if (objects[x] != null && objects[x].ObjectKey == toCheckFor)
            {
                objFound = objects[x];
                return true;
            }
        }

     
        objFound = null;
        return false;
    }

    public Constructable GetConstructableAtPosition(int x,int y,ConstructableType type)
    {
        Constructable retVal = null;
        Vector3 pos = new Vector3(x+.5f, y+.5f);
        Bounds b = new Bounds();
        for(int x1 = 0; x1 < ToBuild.Count; x1++)
        {
            if (ToBuild[x1].GetType() != type)
            {
                continue;
            }
            b = new Bounds(ToBuild[x1].GetPosition(), ToBuild[x1].Size());
            if (b.Contains(pos))
            {
                return ToBuild[x1];
            }
        }

        return retVal;
    }


    public void AddConstructable(Constructable toBuild)
    {
        ToBuild.Add(toBuild);
        if (ShouldDrawEnvironmentObjects() && !toBuild.IsDrawn())
        {
            toBuild.Render();
        }
        SetModifiedIfLoaded();

    }

    public void RemoveConstructable(Constructable toRemove, bool needsCleanup = true)
    {
        if (toRemove == null)
        {
            return;
        }
        if (ToBuild.Contains(toRemove))
        {
            Debug.Log("ADDED TO BUILD removed");
            if (needsCleanup)
            {
                toRemove.Cleanup();
            }
            ToBuild.Remove(toRemove);
        }
        SetModifiedIfLoaded();

    }

    public bool ShouldDrawEnvironmentObjects()
    {
        return EnvironmentObjectsInChunk.Count > 0 && IsRendered;
    }

   
    public bool DrawnEnvironmentObjects()
    {
        if (EnvironmentObjectsInChunk.Count == 0)
        {
            return true;
        }
        return  EnvironmentObjectsInChunk[0].Drawn;
    }

    public void RenderEnvironmentObjects()
    {
        for(int x=0;x<EnvironmentObjectsInChunk.Count;x++)
        {
            EnvironmentObjectsInChunk[x].RenderInstance();
        }

        for (int x = 0; x < ToBuild.Count; x++)
        {
            ToBuild[x].Render();
        }
    }

    public void CleanupEnvironmentObjects()
    {
        for (int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            EnvironmentObjectsInChunk[x].CleanupInstance();
        }

        for (int x = 0; x < ToBuild.Count; x++)
        {
            ToBuild[x].Cleanup();
        }
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.Coords, new Vector2Int(X, Y));
        retVal.AddDataToSerialize(DataKeys.ChunkTiles, GetTileData());
        retVal.AddDataToSerialize(DataKeys.WallTiles, WallData());
        retVal.AddDataToSerialize(DataKeys.EnvironmentObjects, GetEnvObjectData());
        retVal.AddDataToSerialize(DataKeys.Resources, ResourceData());
        retVal.AddDataToSerialize(DataKeys.Constructables, ConstructableData());
        retVal.AddDataToSerialize(DataKeys.LocalCoords,new Vector2Int(LocalXCoord,LocalYCoord));
        return retVal;
    }

    List<DataToSerialize> ConstructableData()
    {
        List<DataToSerialize> retVal = new List<DataToSerialize>();
        for(int x = 0; x < ToBuild.Count; x++)
        {
            retVal.Add(ToBuild[x].GetDataToSerialize());
        }
        return retVal;
    }


    List<DataToSerialize> ResourceData()
    {
        List<DataToSerialize> retVal = new List<DataToSerialize>();
        for(int x=0;x< ResourceObjectsInChunk.Count; x++) {
            retVal.Add(ResourceObjectsInChunk[x].GetDataToSerialize());
        }

        return retVal;
    }

    List<DataToSerialize> WallData()
    {
        List<DataToSerialize> wallData = new List<DataToSerialize>();
        for(int x = 0; x < WallSegments.GetLength(0); x++)
        {
            for(int y=0;y< WallSegments.GetLength(1); y++)
            {
                if (WallSegments[x, y] != null && WallSegments[x,y].WallType!=WallType.None)
                {
                    wallData.Add(WallSegments[x, y].GetDataToSerialize());
                }
            }
        }

        return wallData;
    }

    DataToSerialize[,] GetTileData()
    {
        DataToSerialize[,] retVal = new DataToSerialize[ChunkTiles.GetLength(0), ChunkTiles.GetLength(1)];
        for(int x = 0; x < ChunkTiles.GetLength(0); x++)
        {
            for(int y = 0; y < ChunkTiles.GetLength(1); y++)
            {
                retVal[x, y] = ChunkTiles[x, y].GetDataToSerialize();
            }
        }

        return retVal;
    }

    List<DataToSerialize> GetEnvObjectData()
    {
        List<DataToSerialize> retVal = new List<DataToSerialize>();
        for(int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            retVal.Add(EnvironmentObjectsInChunk[x].GetDataToSerialize());
        }
        return retVal;
    }
    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }
    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value==0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);
        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
        myUid = new UID(uid);
        IDManager.OnUIDCreated(this, myUid);
    }

    public void UnRenderChunk()
    {
        CleanupEnvironmentObjects();
        for(int x = 0; x < WallSegments.GetLength(0); x++)
        {
            for(int y=0;y < WallSegments.GetLength(1); y++)
            {
                WallSegments[x, y].UnRender();
            }
        }
    }
}
