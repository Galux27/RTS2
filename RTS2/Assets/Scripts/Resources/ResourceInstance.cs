using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInstance : MonoBehaviour
{

    ResourceInstanceData data;
    public ResourceInstanceData InstanceData
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            data.instanceInWorld = this.gameObject;
        }
    }
    private void Start()
    {
        Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(this.transform.position);
        WorldChunkManager.Instance.Chunks[chunk.x,chunk.y].AddResourceObject(this);
    }


    private void OnDestroy()
    {
        Vector2Int chunk = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(this.transform.position);
        WorldChunkManager.Instance.Chunks[chunk.x, chunk.y].RemoveResourceObject(this);
    }
}

[System.Serializable]
public class ResourceInstanceData:InventoryObject
{

    public ResourceInstanceData(string resource,int quantity) {
        Resource = resource;
        Quantity= quantity;
    }


    public string Resource;
    public int Quantity;
    public GameObject instanceInWorld;
    public bool CanSplitStack()
    {
        return true;
    }

    public string Name()
    {
        return Resource;
    }

    public void RepopulateData(InventoryObject toRepopulateWith)
    {
        ResourceInstanceData newData = toRepopulateWith as ResourceInstanceData;
        if (newData != null)
        {
            Quantity = newData.Quantity;
        }
    }

    public object[] SplitStack(int quantityWanted)
    {
        ResourceInstanceData[] retVal = new ResourceInstanceData[2];
        retVal[0] = new ResourceInstanceData(Resource, quantityWanted);
        this.Quantity -= quantityWanted;
        retVal[1] = this;
        return retVal;

    }

    public object[] SplitStack(float weightWanted)
    {

        int quantity = 0;
        float currentWeight = 0f;
        float weightPer = ResourceController.Instance.AllResources[Name()].WeightPerUnit;
        while (currentWeight < weightWanted)
        {
            quantity++;
            currentWeight += weightPer;
        }

        ResourceInstanceData[] retVal = new ResourceInstanceData[2];

        retVal[0] = new ResourceInstanceData(Resource, quantity);
        this.Quantity -= quantity;
        retVal[1] = this;

        return retVal;
    }

    public float Weight()
    {
        return ResourceController.Instance.AllResources[Name()].WeightPerUnit * Quantity;
    }

    int InventoryObject.Quantity()
    {
        return Quantity ;
    }

    public void OnAddedToInventory()
    {
        if(instanceInWorld != null)
        {
            GameObject.Destroy(instanceInWorld);
        }
    }

    public void OnRemovedFromInventory()
    {
        if(instanceInWorld== null)
        {
            ResourceController.Instance.CreateResourceInstance(this);
        }
    }

    public bool CanObjectBeEquiped()
    {
        return false;
    }

    public void EquipObject(Unit toEquipTo)
    {
        
    }

    public void MergeWith(InventoryObject obj)
    {
        Quantity += obj.Quantity();
    }
}
