using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string roomName = "";
    public Color displayColour;
    public List<Vector2Int> tilesInRoom = new List<Vector2Int>();
    public RoomUseType roomType;

    public Room()
    {
        roomName = "Room " + RoomManager.Instance.roomList.Count;
        displayColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), .25f);
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
    }

    public void RemoveTiles(List<Vector2Int> tilesInRoom)
    {
        for(int i = 0; i < tilesInRoom.Count; i++)
        {
            this.tilesInRoom.Remove(tilesInRoom[i]);
        }

    }


    public string GetDetailsForRoom()
    {
        return "Room Size: " + tilesInRoom.Count + " tiles" ;
    }

    public string GetValidityDetailsForRoom()
    {
        RoomUtils.IsValid(this);
        return "";
    }
}

public enum RoomUseType 
{
    None,
    Barracks,
    Warehouse
}

