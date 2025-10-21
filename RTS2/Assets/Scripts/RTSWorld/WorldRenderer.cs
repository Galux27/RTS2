using System;
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

    public Tilemap WorldTilemap,WaterTilemap;
    public WorldTiles WorldTiles;
    public WorldTilesManager WorldTilesManager;




    bool init = false;
    private void Init()
    {
        WorldTilesManager = new WorldTilesManager(WorldTiles);



        init = true;
    }


    public void RenderChunk(WorldTile[,] tiles)
    {
        Vector3Int[] positionArray = new Vector3Int[tiles.GetLength(0) * tiles.GetLength(1)];
        TileBase[] tileArray = new TileBase[positionArray.Length];
        int index = 0;
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                //RenderTile(tiles[x, y]);
                AddTileToRender(tiles[x, y], index, ref positionArray, ref tileArray);
                tiles[x, y].Elevation.Render();

                index++;
            }
        }
        WorldTilemap.SetTiles(positionArray,tileArray);

        //positionArray = new Vector3Int[tiles.GetLength(0) * tiles.GetLength(1)];
        //tileArray = new TileBase[positionArray.Length];
        //index = 0;
        //for (int x = 0; x < tiles.GetLength(0); x++)
        //{
        //    for (int y = 0; y < tiles.GetLength(1); y++)
        //    {
        //        //RenderTile(tiles[x, y]);
        //        //AddElevationToRender(tiles[x, y].Elevation, index, ref positionArray, ref tileArray);
        //        index++;
        //    }
        //}
    }

    public void UnrenderChunk(WorldTile[,] tiles)
    {
       
        Vector3Int[] positionArray = new Vector3Int[tiles.GetLength(0) * tiles.GetLength(1)];
        TileBase[] tileArray = new TileBase[positionArray.Length];
        int index = 0;
        Vector3Int coords;
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y].Elevation.Cleanup();
                coords = new Vector3Int(tiles[x, y].x, tiles[x, y].y, 0);
                positionArray[index] = coords;
                tileArray[index] = null;             
                index++;
            }
        }
        WorldTilemap.SetTiles(positionArray, tileArray);
        WaterTilemap.SetTiles(positionArray, tileArray);
        WorldController.Instance.BuildingTilemap.SetTiles(positionArray, tileArray);
        
    }

   

    string lastTilePlaced;
    TileBase lastTileBase;
    TileBase currentTile;
    Vector3Int coords;
    void AddTileToRender(WorldTile tile,int index,ref Vector3Int[] postions,ref TileBase[] tiles)
    {
        if (!init)
        {
            Init();
        }
        coords = new Vector3Int(tile.x, tile.y, 0);
        postions[index]= coords;
        if (lastTilePlaced != tile.tileType)
        {
            currentTile = WorldTilesManager.GetTileBase(tile.tileType);
            lastTilePlaced = tile.tileType;
            lastTileBase = currentTile;
            tiles[index]=currentTile;
        }
        else
        {
            currentTile = lastTileBase;
            tiles[index] = currentTile;
        }
        if (tile.WaterData.WaterLevel > 0f)
        {
            WaterTilemap.SetTile(coords, WorldTilesManager.GetTileForWaterLevel(tile.WaterData.WaterLevel));

        }
    }


    void RenderTile(WorldTile tile)
    {
        Vector3Int coords = new Vector3Int(tile.x, tile.y, 0);
        if (lastTilePlaced != tile.tileType)
        {
            currentTile = WorldTilesManager.GetTileBase(tile.tileType);
            lastTilePlaced = tile.tileType;
            lastTileBase = currentTile;
        }
        else
        {
            currentTile = lastTileBase;
        }
       
        WorldTilemap.SetTile(coords, currentTile) ;
        if (tile.WaterData.WaterLevel > 0f)
        {
            WaterTilemap.SetTile(coords, WorldTilesManager.GetTileForWaterLevel(tile.WaterData.WaterLevel));

        }
    }
}
