using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, Storage, ISerialize
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

    public void CopyItemsIntoOtherInventory(ref Inventory toAddTo)
    {
        for(int x=0;x<ObjectsInInventory.Count;x++)
        {
            toAddTo.AddItemToInventory(ObjectsInInventory[x]);
        }
    }

    public void AddItemToInventory(InventoryObject inventoryObject)
    {
        if (Filter != null && Filter.ItemCanPass(inventoryObject.Name()) == false)
        {
            return;
        }
        if (inventoryObject.Weight() + GetSumOfInventoryWeight() <= InventoryCapacity)
        {

            AddOrMergeWithExisting(inventoryObject);
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
                AddOrMergeWithExisting(split[0] as InventoryObject);
                inventoryObject.RepopulateData(split[1] as InventoryObject);
                ResourceManager.Instance.RefreshResourceData();

            }
        }
    }
    public Action OnInventoryChange;
    void AddOrMergeWithExisting(InventoryObject inventoryObject)
    {
        for(int x = 0; x < ObjectsInInventory.Count; x++)
        {
            if (ObjectsInInventory[x].Name() == inventoryObject.Name())
            {
                ObjectsInInventory[x].MergeWith(inventoryObject);
                OnInventoryChange?.Invoke();
                return;
            }
        }
        OnInventoryChange?.Invoke();
        ObjectsInInventory.Add(inventoryObject);
    }

    public void AddQuantityOfItemToInventory(InventoryObject inventoryObject, int quantity)
    {
        if (inventoryObject.CanSplitStack())
        {
            float weightOfOne = inventoryObject.Weight() / inventoryObject.Quantity();
            object[] split = inventoryObject.SplitStack(weightOfOne);
            AddOrMergeWithExisting(split[0] as InventoryObject);
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
            AddOrMergeWithExisting(inventoryObject);
            RefreshWeightOfCurrentItems();
            ResourceManager.Instance.RefreshResourceData();

        }
        else
        {
            if (inventoryObject.CanSplitStack())
            {
                comingFrom.RemoveItemFromInventory(inventoryObject);
                object[] split = inventoryObject.SplitStack(InventoryCapacity - CurrentItemsWeight);

                AddOrMergeWithExisting(split[0] as InventoryObject);
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

                AddOrMergeWithExisting(split[0] as InventoryObject);

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
        OnInventoryChange?.Invoke();
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

    public void ContainsItem(string name, out int quantity)
    {
        quantity = 0;
        for(int x = 0; x < ObjectsInInventory.Count; x++)
        {
            if (ObjectsInInventory[x].Name() == name)
            {
                quantity += ObjectsInInventory[x].Quantity();
            }
        }
    }

    public void RemoveQuantityOfObject(string name,int quantity)
    {
        int remainingToRemove = quantity;
        InventoryObject checking = null;
        int index = 0;
        bool finished = false;
        while (!finished)
        {
            bool progressIndex = true;

            if (ObjectsInInventory[index].Name() == name)
            {
                checking = ObjectsInInventory[index];

                if (checking.Quantity() >= remainingToRemove)
                {
                   
                    if (checking.Quantity() > remainingToRemove)
                    {
                        RemoveItemFromInventory(checking);
                        object[] split = checking.SplitStack(remainingToRemove);

                       // AddOrMergeWithExisting(split[0] as InventoryObject);
                        InventoryObject remainder = split[1] as InventoryObject;
                        AddItemToInventory(remainder);
                    }
                    else
                    {
                        remainingToRemove -= checking.Quantity();
                        RemoveItemFromInventory(checking);
                        progressIndex = false;
                    }
                    ResourceManager.Instance.RefreshResourceData();

                }
            }
            if (progressIndex)
            {
                index++;
            }
            if (index >= ObjectsInInventory.Count)
            {
                finished = true;
            }
        }

       
    }

    List<DataToSerialize> GetItemsData()
    {
        List<DataToSerialize> retVal = new List<DataToSerialize>();
        DataToSerialize cur = new DataToSerialize();
        for (int x = 0; x < ObjectsInInventory.Count; x++)
        {
            cur.AddDataToSerialize(DataKeys.UID, ObjectsInInventory[x].GetMyUID().Value);

            cur.AddDataToSerialize(DataKeys.ObjectKey, ObjectsInInventory[x].Name());
            cur.AddDataToSerialize(DataKeys.Quantitiy, ObjectsInInventory[x].Quantity());
            retVal.Add( cur);
            cur = new DataToSerialize();
        }
        return retVal;
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.UID, GetMyUID().Value);
        retVal.AddDataToSerialize(DataKeys.ItemsInContainer, GetItemsData());
        return retVal;
    }

    public SerializedData Serialize()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(SerializedData data)
    {
        throw new NotImplementedException();
    }

    public UID GetMyUID()
    {
        throw new NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
        //myUID = new UID(uid);
    }
}
