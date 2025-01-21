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

    public int Quantity()
    {
        return 1;
    }

    public bool CanSplitStack()
    {
        return false;
    }

    public Object[] SplitStack(int quantityWanted)
    {
        return null;
    }

    public Object[] SplitStack(float weightWanted)
    {
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

    public Item MyItem;
    public SpriteRenderer sr;
}
