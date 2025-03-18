using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores utilities used in the various behaviour scripts
/// </summary>
public static class BehaviourUtilities 
{

    //using a cache to not allocate memory every time we need to find units
    static List<Unit> GetUnitCache=new List<Unit>();
    public static List<Unit> GetHostileUnits(Unit searching,float range)
    {
        GetUnitCache.Clear();
       List<WorldChunk> toCheck = WorldChunkManager.Instance.GetChunksInRadius(range, searching.transform.position);
        List<Unit> result = new List<Unit>();
        for(int x = 0; x < toCheck.Count; x++)
        {
            GetUnitCache.AddRange(toCheck[x].UnitsInChunk);
        }
        float dist = 999999f;
        for(int x = 0; x < GetUnitCache.Count; x++)
        {
            if (FactionController.Instance.IsHostile(searching, GetUnitCache[x]))
            {
                dist = Vector3.Distance(GetUnitCache[x].transform.position, searching.transform.position);
                if (dist < range)
                {
                    result.Add(GetUnitCache[x]);
                }
            }
        }
        GetUnitCache.Clear();
        return result;
    }
    const float MaxDistForNearbyObject = 3f;
    public static ObjectInfo GetNearbyObjectToAttack(Unit searching,out bool foundSomething)
    {
        foundSomething = false;
        Vector2Int chunkImNear = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(searching.transform.position);
        WorldChunk toCheck= WorldChunkManager.Instance.Chunks[chunkImNear.x, chunkImNear.y];


        ObjectInfo retVal = null;
        ConstructableObjectInstance constructedObject;
        float dist = 99999999f;
        float dist2 = 0f;
        for(int x = 0; x < toCheck.EnvironmentObjectsInChunk.Count; x++)
        {
            constructedObject = toCheck.EnvironmentObjectsInChunk[x] as ConstructableObjectInstance;
            if (constructedObject != null)
            {
                dist2=Vector3.Distance(constructedObject.Position(), searching.transform.position);
                if (dist2 < dist && dist2<MaxDistForNearbyObject)
                {
                    dist=dist2;
                    retVal = constructedObject;
                    foundSomething = true;
                }
            }
        }

        



       


        return retVal;
    }

    
    public static Unit GetClosestTargetThatsHostile(Unit searching,float range)
    {
        GetUnitCache = GetHostileUnits(searching,range);
        if(GetUnitCache.Count==0) return null;
        float dist = 9999999f;
        Unit retVal = null;
        for(int x=0; x < GetUnitCache.Count;x++)
        {
            float dist2 = Vector3.Distance(searching.transform.position, GetUnitCache[x].transform.position);
            if (dist2 < dist)
            {
                retVal = GetUnitCache[x];
                dist = dist2;
            }
        }
        return retVal;
    }

    public static Vector3 GetPositionAwayFromTarget(Vector3 posToAvoid)
    {
        PathfindingNode runFrom = Pathfinding.GetNodeFromPosition(posToAvoid + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5)) );

        return runFrom.worldPos;


    }

    public static bool CanIMoveInDirection(Vector3 pos,Vector3 dir,Unit performing)
    {
        return Pathfinding.GetNodeFromPosition(pos+dir,performing).GetPassable(performing);
    }


}
