using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstructableObjectInstance : EnvironmentObjectInstance,Selectable
{
    public ConstructableObjectInstance(int x,int y,string envObj):base(x,y,envObj)
    {
        EventManager.Instance.OnConstructableObjectCreated(coords, this);
        if (ConstructableObjectManager.Instance.AllObjects[ObjectKey].MyBehaviour != null)
        {
            myBehaviour = ScriptableObject.Instantiate(ConstructableObjectManager.Instance.AllObjects[ObjectKey].MyBehaviour);
            myBehaviour.myPosition= new Vector3(x,y,0);
        }
        if (ConstructableObjectManager.Instance.AllObjects[ObjectKey].RequiresUpdate)
        {
            Debug.Log("Added to on update");
            GameController.Instance.OnUpdate += OnUpdate;
        }
        UnitCapacityManager.RefreshCapacities();
    }

    EnvironmentObjectBehaviourBase myBehaviour;

    public override void RenderInstance()
    {
        if (Drawn)
        {
            return;
        }

        EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey);
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("EnvironmentObject");
        Object.transform.position = new Vector3(PosX, PosY, 0);
        Object.GetComponent<SpriteRenderer>().sprite = obj.ForwardsSprite;
        Object.SetActive(true);


        if (obj.GetType() == typeof(ConstructableContainer))
        {
            ConstructableContainer parentType = (ConstructableContainer)obj;
            if (parentType != null)
            {
                inventoryObject = new GameObject();
                inventoryObject.transform.position = new Vector3(PosX, PosY, 0);
                inventoryObject.name = "Inventory For Object " + obj.name + " pos " + PosX + "," + PosY;
                parentType.OnObjectConstructed(inventoryObject);
            }
        }
        Drawn = true;

    }

    void OnUpdate()
    {
        if (myBehaviour != null)
        {

            if (myBehaviour.HasUpdate())
            {
                myBehaviour.OnUpdate();
            }
        }
        DebugDrawing.Instance.DrawEnvironmentObjectInstance(this);

    }



    public GameObject inventoryObject;

    public override void CleanupInstance()
    {
       Component.Destroy(Object.GetComponent<ConstructableObjectWorldReference>());
        OnObjectDeselected();
        if (ConstructableObjectManager.Instance.AllObjects[ObjectKey].RequiresUpdate)
        {
            GameController.Instance.OnUpdate -= OnUpdate;
        }
        base.CleanupInstance();
    }

    public void OnObjectDeselected()
    {
        Object.GetComponentInChildren<SelectedOutline>()?.OnDeselect();
    }
    public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(Object, GetSize(),GetSize()/2f);
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
        return EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey).Size() ;
    }
    public bool IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(GetSize(), new Vector3(PosX,PosY,0), point);

    }

    new public string Name()
    {
        return ObjectKey;
    }

    new public string Description()
    {
        return "";
    }

    new public int Quantitiy()
    {
        return 1;
    }

 

    new public Vector3 Position()
    {
        return new Vector3(PosX, PosY, 0);
    }
}
