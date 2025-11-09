using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            for (int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {
                if (chunksToCheck[x].EnvironmentObjectsInChunk[y].GetType() == typeof(ConstructableObjectInstance)) {
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




    public static List<ConstructableObjectInstance> GetConstrutableObjectsInBounds(Vector3 p1, Vector3 p2)
    {
        constructedObjectCache.Clear();
        Vector3 point = Vector3.Lerp(p1, p2, .5f);
        float maxDist = Vector3.Distance(p1, p2);
        Vector3 low = Vector3.zero, high = Vector3.zero;

        SetHighAndLowPoints(p1, p2, ref low, ref high);

        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);
        ConstructableObjectInstance obj;
        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {
                obj = chunksToCheck[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance;

                if (obj!=null
                    && (IsPointInRangeOfBounds(chunksToCheck[x].EnvironmentObjectsInChunk[y].GetPosition(), low, high)
                    || obj.IsPointInBounds(point)))
                {

                    constructedObjectCache.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance);
                }
            }
        }

        return constructedObjectCache;
    }

    static bool IsPointInRangeOfBounds(Vector3 p, Vector3 low, Vector3 high)
    {
        if (p.x < low.x || p.x > high.x)
        {
            return false;
        }
        if (p.y < low.y || p.y > high.y)
        {
            return false;
        }
        //if (p.z < low.z || p.z > high.z)
        //{
        //    return false;
        //}

        return true;
    }

    public static void SetHighAndLowPoints(Vector3 p1, Vector3 p2, ref Vector3 low, ref Vector3 high)
    {
        if (p1.x < p2.x)
        {
            low.x = p1.x;
            high.x = p2.x;
        }
        else
        {
            low.x = p2.x;
            high.x = p1.x;
        }

        if (p1.y < p2.y)
        {
            low.y = p1.y;
            high.y = p2.y;
        }
        else
        {
            low.y = p2.y;
            high.y = p1.y;
        }

        if (p1.z < p2.z)
        {
            low.z = p1.z;
            high.z = p2.z;
        }
        else
        {
            low.z = p2.z;
            high.z = p1.z;
        }
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


    public static List<Constructable> GetConstrutablesInBounds(Vector3 p1, Vector3 p2)
    {
        constructableObjectCache.Clear();
        Vector3 point = Vector3.Lerp(p1, p2, .5f);
        Vector3 low = Vector3.zero, high = Vector3.zero;

        SetHighAndLowPoints(p1, p2, ref low, ref high);
        float maxDist = Vector2.Distance(low, high);

        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].ToBuild.Count; y++)
            {
                if (IsPointInRangeOfBounds(chunksToCheck[x].ToBuild[y].GetPosition(), low, high) || chunksToCheck[x].ToBuild[y].IsPointInBounds(point))
                {
                    constructableObjectCache.Add(chunksToCheck[x].ToBuild[y]);
                }
            }
        }


        return constructableObjectCache;
    }

    static List<ConstructableObjectInstance> constructableObjectInstances=new List<ConstructableObjectInstance>();
    public static ConstructableObjectInstance GetConstructedObjectsInRange(Vector3 point,float maxDist)
    {
        constructableObjectInstances.Clear();
        point.z = 0;
        ConstructableObjectInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {
                if (chunksToCheck[x].EnvironmentObjectsInChunk[y].GetType() == typeof(ConstructableObjectInstance))
                {
                    constructableObjectInstances.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance);
                }
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
                retVal = constructableObjectInstances[x];
                closest = curDist;
            }

        }
        return retVal;
    }

    static List<WallSegment> WallTileCache = new List<WallSegment>();
    public static WallSegment GetWallTilesWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        WallTileCache.Clear();
        point.z = 0;
        WallSegment retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int q = 0; q < chunksToCheck.Count; q++)
        {
           for(int x = 0; x < chunksToCheck[q].WallSegments.GetLength(0); x++)
            {
                for(int y=0;y< chunksToCheck[q].WallSegments.GetLength(1); y++)
                {
                    if (chunksToCheck[q].WallSegments[x, y].WallType != WallType.None)
                    {
                        WallTileCache.Add(chunksToCheck[q].WallSegments[x, y]);
                    }
                }
           }
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < WallTileCache.Count; x++)
        {
            objPosition = WallTileCache[x].Position();
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = WallTileCache[x];
                closest = curDist;
            }

        }
        return retVal;
    }



    public static EnvironmentObjectInstance GetEnvironmentObjectInstanceWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        environmentObjectInstance.Clear();
        point.z = 0;
        EnvironmentObjectInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {

                if (chunksToCheck[x].EnvironmentObjectsInChunk[y] as ConstructableObjectInstance != null)
                {
                    environmentObjectInstance.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y]);
                }
            }
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < environmentObjectInstance.Count; x++)
        {
            objPosition = environmentObjectInstance[x].GetPosition();
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = environmentObjectInstance[x];
                closest = curDist;
            }

        }
        return retVal;
    }

    static List<EnvironmentObjectInstance> environmentObjectInstance = new List<EnvironmentObjectInstance>();
    public static EnvironmentObjectInstance GetHarvestableObjectInstanceWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        environmentObjectInstance.Clear();
        point.z = 0;
        EnvironmentObjectInstance retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].EnvironmentObjectsInChunk.Count; y++)
            {

                if (chunksToCheck[x].EnvironmentObjectsInChunk[y].CanHarvest()) {
                    environmentObjectInstance.Add(chunksToCheck[x].EnvironmentObjectsInChunk[y]);
                }
            }
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < environmentObjectInstance.Count; x++)
        {
            objPosition = environmentObjectInstance[x].GetPosition();
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = environmentObjectInstance[x];
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


    static List<Inventory> InventoryObjectCache = new List<Inventory>();
    public static Inventory GetInventoryObjectWithinRangeOfPoint(Vector3 point, float maxDist)
    {
        InventoryObjectCache.Clear();
        point.z = 0;
        Inventory retVal = null;
        chunksToCheck = WorldChunkManager.Instance.GetChunksInRadius(maxDist + (WorldChunkManager.ChunkSize), point);

        for (int x = 0; x < chunksToCheck.Count; x++)
        {
            for (int y = 0; y < chunksToCheck[x].StaticContainersInChunk.Count; y++)
            {


                InventoryObjectCache.Add(chunksToCheck[x].StaticContainersInChunk[y]);

            }
        }

        float closest = 99999f;
        float curDist = -1f;
        Vector3 objPosition = Vector3.zero;

        for (int x = 0; x < InventoryObjectCache.Count; x++)
        {
            objPosition = InventoryObjectCache[x].transform.position;
            curDist = Vector3.Distance(point, objPosition);

            if (curDist < maxDist && curDist < closest)
            {
                retVal = InventoryObjectCache[x];
                closest = curDist;
            }

        }
        return retVal;
    }


    static List<Unit> toCheck = new List<Unit>();
    static List<WorldChunk> chunksToCheck = new List<WorldChunk>();
    public static Unit GetHostileUnitWithinRangeOfPoint(Vector3 point, float maxDist)
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
            if (toCheck[x] == null)
            {
                continue;
            }
            unitPosition = toCheck[x].transform.position;
            unitPosition.z = 0;
            if (FactionController.Instance.IsHostile(toCheck[x], FactionController.USER_FACTION))
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
            if (toCheck[x] == null)
            {
                continue;
            }
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


    public static SelectableType GetSelectablesInRange(out List<Selectable> selectables)
    {
        selectables = new List<Selectable>();

        List<Unit> units = UnitMoniter.Instance.GetUnitsWithinBounds(CursorSelect.Instance.startPoint, CursorSelect.Instance.endPoint);
        if (units.Count > 0)
        {
            selectables.AddRange(units);
            return SelectableType.Unit;
        }

        Unit hoverdOver = SelectionUtilities.GetUserUnitWithinRangeOfPoint(Vector3.Lerp(CursorSelect.Instance.startPoint, CursorSelect.Instance.endPoint,.5f), 1f);
        if (hoverdOver != null)
        {
            selectables.Add(hoverdOver);
            return SelectableType.Unit;
        }
        List<WallSegment> wallSegments = WorldController.Instance.WallManager.GetWallSegments(CursorSelect.Instance.startPoint, CursorSelect.Instance.endPoint) ;
        if (wallSegments.Count > 0)
        {
            selectables.AddRange(wallSegments);
            return SelectableType.Structure;
        }

        List<ConstructableObjectInstance> constructedObjects = GetConstrutableObjectsInBounds(CursorSelect.Instance.startPoint, CursorSelect.Instance.endPoint);
        if (constructedObjects.Count > 0)
        {
            selectables.AddRange(constructedObjects);
            return SelectableType.ConstructableObject;
        }

        List<Constructable> constructables = GetConstrutablesInBounds(CursorSelect.Instance.startPoint, CursorSelect.Instance.endPoint);
        if (constructables.Count > 0)
        {
            selectables.AddRange(constructables);
            return SelectableType.UnderConstructionObject;
        }


        return SelectableType.None;
    }

    static Bounds BoundsForCheck;

    public static bool IsInBounds(Vector3 size, Vector3 center, Vector3 pointToCheck)
    {
        BoundsForCheck = new Bounds(center, size);
        return BoundsForCheck.Contains(pointToCheck);
    }
}
