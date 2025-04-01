using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceHelpers
{

    public static void CanMeetResourceRequirements(List<ResourceRequirement> resourceRequirements,Vector3 pos,float radius,out bool foundEnough,ref Dictionary<string,List<FoundResourceData>> data)
    {
        foundEnough = true;
        data = new Dictionary<string, List<FoundResourceData>>();
        List<FoundResourceData> foundResources = null;
        for(int x=0;x<resourceRequirements.Count;x++)
        {
            foundResources = new List<FoundResourceData>();
            bool localEnough = false;
            CanMeetResourceRequirement(resourceRequirements[x], pos, radius, out localEnough, out foundResources);
            if(localEnough==false)
            {
                foundEnough = false;
            }
            if (!data.ContainsKey(resourceRequirements[x].ResourceName))
            {
                data.Add(resourceRequirements[x].ResourceName, new List<FoundResourceData>());
                
            }
            data[resourceRequirements[x].ResourceName].AddRange(foundResources);
        }
    }
    public static void CanMeetResourceRequirement(ResourceRequirement requirement,Vector3 position,float radius,out bool foundEnough,out List<FoundResourceData> getFrom)
    {
        foundEnough = false;
        int quantity = 0;
        getFrom = new List<FoundResourceData>();

        List<WorldChunk> toSearch = WorldChunkManager.Instance.GetChunksInRadius(radius, position);
        Inventory searching = null;
        int countFound = 0;
        for(int x=0;x<toSearch.Count;x++)
        {
            for(int y = 0; y < toSearch[x].StaticContainersInChunk.Count;y++)
            {
                countFound = 0;
                searching = toSearch[x].StaticContainersInChunk[y];
                
                searching.ContainsItem(requirement.ResourceName,out countFound);

                if(quantity+countFound > requirement.QuantityRequired)
                {
                    countFound = requirement.QuantityRequired - quantity;
                }
                quantity += countFound;
                if(countFound > 0) {
                    getFrom.Add(new FoundResourceData(requirement.ResourceName, searching, countFound));
                }
               
            }
        }
        foundEnough = quantity >= requirement.QuantityRequired;

    }


    public static void ConsumeResources(Dictionary<string,List<FoundResourceData>> data)
    {
        foreach(KeyValuePair<string,List<FoundResourceData>> kvp in data)
        {
            for(int x = 0; x < kvp.Value.Count; x++)
            {
                kvp.Value[x].toGetFrom.RemoveQuantityOfObject(kvp.Value[x].itemToGet, kvp.Value[x].QuantityToTake);
            }
        }
    }
}

public class FoundResourceData
{
    public Inventory toGetFrom;
    public string itemToGet;
    public int QuantityToTake;

    public FoundResourceData(string toGet,Inventory getFrom,int quantityToTake)
    {
        toGetFrom = getFrom;
        itemToGet = toGet;
        QuantityToTake = quantityToTake;
    }
}
