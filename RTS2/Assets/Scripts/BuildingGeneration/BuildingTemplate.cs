using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTemplate", menuName = "ScriptableObjects/Building Template", order = 1)]
public class BuildingTemplate : ScriptableObject
{
    public string BuildingName;
    public List<BuildingRoomData> PotentialRooms;
    public int MinWidth, MaxWidth, MinHeight, MaxHeight;
    public int MinRooms, MaxRooms;
    public int MaxExternalDoors;
    public string CorridorFloor, CorridorWall;
   public BuildingRoomData GetDataByID(string id)
    {
        for(int x=0;x<PotentialRooms.Count;x++)
        {
            if (PotentialRooms[x].roomTemplate.RoomID == id)
            {
                return PotentialRooms[x];
            }
        }
        return null;
    }

  
}
[System.Serializable]
public class BuildingRoomData
{
    public RoomTemplate roomTemplate;
    public int Min = 0, Max = 1;
}
