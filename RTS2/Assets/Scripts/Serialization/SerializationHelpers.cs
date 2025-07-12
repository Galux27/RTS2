using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class SerializationHelpers
{
   public const string SaveDirectory = "ReclemationCorpSaves",WorkingDir= "ReclemationCorpWorkingDir";
   public const string WorldSectionExtension = ".RCWRLD",UnitsExtension=".RCUNIT",MiscExtension=".RCMISC",RoomExtension=".RCROOM";
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

    public static string GetSaveDirectory(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, saveName);
    }

    public static string GetWorldChunkBatchFilePathFromWorkingCopy(Vector2Int coords)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, WorkingDir, "_" + coords.x + "_" + coords.y + WorldSectionExtension);

    }

    public static string GetWorldChunkBatchFilePath(Vector2Int coords,string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, saveName, "_"+coords.x+"_"+coords.y + WorldSectionExtension);

    }

    public static string GetWorldFilePath(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory,saveName, "CHUNK_TEST" + WorldSectionExtension);
    }
    public static string GetUnitFilePath(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, saveName, "UNITS" + UnitsExtension);

    }
    public static string GetRoomFilePath(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, saveName, "ROOMS" + RoomExtension);

    }
    public static string GetMiscFilePath(string saveName)
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, saveName, "MISC" + MiscExtension);

    }

    public static string GetWorkingCopyDirectory()
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory, WorkingDir);
    }

    public static string GetSaveDir()
    {
        return Path.Combine(GetSaveFolderParentLocation(), SaveDirectory);
    }

    public static void SaveGame(string saveName)
    {
        EasyStopwatch.StartStopwatch();

        string path = GetSaveDir();
        EnsureDirectoryExists(path);
        path=Path.Combine(path,saveName);
        EnsureDirectoryExists(path);
        SaveMiscData(path);
        SaveLoadedWorld(path);
        SaveUnits(path);
        SaveRooms(path);
        CopyWorkingCopyToSaveDir(path);
        Debug.Log("Saving took " + EasyStopwatch.GetStopwatchElapsedTime() + "s");

    }

    static void CopyWorkingCopyToSaveDir(string saveName)
    {
        string workingPath = GetWorkingCopyDirectory();
        if (Directory.Exists(workingPath))
        {
            string savePath = GetSaveDir();
            savePath = Path.Combine(savePath, saveName);
            string[] alLFiles = Directory.GetFiles(workingPath);
            string fileName = "", destFilePath = "";
            for (int x = 0; x < alLFiles.Length; x++)
            {
                fileName = Path.GetFileName(alLFiles[x]);
                destFilePath = Path.Combine(workingPath, fileName);
                File.Copy(alLFiles[x], destFilePath);
            }
        }
        }
        static void ClearWorkingCopyDirectory()
    {
        string workingPath = GetWorkingCopyDirectory();
        if (Directory.Exists(workingPath))
        {
            Directory.Delete(workingPath, true);
        }
        }
        public static void SaveMiscData(string path)
    {
        string name = "MISC" + MiscExtension;
        List<string> dataWriting = new List<string>();
        dataWriting.Add(MiscDataSerialization.GetMiscData().Data);
        WriteToFile(path, name, dataWriting);

    }


    public static void SaveRooms(string path)
    {
        string name = "ROOMS" + RoomExtension;
        List<string> dataWriting = new List<string>();
        for (int x = 0; x < RoomManager.Instance.roomList.Count; x++)
        {
            dataWriting.Add(RoomManager.Instance.roomList[x].Serialize().Data);
        }


        if (dataWriting.Count > 0)
        {
            WriteToFile(path, name, dataWriting);
        }
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

    public static void SaveChunkBatchToWorkingCopy(WorldChunkBatch wb)
    {
        string path = GetWorkingCopyDirectory();
        if (Directory.Exists(path) ==false)
        {
           Directory.CreateDirectory(path);
        }
        List<string> dataWriting = new List<string>();
        string name = "_" + wb.coords.x + "_" + wb.coords.y + WorldSectionExtension;
        for (int x = 0; x < wb.Chunks.GetLength(0); x++)
        {
            for (int y = 0; y < wb.Chunks.GetLength(1); y++)
            {
                dataWriting.Add(wb.Chunks[x, y].Serialize().Data);
            }
        }
        WriteToFile(path, name, dataWriting);
        WorldChunkManager.Instance.AddChunkStoredInWorkingCopy(wb.coords);
    }


    public static void SaveLoadedWorld(string path)
    {
        string name = "";
        List<string> dataWriting = new List<string>();

        foreach(KeyValuePair<Vector2Int,WorldChunkBatch> kvp in WorldChunkManager.Instance.ChunkBatches)
        {
            name = "_"+kvp.Value.coords.x+"_"+kvp.Value.coords.y + WorldSectionExtension;
            for (int x = 0; x < kvp.Value.Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < kvp.Value.Chunks.GetLength(1); y++)
                {
                    dataWriting.Add(kvp.Value.Chunks[x, y].Serialize().Data);
                }
            }
            WriteToFile(path, name, dataWriting);
            dataWriting.Clear();
        }


        EasyStopwatch.StopStopwatch();
        Debug.Log("Saving Chunks took " + EasyStopwatch.GetStopwatchElapsedTime());
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
        Debug.Log("Loading game " + name);
        EasyStopwatch.StartStopwatch();
        ClearWorkingCopyDirectory();
        ReadMiscFile(name);

        IDManager.OnLevelLoaded();
        //shit workaround to instance overwriting 0,0
        GameObject.FindObjectOfType<WorldChunkManager>().LoadChunksFromFile(name);
       // WorldChunkManager.Instance.LoadChunksFromFile(name);
        ReadUnitFile(name);
        BehaviourDeserializer.DeserializeBehaviours();
        InventoryDeserializer.DeserializeInventorys();
        ReadRoomsFile(name);

        for (int x=0;x<RoomManager.Instance.roomList.Count;x++)
        {
            RoomManager.Instance.roomList[x].RefreshRoom();
        }
        Debug.Log("reading took " + EasyStopwatch.GetStopwatchElapsedTime() + "s");
    }


    static void ReadMiscFile(string name)
    {
        List<string> dataFromFile = SerializationHelpers.ReadFile(SerializationHelpers.GetMiscFilePath(name));
        MiscDataSerialization.DeserializeMiscData(dataFromFile);

    }

    static void ReadRoomsFile(string name)
    {
        if (!File.Exists(SerializationHelpers.GetRoomFilePath(name)))
        {
            return;
        }
        List<string> dataFromFile = SerializationHelpers.ReadFile(SerializationHelpers.GetRoomFilePath(name));
        for (int x = 0; x < dataFromFile.Count; x++)
        {
            RoomDeserializer.DeserializeRooms(dataFromFile[x]);
        }
    }

    static void ReadUnitFile(string name)
    {
        if (!File.Exists(SerializationHelpers.GetUnitFilePath(name)))
        {
            return;
        }
        List<string> dataFromFile = SerializationHelpers.ReadFile(SerializationHelpers.GetUnitFilePath(name));
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
    public const string Coords = "CD";
    public const string LocalCoords = "LCD";
    public const string Pos = "POS";
    public const string TileType = "TT";
    public const string WaterLevel = "WLL";
    public const string ChunkTiles = "CT";//todo
    public const string WallType = "WTY";
    public const string WallVisual = "WV";
    public const string Health = "HP";
    public const string MaxHealth = "MHP";
    public const string WallTiles = "WT";//todo
    public const string UID = "UID";
    public const string TargetUID = "TGT";
    public const string UIDType = "UIDT";
    public const string ObjectKey = "OKEY";
    public const string EnvironmentObjects = "EOBJ";//todo
    public const string Quantitiy = "QUA";
    public const string Resources = "ROBJ";//todo
    public const string ResourcesStored = "RESST";
    public const string ItemUID = "ITEM_UID";
    public const string ItemsInContainer = "CC";//todo
    public const string CurrentProgress = "CPR";
    public const string MaxProgress = "MPR";
    public const string ConstructableType = "CNTY";
    public const string Constructables = "CN";//todo
    public const string UnitType = "UT";
    public const string UnitFaction = "UF";
    public const string RoomName = "RN";
    public const string RoomType = "RT";
    public const string RoomTiles = "RTLS";
    public const string Behaviour = "BH";
    public const string BehaviourType = "BHT";
    public const string MiscString = "MSCT";
    public const string CameraZoom = "ZM";
    public const string IDMax = "ID_MAX";
    public const string InventoryUID = "INVID";
    public const string Inventory = "INV";
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

    public SerializedData(DataToSerialize data, bool addObjectSplit = true)
    {
        List<string> dataToWrite = new List<string>();
        foreach(KeyValuePair<string,object> pair in data.data)
        {
            dataToWrite.Add(SerializeDataHelpers.SerializeData(pair.Key, pair.Value));
            if (addObjectSplit)
            {
                dataToWrite.Add(SerializeDataHelpers.DATA_OBJECT_SPLIT.ToString());
            }
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

    public const char INVENTORY_ELEMENT_SPLIT = '|';
    public const char INVENTORY_MARKER = '[';
    public const char INVENTORY_MARKER_TWO = '}';

    public const char INVENTORY_SPLIT_TWO = ']';
    public static string SerializeData(string key,object value)
    {
        if (key == DataKeys.Coords||key==DataKeys.LocalCoords)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), SerializeVector2Int(value), DATA_ELEMENT_SPLIT.ToString());
        }
        else if (key == DataKeys.Pos)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), SerializeVector3(value), DATA_ELEMENT_SPLIT.ToString());
        }
        else if(key == DataKeys.ItemsInContainer)
        {
            return CombineStrings(INVENTORY_MARKER_TWO.ToString(),key, KEY_OBJECT_SPLIT.ToString(), value.ToString(), DATA_ELEMENT_SPLIT.ToString());
        }
        else if (key == DataKeys.TileType || key == DataKeys.WaterLevel || key == DataKeys.WallType
            || key == DataKeys.WallVisual || key == DataKeys.Health || key == DataKeys.MaxHealth || key == DataKeys.UID ||
            key == DataKeys.ObjectKey || key == DataKeys.Quantitiy || key == DataKeys.ItemUID || key == DataKeys.CurrentProgress
            || key == DataKeys.MaxProgress || key == DataKeys.ConstructableType || key == DataKeys.UnitType || key == DataKeys.UnitFaction
             ||key == DataKeys.RoomName || key == DataKeys.RoomType||key==DataKeys.BehaviourType
            ||key==DataKeys.TargetUID|| key == DataKeys.InventoryUID || key==DataKeys.MiscString||key==DataKeys.CameraZoom)
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
        }else if (key == DataKeys.Inventory)
        {
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), INVENTORY_MARKER.ToString(), value.ToString(), INVENTORY_MARKER.ToString());
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
            ||key==DataKeys.Constructables)
        {
            List<DataToSerialize> data = (List<DataToSerialize>)value;
            List<string> stored = new List<string>();
            for (int x = 0; x < data.Count; x++)
            {
                foreach (KeyValuePair<string, object> kvp in data[x].data)
                {
                    stored.Add(SerializeData(kvp.Key, kvp.Value));
                    stored.Add(DATA_ELEMENT_SPLIT.ToString());

                }
                stored.Add(LIST_ELEMENT_SPLIT.ToString());

            }
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), stored);
        }else if (key == DataKeys.ResourcesStored)
        {
            List<string> data = new List<string>();
            Dictionary<string,ResourceData> stored = (Dictionary<string, ResourceData>)value;
            foreach(KeyValuePair<string,ResourceData> kvp in stored)
            {
                data.Add(kvp.Key);
                data.Add(DATA_SPLIT.ToString());
                data.Add(kvp.Value.Quantity.ToString());
                data.Add(LIST_ELEMENT_SPLIT.ToString());
            }
            return CombineStrings(key, KEY_OBJECT_SPLIT.ToString(), data);
        }
        Debug.LogError("Could not serialize " + key + " as its been assigned a serializer");
        return "";
    }

    public static string SerializeListOfData(List<DataToSerialize> data)
    {
        List<string> stored = new List<string>();
        for (int x = 0; x < data.Count; x++)
        {
            foreach (KeyValuePair<string, object> kvp in data[x].data)
            {
                stored.Add(SerializeData(kvp.Key, kvp.Value));
                stored.Add(INVENTORY_SPLIT_TWO.ToString());

            }
            stored.Add(INVENTORY_ELEMENT_SPLIT.ToString());

        }

        return CombineStrings( stored);
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




