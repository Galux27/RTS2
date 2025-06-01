using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    static ResourceManager instance;
    public static ResourceManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<ResourceManager>(true);
            }
            return instance;
        }
    }

    public Dictionary<string, ResourceData> UserResources;

    private void Start()
    {
        InitResourceManager();
    }

    void InitResourceManager()
    {
        UserResources = new Dictionary<string, ResourceData>();
        foreach(KeyValuePair<string,Resource>kvp in ResourceController.Instance.AllResources)
        {
            AddResource(kvp.Value.Name, new ResourceData(kvp.Value.name));
        }
    }

    void AddResource(string name, ResourceData data)
    {
        UserResources.Add(name, data);
        ResourcesDisplayUI.Instance.CreateUIElement(data);
        Debug.Log("Resources: added resource "+  name);

    }

    public void RefreshResourceData()
    {

       // InitResourceManager();
      
        //ConstructableObjectInstance cc;
        //for(int x = 0; x < RoomManager.Instance.roomList.Count; x++)
        //{
        //    if (RoomManager.Instance.roomList[x].roomType == RoomUseType.Warehouse)
        //    {
        //        for (int y = 0; y < RoomManager.Instance.roomList[x].ObjectsInRoom.Count; y++)
        //        {
        //            cc = RoomManager.Instance.roomList[x].ObjectsInRoom[y];
        //            if (cc.inventoryObject != null)
        //            {
        //                Inventory i = cc.inventoryObject.GetComponent<Inventory>();
        //                Debug.Log("Inventory in room item count " + i.gameObject.name + " " + i.ObjectsInInventory.Count+" " + i.GetSumOfInventoryWeight());
        //                for (int q = 0; q < i.ObjectsInInventory.Count; q++)
        //                {
        //                    if (ResourceManager.instance.UserResources.ContainsKey(i.ObjectsInInventory[q].Name()))
        //                    {
        //                        AddQuantityOfResource(i.ObjectsInInventory[q].Name(), i.ObjectsInInventory[q].Quantity());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        
    }

    

    public void AddQuantityOfResource(string key,int quantity)
    {
        Debug.Log("RES: Adding quantity of resource " + key + " q" + quantity + "|" + UserResources[key].Quantity);
        UserResources[key].IncreaseQuantitiy(quantity);
        ResourcesDisplayUI.Instance.UpdateUIElement(UserResources[key]);
        Debug.Log("RES: Total after "+ UserResources[key].ResourceName+"|" + UserResources[key].Quantity);

    }

    public void ReduceQuantity(string key,int quantity)
    {
        UserResources[key].DecreaseQuantity( quantity);
        ResourcesDisplayUI.Instance.UpdateUIElement(UserResources[key]);
    }

    public Action OnRefreshResourceData;
    public List<Storage> StoragesToUse = new List<Storage>();
}

public class ResourceData
{
    public string ResourceName;
    public int Quantity;

    public ResourceData(string name)
    {
        ResourceName = name;
        Quantity = 0;
    }

    public void IncreaseQuantitiy(int val)
    {
        Quantity += val;
    }

    public void DecreaseQuantity(int val)
    {
        Quantity -= val;
    }
}
