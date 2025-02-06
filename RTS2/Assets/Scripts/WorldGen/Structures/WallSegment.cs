using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class to represent a tile of wall within the world
/// used to know the location to then identify what tiles are going to be used
/// </summary>
public class WallSegment:Selectable 
{
    public int x, y;
    public bool HasWallUnderConstruction=false;
    public WallType WallType=WallType.None;
    public GameObject Collider;
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

    public bool HasDoor
    {
        get
        {
            return WallType == WallType.Door && HasWallUnderConstruction == false;

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

    public void OnObjectDeselected()
    {
        Collider.gameObject.GetComponent<SelectedOutline>()?.OnDeselect();
    }
    public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(Collider,GetSize());
    }


    public SelectableType GetSelectableType()
    {
        throw new System.NotImplementedException();
    }

    public bool GetIsSelected()
    {
        throw new System.NotImplementedException();
    }

    public bool IsSelectable()
    {
        throw new System.NotImplementedException();
    }

    public void SetIsSelected(bool val)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetSize()
    {
        return Vector3.one;
    }

    public bool IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(GetSize(),new Vector3(x,y,0), point);
    }
}

public enum WallType 
{
    None,
    Wall,
    Door
}

