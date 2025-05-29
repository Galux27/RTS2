using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomDeserializer
{
    public static void DeserializeRooms(string data)
    {
        string[] split = data.Split(SerializeDataHelpers.DATA_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        string[] KeyObjectSplit = null;
        Dictionary<string, object> deserializedData = new Dictionary<string, object>();
        for(int x=0;x<split.Length; x++)
        {
            KeyObjectSplit = split[x].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT);
            if (KeyObjectSplit.Length > 0)
            {
                KeyObjectSplit[1] = KeyObjectSplit[1].Replace(SerializeDataHelpers.DATA_ELEMENT_SPLIT.ToString(), "");
                deserializedData.Add(KeyObjectSplit[0], DataReaders.ParseDataObject(KeyObjectSplit[0], KeyObjectSplit[1]));
            }
        }
        Room r = RoomManager.Instance.CreateRoom((List<Vector2Int>)deserializedData[DataKeys.RoomTiles]);
        r.roomName = (string)deserializedData[DataKeys.RoomName];
        r.roomType = (RoomUseType)deserializedData[DataKeys.RoomType];
      
        
    }
}
