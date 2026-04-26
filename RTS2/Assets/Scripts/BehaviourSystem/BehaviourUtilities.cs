using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores utilities used in the various behaviour scripts
/// </summary>
public static class BehaviourUtilities 
{

    //using a cache to not allocate memory every time we need to find units
    static List<Unit> GetUnitCache=new List<Unit>(),GetHostileCache=new List<Unit>();
    public static List<Unit> GetHostileUnits(Unit searching,float range)
    {
        ChunksToCheck.Clear();

        GetUnitCache.Clear();
        ChunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(range, searching.transform.position);
        GetHostileCache.Clear();
            
         for (int x = 0; x < ChunksToCheck.Count; x++)
        {
            GetUnitCache.AddRange(ChunksToCheck[x].UnitsInChunk);
        }
        float dist = 999999f;
        for(int x = 0; x < GetUnitCache.Count; x++)
        {
            if (GetUnitCache[x] == null)
            {
                continue;
            }
            if (FactionController.Instance.IsHostile(searching, GetUnitCache[x]))
            {
                dist = Vector3.Distance(GetUnitCache[x].transform.position, searching.transform.position);
                if (dist < range)
                {
                    GetHostileCache.Add(GetUnitCache[x]);
                }
            }
        }
        return GetHostileCache;
    }
    static List<WallSegment> WallSectionCache = new List<WallSegment>();
    const int WallCheckRadius = 3;
    static WallSegment wallChecking;
    public static WallSegment GetNearbyWallSegmentToAttack(Unit searching,out bool foundSomething)
    {
        foundSomething = false;
        ChunksToCheck.Clear();
        //wallChecking = null;
        WallSectionCache.Clear();
        //foundSomething = false;
        //Vector2Int center = WorldController.Instance.ConvertWorldToTileCoords(searching.transform.position);
        ChunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(WallCheckRadius, searching.transform.position);
        for(int q = 0; q < ChunksToCheck.Count; q++)
        {
            for(int x = 0; x < WorldChunkManager.ChunkSize; x++)
            {
                for(int y=0;y < WorldChunkManager.ChunkSize; y++)
                {
                    wallChecking = ChunksToCheck[q].WallSegments[x, y];
                    if (wallChecking != null && wallChecking.WallType != WallType.None)
                    {
                        WallSectionCache.Add(wallChecking);
                    }
                }
            }
        }
        //for(int x = center.x - WallCheckRadius; x < center.x + WallCheckRadius; x++)
        //{
        //    for (int y = center.y - WallCheckRadius; y< center.y + WallCheckRadius; y++)
        //    {
        //        if (WorldController.Instance.WallManager.CoordsValid(x, y))
        //        {
        //            wallChecking=WallHelpers.GetWallAtCoords(x, y);
        //            if (wallChecking!=null && wallChecking.WallType!=WallType.None)
        //            {
        //                WallSectionCache.Add(wallChecking);
        //            }
        //        }
        //    }
        //}
        WallSegment retVal = null;
        float dist = 9999999f, dist2 = 0f;
        for (int x = 0; x < WallSectionCache.Count; x++)
        {
            dist2 = Vector3.Distance(WallSectionCache[x].Position(), searching.Position());
            if (dist2 < dist)
            {
                retVal = WallSectionCache[x];
                dist = dist2;
                foundSomething = true;
            }
        }
        return retVal;
    }


    const float MaxDistForNearbyObject =7f,ObjectCheckDist=20f;
    static List<EnvironmentObjectInstance> EnvironmentObjectCache=new List<EnvironmentObjectInstance>();
    static List<EnvironmentObjectInstance> AllObjectsCache=new List<EnvironmentObjectInstance>();
    static List<WorldChunk> ChunksToCheck = new List<WorldChunk>();
    public static ObjectInfo GetNearbyObjectToAttack(Unit searching,out bool foundSomething)
    {
        EnvironmentObjectCache.Clear();
        foundSomething = false;
        ChunksToCheck.Clear();
        AllObjectsCache.Clear();
        ChunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(ObjectCheckDist, searching.transform.position);
        for(int x = 0; x < ChunksToCheck.Count; x++)
        {
            AllObjectsCache.AddRange(ChunksToCheck[x].EnvironmentObjectsInChunk);
        }

        Vector2Int chunkImNear = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(searching.transform.position);


        ObjectInfo retVal = null;
        ConstructableObjectInstance constructedObject;
        float dist = 99999999f;
        float dist2 = 0f;
        for(int x = 0; x < AllObjectsCache.Count; x++)
        {
            constructedObject = AllObjectsCache[x] as ConstructableObjectInstance;
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

    static Unit UnitCache;
    public static Unit GetClosestTargetThatsHostile(Unit searching,float range)
    {
        GetHostileUnits(searching,range);
        if(GetHostileCache.Count==0) return null;
        float dist = 9999999f;
        UnitCache = null;
        for(int x=0; x < GetHostileCache.Count;x++)
        {
            float dist2 = Vector3.Distance(searching.transform.position, GetHostileCache[x].transform.position);
            if (dist2 < dist)
            {
                UnitCache = GetHostileCache[x];
                dist = dist2;
            }
        }
        return UnitCache;
    }

    public static Vector3 GetPositionAwayFromTarget(Vector3 posToAvoid)
    {

        Vector3 TargetPos = posToAvoid + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));

        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(TargetPos.x, TargetPos.y, out batchCoords, out chunkCoords, out tileCoords);

        while (WorldChunkManager.Instance.DoesBatchExist(batchCoords) == false)
        {
            TargetPos = posToAvoid + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));

            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(TargetPos.x, TargetPos.y, out batchCoords, out chunkCoords, out tileCoords);

        }
        PathfindingNode runFrom = Pathfinding.GetNodeFromPosition(TargetPos);

        return runFrom.worldPos;


    }
    static Vector2Int batchCoords, chunkCoords, tileCoords;

    public static bool CanIMoveInDirection(Vector3 pos,Vector3 dir,Unit performing)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(performing.transform.position.x + dir.x, performing.transform.position.y + dir.y, out batchCoords, out chunkCoords, out tileCoords);

        WallSegment wall = WorldChunkManager.Instance.GetChunkBatch(batchCoords).Chunks[chunkCoords.x, chunkCoords.y].WallSegments[tileCoords.x, tileCoords.y];
        if (wall.HasWall)
        {
            return false;
        }

        if (wall.HasDoor)
        {
            DoorSegment door = wall as DoorSegment;
            if (door.UnitCanUseDoor(performing))
            {
                return true;
            }
        }
        if (!WorldChunkManager.Instance.GetChunkBatch(batchCoords).Chunks[chunkCoords.x, chunkCoords.y].PathfindingNodes[tileCoords.x, tileCoords.y].IsPassable)
        {
            return false;
        }
        return true;
    }


}
