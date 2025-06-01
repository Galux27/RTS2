using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Stores details on capacity for resource storage that an object will provide
/// </summary>
[System.Serializable]
public class ResourceCapacity 
{
    public string CapacityProvidedFor;
    public int CapacityProvided;
}

[System.Serializable]
public class ResourceCapacityData
{
    public List<ResourceCapacity> CapacityData; 

    public int GetCapacityIncreaseForResource(string name)
    {
        for (int x = 0; x < CapacityData.Count; x++)
        {
            if (CapacityData[x].CapacityProvidedFor == name)
            {
                return CapacityData[x].CapacityProvided;
            }
        }
        return 0;
    }
    public bool IncreasesCapacityForResource(string name)
    {
        for(int x=0;x<CapacityData.Count;x++)
        {
            if (CapacityData[x].CapacityProvidedFor == name)
            {
                return true;
            }
        }
        return false;
    }
}
