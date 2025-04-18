using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface Storage 
{
    public bool CanAddItemToInventory(InventoryObject inventoryObject);
    public bool CanAddSomeOfItemToInventory(InventoryObject inventoryObject);
    public void AddItemToInventory(InventoryObject inventoryObject);

    public void AddQuantityOfItemToInventory(InventoryObject inventoryObject, int quantity);

    public void ContainsItem(string name, out int quantity);

    public void TransferItemBetweenInventory(InventoryObject inventoryObject, Inventory comingFrom);
    public void TransferItemBetweenInventory(InventoryObject inventoryObject, Inventory comingFrom, int quantity);
    public void RemoveItemFromInventory(InventoryObject inventoryObject);
    public float GetSumOfInventoryWeight();
    public void RefreshWeightOfCurrentItems();
    public float GetRemainingCapacity();
    public bool IsNotFull();


}
