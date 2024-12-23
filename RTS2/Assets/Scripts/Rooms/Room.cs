using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string roomName = "";
    public Color displayColour;
    public List<Vector2Int> tilesInRoom = new List<Vector2Int>();
    public RoomType roomType;

    public Room()
    {
        Debug.Log("Room: created new room " + tilesInRoom.Count);

        roomName = "Room " + RoomManager.Instance.roomList.Count;
        displayColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), .25f);
    }


    public void AddTiles(List<Vector2Int> tilesInRoom)
    {
        Debug.Log("Room: adding tiles " + tilesInRoom.Count);
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
}

public enum RoomType 
{
    None,
    Barracks,
    Warehouse
}

