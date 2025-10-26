using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldController : MonoBehaviour
{
    static WorldController instance;
    public static WorldController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorldController>();
            }
            return instance;
        }
    }


    public int WorldWidthInChunks = 10, WorldHeightInChunks = 10;

    public int WorldWidth
    {
        get
        {
            return WorldWidthInChunks * WorldChunkManager.ChunkSize;
        }

    }
        
    public int WorldHeight{
        get
        {
            return WorldHeightInChunks * WorldChunkManager.ChunkSize;
        }
    }

    //public WorldTile[,] WorldTiles;


    public WallManager WallManager;
    public Tilemap BuildingTilemap;
    public WallTile WallTest;
    public GameObject WallCollider;
    private void Start()
    {
        WallManager = new WallManager(WorldWidth, WorldHeight);
        
        InitWorld();
    }
    public bool LoadWorld = false;
    bool DoWeLoadWorld()
    {
        return LoadWorld || SaveLoadHelpers.DoWeLoadWorld;
    }

    public void InitWorld()
    {
        if (!DoWeLoadWorld())
        {
            OverworldGenerator.Instance.GenerateWithoutCoroutine();
        }
        else
        {
            if (SaveLoadHelpers.DoWeLoadWorld)
            {
                SerializationHelpers.LoadGame(SaveLoadHelpers.SaveToLoad);
                //SaveLoadHelpers.SaveToLoad = "";
                SaveLoadHelpers.DoWeLoadWorld = false;

            }
            else
            {
                OverworldGenerator.Instance.Generate();

            }
        }
        WorldChunkManager.Instance.RenderWorldChunks();
       // WallManager.RenderWalls(BuildingTilemap);

    }

    public Vector2Int ConvertWorldToTileCoords(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public Vector3Int ConvertWorldToTileCoordsVec3(Vector3 pos)
    {
        return new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z ));
    }

    public void SetTilesAroundEnvrionmentObjectTraversable(EnvironmentObjectInstance toSet, bool traversable)
    {
        EnvironmentObject data = EnvironmentObjectHelpers.GetEnvironmentObject(toSet.ObjectKey);
       
        Vector2Int coords = toSet.coords;//WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        
        Color c = Color.green;

        for (int x = coords.x-data.GetWidth/2; x < coords.x + (data.GetWidth/2) +1; x++)
        {
            for (int y = coords.y-data.GetWidth/2 ; y < coords.y + (data.GetHeight/2) +1; y++)
            {
                SetTraversible(x, y, traversable);
            }
        }
    }


     public void SetTraversible(int x,int y,bool traversable)
    {
        
            WorldTileHelpers.UpdateTileTraversible(x, y, traversable);
            Pathfinding.UpdateNodeData(x, y, traversable);
        

   }

    public void AddPathfindingModifier(int x,int y, PathNodeModifier toAdd)
    {
        Pathfinding.AddPathNodeModifier(x, y, toAdd);

    }

    public Action<Vector2Int, Unit> OnTileEnterAction, OnTileExitAction;
    public void OnTileEnter(Vector2Int coords, Unit unit)
    {
        try
        {
            WorldTileHelpers.GetTileFromCoords(coords.x, coords.y).OnTileEntered(coords);
            OnTileEnterAction?.Invoke(coords, unit);
        }
        catch
        {
            Debug.LogError("Error entering tile at " + coords.ToString());
        }
    }

    
    public void OnTileExit(Vector2Int coords, Unit unit)
    {
        try
        {
            WorldTileHelpers.GetTileFromCoords(coords.x, coords.y).OnTileExit(coords);
            OnTileExitAction?.Invoke(coords, unit);
        }
        catch
        {
            Debug.LogError("Error exiting tile at " + coords.ToString());

        }
    }
   

    public bool IsTraversible(int x,int y)
    {
        //if(x<0 || y<0) return false;
        //if(x>WorldWidth || y>WorldHeight) return false;

        WorldTile tile = WorldTileHelpers.GetTileFromCoords(x, y);
        if (tile == null)
        {
           // Debug.Log("Furniture Click: tile at " + x + "," + y + " was null");
            return false;
        }
        return tile.TileTraversable();

    }
}

[System.Serializable]
public class WorldTile:ISerialize
{
    public int x,y;
    public bool traversable = true, CanPutDecorationsOn = true;
    public string tileType;
    public uint TileID;
    public WaterData WaterData;
    public ElevationTile Elevation;
    public Vector2Int Chunk, Batch,Local;
    public Vector2Int Coords()
    {
        return new Vector2Int(x, y);
    }


    public WorldTile(int x,int y,Vector2Int chunk,Vector2Int batch,int localX,int localY)
    {
       Init(x,y,chunk,batch,localX,localY);
    }

    public void Init(int x, int y, Vector2Int chunk, Vector2Int batch, int localX, int localY)
    {
        this.x = x;
        this.y = y;
        tileType = "Ground";
        WaterData.Init(0f);// = new WaterData(0f);
        Elevation.Init(new Vector3Int(x, y, 0));// = new ElevationTile(new Vector3Int(x, y, 0));    
        Chunk = chunk;
        Batch = batch;
        Local = new Vector2Int(localX, localY);
    }

    public void SetElevation(float value,bool updatePathfinding=true)
    {
        Elevation.SetElevation(value);// = value;
        if (Elevation.GetElevation() < OverworldGenerator.Instance.SeaLevel)
        {
            UpdateWaterLevel(10f,updatePathfinding);
        }
        else
        {
            if (updatePathfinding)
            {
                Pathfinding.UpdateNodeData(x, y, TileTraversable());
            }
        }
    }

    public bool TileTraversable()
    {
        return traversable && Elevation.IsPassible() && WaterData.WaterLevel<1f;
    }


    public void UpdateWaterLevel(float val,bool updatePathfindingNode=true)
    {
        WaterData.UpdateWaterLevel(val);
        if (updatePathfindingNode)
        {
            Pathfinding.UpdateNodeData(x, y, TileTraversable());
        }
        }


        public void OnTileEntered(Vector2Int vector2Int)
    {

    }

    public void OnTileExit(Vector2Int vector2Int)
    {

    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize data = new DataToSerialize();

        data.AddDataToSerialize(DataKeys.Coords, new Vector2Int(x, y));
        data.AddDataToSerialize(DataKeys.TileType, tileType);
        data.AddDataToSerialize(DataKeys.WaterLevel, WaterData.WaterLevel);
        data.AddDataToSerialize(DataKeys.Elevation, Elevation.GetElevation());
        return data;
    }

    public void UpdateTileType(string type,uint id)
    {      
       
        tileType = type;
        TileID = id;
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].NeedsUpdate = true;
    }


    public SerializedData Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(SerializedData data)
    {
        throw new NotImplementedException();
    }

    public UID GetMyUID()
    {
        throw new NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
       // myUID = new UID(uid);
    }
}

public enum TileType 
{ 
    Ground,
    Sand,
    Mud,
    Gravel,
    Road,
    Paved
}


