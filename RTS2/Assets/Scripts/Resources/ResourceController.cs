using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    static ResourceController instance;
    public static ResourceController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ResourceController>(true);
            }
            return instance;
        }
    }


    public Dictionary<string, Resource> AllResources;

    private void Awake()
    {
        LoadResourceTypes();
    }
    const string ResourceLocation = "Game_Resources";

    void LoadResourceTypes()
    {
        AllResources = new Dictionary<string, Resource>();
        Object[] resources = Resources.LoadAll(ResourceLocation);
        for (int x = 0; x < resources.Length; x++)
        {
            Resource i = (Resource)resources[x];
            if (AllResources.ContainsKey(i.Name) == false)
            {
                AllResources.Add(i.Name, i);
            }
        }
        Debug.Log("Resources: loaded " + AllResources.Count);
    }

    public GameObject CreateResourceInstance(ResourceInstanceData toRender, Vector3 positionOverride = default)
    {
        GameObject retVal = new GameObject();
        retVal.name = toRender.Name();
        SpriteRenderer sr = retVal.AddComponent<SpriteRenderer>();
        sr.sprite = AllResources[toRender.Name()].Item;
        ResourceInstance resourceInstance = retVal.AddComponent<ResourceInstance>();
        resourceInstance.InstanceData = toRender;
        if (positionOverride != default)
        {
            resourceInstance.transform.position = positionOverride;
        }

        return retVal;
    }


    public GameObject CreateResourceInstance(Resource toCreate, int quantity)
    {
        GameObject retVal = new GameObject();
        retVal.name = toCreate.name;
        SpriteRenderer sr = retVal.AddComponent<SpriteRenderer>();
        sr.sprite = toCreate.Item;
        ResourceInstance resourceInstance = retVal.AddComponent<ResourceInstance>();
        resourceInstance.InstanceData = new ResourceInstanceData(toCreate.name, quantity);


        return retVal;
    }

    public bool DoesResourceTypeExist(string key)
    {
        return AllResources.ContainsKey(key);
    }
}


