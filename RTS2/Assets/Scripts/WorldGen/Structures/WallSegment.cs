using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class to represent a tile of wall within the world
/// used to know the location to then identify what tiles are going to be used
/// </summary>
public class WallSegment:Selectable ,ObjectInfo,ISerialize
{
    public int x, y;
    public Vector2Int localCoords;
    public bool HasWallUnderConstruction=false;
    public WallType WallType=WallType.None;
    public GameObject Collider;
   public WallTile baseWallType;
    public WallSegment(int x, int y,WallTile wallType,int localX,int localY)
    {
        Init(x,y,wallType,localX,localY);
    }

    public void Init(int x, int y, WallTile wallType, int localX, int localY)
    {
        this.x = x;
        this.y = y;
      
        

        if (wallType != null)
        {
            MyHealth = new ObjectHealth();
            MyHealth.MaxHealth = wallType.Health;
            MyHealth.CurrentHealth = wallType.Health;
            this.baseWallType = wallType;
        }
       
        localCoords = new Vector2Int(localX, localY);
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
            MyHealth = new ObjectHealth();
            MyHealth.MaxHealth = wallType.Health;
            MyHealth.CurrentHealth = wallType.Health;
            this.baseWallType = wallType;
            
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
      
        Debug.Log("Set tile sprite for " + x + "," + y + " to " + tile.sprite.name.ToString()+"|"+WallType);
        ToDraw = tile;
    }

    public void RenderWall()
    {
        WorldController.Instance.BuildingTilemap.SetTile(new Vector3Int(x, y, 0), ToDraw);
        Drawn = true;
    }

    public void UnRender()
    {
        WorldController.Instance.BuildingTilemap.SetTile(new Vector3Int(x, y, 0), null) ;

        Drawn = false;
    }

    public virtual void DestroyWall()
    {
        ToDraw = null;
        Drawn = false;
        WallType = WallType.None;
        Vector2Int coords = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(Position());
        Pathfinding.UpdateNodeData(x, y, true);
        if (Collider != null)
        {
            GameObject.Destroy(Collider);
        }
    }

    public void OnObjectDeselected()
    {
        if (Collider!=null && Collider.gameObject.GetComponentInChildren<SelectedOutline>())
        {
            Collider.gameObject.GetComponentInChildren<SelectedOutline>()?.OnDeselect();
        }
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
        return baseWallType.WallName + " " + Health() + "/" + MaxHealth() ;
    }

    public int Quantitiy()
    {
        return 1;
    }
    public ObjectHealth MyHealth;
    public void OverrideHealthValues(float val, float max)
    {
        MyHealth.MaxHealth = max;
        MyHealth.CurrentHealth = val;
    }


    public float Health()
    {
        if (MyHealth == null)
        {
            return 0f;
        }
        return MyHealth.MaxHealth;
    }

    public float MaxHealth()
    {
        if (MyHealth == null)
        {
            return 0f;
        }
        return MyHealth.MaxHealth;
    }


    public Vector3 Position()
    {
        return new Vector3(x+.5f, y+.5f, 0);
    }

   public void AdjustHealth(float value)
    {
        if (value > 0)
        {
            MyHealth.IncreaseHealth(value);
        }
        else
        {
            MyHealth.DecreaseHealth(value);
        }

        if (Health() > MaxHealth())
        {
            MyHealth.CurrentHealth = MaxHealth();
        }
        else if (Health() < 0)
        {
            OnDeath();
        }


        if (healthUI == null)
        {
            DrawHealthUI();
        }
        UpdateHealthUI();
    }

    HealthUI healthUI;
    void DrawHealthUI()
    {
        if (!Drawn)
        {
            return;
        }
        healthUI = GameObjectPoolManager.Instance.GetObjectFromPool("WorldspaceHealthBar").GetComponent<HealthUI>();
        healthUI.gameObject.SetActive(true);
        healthUI.LinkToObjectInfo(this);
    }

    void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHealth();
        }
    }

    public void OnDeath()
    {
        WorldController.Instance.WallManager.RemoveSingleWall(x, y, WorldController.Instance.BuildingTilemap, WallTypeManager.Instance.SelectedWallTile);
        if (healthUI != null)
        {
            GameObjectPoolManager.Instance.ReturnObjectToPool(healthUI.gameObject, "WorldspaceHealthBar");
        }
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.UID, GetMyUID().Value);
        retVal.AddDataToSerialize(DataKeys.Coords, new Vector2Int(x, y));
        retVal.AddDataToSerialize(DataKeys.WallType, (int)WallType);
        retVal.AddDataToSerialize(DataKeys.WallVisual, baseWallType.WallName);
        retVal.AddDataToSerialize(DataKeys.Health, Health());
        retVal.AddDataToSerialize(DataKeys.MaxHealth, MaxHealth());
        retVal.AddDataToSerialize(DataKeys.LocalCoords, localCoords);
        return retVal;
    }

    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }
    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value == 0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);

        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
        myUid = new UID(uid);
        IDManager.OnUIDCreated(this, myUid);
    }

    public UID MyUID()
    {
        return GetMyUID();
    }
}

public enum WallType 
{
    None,
    Wall,
    Door
}

