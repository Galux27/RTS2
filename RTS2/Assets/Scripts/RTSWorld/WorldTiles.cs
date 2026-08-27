using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    Dictionary<string, uint> WorldTileIndexes;
    Dictionary<float, WaterTile> WaterTiles;
    Dictionary<string, HashSet<uint>> CanPlaceDictionary;
    public WorldTilesManager(WorldTiles toUse)
    {
        List<WorldTileType> tileTypes = toUse.tileTypes;
        WorldTiles = new Dictionary<string, WorldTileType>();
        WorldTileIndexes = new Dictionary<string, uint>();
        for(int x=0; x<tileTypes.Count; x++)
        {
            WorldTiles.Add(tileTypes[x].tileType, tileTypes[x]);
            WorldTileIndexes.Add(tileTypes[x].tileType, (uint)x);
        }

        WaterTiles = new Dictionary<float, WaterTile>();
        for(int x = 0; x < toUse.WaterTiles.Count; x++)
        {
            WaterTiles.Add(toUse.WaterTiles[x].WaterHeight, toUse.WaterTiles[x]);
        }

        CanPlaceDictionary = new Dictionary<string, HashSet<uint>>();
        for (int x = 0; x < tileTypes.Count; x++)
        {
            if (tileTypes[x].TilesICantBePlacedOn.Count > 0)
            {
                CanPlaceDictionary.Add(tileTypes[x].tileType, new HashSet<uint>());
                for(int i=0;i< tileTypes[x].TilesICantBePlacedOn.Count; i++)
                {
                    CanPlaceDictionary[tileTypes[x].tileType].Add(WorldTileIndexes[tileTypes[x].TilesICantBePlacedOn[i]]);
                }
            }
        }
    }

    public bool CanTileBePlacedOnAnother(string tileToPlace,uint placingOver)
    {
        if(!CanPlaceDictionary.ContainsKey(tileToPlace)) return true;

        return !CanPlaceDictionary[tileToPlace].Contains(placingOver);
    }

    public TileBase GetTileForWaterLevel(float level)
    {
        float difference = 9999999f;
        float diffComp = 0f;
        WaterTile retVal = null;
        foreach(KeyValuePair<float,WaterTile> waterTile in WaterTiles)
        {
            diffComp = Mathf.Abs(level - waterTile.Key);
            if (diffComp < difference)
            {
                difference = diffComp;
                retVal = waterTile.Value;
            }
        }
        return retVal.Tiles[Random.Range(0, retVal.Tiles.Length)] ;
    }

    public uint GetTileID(string type)
    {
        if (type == null)
        {
            return 0;
        }
        if (WorldTileIndexes.ContainsKey(type))
        {
            return WorldTileIndexes[type];
        }
        
        return 0;
    }
    public Color GetTileMinimapColour(string key)
    {
        if (WorldTiles.ContainsKey(key))
        {
            return WorldTiles[key].MinimapColour;
        }
        else
        {
            return Color.cyan;
        }
    }

    public TileBase GetTileBase(string type)
    {
        if (WorldTiles.ContainsKey(type))
        {
            return WorldTiles[type].tileBase;
        }
        return WorldTiles[ErrorTile].tileBase;
    }
    const string ErrorTile = "Error";
}


[System.Serializable]
public class WorldTileType 
{
    public string tileType;
    public TileBase tileBase;
    public Color MinimapColour;
    public List<string> TilesICantBePlacedOn;

}
[System.Serializable]
public class WaterTile
{
    public float WaterHeight;
    public TileBase[] Tiles;
}

