using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class to store data of objects within a given area (units, props, items etc...)
/// </summary>
public class WorldChunk:ISerialize
{
    public List<Unit> UnitsInChunk=new List<Unit>();
    public List<EnvironmentObjectInstance> EnvironmentObjectsInChunk = new List<EnvironmentObjectInstance>();
    public List<ResourceInstance> ResourceObjectsInChunk = new List<ResourceInstance>();
    public List<Inventory> StaticContainersInChunk = new List<Inventory>();
    public List<Constructable> ToBuild=new List<Constructable>();
    public Color DebugColor;
    public int X, Y;

    public WallSegment[,] WallSegments;
    public PathfindingNode[,] PathfindingNodes;
    public WorldTile[,] ChunkTiles;
    public Vector2Int WorldCoords;
    public WorldChunk(int x,int y)
    {
        DebugColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f,1f),1f);
        X = x;
        Y = y;
        WorldCoords=new Vector2Int(x* WorldChunkManager.ChunkSize, y* WorldChunkManager.ChunkSize);
        GenerateTilesForChunk();
        GenerateWallsForChunk();
        GeneratePathfindingNodes();
    }

    void GenerateTilesForChunk()
    {
        ChunkTiles = new WorldTile[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        int xStart = WorldChunkManager.ChunkSize * X;
        int yStart = WorldChunkManager.ChunkSize * Y;
        int localx = 0, localy = 0;
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

    void GeneratePathfindingNodes()
    {
        PathfindingNodes=new PathfindingNode[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        int xStart = WorldChunkManager.ChunkSize * X;
        int yStart = WorldChunkManager.ChunkSize * Y;
        int localx = 0, localy = 0;
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

    public void InitPathfindingNodes()
    {
        int xStart = WorldChunkManager.ChunkSize * X;
        int yStart = WorldChunkManager.ChunkSize * Y;
        int localx = 0, localy = 0;
        for (int x = xStart; x < xStart + WorldChunkManager.ChunkSize; x++)
        {

            for (int y = yStart; y < yStart + WorldChunkManager.ChunkSize; y++)
            {
                PathfindingNodes[localx, localy].InitData();
                localy++;
            }
            localx++;
            localy = 0;
        }
    }

    void GenerateWallsForChunk()
    {
        WallSegments = new WallSegment[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        int xStart = WorldChunkManager.ChunkSize *X;
        int yStart = WorldChunkManager.ChunkSize*Y;
        int localx = 0, localy = 0;
        for(int x=xStart; x<xStart+WorldChunkManager.ChunkSize; x++)
        {
           
            for (int y = yStart; y < yStart + WorldChunkManager.ChunkSize; y++)
            {
                WallSegments[localx, localy] = new WallSegment(x, y, null);
                localy++;
            }
            localx++;
            localy = 0;
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

    public void AddResourceObject(ResourceInstance resourceInstance)
    {
        ResourceObjectsInChunk.Add(resourceInstance);
    }

    public void RemoveResourceObject(ResourceInstance resourceInstance)
    {
        ResourceObjectsInChunk.Remove(resourceInstance);
    }


    public void AddContainerObject(Inventory container)
    {
        StaticContainersInChunk.Add(container);
    }

    public void RemoveContainerObject(Inventory container)
    {
        StaticContainersInChunk.Remove(container);
    }

    public void AddEnvironmentObject(EnvironmentObjectInstance environmentObject)
    {
        EnvironmentObjectsInChunk.Add(environmentObject);
        environmentObject.SetChunk(this);
        if (ShouldDrawEnvironmentObjects() && environmentObject.Drawn == false)
        {
            environmentObject.RenderInstance();
        }
    }

    public void RemoveEnvironmentObject(EnvironmentObjectInstance instance)
    {
        if (instance.Drawn)
        {
            instance.CleanupInstance();
        }
        
        EnvironmentObjectsInChunk.Remove(instance);
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
    }

    public void RemoveConstructable(Constructable toRemove, bool needsCleanup = true)
    {
        if (toRemove == null)
        {
            return;
        }
        if (ToBuild.Contains(toRemove))
        {
            Debug.Log("Removed Constructable");
            if (needsCleanup)
            {
                toRemove.Cleanup();
            }
            ToBuild.Remove(toRemove);
        }
    }

    public bool ShouldDrawEnvironmentObjects()
    {
        return EnvironmentObjectsInChunk.Count > 0;
    }

    public bool DrawnEnvironmentObjects()
    {
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
        throw new System.NotImplementedException();
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
        }
        return myUid;
    }
}
