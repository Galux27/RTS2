using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BehaviourUtilities 
{
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
        return Pathfinding.GetNodeFromPosition(pos+dir).GetPassable(performing);
    }


}
