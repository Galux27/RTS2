using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class SerializationHelpers
{
    const string SaveDirectory = "ReclemationCorpSaves";
    const string WorldSectionExtension = ".RCWRLD",UnitsExtension=".RCUNIT";
    static string GetSaveFolderParentLocation()
    {
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    }

   static void EnsureDirectoryExists(string path)
    {
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static string GetWorldFilePath(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory,saveName, "CHUNK_TEST" + WorldSectionExtension);
    }
    public static string GetUnitFilePath(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, saveName, "UNITS" + UnitsExtension);

    }

    public static void SaveGame(string saveName)
    {
        EasyStopwatch.StartStopwatch();

        string path = Path.Combine(GetSaveFolderParentLocation(), SaveDirectory);
        EnsureDirectoryExists(path);
        path=Path.Combine(path,saveName);
        EnsureDirectoryExists(path);
        SaveLoadedWorld(path);
        SaveUnits(path);
        Debug.Log("Saving took " + EasyStopwatch.GetStopwatchElapsedTime() + "s");

    }

    public static void SaveUnits(string path)
    {
        string name = "UNITS" + UnitsExtension;
        List<string> dataWriting = new List<string>();
        for(int x = 0; x < UnitMoniter.Instance.AllUnits.Count; x++)
        {
            dataWriting.Add(UnitMoniter.Instance.AllUnits[x].Serialize().Data);
        }


        if (dataWriting.Count > 0)
        {
            WriteToFile(path, name, dataWriting);
        }
    }

        public static void SaveLoadedWorld(string path)
    {
        string name = "CHUNK_TEST" + WorldSectionExtension ;
        List<string> dataWriting = new List<string>();
        for (int x = 0; x < WorldChunkManager.Instance.Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < WorldChunkManager.Instance.Chunks.GetLength(1); y++)
            {
                dataWriting.Add(WorldChunkManager.Instance.Chunks[x, y].Serialize().Data);
            }
        }
        EasyStopwatch.StopStopwatch();
        WriteToFile(path,name,dataWriting);
    }

    public static List<string> ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        List<string> retVal = new List<string>();
        StreamReader sr = new StreamReader(path);
        string line=sr.ReadLine();
        while(line != null)
        {
            retVal.Add(line);
            line = sr.ReadLine();
        }
        sr.Close();
        sr.Dispose();
        return retVal;
    }

    public static void LoadGame(string name)
    {
        EasyStopwatch.StartStopwatch();
        WorldChunkManager.Instance.LoadChunksFromFile(name);
        ReadUnitFile(name);
        BehaviourDeserializer.DeserializeBehaviours();
   
        Debug.Log("reading took " + EasyStopwatch.GetStopwatchElapsedTime() + "s");
    }

    static void ReadUnitFile(string name)
    {
        List<string> dataFromFile = SerializationHelpers.ReadFile(SerializationHelpers.GetUnitFilePath("TestWorld"));
        for(int x=0;x<dataFromFile.Count;x++)
        {
            UnitPrefabController.Instance.CreateUnitFromSavedData(dataFromFile[x]);
        }
    }

   public static void WriteToFile(string path,string fileName,List<string> dataToWrite)
    {
        string fullPath = Path.Combine(path, fileName);
        if(File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }


        StreamWriter sw = new StreamWriter(fullPath);
        for(int x=0;x<dataToWrite.Count;x++)
        {
            sw.WriteLine(dataToWrite[x]);
        }
        sw.Dispose();
    }
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
    public const string TargetUID = "TARGET";
    public const string UIDType = "UID_TYPE";
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
    public const string UnitType = "UNIT_TYPE";
    public const string UnitFaction = "UNIT_FACTION";
    public const string RoomName = "ROOM_NAME";
    public const string RoomType = "ROOM_TYPE";
    public const string RoomTiles = "ROOM_TILES";
    public const string Behaviour = "BEHAVIOUR";
    public const string BehaviourType = "BEHAVIOUR_TYPE";
    public const string MiscString = "MISC_STRING";
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
            dataToWrite.Add(SerializeDataHelpers.DATA_OBJECT_SPLIT.ToString());
        }
       
        Data = SerializeDataHelpers.CombineStrings(dataToWrite);
    }
}

