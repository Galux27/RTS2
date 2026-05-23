using System;
using System.Collections;
using System.Collections.Generic;
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
            myBehaviour.PassInVector( new Vector3(x,y,0),"POS");
        }
        if (ConstructableObjectManager.Instance.AllObjects[ObjectKey].RequiresUpdate)
        {
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
            if (parentType != null && inventoryObject==null)
            {
                inventoryObject = new GameObject();
                inventoryObject.transform.position = new Vector3(PosX, PosY, 0);
                inventoryObject.name = "Inventory For Object " + obj.name + " pos " + PosX + "," + PosY;
                parentType.OnObjectConstructed(inventoryObject);
            }
        }
        Drawn = true;

    }

    public override Vector3 GetPosition()
    {
        return base.Position() + (GetSize() / 2f);
    }

    public void InitInventoryObject(ulong uid)
    {
        EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey);
        ConstructableContainer parentType = (ConstructableContainer)obj;
        if (parentType == null)
        {
            Debug.LogError("Error assigning inventory for " + ObjectKey);
            return;
        }
        inventoryObject = new GameObject();
        inventoryObject.transform.position = new Vector3(PosX, PosY, 0);
        inventoryObject.name = "Inventory For Object " + obj.name + " pos " + PosX + "," + PosY;      
        parentType.OnObjectConstructed(inventoryObject);
        inventoryObject.GetComponent<Inventory>().SetMyUID(uid);
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
        return SelectionUtilities.IsInBounds(GetSize(), GetPosition(), point);

    }

    public override DataToSerialize GetExtraDataToSerialize()
    {
        //if (inventoryObject != null)
       // {
           // DataToSerialize data = new DataToSerialize();
           // data.AddDataToSerialize(DataKeys.InventoryUID, inventoryObject.GetComponent<Inventory>().GetMyUID().Value);
          //  data.AddDataToSerialize(DataKeys.Inventory, inventoryObject.GetComponent<Inventory>().Serialize().Data);
          //  return data;
       // }
        return null;
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
