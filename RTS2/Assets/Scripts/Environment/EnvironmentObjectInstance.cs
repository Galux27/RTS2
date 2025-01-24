using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Stores information about an instance of an EnvironmentObject in the world (Does not mean that the object is being drawn)
/// </summary>
public class EnvironmentObjectInstance
{
    public string ObjectKey;
    public int PosX, PosY;
    public bool Drawn = false;
    public GameObject Object;
    WorldChunk myChunk;
    Vector3 position;
    public void SetChunk(WorldChunk chunk)
    {
        myChunk= chunk;
    }

    public EnvironmentObjectInstance(int x,int y,string envObj)
    {
        ObjectKey = envObj;
        PosX= x; PosY = y;
        position = new Vector3(x, y);
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

    ProgressBarUI ProgressBar;
    float harvestTimer = 0;
   public bool isHarvested = false;
    public virtual void Harvest()
    {
        if (CanHarvest() == false|| isHarvested)
        {
            return;
        }
        if (ProgressBar == null)
        {
            ProgressBar = ProgressBarUI.CreateProgressBar();
            ProgressBar.InitProgressBar(EnvironmentObjectManager.Instance.AllObjects[ObjectKey].Resources.HarvestLength, 0, GetPosition());
        }
        harvestTimer += Time.deltaTime;
        ProgressBar.UpdateCurrent(harvestTimer);
        
        if (harvestTimer >= EnvironmentObjectManager.Instance.AllObjects[ObjectKey].Resources.HarvestLength)
        {
            EnvironmentObjectManager.Instance.AllObjects[ObjectKey].Resources.GenerateResoruces(new Vector3(PosX, PosY, 0));
            DestroyInstance();
            ProgressBar.ReturnProgressBar();
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
}
