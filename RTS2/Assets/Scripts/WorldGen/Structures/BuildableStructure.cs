using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableStructure : Constructable,ObjectInfo
{

    public BuildableStructure(int x, int y, float maxProgress, bool forceComplete, 
        Action onComplete, Vector3 size, Vector3 offset,ConstructableType myType,string toConstruct)
    {
        this.x = x; this.y = y;
        this.maxProgress = maxProgress;
        pos=new Vector3(x,y,0);
        this.onComplete = onComplete;
        this.size= size;
        this.offset = offset;
        this.myType = myType;
        constructOnComplete = toConstruct;
        Vector2Int coords= WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(new Vector2Int(x,y));
        WorldChunkManager.Instance.Chunks[coords.x, coords.y].AddConstructable(this);
        if (forceComplete||DebugCheats.Instance.InstantConstruct())
        {
            OnObjectConstructed();
        }
        else
        {
            Render();
        }
    }
    string constructOnComplete;
    Action onComplete;
    public int x, y;
    float progress = 0f, maxProgress = 1f;
    Vector3 offset;

    Vector3 pos,size;
    bool isBuilt = false;


    public void ConstructObject()
    {
        if (Timer == null)
        {
            Timer = new Timer(maxProgress, progress);
            Timer.CreateProgressBarFromTimer(Object.transform.position);
        }
        Timer.ProgressTime( DeltaTimeWrapper.GameplayDelta);
        if (Timer.IsTimerFinished())
        {
            OnObjectConstructed();
        }
    }

    public void OnObjectConstructed()
    {
        SetBuilt(true);
        WorldChunkManager.Instance.OnBuildableFinished(this);
        Cleanup();
        onComplete?.Invoke();
    }

    public Vector3 GetPosition()
    {
       return pos+offset;
    }

   public float MaxDistToConstruct()
    {
        return 1f;
    }

    public void SetBuilt(bool val)
    {
       isBuilt= val;
       
    }

    public bool IsBuilt()
    {
        return isBuilt;
    }

    public Vector3 Size()
    {
        return size;
    }

    public bool isDrawn = false;
    GameObject Object;
    Timer Timer;



    public void Render()
    {
        if (isBuilt)
        {

            return;
        }
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("ConstructionMarker");
        Object.GetComponent<ConstructableObjectUI>().InitUI(size, pos+offset);
        Object.SetActive(true);
        isDrawn = true;
    }

    public void Cleanup()
    {   
        GameObjectPoolManager.Instance.ReturnObjectToPool(Object, "ConstructionMarker");
        Vector2Int coords= WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(new Vector2Int (x, y));
        WorldChunkManager.Instance.Chunks[coords.x, coords.y].RemoveConstructable(this,false);
        Object = null;
        isDrawn = false;
       
    }

        public bool IsDrawn()
    {
        return isDrawn;
    }

    public void OnHover()
    {
        if(Object != null)
        {
            Object.GetComponent<ConstructableObjectUI>().SetSpriteRendererColour(Color.green);
        }
    }

    public void OnHoverExit()
    {
        if (Object != null)
        {
            Object.GetComponent<ConstructableObjectUI>().SetSpriteRendererColour(Color.white);
        }
    }
    ConstructableType myType;
    ConstructableType Constructable.GetType()
    {
        return myType;
    }

    public void OnObjectDeselected()
    {
        if (Object != null)
        {
            Object.GetComponent<ConstructableObjectUI>().SetSpriteRendererColour(Color.white);
            Object.GetComponentInChildren<SelectedOutline>()?.OnDeselect();
        }
    }
    public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(Object,  GetSize(),GetSize()/2f);
    }

    SelectableType Selectable.GetSelectableType()
    {
        return SelectableType.ConstructableObject;
    }

    bool Selectable.GetIsSelected()
    {
        return selected;
    }
    bool selected = false;
    bool Selectable.IsSelectable()
    {
        return true;
    }

    void Selectable.SetIsSelected(bool val)
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
        return size;
    }

    bool Selectable.IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(size, pos+offset, point);
    }

    public string Name()
    {
        return "Under Construction " + myType.ToString()+"("+ constructOnComplete+")";
    }

    public string Description()
    {
        return "Under Construction " + myType.ToString()  +"(" + constructOnComplete + ")";
    }

    public int Quantitiy()
    {
        return 1;
    }

    public float Health()
    {
        return 1f;
    }

    public float MaxHealth()
    {
        return 1f;
    }

    public Vector3 Position()
    {
        return GetPosition();
    }

    public void AdjustHealth(float value)
    {
       
    }

    public void OnDeath()
    {
        
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();

        retVal.AddDataToSerialize(DataKeys.UID, GetMyUID().Value);
        retVal.AddDataToSerialize(DataKeys.Coords, new Vector2Int(x, y));
        retVal.AddDataToSerialize(DataKeys.Health, Health());
        retVal.AddDataToSerialize(DataKeys.MaxHealth, MaxHealth());
        retVal.AddDataToSerialize(DataKeys.CurrentProgress, Timer.GetCurrentTime);
        retVal.AddDataToSerialize(DataKeys.MaxProgress, Timer.TimeLimit);
        retVal.AddDataToSerialize(DataKeys.ObjectKey, constructOnComplete);

        return retVal;
    }

    public SerializedData Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(SerializedData data)
    {
        throw new NotImplementedException();
    }
    UID MyUID;
    public UID GetMyUID()
    {
        if (MyUID.Value == 0)
        {
            MyUID = IDManager.GetUIDForObject();
        }
        return MyUID;
    }
}

