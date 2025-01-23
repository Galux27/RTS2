using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

[System.Serializable]
public class HarvestableResource
{
    public string ResourceToSpawn;
    public int QuantityToSpawn;
    public Vector3 RangeToSpawn;

    public void GenerateResources(Vector3 pos)
    {
        GameObject g = ResourceController.Instance.CreateResourceInstance(ResourceController.Instance.AllResources[ResourceToSpawn],Random.Range(1,QuantityToSpawn));
        g.transform.position= pos + new Vector3(Random.Range(0, RangeToSpawn.x), Random.Range(0, RangeToSpawn.y));
    }
}

[System.Serializable]
public class HarvestableResourceData
{
    public List<HarvestableResource> resources;
    public float HarvestLength = 0f;
    public void GenerateResoruces(Vector3 pos)
    {
        for(int x=0;x<resources.Count; x++)
        {
            resources[x].GenerateResources(pos);
        }
    }
}