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

        WallManager = new WallManager(WorldWidth, WorldHeight);
        WallManager.DrawSomeRandomWalls();

        WorldRenderer.Instance.RenderWorld(WorldTiles);
        WallManager.RenderWalls(BuildingTilemap, WallTest);
        Pathfinding.CreateNodesFromWorld(WorldTiles);
    }

    public Vector2Int ConvertWorldToTileCoords(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x-.5f), Mathf.RoundToInt(pos.y-.5f));
    }

    public Vector3Int ConvertWorldToTileCoordsVec3(Vector3 pos)
    {
        return new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z ));
    }


    public void SetTraversible(int x,int y,bool traversable)
    {
        WorldTiles[x, y].traversable = traversable;
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
}

public enum TileType 
{ 
    Ground,
    Water
}


