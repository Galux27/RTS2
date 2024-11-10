using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void RenderInstance()
    {
        if (Drawn)
        {
            return;
        }

        EnvironmentObject obj = EnvironmentObjectManager.Instance.AllObjects[ObjectKey];
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("EnvironmentObject");
        Object.transform.position = new Vector3(PosX, PosY, 0);
        Object.GetComponent<SpriteRenderer>().sprite = obj.Sprite;     
        Object.SetActive(true);
        Drawn = true;
    }

    public void CleanupInstance()
    {
        if(!Drawn) { return; }
        GameObjectPoolManager.Instance.ReturnObjectToPool(Object,"EnvironmentObject");
        Drawn = false;
    }
}
