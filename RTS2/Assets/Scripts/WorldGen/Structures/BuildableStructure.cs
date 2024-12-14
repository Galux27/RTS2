using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableStructure:Constructable
{

    public BuildableStructure(int x, int y, float maxProgress, bool forceComplete, Action onComplete, Vector3 size, Vector3 offset)
    {
        this.x = x; this.y = y;
        this.maxProgress = maxProgress;
        pos=new Vector3(x,y,0);
        this.onComplete = onComplete;
        this.size= size;
        this.offset = offset;
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
        progress += Time.deltaTime;
        if (progress > maxProgress)
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
       return pos;
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
}
