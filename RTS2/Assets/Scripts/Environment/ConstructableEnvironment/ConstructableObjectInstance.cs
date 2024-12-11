using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructableObjectInstance : EnvironmentObjectInstance, Constructable
{
    public ConstructableObjectInstance(int x,int y,string envObj,bool forceBuilt,float buildTarget):base(x,y,envObj)
    {

        if (forceBuilt)
        {
            buildProgress = buildTarget;
            buildAmountTarget = buildTarget;
            SetBuilt(true);
        }
        else
        {
            buildAmountTarget = buildTarget;

        }


    }

    bool isBuilt = false;
    float buildProgress = 0f,buildAmountTarget=0f;

    public void SetBuilt(bool val)
    {
        isBuilt = val;
        if (!isBuilt)
        {
            Object.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f);
        }
        else
        {
            OnObjectConstructed();
        }
    }



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

    public void ConstructObject()
    {
        if(!isBuilt)
        {
            return;
        }
        buildProgress += Time.deltaTime;
        if(buildProgress >= buildAmountTarget)
        {
            SetBuilt(true);
        }
    }

    public void OnObjectConstructed()
    {

    }
}
