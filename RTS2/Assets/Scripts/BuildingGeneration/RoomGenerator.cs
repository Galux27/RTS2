using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator 
{
    public virtual GeneratedRoom GenerateRoom(Vector2Int pos,Vector2Int size,RoomTemplate template)
    {
        GeneratedRoom room = new GeneratedRoom(size, pos);
        PopulateWallTiles(room, template);
        PopulateFloorTiles(room, template);

        return room;
    }

    void PopulateFloorTiles(GeneratedRoom room, RoomTemplate template)
    {
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);
        for (int x = 0; x < width; x++)
        {
          for(int y = 0; y < height; y++)
            {
                room.RoomTiles[x, y].SetFloor(template.Floor);
            }

        }
    }

    void PopulateWallTiles(GeneratedRoom room,RoomTemplate template)
    {
        int width = room.RoomTiles.GetLength(0);
        int height = room.RoomTiles.GetLength(1);
        for(int x=0;x<width; x++)
        {
            room.RoomTiles[x, 0].SetWall(template.Wall);
            room.RoomTiles[x, height-1].SetWall(template.Wall);

        }

        for (int x = 0; x < height; x++)
        {
            room.RoomTiles[0, x].SetWall(template.Wall);
            room.RoomTiles[width-1, x].SetWall(template.Wall);

        }
    }
}

public class GeneratedRoom
{
    public RoomTile[,] RoomTiles;
    public Vector2Int Position;
    public Vector2Int size;
    public GeneratedRoom(Vector2Int size,Vector2Int pos)
    {
        RoomTiles = new RoomTile[size.x, size.y];
        for(int x=0; x<size.x; x++)
        {
            for(int y=0; y<size.y; y++)
            {
                RoomTiles[x,y]= new RoomTile(); 
            }
        }
        Position = pos;
        this.size = size;
    }
}

public class RoomTile
{
    public string FloorTile, WallTile,DoorTile;
    public bool HasWall = false, HasFloor = false, HasDoor = false;
    public void SetWall(string type)
    {
        WallTile = type;
        HasWall = true;
    }

    public void SetDoor(string type)
    {
        DoorTile = type;
        HasDoor = true;
    }

    public void SetFloor(string type)
    {
        FloorTile = type;
        HasFloor = true;
    }

}