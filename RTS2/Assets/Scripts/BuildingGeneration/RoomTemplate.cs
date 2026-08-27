using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate", menuName = "ScriptableObjects/Room Template", order = 1)]
public class RoomTemplate : ScriptableObject
{
    public string RoomID;
    public string Wall, Door, Floor;
    public List<RoomTemplateProp> Props;
    public bool CanHaveWindows = false,CanHaveExternalDoor=false,CanBeGridBased=false,CanHaveInternalWalls=false,CanGenerateAnyWalls=true;
    public int MinWidth, MaxWidth, MinHeight, MaxHeight;

    public List<BuildingRoomData> AttachedRooms;

}
[System.Serializable]
public class RoomTemplateProp
{
    public string PropName;
    public int MaxQuantity;
    public bool NeedsEdge = false,MustBeOnRoomEdge=false;
}
