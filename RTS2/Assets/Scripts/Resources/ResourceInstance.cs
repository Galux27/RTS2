using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInstance : MonoBehaviour,ISerialize
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

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.ObjectKey, data.Name());
        retVal.AddDataToSerialize(DataKeys.Quantitiy, data.Quantity);
        retVal.AddDataToSerialize(DataKeys.Pos, this.transform.position);
        retVal.AddDataToSerialize(DataKeys.UID, GetMyUID().Value);


        return retVal;
    }

    public SerializedData Serialize()
    {
        throw new System.NotImplementedException();
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value == 0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);

        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
        myUid = new UID(uid);
        IDManager.OnUIDCreated(this, myUid);
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

    public DataToSerialize GetDataToSerialize()
    {
        throw new System.NotImplementedException();
    }

    public SerializedData Serialize()
    {
        throw new System.NotImplementedException();
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }

    public UID GetMyUID()
    {
        throw new System.NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
        //myUID = new UID(uid);
    }
}
