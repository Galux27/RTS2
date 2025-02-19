using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Inventory : MonoBehaviour, Storage
{


    private void Start()
    {
        if (!this.GetComponent<Unit>())
        {
            Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(this.transform.position);
            WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].AddContainerObject(this);
        }
    }


    private void OnDestroy()
    {
        if (!this.GetComponent<Unit>())
        {
            Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(this.transform.position);
            WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].RemoveContainerObject(this);
        }
    }

    public bool CanAddItemToInventory(InventoryObject inventoryObject)
    {
        if (Filter != null && Filter.ItemCanPass(inventoryObject.Name()) == false)
        {
            return false;
        }
        if (inventoryObject.Weight() + GetSumOfInventoryWeight() > InventoryCapacity)
        {

            return CanAddItemToInventory(inventoryObject);
        }
        return true;
    }

    public bool CanAddSomeOfItemToInventory(InventoryObject inventoryObject)
    {
        if (inventoryObject.CanSplitStack())
        {
            if (inventoryObject.Weight() / inventoryObject.Quantity() >= GetRemainingCapacity())
            { 
                return true;
            }
        }
            return false;
    }

    public List<InventoryObject> ObjectsInInventory = new List<InventoryObject>();
    public float InventoryCapacity;
    float CurrentItemsWeight = 0;
    public ItemFilter Filter;
    public void AddItemToInventory(InventoryObject inventoryObject)
    {
        if (Filter != null && Filter.ItemCanPass(inventoryObject.Name()) == false)
        {
            return;
        }
        if (inventoryObject.Weight() + GetSumOfInventoryWeight() <= InventoryCapacity)
        {
            
            ObjectsInInventory.Add(inventoryObject);
            inventoryObject.OnAddedToInventory();
            ResourceManager.Instance.RefreshResourceData();
            RefreshWeightOfCurrentItems();
        }
        else
        {
            if (inventoryObject.CanSplitStack())
            {
                if (ObjectsInInventory == null)
                {
                    ObjectsInInventory = new List<InventoryObject>();
                }
                object[] split = inventoryObject.SplitStack(InventoryCapacity - CurrentItemsWeight);
                ObjectsInInventory.Add(split[0] as InventoryObject);
                inventoryObject.RepopulateData(split[1] as InventoryObject);
                ResourceManager.Instance.RefreshResourceData();

            }
        }
    }

    public void AddQuantityOfItemToInventory(InventoryObject inventoryObject, int quantity)
    {
        if (inventoryObject.CanSplitStack())
        {
            float weightOfOne = inventoryObject.Weight() / inventoryObject.Quantity();
            object[] split = inventoryObject.SplitStack(weightOfOne);
            ObjectsInInventory.Add(split[0] as InventoryObject);
            inventoryObject.RepopulateData(split[1] as InventoryObject);
            ResourceManager.Instance.RefreshResourceData();

        }
    }

    public void TransferItemBetweenInventory(InventoryObject inventoryObject, Inventory comingFrom)
    {
        if (Filter.ItemCanPass(inventoryObject.Name()) == false)
        {
            return;
        }
        if (inventoryObject.Weight() + GetSumOfInventoryWeight() <= InventoryCapacity)
        {
            comingFrom.RemoveItemFromInventory(inventoryObject);
            ObjectsInInventory.Add(inventoryObject);
            RefreshWeightOfCurrentItems();
            ResourceManager.Instance.RefreshResourceData();

        }
        else
        {
            if (inventoryObject.CanSplitStack())
            {
                comingFrom.RemoveItemFromInventory(inventoryObject);
                object[] split = inventoryObject.SplitStack(InventoryCapacity - CurrentItemsWeight);

                ObjectsInInventory.Add(split[0] as InventoryObject);
                InventoryObject remainder = split[1] as InventoryObject;
                if (remainder.Quantity() > 0)
                {
                    comingFrom.AddItemToInventory(remainder);
                }
                ResourceManager.Instance.RefreshResourceData();

            }
        }
    }

    public void TransferItemBetweenInventory(InventoryObject inventoryObject, Inventory comingFrom,int quantity)
    {
        if (Filter.ItemCanPass(inventoryObject.Name()) == false)
        {
            return;
        }

        float individiualWeight = inventoryObject.Weight() / inventoryObject.Quantity();
        if (GetRemainingCapacity() >= individiualWeight*quantity)
        {
            if (inventoryObject.CanSplitStack())
            {
                comingFrom.RemoveItemFromInventory(inventoryObject);
                object[] split = inventoryObject.SplitStack(quantity);

                ObjectsInInventory.Add(split[0] as InventoryObject);

                InventoryObject remainder = split[1] as InventoryObject;
                if (remainder.Quantity() > 0)
                {
                    comingFrom.AddItemToInventory(remainder);
                }
                ResourceManager.Instance.RefreshResourceData();

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
        ResourceManager.Instance.RefreshResourceData();

    }


    public float GetSumOfInventoryWeight()
    {
        return CurrentItemsWeight;
    }

    public void RefreshWeightOfCurrentItems()
    {
        CurrentItemsWeight = 0;
        for (int i = 0; i < ObjectsInInventory.Count; i++)
        {
            CurrentItemsWeight += ObjectsInInventory[i].Weight();
        }
    }

    public float GetRemainingCapacity()
    {
        return InventoryCapacity - CurrentItemsWeight;
    }

    public bool IsNotFull()
    {
        return CurrentItemsWeight < InventoryCapacity;
    }

}
