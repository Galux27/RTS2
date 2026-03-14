using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemInWorld : MonoBehaviour,InventoryObject
{
    public static ItemInWorld CreateItemInstanceInWorld(Item toCreate)
    {
        GameObject g = new GameObject();
        g.name = toCreate.name + " instance";
        ItemInWorld iw = g.AddComponent<ItemInWorld>();
        iw.SetItem(toCreate);
        SpriteRenderer sr = g.AddComponent<SpriteRenderer>();
        sr.spriteSortPoint = SpriteSortPoint.Pivot;
        sr.sprite = toCreate.Sprite;
        iw.sr = sr;
        return iw;
    }
    private void Start()
    {
        ItemController.Instance.AllItemsInWorld.Add(this);
    }

    public void SetItem(Item item)
    {
        MyItem = item;
    }

    public string Name()
    {
        return MyItem.Name;
    }

    public float Weight()
    {
        return MyItem.Weight;
    }
    public int QuantityVal = 1;
    public int Quantity()
    {
        return QuantityVal ;
    }

    public bool CanSplitStack()
    {
        return false;
    }

    public object[] SplitStack(int quantityWanted)
    {
        //ResourceInstanceData[] retVal = new ResourceInstanceData[2];
        //retVal[0] = new ResourceInstanceData(Resource, quantityWanted);
        //this.Quantity -= quantityWanted;
        //retVal[1] = this;
        //return retVal;
        return null;
    }

    public object[] SplitStack(float weightWanted)
    {

        //int quantity = 0;
        //float currentWeight = 0f;
        //float weightPer = ResourceController.Instance.AllResources[Name()].WeightPerUnit;
        //while (currentWeight < weightWanted)
        //{
        //    quantity++;
        //    currentWeight += weightPer;
        //}

        //ResourceInstanceData[] retVal = new ResourceInstanceData[2];

        //retVal[0] = new ResourceInstanceData(Resource, quantity);
        //this.Quantity -= quantity;
        //retVal[1] = this;

        //return retVal;
        return null;
    }
    public void RepopulateData(InventoryObject toRepopulateWith)
    {
        
    }

    public void OnAddedToInventory()
    {
        
    }

    public void OnRemovedFromInventory()
    {
        this.transform.parent = null;
    }

    public bool CanObjectBeEquiped()
    {
        return MyItem.Slot == ItemEquipSlot.Hands;
    }

    public void EquipObject(Unit toEquipTo)
    {
        toEquipTo.GetComponentInChildren<ItemHolder>().SetHolding(this);

    }

    public void MergeWith(InventoryObject obj)
    {
        
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.UID, GetMyUID().Value);
        retVal.AddDataToSerialize(DataKeys.ObjectKey, MyItem.Name);
        retVal.AddDataToSerialize(DataKeys.Pos, this.transform.position);
        return retVal;
    }

    public SerializedData Serialize()
    {
        throw new System.NotImplementedException();
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }
    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value == 0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);

        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
        myUid = new UID(uid);
        IDManager.OnUIDCreated(this, myUid);

    }
    public Item MyItem;
    public SpriteRenderer sr;
}
