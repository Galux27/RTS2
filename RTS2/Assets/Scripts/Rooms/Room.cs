using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public List<Vector2Int> tilesInRoom = new List<Vector2Int>();
    public RoomType roomType;

    public void AddTiles(List<Vector2Int> tilesInRoom)
    {
        tilesInRoom.AddRange(tilesInRoom);
    }

    public void RemoveTiles(List<Vector2Int> tilesInRoom)
    {
        for(int i = 0; i < tilesInRoom.Count; i++)
        {
            tilesInRoom.Remove(tilesInRoom[i]);
        }
    }
}

public enum RoomType 
{
    None,
    Barracks,
    Warehouse
}

