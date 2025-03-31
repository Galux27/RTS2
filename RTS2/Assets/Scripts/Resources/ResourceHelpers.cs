using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceHelpers
{
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
