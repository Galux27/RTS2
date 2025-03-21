using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class to represent a tile of wall within the world
/// used to know the location to then identify what tiles are going to be used
/// </summary>
public class WallSegment:Selectable ,ObjectInfo
{
    public int x, y;
    public bool HasWallUnderConstruction=false;
    public WallType WallType=WallType.None;
    public GameObject Collider;
    public float HealthVal, MaxHealthVal;
    WallTile wallType;
    public WallSegment(int x, int y,WallTile wallType)
    {
        this.x = x;
        this.y = y;
        if (wallType != null)
        {
            this.HealthVal = wallType.Health;
            this.MaxHealthVal = HealthVal;
            this.wallType = wallType;
        }
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

    public void SetWallType(WallTile wallType)
    {
        if (wallType != null)
        {
            this.HealthVal = wallType.Health;
            this.MaxHealthVal = HealthVal;
            this.wallType = wallType;
            
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
        Collider.gameObject.GetComponentInChildren<SelectedOutline>()?.OnDeselect();
    }
    public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(Collider,GetSize());
    }


    public SelectableType GetSelectableType()
    {
        return SelectableType.Structure;
    }

    public bool GetIsSelected()
    {
        return selected;
    }
    bool selected = false;
    public bool IsSelectable()
    {
        return HasDoor||HasWall;
    }

    public void SetIsSelected(bool val)
    {
        if (val)
        {
            OnObjectSelected();
        }
        else
        {
            OnObjectDeselected();
        }
        selected = val;
    }

    public Vector3 GetSize()
    {
        return Vector3.one;
    }

    public bool IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(GetSize(),new Vector3(x,y,0), point);
    }

    public string Name()
    {
        if (HasDoor)
        {
            return "Door";
        }else if (HasWall)
        {
            return "Wall";
        }
        return "Empty";
    }

    public string Description()
    {
        return wallType.WallName + " " + Health() + "/" + MaxHealth() ;
    }

    public int Quantitiy()
    {
        return 1;
    }

    public float Health()
    {
        return HealthVal;
    }

    public float MaxHealth()
    {
        return MaxHealthVal;
    }

    public Vector3 Position()
    {
        return new Vector3(x+.5f, y+.5f, 0);
    }

   public void AdjustHealth(float value)
    {
        HealthVal += value;
        Debug.Log("Wall damage,health at " + HealthVal);
        if (HealthVal < 0 &&WallType!=WallType.None)
        {
            OnDeath();
        }
    }

   public void OnDeath()
    {
        WorldController.Instance.WallManager.RemoveSingleWall(x, y, WorldController.Instance.BuildingTilemap, WorldController.Instance.WallTest);
    }
}

public enum WallType 
{
    None,
    Wall,
    Door
}

