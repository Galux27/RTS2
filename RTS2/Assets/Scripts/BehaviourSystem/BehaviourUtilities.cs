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
    static List<WallSegment> WallSectionCache = new List<WallSegment>();
    const int WallCheckRadius = 3;
    static WallSegment wallChecking;
    public static WallSegment GetNearbyWallSegmentToAttack(Unit searching,out bool foundSomething)
    {
        wallChecking = null;
        WallSectionCache.Clear();
        foundSomething = false;
        Vector2Int center = WorldController.Instance.ConvertWorldToTileCoords(searching.transform.position);
        
        for(int x = center.x - WallCheckRadius; x < center.x + WallCheckRadius; x++)
        {
            for (int y = center.y - WallCheckRadius; y< center.y + WallCheckRadius; y++)
            {
                if (WorldController.Instance.WallManager.CoordsValid(x, y))
                {
                    wallChecking=WallHelpers.GetWallAtCoords(x, y);
                    if (wallChecking.WallType!=WallType.None)
                    {
                        WallSectionCache.Add(wallChecking);
                    }
                }
            }
        }
        WallSegment retVal = null;
        float dist = 9999999f, dist2 = 0f ;
        for(int x=0;x<WallSectionCache.Count;x++)
        {
            dist2 = Vector3.Distance(WallSectionCache[x].Position(),searching.Position());
            if(dist2 < dist)
            {
                retVal = WallSectionCache[x];
                dist=dist2;
                foundSomething = true;
            }
        }
        return retVal;
    }


    const float MaxDistForNearbyObject =7f,ObjectCheckDist=20f;
    static List<EnvironmentObjectInstance> EnvironmentObjectCache=new List<EnvironmentObjectInstance>();
    public static ObjectInfo GetNearbyObjectToAttack(Unit searching,out bool foundSomething)
    {
        EnvironmentObjectCache.Clear();
        foundSomething = false;
        List<WorldChunk> chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(ObjectCheckDist, searching.transform.position);
        List<EnvironmentObjectInstance> allObjects = new List<EnvironmentObjectInstance>();
        for(int x = 0; x < chunksToCheck.Count; x++)
        {
            allObjects.AddRange(chunksToCheck[x].EnvironmentObjectsInChunk);
        }

        Vector2Int chunkImNear = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(searching.transform.position);


        ObjectInfo retVal = null;
        ConstructableObjectInstance constructedObject;
        float dist = 99999999f;
        float dist2 = 0f;
        for(int x = 0; x < allObjects.Count; x++)
        {
            constructedObject = allObjects[x] as ConstructableObjectInstance;
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
