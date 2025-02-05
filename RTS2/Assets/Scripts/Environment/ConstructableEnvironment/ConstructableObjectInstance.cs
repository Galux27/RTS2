using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ConstructableObjectInstance : EnvironmentObjectInstance,Selectable
{
    public ConstructableObjectInstance(int x,int y,string envObj):base(x,y,envObj)
    {
        PosX = x;
        PosY= y;

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



        ConstructableContainer parentType = (ConstructableContainer)obj;
        if (parentType != null)
        {
            inventoryObject = new GameObject();
            inventoryObject.transform.position = new Vector3(PosX, PosY, 0);
            inventoryObject.name = "Inventory For Object " + obj.name + " pos " + PosX + "," + PosY;
            parentType.OnObjectConstructed(inventoryObject);
        }
        Drawn = true;

    }

    GameObject inventoryObject;

    public override void CleanupInstance()
    {
       Component.Destroy(Object.GetComponent<ConstructableObjectWorldReference>());
        base.CleanupInstance();
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
        return SelectableType.ConstructableObject;
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

    public Vector3 GetSize()
    {
        return ConstructableObjectManager.Instance.AllObjects[ObjectKey].Size() ;
    }
    public bool IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(GetSize(), new Vector3(PosX,PosY,0), point);

    }
}
