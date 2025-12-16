using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MiscDataSerialization
{
   public static SerializedData GetMiscData()
   {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.Pos, CameraController.Instance.transform.position);
        retVal.AddDataToSerialize(DataKeys.CameraZoom, CameraController.Instance.GetCameraZoom());
        retVal.AddDataToSerialize(DataKeys.UID, IDManager.GetCurrentID());
        retVal.AddDataToSerialize(DataKeys.WaterLevel, PauseMenuUIElement.SpeedGameAtWhenOpened);
        retVal.AddDataToSerialize(DataKeys.ResourcesStored, ResourceManager.Instance.UserResources);
        retVal.AddDataToSerialize(DataKeys.GenStart,OverworldGenerator.Instance.GetOverworldStartingCoords());
        return new SerializedData(retVal);
   }
    static bool DisplayedOne = false;
    public static void DeserialzieOverworld(List<string> data)
    {
        string[] overworld = data[0].Substring(4, data[0].Length-4).Split(SerializeDataHelpers.LIST_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        OverworldTile[,] overworldData = new OverworldTile[OverworldGenerator.Instance.OverworldWidth, OverworldGenerator.Instance.OverworldHeight];


        OverworldTile toAdd = null;
        for (int x=0;x< overworld.Length-1; x++)
        {
            toAdd = DeserializeData(overworld[x]);
            overworldData[toAdd.X, toAdd.Y] = toAdd;
        }
        OverworldGenerator.Instance.OverworldTiles = overworldData;
        OverworldRenderer.Instance.RenderWorld();
    }

    static OverworldTile DeserializeData(string data)
    {
        string[] tileData = data.Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        //CD;22,289::O_ELE;55.5206::}O_FET;OKEY;Settlement:]|OKEY;MajorRoad:]|OKEY;MinorRoad:]|::O_POP;85::O_SET;17:

        Dictionary<string, object> deserializedData = new Dictionary<string, object>();

        for (int x = 0; x < tileData.Length; x++)
        {
            string[] keyObjectSplit = tileData[x].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
            if (keyObjectSplit.Length == 2)
            {
                deserializedData.Add(keyObjectSplit[0], DataReaders.ParseDataObject(keyObjectSplit[0], keyObjectSplit[1]));
            }
        }
        Vector2Int coords = (Vector2Int)deserializedData[DataKeys.Coords];
        OverworldTile retVal = new OverworldTile(coords.x, coords.y, (float)deserializedData[DataKeys.OverElevation]);
        if (deserializedData.ContainsKey(DataKeys.OverFeature))
        {
            retVal.Features = (List<OverworldFeature>)deserializedData[DataKeys.OverFeature];
        }
        retVal.RiverPoint = (Vector2Int)deserializedData[DataKeys.OverRiverCoords];
            return retVal;
    }


    public static void DeserializeMiscData(List<string> data)
    {
        string[] split = data[0].Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string[] keyObjectSplit = null;
        Dictionary<string,object> deserializedData = new Dictionary<string,object>();
        for(int x=0;x<split.Length;x++)
        {
            split[x] = split[x].Replace(SerializeDataHelpers.DATA_OBJECT_SPLIT.ToString(), "");
            keyObjectSplit = split[x].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
            if (keyObjectSplit.Length > 1)
            {
                deserializedData.Add(keyObjectSplit[0], DataReaders.ParseDataObject(keyObjectSplit[0], keyObjectSplit[1]));
            }
        }
        Vector3 cameraPos = (Vector3)deserializedData[DataKeys.Pos];
        OverworldGenerator.Instance.SetOverworldStartingCoords((Vector2Int)deserializedData[DataKeys.GenStart]);
        cameraPos.z = -10f;
        CameraController.Instance.transform.position = cameraPos;
        CameraController.Instance.SetCameraZoom((float)deserializedData[DataKeys.CameraZoom]);
        DeltaTimeWrapper.GameplayDeltaMultiplier = (float)deserializedData[DataKeys.WaterLevel];
        ResourceManager.Instance.SetUserResources( (Dictionary<string, ResourceData>)deserializedData[DataKeys.ResourcesStored]);
        IDManager.SetBaseUID((ulong)deserializedData[DataKeys.UID]);
    }
}
