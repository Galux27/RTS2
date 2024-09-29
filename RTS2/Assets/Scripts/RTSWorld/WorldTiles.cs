using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "World Tiles", menuName = "Tiles/World Tiles Store", order = 1)]
public class WorldTiles : ScriptableObject
{
    public List<WorldTileType> tileTypes;
}

public class WorldTilesManager
{
    Dictionary<TileType, WorldTileType> WorldTiles;
    public WorldTilesManager(List<WorldTileType> tileTypes)
    {
        WorldTiles = new Dictionary<TileType, WorldTileType>();
        for(int x=0; x<tileTypes.Count; x++)
        {
            WorldTiles.Add(tileTypes[x].tileType, tileTypes[x]);
        }
    }

    public TileBase GetTileBase(TileType type)
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
    public TileType tileType;
    public TileBase tileBase;

}

