using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "World Tiles", menuName = "Tiles/World Tiles Store", order = 1)]
public class WorldTiles : ScriptableObject
{
    public List<WorldTileType> tileTypes;
    public List<WaterTile> WaterTiles;
}

public class WorldTilesManager
{
    Dictionary<string, WorldTileType> WorldTiles;
    Dictionary<float, WaterTile> WaterTiles;
    public WorldTilesManager(WorldTiles toUse)
    {
        List<WorldTileType> tileTypes = toUse.tileTypes;
        WorldTiles = new Dictionary<string, WorldTileType>();
        for(int x=0; x<tileTypes.Count; x++)
        {
            WorldTiles.Add(tileTypes[x].tileType, tileTypes[x]);
        }

        WaterTiles = new Dictionary<float, WaterTile>();
        for(int x = 0; x < toUse.WaterTiles.Count; x++)
        {
            WaterTiles.Add(toUse.WaterTiles[x].WaterHeight, toUse.WaterTiles[x]);
        }
    }

    public TileBase GetTileBase(string type)
    {
        if (WorldTiles.ContainsKey(type))
        {
            return WorldTiles[type].tileBase;
        }
        return null;
    }

}


[System.Serializable]
public class WorldTileType 
{
    public string tileType;
    public TileBase tileBase;

}
[System.Serializable]
public class WaterTile
{
    public float WaterHeight;
    public TileBase[] Tiles;
}

