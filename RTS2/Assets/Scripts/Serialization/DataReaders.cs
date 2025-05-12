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
        ParseData(key,data);
    }
    static Vector2Int currentLoadingChunkCoords;
    public static WorldChunk ParseWorldChunk(string data)
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
        currentLoadingChunkCoords = coords;
        WorldChunk chunk = new WorldChunk(coords.x, coords.y);
        keyValueSplit = splitListFromData[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
       chunk.ChunkTiles = (WorldTile[,])ParseDataObject(keyValueSplit[0], splitListFromData[1].Substring(keyValueSplit[0].Length));

        keyValueSplit = splitListFromData[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        if (keyValueSplit.Length > 1)
        {
            chunk.WallSegments = (WallSegment[,])ParseDataObject(keyValueSplit[0], splitListFromData[2].Substring(keyValueSplit[0].Length));
           
        }

        keyValueSplit = splitListFromData[3].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        if (keyValueSplit.Length > 1)
        {
            chunk.EnvironmentObjectsInChunk = (List<EnvironmentObjectInstance>)ParseDataObject(keyValueSplit[0], splitListFromData[3].Substring(keyValueSplit[0].Length));
            for(int x = 0; x < chunk.EnvironmentObjectsInChunk.Count; x++)
            {
                chunk.EnvironmentObjectsInChunk[x].SetChunk(chunk);
            }
        
        }

        keyValueSplit = splitListFromData[5].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        if (keyValueSplit.Length > 1)
        { 
            List<BuildableStructure> buildableStructures = (List<BuildableStructure>)ParseDataObject(keyValueSplit[0], splitListFromData[5].Substring(keyValueSplit[0].Length));
            chunk.ToBuild = new List<Constructable>();
            for(int x = 0; x < buildableStructures.Count; x++)
            {
                
                chunk.ToBuild.Add(buildableStructures[x]);
            }
        }
        return chunk;
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
            case DataKeys.WallTiles:
                return ParseWallSegments(data);
            case DataKeys.TileType:    
            case DataKeys.WallVisual:
            case DataKeys.ObjectKey:
            case DataKeys.UnitFaction:
            case DataKeys.RoomName:
                return data;
            case DataKeys.UID:
                return ParseULong(data);
                break;
            case DataKeys.Quantitiy:
            case DataKeys.WallType:
            case DataKeys.ConstructableType:
            case DataKeys.UnitType:
            case DataKeys.RoomType:
                return ParseInt(data);
                break;
            case DataKeys.EnvironmentObjects:
                return ParseEnvironmentObjects(data);
                break;
            case DataKeys.Constructables: 
                return ParseConstructableObjects(data);
                break;
            case DataKeys.Resources:
            case DataKeys.ItemUID:
            case DataKeys.ItemsInContainer:
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

    static ulong ParseULong(string val)
    {
        return ulong.Parse(val);
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

    public static List<BuildableStructure> ParseConstructableObjects(string data)
    {
        List<BuildableStructure> retVal = new List<BuildableStructure>();
        string[] objects = data.Split(SerializeDataHelpers.LIST_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);


        for (int x = 0; x < objects.Length; x++)
        {
            BuildableStructure toAdd = ParseConstructableObject(objects[x]);
            if (toAdd != null)
            {
                retVal.Add(toAdd);
            }
        }

        return retVal;
    }

    static BuildableStructure ParseConstructableObject(string data)
    {
        Debug.Log("Constructable object data " + data);
        //;UID;62::COORDS;8,14::HEALTH;1::MAX_HEALTH;1::CURRENT_PROGRESS;0::MAX_PROGRESS;10::OBJECT_KEY;Fuel Tank::
        string[] objects = data.Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string[] parsing = objects[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        ulong uid = (ulong)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
       Vector2Int coords = (Vector2Int)ParseDataObject(parsing[0], parsing[1]);
        float health = 0f, maxHealth = 0f;
        parsing = objects[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        health = (float)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[3].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        maxHealth = (float)ParseDataObject(parsing[0], parsing[1]);
        float progress = 0f, maxProgress = 0f;
        parsing = objects[4].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        progress = (float)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[5].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        maxProgress = (float)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[6].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string key = (string)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[7].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        ConstructableType myType = (ConstructableType)ParseDataObject(parsing[0], parsing[1]);

        if (myType == ConstructableType.Furniture)
        {
            System.Action OnBuilt = ConstructableObjectManager.Instance.GetActionForConstructableOnBuilt(coords, new Vector3(coords.x, coords.y, 0), key);
            ConstructableObject buildingData = ConstructableObjectManager.Instance.GetData(key);
            BuildableStructure retVal = new BuildableStructure(coords.x, coords.y, maxProgress, false, OnBuilt, buildingData.Size(), default, myType, key);
            return retVal;

        }
        else if (myType == ConstructableType.Wall || myType == ConstructableType.Door)
        {
            System.Action OnBuilt = WallHelpers.GetOnBuilt(coords, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.AllObjects[key]);
            BuildableStructure retVal = new BuildableStructure(coords.x, coords.y, maxProgress, false, OnBuilt, Vector3.one, default, myType, key);
            return retVal;

        }
        return null;
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
        ulong uid = (ulong)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[3].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);

        health =  (float)ParseDataObject(parsing[0], parsing[1]);
        parsing = objects[4].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        maxHealth = (float)ParseDataObject(parsing[0], parsing[1]);
        obj.OverrideHealth(health, maxHealth);
        obj.SetMyUID(uid);
        return obj;
    }

    static WorldTile[,] ParseChunkTiles(string data)
    {
        string[] objects = data.Split(SerializeDataHelpers.LIST_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        WorldTile[,] tiles = new WorldTile[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        WorldTile currentTile = null;
        int x = 0, y = 0;
        bool gotCorner = false;
        int xc = 0, yc = 0;
        for (int q = 0; q < objects.Length; q++)
        {
            currentTile = ParseChunkTile(objects[q]);
            if (!gotCorner)
            {
                xc = RoundToMultiple(currentTile.x, WorldChunkManager.ChunkSize);
                yc=RoundToMultiple(currentTile.y, WorldChunkManager.ChunkSize);
                gotCorner = true;
            }
            if (xc == 0)
            {
                x = currentTile.x;

            }
            else
            {
                x = currentTile.x - (( xc));

            }
            if(yc == 0)
            {
                y = currentTile.y;

            }
            else
            {
                y = currentTile.y - (( yc));

            }
           
          

            tiles[x, y] = currentTile;
        }
        return tiles;
    }

    public static WallSegment[,] ParseWallSegments(string data)
    {
        WallSegment[,] retVal = new WallSegment[WorldChunkManager.ChunkSize, WorldChunkManager.ChunkSize];
        string[] objects = data.Split(SerializeDataHelpers.LIST_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        WallSegment currentTile = null;
        for(int x1 = 0; x1 < retVal.GetLength(0); x1++)
        {
            for(int y1 = 0; y1 < retVal.GetLength(1); y1++)
            {
                retVal[x1, y1] = new WallSegment(x1+currentLoadingChunkCoords.x, y1+currentLoadingChunkCoords.y, null);
            }
        }


        int x = 0, y = 0;
        bool gotCorner = false;
        int xc = 0, yc = 0;
        
        for (int q = 0; q < objects.Length; q++)
        {
            currentTile = ParseWallSegment(objects[q]);
            if (currentTile != null)
            {
                if (!gotCorner)
                {
                    xc = RoundToMultiple(currentTile.x, WorldChunkManager.ChunkSize);
                    yc = RoundToMultiple(currentTile.y, WorldChunkManager.ChunkSize);
                    gotCorner = true;
                }
                if (xc == 0)
                {
                    x = currentTile.x;

                }
                else
                {
                    x = currentTile.x - ((xc));

                }
                if (yc == 0)
                {
                    y = currentTile.y;

                }
                else
                {
                    y = currentTile.y - ((yc));

                }



                retVal[x, y] = currentTile;
            }
        }

        return retVal;
    }

    static WallSegment ParseWallSegment(string data)
    {
        Debug.Log("Wall segement data " + data);
        // ; UID; 5::COORDS; 7,17::WALL_TYPE; Wall::WALL_VISUAL; Concrete::HEALTH; 100::MAX_HEALTH; 100::
        string[] objects = data.Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string[] split = null;

        split = objects[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        ulong uid = (ulong)ParseDataObject(split[0], split[1]);
        split = objects[1].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);

        Vector2Int coords = (Vector2Int)ParseDataObject(split[0], split[1]);
        split = objects[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        WallType wallType = (WallType)ParseDataObject(split[0], split[1]);
        split = objects[3].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string wallVisualType = (string)ParseDataObject(split[0], split[1]);
        WallSegment retVal = new WallSegment(coords.x, coords.y, WallTypeManager.Instance.AllObjects[wallVisualType]);
        retVal.SetMyUID(uid);
        retVal.WallType = wallType;
        
        split = objects[4].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        float health = (float)ParseDataObject(split[0], split[1]);
        split = objects[5].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        float maxHealth = (float)ParseDataObject(split[0], split[1]);
        retVal.OverrideHealthValues(health, maxHealth);
        return retVal;
    }


    public static int RoundToMultiple(int value, int roundTo)
    {
        return Mathf.CeilToInt(value / roundTo) * roundTo;
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
