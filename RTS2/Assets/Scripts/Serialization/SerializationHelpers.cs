using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public static class SerializationHelpers
{
   
}

public class DataKeys
{
    public const string Coords = "COORDS";
    public const string Pos = "POS";
    public const string TileType = "TILE_TYPE";
    public const string WaterLevel = "WATER_LEVEL";
    public const string ChunkTiles = "CHUNK_TILES";//todo
    public const string WallType = "WALL_TYPE";
    public const string WallVisual = "WALL_VISUAL";
    public const string Health = "HEALTH";
    public const string MaxHealth = "MAX_HEALTH";
    public const string WallTiles = "WALL_TILES";//todo
    public const string UID = "UID";
    public const string ObjectKey = "OBJECT_KEY";
    public const string EnvironmentObjects = "ENV_OBJECTS";//todo
    public const string Quantitiy = "QUANTITY";
    public const string Resources = "RESOURCE_OBJECTS";//todo
    public const string ItemUID = "ITEM_UID";
    public const string ItemsInContainer = "CONTAINER_CONTENTS";//todo
    public const string CurrentProgress = "CURRENT_PROGRESS";
    public const string MaxProgress = "MAX_PROGRESS";
    public const string ConstructableType = "CONSTRUCTABLE_TYPE";
    public const string Constructables = "CONSTRUCTABLES";//todo
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
    public string Data;

    public SerializedData(DataToSerialize data)
    {
        List<string> dataToWrite = new List<string>();
        foreach(KeyValuePair<string,object> pair in data.data)
        {
            dataToWrite.Add(SerializeDataHelpers.SerializeData(pair.Key, pair.Value));
            dataToWrite.Add(SerializeDataHelpers.DATA_ELEMENT_SPLIT);
        }
        Data = SerializeDataHelpers.CombineStrings(dataToWrite);
    }
}

public static class SerializeDataHelpers 
{
    //splits data from the key
    public const string KEY_OBJECT_SPLIT = ";";
    //splits data that forms part of the same element
    public const string DATA_SPLIT = ",";
    //splits data from the next in the DataToSerialize instance
    public const string DATA_ELEMENT_SPLIT = ":";
    //splits data that is stored in the same list
    public const string LIST_ELEMENT_SPLIT = "`";
    public static string SerializeData(string key,object value)
    {
        if (key== DataKeys.Coords)
        {
            return CombineStrings(key , KEY_OBJECT_SPLIT,SerializeVector2Int(value));
        }
        else if (key == DataKeys.Pos)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT, SerializeVector3(value));
        }
        else if (key == DataKeys.TileType|| key == DataKeys.WaterLevel|| key == DataKeys.WallType
            ||key==DataKeys.WallVisual||key==DataKeys.Health||key==DataKeys.MaxHealth||key==DataKeys.UID||
            key==DataKeys.ObjectKey||key==DataKeys.Quantitiy||key==DataKeys.ItemUID||key==DataKeys.CurrentProgress||key==DataKeys.MaxProgress)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT, value.ToString());
        }
        else if(key==DataKeys.ChunkTiles)
        {
            DataToSerialize[,] data = (DataToSerialize[,])value;
            List<string> stored = new List<string>();
            DataToSerialize element = null;
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for(int y=0;y < data.GetLength(1); y++)
                {
                    element = data[x, y];
                    foreach(KeyValuePair<string,object> kvp in element.data)
                    {
                        stored.Add(SerializeData(kvp.Key,kvp.Value));
                        stored.Add(DATA_ELEMENT_SPLIT);
                    }
                    stored.Add(LIST_ELEMENT_SPLIT);
                }
            }
            return CombineStrings(key,KEY_OBJECT_SPLIT,stored);
        }else if (key == DataKeys.WallTiles|| key == DataKeys.EnvironmentObjects||key==DataKeys.Resources||key==DataKeys.ItemsInContainer||key==DataKeys.Constructables)
        {
            List<DataToSerialize> data = (List<DataToSerialize>)value;
            List<string> stored = new List<string>();
            for(int x = 0; x < data.Count; x++)
            {
                foreach (KeyValuePair<string, object> kvp in data[x].data)
                {
                    stored.Add(SerializeData(kvp.Key, kvp.Value));
                    stored.Add(DATA_ELEMENT_SPLIT);

                }
                stored.Add(LIST_ELEMENT_SPLIT);

            }

            return CombineStrings(key, KEY_OBJECT_SPLIT, stored);
        }
        Debug.LogError("Could not serialize " + key + " as its been assigned a serializer");
        return "";
    }

    static string SerializeVector2Int(object value)
    {
        Vector2Int val =(Vector2Int)value;
        return CombineStrings( val.x.ToString() , DATA_SPLIT, val.y.ToString());
    }
    static string SerializeVector3(object value)
    {
        Vector3 val = (Vector3)value;
        return CombineStrings(val.x.ToString(), DATA_SPLIT, val.y.ToString());
    }
    static StringBuilder builder = new StringBuilder();
    public static string CombineStrings( List<string> data)
    {
        builder.Clear();
       
        for (int x = 0; x < data.Count; x++)
        {
            builder.Append(data[x]);
        }
        return builder.ToString();
    }
    public static string CombineStrings(string key,string split,List<string> data)
    {
        builder.Clear();
        builder.Append(key);
        builder.Append(split);
        for (int x = 0; x < data.Count; x++)
        {
            builder.Append(data[x]);
        }
        return builder.ToString();
    }
    static string CombineStrings(params string[] data)
    {
        builder.Clear();
        for(int x = 0; x < data.Length; x++)
        {
            builder.Append(data[x]);
        }
        return builder.ToString();
    }
}




