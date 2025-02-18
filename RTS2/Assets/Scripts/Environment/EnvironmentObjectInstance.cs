using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


/// <summary>
/// Stores information about an instance of an EnvironmentObject in the world (Does not mean that the object is being drawn)
/// </summary>
public class EnvironmentObjectInstance:ObjectInfo
{
    public string ObjectKey;
    public int PosX
    {
        get
        {
            return coords.x;
        }
    }
        
     public int PosY
    {
        get
        {
            return coords.y;
        }
    }
    public bool Drawn = false;
    public GameObject Object;
    WorldChunk myChunk;
    Vector3 position;
    public Vector2Int coords;

    public void SetChunk(WorldChunk chunk)
    {
        myChunk= chunk;
    }

    public EnvironmentObjectInstance(int x,int y,string envObj)
    {
        ObjectKey = envObj;
        position = new Vector3(x, y);
        coords = new Vector2Int(x, y);
    }


    public virtual void RenderInstance()
    {
        if (Drawn)
        {
            return;
        }

        EnvironmentObject obj = EnvironmentObjectManager.Instance.AllObjects[ObjectKey];
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("EnvironmentObject");
        Object.transform.position = new Vector3(PosX, PosY, 0);
        Object.GetComponent<SpriteRenderer>().sprite = obj.ForwardsSprite;     
        Object.SetActive(true);
        Drawn = true;
    }

    public virtual void CleanupInstance()
    {
        if(!Drawn) { return; }
        GameObjectPoolManager.Instance.ReturnObjectToPool(Object,"EnvironmentObject");
        Drawn = false;
    }

    public virtual bool CanHarvest()
    {
        return EnvironmentObjectManager.Instance.AllObjects.ContainsKey(ObjectKey) &&
            EnvironmentObjectManager.Instance.AllObjects[ObjectKey].CanHarvest;
    }

    Timer Timer;
    float harvestTimer = 0;
   public bool isHarvested = false;
    public virtual void Harvest()
    {
        if (CanHarvest() == false|| isHarvested)
        {
            return;
        }
        if (Timer == null)
        {
            Timer = new Timer(EnvironmentObjectManager.Instance.AllObjects[ObjectKey].Resources.HarvestLength);
            Timer.CreateProgressBarFromTimer(GetPosition());
            
        }
        Timer.ProgressTime(DeltaTimeWrapper.GameplayDelta);
        
        if (Timer.IsTimerFinished())
        {
            EnvironmentObjectManager.Instance.AllObjects[ObjectKey].Resources.GenerateResoruces(new Vector3(PosX, PosY, 0));
            DestroyInstance();
            isHarvested = true;
        }
    }

        public Vector3 GetPosition()
    {
        return position;
    }

    void DestroyInstance()
    {
        if (Drawn)
        {
            CleanupInstance();
        }
        myChunk.RemoveEnvironmentObject(this);
    }

    public string Name()
    {
        return ObjectKey;
    }

    public string Description()
    {
        return "";
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
        return position;
    }
}
