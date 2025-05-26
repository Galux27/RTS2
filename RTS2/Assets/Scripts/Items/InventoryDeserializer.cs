using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class InventoryDeserializer
{
    static List<string> inventorysToDeserialize;
    static List<Type> inventoryTypes;
    public static void AddInventoryToDeserialize(string inventory,Type inventoryType)
    {
        if(inventorysToDeserialize == null)
        {
            inventorysToDeserialize = new List<string>();
            inventoryTypes = new List<Type>();
        }
        Debug.Log("Inventory: adding inventory to deserialize "+  inventory);
        inventorysToDeserialize.Add(inventory);
        inventoryTypes.Add(inventoryType);
    }

    public static void DeserializeInventorys()
    {
        if(inventorysToDeserialize == null)
        {
            return;
        }
        for(int x=0;x<inventorysToDeserialize.Count;x++)
        {
            DeserializeInventory(inventorysToDeserialize[x], inventoryTypes[x]);
        }
        inventoryTypes = null;
        inventorysToDeserialize = null;
    }

    static void DeserializeInventory(string  inventory,Type inventoryType)
    {
        Debug.Log("Inventory: Deserializing inventory " + inventory);
        string[] keyValueSplit = inventory.Split(SerializeDataHelpers.INVENTORY_MARKER_TWO);
        string[] uidData = keyValueSplit[0].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT);
        Debug.Log("inventory: UID  data " + keyValueSplit[0]);
        uidData[1] = uidData[1].Replace(SerializeDataHelpers.DATA_ELEMENT_SPLIT.ToString(), "");

        List<InventoryObject> items = new List<InventoryObject>();

         ulong uid =(ulong) DataReaders.ParseDataObject(uidData[0], uidData[1]);
        keyValueSplit[1]=keyValueSplit[1].Replace(DataKeys.ItemsInContainer, "");
        string[] inventoryContents = keyValueSplit[1].Split(SerializeDataHelpers.INVENTORY_ELEMENT_SPLIT);
        string[] inventoryItem = null;
        string[] keyObjectSplit = null;
        Dictionary<string, object> itemData = null;
        for(int x=0;x<inventoryContents.Length;x++)
        {
            inventoryItem = inventoryContents[x].Split(SerializeDataHelpers.INVENTORY_SPLIT_TWO, System.StringSplitOptions.RemoveEmptyEntries);
            itemData = new Dictionary<string, object>();
            for(int i=0;i<inventoryItem.Length;i++)
            {
                inventoryItem[i] = inventoryItem[i].Replace(SerializeDataHelpers.DATA_ELEMENT_SPLIT.ToString(), "");
                if (inventoryItem[i] != "")
                {
                    keyObjectSplit = inventoryItem[i].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
                  keyObjectSplit[0] = keyObjectSplit[0].Replace(SerializeDataHelpers.KEY_OBJECT_SPLIT.ToString(), "");
                  Debug.Log("Inventory: deserializing from " + inventoryItem[i] + "|" + keyObjectSplit.Length + "|" + keyObjectSplit[0]);
               
                    itemData.Add(keyObjectSplit[0], DataReaders.ParseDataObject(keyObjectSplit[0], keyObjectSplit[1]));
                }
            }
            InventoryObject toadd = null;
            if (itemData.ContainsKey(DataKeys.ObjectKey))
            {
                toadd = CreateItemFromReadData(itemData);
                if (toadd != null)
                {
                    items.Add(toadd);
                }
            }
            
        }
        object inventoryToAddTo = IDManager.GetObjectByUID(inventoryType, uid);
        if(inventoryToAddTo != null)
        {
            Inventory i = GetInventoryFromObjectUID(inventoryToAddTo, inventoryType);
           for(int x=0;x<items.Count; x++)
            {
                i.AddItemToInventory(items[x]);
            }
        }
        else
        {
            Debug.Log("Inventory: was nul,sadge "+uid+" TYPE " +inventoryType.ToString());

        }
        //UID;4214:}CONTAINER_CONTENTS;OBJECT_KEY;Rifle:]QUANTITY;1:]|OBJECT_KEY;Construction Supplies:]QUANTITY;63:]|:
    }

    static Inventory GetInventoryFromObjectUID(object inventoryObject,Type typeOf)
    {
        return (Inventory)inventoryObject;
    }

    static InventoryObject CreateItemFromReadData(Dictionary<string,object> data)
    {
        InventoryObject retVal = null;
        string key =(string)data[DataKeys.ObjectKey];
        int quantitiy = (int)data[DataKeys.Quantitiy];
        if (ItemController.Instance.DoesItemExist(key)) {
            retVal = ItemInWorld.CreateItemInstanceInWorld(ItemController.Instance.AllItems[key]);
        }
        else if(ResourceController.Instance.DoesResourceTypeExist(key))
        {
            retVal = new ResourceInstanceData(key, quantitiy);
        }
        Debug.Log("Inventory: created item " + key + " quantity " + quantitiy);
        return retVal;
    }
}
