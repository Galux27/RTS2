using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        WorldRenderer.Instance.RenderWorld(WorldTiles);
        Pathfinding.CreateNodesFromWorld(WorldTiles);
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


