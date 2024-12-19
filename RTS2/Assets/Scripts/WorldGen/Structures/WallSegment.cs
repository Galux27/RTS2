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
    public bool HasWallUnderConstruction=false;
    public WallType WallType=WallType.None;
    public WallSegment(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool HasWall
    {
        get
        {
            return WallType == WallType.Wall && HasWallUnderConstruction == false;
        }
    }


    public void SetHasWall(bool hasWall)
    {
        if (hasWall)
        {
            SetWallUnderConstruction(false);
            WallType = WallType.Wall;
        }
        else
        {
            DestroyWall();
        }
    }

    public void SetWallUnderConstruction(bool val,WallType typeOverride = WallType.None)
    {
        HasWallUnderConstruction = val;
        if (typeOverride != WallType.None)
        {
            WallType = typeOverride;
        }
    }

    public bool Drawn = false;
    public Tile ToDraw;
    public void SetTile(Tile tile)
    {
        ToDraw = tile;
        Drawn = true;
    }

    public virtual void DestroyWall()
    {
        ToDraw = null;
        WallType = WallType.None;
        Pathfinding.UpdateNodeData(x, y, true);
    }

}

public enum WallType 
{
    None,
    Wall,
    Door
}

