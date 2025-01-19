using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string roomName = "";
    public Color displayColour;
    public List<Vector2Int> tilesInRoom = new List<Vector2Int>();
    public RoomUseType roomType;
    public List<Vector2Int> EdgeTiles,InvalidEdge;
    public static Action<Room> OnRoomChanged;
    public Room()
    {
        roomName = "Room " + RoomManager.Instance.roomList.Count;
        displayColour = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), .25f);
    }


    public void AddTiles(List<Vector2Int> tilesInRoom)
    {
        for(int x=0;x<tilesInRoom.Count;x++)
        {
            if (!this.tilesInRoom.Contains(tilesInRoom[x]))
            {
                this.tilesInRoom.Add(tilesInRoom[x]);
            }
        }
        OnRoomChanged?.Invoke(this);
        GetValidityDetailsForRoom(this);
    }

    public void RemoveTiles(List<Vector2Int> tilesInRoom)
    {
        for(int i = 0; i < tilesInRoom.Count; i++)
        {
            this.tilesInRoom.Remove(tilesInRoom[i]);
        }
        OnRoomChanged?.Invoke(this);
        GetValidityDetailsForRoom(this);


    }


    public string GetDetailsForRoom()
    {
        return "Room Size: " + tilesInRoom.Count + " tiles" ;
    }


    public bool DoesRoomHaveNeededObjects()
    {
        return RoomUtils.IsValid(this);
    }

    public string GetValidityDetailsForRoom(Room r)
    {
        bool isValid = RoomUtils.IsValid(this);

        if (isValid)
        {
            return "True";
        }
        else
        {
            return isValid.ToString()+RoomUtils.GetValiditiyIssues(this);
        }
    }

    bool CanUseRoomValue = false;
    public virtual bool CanUseRoom()
    {
        return CanUseRoomValue;
    }
    

    public virtual void SetCanUseRoom(bool value)
    {
        CanUseRoomValue = value;
    }
}

public enum RoomUseType 
{
    None,
    Barracks,
    Warehouse
}

