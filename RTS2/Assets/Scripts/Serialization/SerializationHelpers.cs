using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializationHelpers
{
   
}

public class DataKeys
{
    public const string Coords = "COORDS";
    public const string TileType = "TILE_TYPE";
    public const string WaterLevel = "WATER_LEVEL";
    public const string ChunkTiles = "CHUNK_TILES";
    public const string WallType = "WALL_TYPE";
    public const string WallVisual = "WALL_VISUAL";
    public const string Health = "HEALTH";
    public const string MaxHealth = "MAX_HEALTH";
    public const string WallTiles = "WALL_TILES";
    public const string UID = "UID";
    public const string ObjectKey = "OBJECT_KEY";
    public const string EnvironmentObjects = "ENV_OBJECTS";
    public const string Quantitiy = "QUANTITY";
    public const string Resources = "RESOURCE_OBJECTS";
    public const string ItemUID = "ITEM_UID";
    public const string ItemsInContainer = "CONTAINER_CONTENTS";
    public const string CurrentProgress = "CURRENT_PROGRESS";
    public const string MaxProgress = "MAX_PROGRESS";
    public const string ConstructableType = "CONSTRUCTABLE_TYPE";
    public const string Constructables = "CONSTRUCTABLES";
}

public enum DataType
{
    None,
    Unit,
    Chunk,
    Tile,
    EnvironmentObject,
    Behaviour
}

public class DataToSerialize
{
    public Dictionary<string,object> data;
    public DataToSerialize()
    {
        data = new Dictionary<string, object>();
    }

    public void AddDataToSerialize(string key,object value)
    {
        if (!data.ContainsKey(key))
        {
            data.Add(key, value);
        }
        else
        {
            data[key] = value;
        }
    }

}

public class SerializedData
{
    public Dictionary<string, string> data;
}
