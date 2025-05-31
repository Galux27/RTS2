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
        return new SerializedData(retVal);
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
        cameraPos.z = -10f;
        CameraController.Instance.transform.position = cameraPos;
        CameraController.Instance.SetCameraZoom((float)deserializedData[DataKeys.CameraZoom]);
        DeltaTimeWrapper.GameplayDeltaMultiplier = (float)deserializedData[DataKeys.WaterLevel];
        IDManager.SetBaseUID((ulong)deserializedData[DataKeys.UID]);
    }
}
