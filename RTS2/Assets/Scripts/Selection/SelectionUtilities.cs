using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectionUtilities
{
    static List<ConstructableObjectInstance> construcableObjectCache = new List<ConstructableObjectInstance>();
    public static ConstructableObjectInstance GetConstructableObjectInstanceWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        construcableObjectCache.Clear();
       point.z = 0;
        ConstructableObjectInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for(int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {
                if (chunksToCheck[x].EnvironmentObjectsInChunk[y].GetType() == typeof(ConstructableObjectInstance)){
                    construcableObjectCache.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance);
                }
            }
        }


        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < construcableObjectCache.Count; x++)
        {
            objPosition.x = construcableObjectCache[x].PosX;
            objPosition.y = construcableObjectCache[x].PosY;
            curDist = Vector3.Distance(point, objPosition);
            Debug.Log("On Hover Constructable pos at " + objPosition + " cursor at " + point +" dist " +curDist);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = construcableObjectCache[x];
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
