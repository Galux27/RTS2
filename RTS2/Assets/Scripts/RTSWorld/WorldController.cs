using System;
using System.Collections;
using System.Collections.Generic;
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

    public int WorldWidth=100, WorldHeight=100;

    public WorldTile[,] WorldTiles;


    public WallManager WallManager;
    public Tilemap BuildingTilemap;
    public WallTile WallTest;
    public GameObject WallCollider;
    private void Start()
    {
        InitWorld();
    }

    public void InitWorld()
    {
        WorldTiles = new WorldTile[WorldWidth, WorldHeight];
        for(int x=0; x < WorldWidth; x++)
        {
            for(int y=0; y < WorldHeight; y++)
            {
                WorldTiles[x, y] = new WorldTile(x, y);
            }
        }
        Pathfinding.CreateNodesFromWorld(WorldTiles);

        WallManager = new WallManager(WorldWidth, WorldHeight);
        WorldRenderer.Instance.RenderWorld(WorldTiles);
        WallManager.RenderWalls(BuildingTilemap, WallTest);

        EnvironmentObjectManager.Instance.GenerateEnvironmentObjects();
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
        EnvironmentObject data = EnvironmentObjectManager.Instance.AllObjects[toSet.ObjectKey];
        
        Vector2Int coords = toSet.coords;//WorldController.Instance.ConvertWorldToTileCoords(cursorPos);
        
        Color c = Color.green;

        for (int x = coords.x - data.HalfWidth; x < coords.x + data.HalfWidth; x++)
        {
            for (int y = coords.y - data.HalfHeight; y < coords.y + data.HalfHeight; y++)
            {
                SetTraversible(x, y, traversable);
            }
        }
    }


     public void SetTraversible(int x,int y,bool traversable)
    {
        if (CoordsValid(x, y))
        {
            WorldTiles[x, y].traversable = traversable;
            Pathfinding.UpdateNodeData(x, y, traversable);
        }

   }

        public void AddPathfindingModifier(int x,int y, PathNodeModifier toAdd)
    {
        Pathfinding.AddPathNodeModifier(x, y, toAdd);

    }

    public Action<Vector2Int, Unit> OnTileEnterAction, OnTileExitAction;
    public void OnTileEnter(Vector2Int coords, Unit unit)
    {
        WorldTiles[coords.x, coords.y].OnTileEntered(coords);
        OnTileEnterAction?.Invoke(coords, unit);
    }

    public void OnTileExit(Vector2Int coords, Unit unit)
    {
        WorldTiles[coords.x, coords.y].OnTileExit(coords);
        OnTileExitAction?.Invoke(coords,unit);

    }
    bool CoordsValid(int x,int y)
    {
        if (x < 0 || y < 0) return false;
        if (x > WorldWidth || y > WorldHeight) return false;
        return true;
    }

    public bool IsTraversible(int x,int y)
    {
        if(x<0 || y<0) return false;
        if(x>WorldWidth || y>WorldHeight) return false;
        return WorldTiles[x, y].traversable;

    }
}

public class WorldTile 
{
    public int x,y;
    public bool traversable = true;
    public TileType tileType;

    public WorldTile(int x,int y)
    {
        this.x = x;
        this.y = y;
    }

    public void OnTileEntered(Vector2Int vector2Int)
    {

    }

    public void OnTileExit(Vector2Int vector2Int)
    {

    }
}

public enum TileType 
{ 
    Ground,
    Water
}


