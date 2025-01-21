using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory:MonoBehaviour
{
    public List<InventoryObject> ObjectsInInventory =new List<InventoryObject>();
    public float InventoryCapacity;
    float CurrentItemsWeight=0;
    public void AddItemToInventory(InventoryObject inventoryObject)
    {
        if(inventoryObject.Weight() + GetSubOfInventoryWeight() <= InventoryCapacity)
        {
            ObjectsInInventory.Add(inventoryObject);
            inventoryObject.OnAddedToInventory();
            RefreshWeightOfCurrentItems();
        }
        else
        {
            if(inventoryObject.CanSplitStack())
            {
                Object[] split = inventoryObject.SplitStack(InventoryCapacity - CurrentItemsWeight);
                ObjectsInInventory.Add(split[0] as InventoryObject);
                inventoryObject.RepopulateData(split[1] as InventoryObject);
            }
        }
    }

    public void TransferItemBetweenInventory(InventoryObject inventoryObject,Inventory comingFrom)
    {
        if (inventoryObject.Weight() + GetSubOfInventoryWeight() <= InventoryCapacity)
        {
            comingFrom.RemoveItemFromInventory(inventoryObject);
            ObjectsInInventory.Add(inventoryObject);
            RefreshWeightOfCurrentItems();
        }
        else
        {
            if (inventoryObject.CanSplitStack())
            {
                comingFrom.RemoveItemFromInventory(inventoryObject);
                Object[] split = inventoryObject.SplitStack(InventoryCapacity- CurrentItemsWeight);

                ObjectsInInventory.Add(split[0] as InventoryObject);
                comingFrom.AddItemToInventory(split[1] as InventoryObject);
            }
        }
    }

    public void RemoveItemFromInventory(InventoryObject inventoryObject)
    {
        if (!ObjectsInInventory.Contains(inventoryObject))
        {
            return;
        }
        ObjectsInInventory.Remove(inventoryObject);
        RefreshWeightOfCurrentItems();
    }


    float GetSubOfInventoryWeight()
    {
        return CurrentItemsWeight;
    }

    void RefreshWeightOfCurrentItems()
    {
        CurrentItemsWeight = 0;
        for(int i = 0; i < ObjectsInInventory.Count; i++)
        {
            CurrentItemsWeight += ObjectsInInventory[i].Weight();
        }
    }

}
