using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldRenderer : MonoBehaviour
{
    static WorldRenderer instance;
    public static WorldRenderer Instance { 
        get {
            if(instance == null)
            {
                instance = FindObjectOfType<WorldRenderer>();
                if (!instance.init)
                {
                    instance.Init();
                }
            }
            return instance; 
        } 
    }

    public Tilemap WorldTilemap;
    public WorldTiles WorldTiles;
    WorldTilesManager WorldTilesManager;
    bool init = false;
    private void Init()
    {
        WorldTilesManager = new WorldTilesManager(WorldTiles.tileTypes);

        init = true;
    }


    public void RenderWorld(WorldTile[,] tiles)
    {
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                RenderTile(tiles[x, y]);
            }
        }
    }

    void RenderTile(WorldTile tile)
    {
        WorldTilemap.SetTile(new Vector3Int(tile.x, tile.y,0), WorldTilesManager.GetTileBase(tile.tileType));
    }
}
