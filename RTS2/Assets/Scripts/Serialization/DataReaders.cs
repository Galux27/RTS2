using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataReaders
{
   public static void ReadData(string data)
   {
        if(data.Length==0||data==string.Empty) return;
        int firstSplit = -1;
        char lookingFor = SerializeDataHelpers.KEY_OBJECT_SPLIT;
        for (int x = 0; x < data.Length; x++)
        {
            if (data[x] == lookingFor)
            {
                firstSplit = x;
                break;
            }
        }
        string key = data.Substring(0, firstSplit);
      //  Debug.Log("Loading data " + (firstSplit + "|" + lookingFor + "|") + key);

        ParseData(key,data);
    }

    public static void ParseWorldChunks(string data)
    {
        string[] splitListFromData = data.Split(SerializeDataHelpers.DATA_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        for(int x=0;x<splitListFromData.Length;x++)
        {
            Debug.Log("Split list from data " + x +" " + splitListFromData[x]);
          
        }

        //0 coords
        //1 tile data
        //2 walls
        //3 env objects
        //4 resoruces
        //5 constructables
        string[] keyValueSplit = splitListFromData[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        Vector2Int coords = (Vector2Int)ParseDataObject(keyValueSplit[0], keyValueSplit[1]);
        WorldChunk chunk = new WorldChunk(coords.x, coords.y);
        keyValueSplit = splitListFromData[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
       chunk.ChunkTiles = (WorldTile[,])ParseDataObject(keyValueSplit[0], splitListFromData[1].Substring(keyValueSplit[0].Length));

        keyValueSplit = splitListFromData[3].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        if (keyValueSplit.Length > 1)
        {
            chunk.EnvironmentObjectsInChunk = (List<EnvironmentObjectInstance>)ParseDataObject(keyValueSplit[0], splitListFromData[3].Substring(keyValueSplit[0].Length));
        }
    }

    public static void ParseData(string key,string remainder)
    {
        string[] objects = remainder.Split(SerializeDataHelpers.DATA_OBJECT_SPLIT,System.StringSplitOptions.RemoveEmptyEntries);
        string[] keyObjectSplit = null;
        for(int x = 0; x < objects.Length; x++)
        {
            keyObjectSplit = objects[x].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
            ParseDataObject(keyObjectSplit[0], objects[x].Substring(keyObjectSplit[0].Length));
        } 
    }

    static object ParseDataObject(string key,string data)
    {
        switch (key)
        {
            case DataKeys.Coords:
                return ParseVector2Int(data);
                break;
            case DataKeys.ChunkTiles:
                return ParseChunkTiles(data);
                break;
            case DataKeys.CurrentProgress:
            case DataKeys.MaxProgress:
            case DataKeys.Health:
            case DataKeys.MaxHealth:
            case DataKeys.WaterLevel:
                return ParseFloat(data);
                break;        
            case DataKeys.Pos:
                return ParseVector3(data);
                break;
            case DataKeys.TileType:
            case DataKeys.WallTiles:
            case DataKeys.WallType:
            case DataKeys.WallVisual:
            case DataKeys.ObjectKey:
                return data;
            case DataKeys.UID:
                return ParseLong(data);
                break;
            case DataKeys.Quantitiy:
                return ParseInt(data);
                break;
            case DataKeys.EnvironmentObjects:
                return ParseEnvironmentObjects(data);
                break;
            case DataKeys.Resources:
            case DataKeys.ItemUID:
            case DataKeys.ItemsInContainer:
            case DataKeys.ConstructableType:
            case DataKeys.Constructables:          
            default:
                break;
        }
        return null;
    }

    static Vector3 ParseVector3(string val)
    {
        string[] split = val.Split(SerializeDataHelpers.DATA_SPLIT);
        return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));    
    }

    static float ParseLong(string val)
    {
        return long.Parse(val);
    }

    static int ParseInt(string val)
    {
        return int.Parse(val);
    }

    static float ParseFloat(string val)
    {
        return float.Parse(val);
    }

    static void RemoveMarkers(ref string[] data)
    {
        data[0] = data[0].Replace(";", "");
        data[1] = data[1].Replace(":", "");
    }

    static Vector2Int ParseVector2Int(string val)
    {
        string[] data = val.Split(SerializeDataHelpers.DATA_SPLIT);
        RemoveMarkers(ref data);

       // Debug.Log("vec2int parse " + data[0] + "," + data[1]);
        
        return new Vector2Int(int.Parse(data[0]), int.Parse(data[1]));
    }

    public static List<EnvironmentObjectInstance> ParseEnvironmentObjects(string data)
    {
        List<EnvironmentObjectInstance> instances = new List<EnvironmentObjectInstance>();

        string[] objects = data.Split(SerializeDataHelpers.LIST_ELEMENT_SPLIT,System.StringSplitOptions.RemoveEmptyEntries);
        
        
        for(int x = 0; x < objects.Length; x++)
        {
            EnvironmentObjectInstance toAdd = ParseEnvironmentObject(objects[x]);
            if (toAdd != null) {
                instances.Add(toAdd);
            }
        }
        return instances;
    }

    public static EnvironmentObjectInstance ParseEnvironmentObject(string data)
    {
        string[] objects = data.Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string[] parsing = objects[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        Vector2Int coords = (Vector2Int)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string key = (string)ParseDataObject(parsing[0],parsing[1]);
        //COORDS;14,45::OBJECT_KEY;Bush::UID;102::HEALTH;5::MAX_HEALTH;5::
        EnvironmentObjectInstance obj = new EnvironmentObjectInstance(coords.x,coords.y,key);
        float health = 0f, maxHealth = 0f;
        parsing = objects[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);

        health = (float)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[3].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        maxHealth = (float)ParseDataObject(parsing[0], parsing[1]);
        obj.OverrideHealth(health, maxHealth);

        return obj;
    }

    static WorldTile[,] ParseChunkTiles(string data)
    {
        string[] objects = data.Split(SerializeDataHelpers.LIST_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        WorldTile[,] tiles = new WorldTile[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        WorldTile currentTile = null;
       // Debug.Log("Parsing chunk tiles from " + data);
        for (int x = 0; x < objects.Length; x++)
        {
            currentTile = ParseChunkTile(objects[x]);
        }
        return tiles;
    }

    static WorldTile ParseChunkTile(string data)
    {
        string[] objects = data.Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT,System.StringSplitOptions.RemoveEmptyEntries);
        string[] keyObjectSplit = objects[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        Vector2Int coords = (Vector2Int)ParseDataObject(keyObjectSplit[0], keyObjectSplit[1]);
       
       //  Debug.Log("Parsed Chunk Tile data element is " + coords);
        keyObjectSplit = objects[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string tileType = (string)ParseDataObject(keyObjectSplit[0], keyObjectSplit[1]);
        
        keyObjectSplit = objects[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        float waterLevel = (float)ParseDataObject(keyObjectSplit[0], keyObjectSplit[1]);


        //CHUNK_TILES;COORDS;144,144::TILE_TYPE;Ground::WATER_LEVEL;0::`
        WorldTile retVal = new WorldTile(coords.x,coords.y);
        retVal.tileType = tileType;
        retVal.WaterData = new WaterData(waterLevel);
        return retVal;
    }
}
