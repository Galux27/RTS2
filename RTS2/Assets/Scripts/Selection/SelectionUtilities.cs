using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class SelectionUtilities
{
    static List<ConstructableObjectInstance> constructedObjectCache = new List<ConstructableObjectInstance>();
    public static ConstructableObjectInstance GetConstructedObjectInRangeOfPoint(Vector3 point, float maxDist)
    {
        constructedObjectCache.Clear();
       point.z = 0;
        ConstructableObjectInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for(int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {
                if (chunksToCheck[x].EnvironmentObjectsInChunk[y].GetType() == typeof(ConstructableObjectInstance)){
                    constructedObjectCache.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance);
                }
            }
        }


        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < constructedObjectCache.Count; x++)
        {
            objPosition.x = constructedObjectCache[x].PosX;
            objPosition.y = constructedObjectCache[x].PosY;
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = constructedObjectCache[x];
                closest = curDist;
            }

        }
        return retVal;
    }


    public static List<Constructable> GetAllConstructablesInRangeOfObject(Vector3 point, float maxDist)
    {
        List<Constructable> retVal = new List<Constructable>();

        constructableObjectCache.Clear();
        point.z = 0;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].ToBuild.Count; y++)
            {
                constructableObjectCache.Add(chunksToCheck[x].ToBuild[y]);
            }
        }

        retVal.AddRange(constructableObjectCache);
        return retVal;
    }

    static List<Constructable> constructableObjectCache = new List<Constructable>();
    public static Constructable GetConstructableObjectInstanceWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        constructableObjectCache.Clear();
        point.z = 0;
        Constructable retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].ToBuild.Count; y++)
            {
                constructableObjectCache.Add(chunksToCheck[x].ToBuild[y]);
            }
        }


        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < constructableObjectCache.Count; x++)
        {
            objPosition = constructableObjectCache[x].GetPosition();
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = constructableObjectCache[x];
                closest = curDist;
            }

        }
        return retVal;
    }

    static List<EnvironmentObjectInstance> harvestableObjectCache = new List<EnvironmentObjectInstance>();
    public static EnvironmentObjectInstance GetHarvestableObjectInstanceWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        harvestableObjectCache.Clear();
        point.z = 0;
        EnvironmentObjectInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {
               
                if (chunksToCheck[x].EnvironmentObjectsInChunk[y].CanHarvest()){
                    harvestableObjectCache.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y]);
                }
            }
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < harvestableObjectCache.Count; x++)
        {
            objPosition = harvestableObjectCache[x].GetPosition();
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = harvestableObjectCache[x];
                closest = curDist;
            }

        }
        return retVal;
    }



    static List<ResourceInstance> ResourceInstanceObjectCache = new List<ResourceInstance>();
    public static ResourceInstance GetResourceInstanceObjectInstanceWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        ResourceInstanceObjectCache.Clear();
        point.z = 0;
        ResourceInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].ResourceObjectsInChunk.Count; y++)
            {


                ResourceInstanceObjectCache.Add(chunksToCheck[x].ResourceObjectsInChunk[y]);
               
            }
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < ResourceInstanceObjectCache.Count; x++)
        {
            objPosition = ResourceInstanceObjectCache[x].transform.position;
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = ResourceInstanceObjectCache[x];
                closest = curDist;
            }

        }
        return retVal;
    }

    static List<Unit> toCheck=new List<Unit>();
    static List<WorldChunk> chunksToCheck = new List<WorldChunk>();
    public static Unit GetHostileUnitWithinRangeOfPoint(Vector3 point,float maxDist)
    {
        point.z = 0;
        Unit retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist+(WorldChunkManager.ChunkSize), point);
        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            
            toCheck.AddRange(chunksToCheck[x].UnitsInChunk);
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 unitPosition = Vector3.zero;
        for (int x = 0; x < toCheck.Count;x++)
        {
            unitPosition = toCheck[x].transform.position;
            unitPosition.z= 0;
            if (FactionController.Instance.IsHostile(toCheck[x],FactionController.USER_FACTION))
            {
                curDist = Vector3.Distance(point, unitPosition);
                if (curDist < maxDist && curDist < closest)
                {
                    retVal = toCheck[x];
                    closest = curDist;
                }
            }
        }
        toCheck.Clear();
        chunksToCheck.Clear();
        return retVal;
    }

    public static Unit GetUserUnitWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        point.z = 0;
        Unit retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);
        for (int x = 0; x < chunksToCheck.Count; x++)
        {

            toCheck.AddRange(chunksToCheck[x].UnitsInChunk);
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 unitPosition = Vector3.zero;
        for (int x = 0; x < toCheck.Count; x++)
        {
            unitPosition = toCheck[x].transform.position;
            unitPosition.z = 0;
            if (toCheck[x].MyFaction.MyFactionID == FactionController.USER_FACTION)
            {
                curDist = Vector3.Distance(point, unitPosition);
                if (curDist < maxDist && curDist < closest)
                {
                    retVal = toCheck[x];
                    closest = curDist;
                }
            }
        }
        toCheck.Clear();
        chunksToCheck.Clear();
        return retVal;
    }

}
