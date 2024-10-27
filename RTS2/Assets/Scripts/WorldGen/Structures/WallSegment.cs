using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class to represent a tile of wall within the world
/// used to know the location to then identify what tiles are going to be used
/// </summary>
public class WallSegment
{
    public int x, y;
    public bool HasWall = false;
    public WallSegment(int x, int y, bool hasWall)
    {
        this.x = x;
        this.y = y;
        HasWall = hasWall;
    }

    public void SetHasWall(bool hasWall)
    {
        this.HasWall = hasWall;
    }


    public bool Drawn = false;
    public Tile ToDraw;
    public void SetTile(Tile tile)
    {
        if (Input.GetKey(KeyCode.LeftControl) && ToDraw!=tile)
        {
            Debug.Log("Set tile " + x + "," + y + " to " + tile.sprite.name.ToString());
        }


        ToDraw = tile;
        Drawn = true;
    }
}
