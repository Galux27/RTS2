using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BehaviourUtilities 
{
    static List<Unit> GetUnitCache=new List<Unit>();
    public static List<Unit> GetTargetsThatAreNotType(Unit searching,float range,UnitType toFilter)
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
            if (GetUnitCache[x].MyType != toFilter)
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

    public static Unit GetClosestTargetThatsNotType(Unit searching,float range,UnitType toFilter)
    {
        GetUnitCache = GetTargetsThatAreNotType(searching,range,toFilter);
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
}
