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
        return Quantity>1;
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

    public Object[] SplitStack(int quantityWanted)
    {
        return null;
    }

    public Object[] SplitStack(float weightWanted)
    {
        return null;
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
}
