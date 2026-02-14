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

    private void Awake()
    {
        InitResourceManager();
    }

    public void SetUserResources(Dictionary<string, ResourceData> data)
    {
        if (UserResources == null)
        {
            InitResourceManager();
        }
        foreach(KeyValuePair<string, ResourceData> kvp in data)
        {
            AddQuantityOfResource(kvp.Value.ResourceName, kvp.Value.Quantity);
        }
    }

    public int GetResourceCapacity(string resourceKey)
    {
        if (UserResources.ContainsKey(resourceKey))
        {
            return UserResources[resourceKey].GetCapacity();
        }
        return 0;
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
        ResourceCostUI.Instance.CreateUIElement(data);
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

    public void UpdateResourceCapacity(string name)
    {
        UserResources[name].NeedsCapacityRefresh = true;

    }
    public void UpdateResourceUI()
    {
        foreach(KeyValuePair<string,ResourceData> kvp in UserResources)
        {
            ResourcesDisplayUI.Instance.UpdateUIElement(kvp.Value);

        }
    }

    public void AddQuantityOfResource(string key,int quantity)
    {
        Debug.Log("Adding quantity of " + key + " q " + quantity);
        UserResources[key].IncreaseQuantitiy(quantity);
        ResourcesDisplayUI.Instance.UpdateUIElement(UserResources[key]);
        OnRefreshResourceData?.Invoke();
    }

    public void ReduceQuantity(string key,int quantity)
    {
        UserResources[key].DecreaseQuantity( quantity);
        ResourcesDisplayUI.Instance.UpdateUIElement(UserResources[key]);
        OnRefreshResourceData?.Invoke();
    }

    public bool DoWeHaveEnoughSpaceForResource(string key)
    {
        return UserResources[key].Quantity < GetResourceCapacity(key);
    }

    public bool DoWeHaveEnoughOfResource(string key,int needed)
    {
        return UserResources[key].Quantity >= needed;
    }
   

    public Action OnRefreshResourceData;
    public List<Storage> StoragesToUse = new List<Storage>();
}


/// <summary>
/// Class to store the data on a resource in the users game
/// </summary>
public class ResourceData
{
    public string ResourceName;
    public int Quantity;
    public bool NeedsCapacityRefresh = true;
    public int CapacityCache = 0;
    public ResourceData(string name)
    {
        ResourceName = name;
        Quantity = 0;
    }

    public void IncreaseQuantitiy(int val)
    {
        Quantity += val;
        //if (Quantity > GetCapacity())
        //{
        //    Quantity = GetCapacity();
        //}
    }

    public void DecreaseQuantity(int val)
    {
        Quantity -= val;
    }

    public int GetCapacity()
    {
        if (NeedsCapacityRefresh)
        {
            CapacityCache = 0;
            CapacityCache = ResourceController.Instance.AllResources[ResourceName].BaseCapacity;

            for(int x = 0; x < RoomManager.Instance.roomList.Count; x++)
            {
                if (!RoomManager.Instance.roomList[x].CanUseRoom())
                {
                    continue;
                }
                for(int q = 0; q < RoomManager.Instance.roomList[x].ObjectsInRoom.Count; q++)
                {
                    string key = (RoomManager.Instance.roomList[x].ObjectsInRoom[q].ObjectKey);
                    EnvironmentObject envObj = EnvironmentObjectHelpers.GetEnvironmentObject(key);
                    if (envObj != null)
                    {
                        if (envObj.CapacityData != null && envObj.CapacityData.IncreasesCapacityForResource(ResourceName))
                        {
                            CapacityCache += envObj.CapacityData.GetCapacityIncreaseForResource(ResourceName);
                        }
                    }
                }
            }

        }
        return CapacityCache;
    }
}
