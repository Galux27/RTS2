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


    public EnvironmentObjectInstance(int x,int y,string envObj)
    {
        ObjectKey = envObj;
        PosX= x; PosY = y;
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
}
