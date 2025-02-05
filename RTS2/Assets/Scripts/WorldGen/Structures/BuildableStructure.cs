using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableStructure : Constructable
{

    public BuildableStructure(int x, int y, float maxProgress, bool forceComplete, Action onComplete, Vector3 size, Vector3 offset,ConstructableType myType)
    {
        this.x = x; this.y = y;
        this.maxProgress = maxProgress;
        pos=new Vector3(x,y,0);
        this.onComplete = onComplete;
        this.size= size;
        this.offset = offset;
        this.myType = myType;
        if (forceComplete)
        {
            OnObjectConstructed();
        }
    }
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
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("ConstructionMarker");
        Object.GetComponent<ConstructableObjectUI>().InitUI(size, pos+offset);
        Object.SetActive(true);
        isDrawn = true;
    }

    public void Cleanup()
    {
        GameObjectPoolManager.Instance.ReturnObjectToPool(Object, "ConstructionMarker");
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

    void Selectable.OnObjectSelected()
    {
        throw new NotImplementedException();
    }

    void Selectable.OnObjectDeselected()
    {
        throw new NotImplementedException();
    }

    SelectableType Selectable.GetSelectableType()
    {
        throw new NotImplementedException();
    }

    bool Selectable.GetIsSelected()
    {
        throw new NotImplementedException();
    }

    bool Selectable.IsSelectable()
    {
        throw new NotImplementedException();
    }

    void Selectable.SetIsSelected(bool val)
    {
        throw new NotImplementedException();
    }

  


    Vector3 Selectable.GetSize()
    {
        return size;
    }

    bool Selectable.IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(size, pos+offset, point);
    }
}

