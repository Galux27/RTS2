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
    }

    public void RefreshResourceData()
    {
        InitResourceManager();
        OnRefreshResourceData?.Invoke();
        
    }

    

    public void AddQuantityOfResource(string key,int quantity)
    {
        UserResources[key].DecreaseQuantity(quantity);
    }

    public void ReduceQuantity(string key,int quantity)
    {
        UserResources[key].DecreaseQuantity( quantity);
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
