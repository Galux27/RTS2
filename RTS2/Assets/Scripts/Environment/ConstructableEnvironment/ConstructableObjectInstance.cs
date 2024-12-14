using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructableObjectInstance : EnvironmentObjectInstance
{
    public ConstructableObjectInstance(int x,int y,string envObj):base(x,y,envObj)
    {
        pos = new Vector3(x, y);

    }
    Vector3 pos;
    

    public override void RenderInstance()
    {
        if (Drawn)
        {
            return;
        }

        EnvironmentObject obj = ConstructableObjectManager.Instance.AllObjects[ObjectKey];
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("EnvironmentObject");
        Object.transform.position = new Vector3(PosX, PosY, 0);
        Object.GetComponent<SpriteRenderer>().sprite = obj.ForwardsSprite;
        Object.SetActive(true);
        Drawn = true;

    }
    public override void CleanupInstance()
    {
       Component.Destroy(Object.GetComponent<ConstructableObjectWorldReference>());
        base.CleanupInstance();
    }

    


    
}
