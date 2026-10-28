using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTemplate", menuName = "ScriptableObjects/Building Template", order = 1)]
public class BuildingTemplate : ScriptableObject
{
    public string BuildingName;
    public List<BuildingRoomData> PotentialRooms;
    public BuildingRoomData MainRoom;
    public int MinWidth, MaxWidth, MinHeight, MaxHeight;
    public int MinRooms, MaxRooms;
    public int MaxExternalDoors;
    public string CorridorFloor, CorridorWall,ExternalWall;
    public BuildingFloorplanType FloorplanType;
    public bool ShiftHorizontalFloorTiles,ShiftVerticalFloorTiles;
    Dictionary<string, int> PotentialRoomIndexes = null;
    void GeneratePotentialRoomIndexes()
    {
        PotentialRoomIndexes = new Dictionary<string, int>();
        for (int x = 0; x < PotentialRooms.Count; x++)
        {
            PotentialRoomIndexes.Add(PotentialRooms[x].roomTemplate.RoomID, x);
            
        }
    }
   public BuildingRoomData GetDataByID(string id)
    {
        if (PotentialRoomIndexes == null)
        {
            GeneratePotentialRoomIndexes();
        }
        if (PotentialRoomIndexes.ContainsKey(id))
        {
            return PotentialRooms[PotentialRoomIndexes[id]];
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
