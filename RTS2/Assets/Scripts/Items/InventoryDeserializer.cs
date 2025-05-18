using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryDeserializer
{
    static List<string> inventorysToDeserialize;
    public static void AddInventoryToDeserialize(string inventory)
    {
        if(inventorysToDeserialize == null)
        {
            inventorysToDeserialize = new List<string>();
        }
        Debug.Log("Inventory: adding inventory to deserialize "+  inventory);
        inventorysToDeserialize.Add(inventory);
    }

    public static void DeserializeInventorys()
    {
        if(inventorysToDeserialize == null)
        {
            return;
        }
        for(int x=0;x<inventorysToDeserialize.Count;x++)
        {
            DeserializeInventory(inventorysToDeserialize[x]);
        }
    }

    static void DeserializeInventory(string  inventory)
    {

    }
}