public static class SerializeDataHelpers 
{
    //splits data from the key
    public const char KEY_OBJECT_SPLIT = ';';
    //splits data that forms part of the same element
    public const char DATA_SPLIT = ',';
    //splits data from the next in the DataToSerialize instance
    public const char DATA_ELEMENT_SPLIT = ':';
    //splits data that is stored in the same list
    public const char LIST_ELEMENT_SPLIT = '`';
    //splits data on different objects in the same file
    public const char DATA_OBJECT_SPLIT = '^';

    public const char BEHAVIOUR_MARKER = '~';
    public static string SerializeData(string key,object value)
    {
        if (key == DataKeys.Coords)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), SerializeVector2Int(value), DATA_ELEMENT_SPLIT.ToString());
        }
        else if (key == DataKeys.Pos)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), SerializeVector3(value), DATA_ELEMENT_SPLIT.ToString());
        }
        else if (key == DataKeys.TileType || key == DataKeys.WaterLevel || key == DataKeys.WallType
            || key == DataKeys.WallVisual || key == DataKeys.Health || key == DataKeys.MaxHealth || key == DataKeys.UID ||
            key == DataKeys.ObjectKey || key == DataKeys.Quantitiy || key == DataKeys.ItemUID || key == DataKeys.CurrentProgress
            || key == DataKeys.MaxProgress || key == DataKeys.ConstructableType || key == DataKeys.UnitType || key == DataKeys.UnitFaction
            || key == DataKeys.RoomName || key == DataKeys.RoomType||key==DataKeys.BehaviourType||key==DataKeys.TargetUID||key==DataKeys.MiscString)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), value.ToString(), DATA_ELEMENT_SPLIT.ToString());
        } 
        else if (key == DataKeys.RoomTiles)
        {
            List<string> stored = new List<string>();
            List<Vector2Int> obj = (List<Vector2Int>)value;
            for(int x=0;x<obj.Count; x++)
            {
                stored.Add(SerializeVector2Int(obj[x]));
                stored.Add(LIST_ELEMENT_SPLIT.ToString());
            }
            stored.Add(DATA_ELEMENT_SPLIT.ToString());
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), stored);
        }
        else if (key == DataKeys.Behaviour)
        {
            BehaviourBase b = (BehaviourBase)value;
            if (b != null)
            {
                SerializedData data = b.Serialize();
                return CombineStrings(BEHAVIOUR_MARKER.ToString(),key, KEY_OBJECT_SPLIT.ToString(), data.Data, BEHAVIOUR_MARKER.ToString());
            }
            else
            {
                return CombineStrings(BEHAVIOUR_MARKER.ToString(),key, KEY_OBJECT_SPLIT.ToString(), "null", BEHAVIOUR_MARKER.ToString());

            }
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
                        stored.Add(DATA_ELEMENT_SPLIT.ToString());
                    }
                    stored.Add(LIST_ELEMENT_SPLIT.ToString());
                }
            }
            stored.Add(DATA_OBJECT_SPLIT.ToString());
            return CombineStrings(key,KEY_OBJECT_SPLIT.ToString(), stored);
        }else if (key == DataKeys.WallTiles
            || key == DataKeys.EnvironmentObjects
            ||key==DataKeys.Resources
            ||key==DataKeys.ItemsInContainer
            ||key==DataKeys.Constructables)
        {
            List<DataToSerialize> data = (List<DataToSerialize>)value;
            List<string> stored = new List<string>();
            for(int x = 0; x < data.Count; x++)
            {
                foreach (KeyValuePair<string, object> kvp in data[x].data)
                {
                    stored.Add(SerializeData(kvp.Key, kvp.Value));
                    stored.Add(DATA_ELEMENT_SPLIT.ToString());

                }
                stored.Add(LIST_ELEMENT_SPLIT.ToString());

            }

            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), stored);
        }
        Debug.LogError("Could not serialize " + key + " as its been assigned a serializer");
        return "";
    }

    static string SerializeVector2Int(object value)
    {
        Vector2Int val =(Vector2Int)value;
        return CombineStrings( val.x.ToString() , DATA_SPLIT.ToString(), val.y.ToString());
    }
    static string SerializeVector3(object value)
    {
        Vector3 val = (Vector3)value;
        return CombineStrings(val.x.ToString(), DATA_SPLIT.ToString(), val.y.ToString());
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




