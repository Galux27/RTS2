using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallManager
{
    public int width, height;
   public WallManager(int width,int height)
   {
        this.width = width;
        this.height = height;
       
        WorldController.Instance.OnTileEnterAction += OnTileEnter;
        WorldController.Instance.OnTileExitAction += OnTileExit;
    }

    public void OnTileEnter(Vector2Int coords,Unit unit)
    {
        WallSegment wall = WallHelpers.GetWallAtCoords(coords);

        if (wall.WallType == WallType.Door) 
        {
            DoorSegment ds = wall as DoorSegment;
            if (ds==null)
            {
                return;
            }
            ds.UnitEnterDoor(unit);
        }

    }

    public bool DoesSomethingExistAtCoords(Vector2Int coords)
    {
        WallSegment wall = WallHelpers.GetWallAtCoords(coords);


        if (wall.WallType == WallType.Door|| wall.WallType == WallType.Wall)
        {
            return true;
        }
        return false;
    }

    public DoorSegment IsThereADoorAtCoords(int x, int y)
    {
        WallSegment wall = WallHelpers.GetWallAtCoords(x,y);

        if (wall.WallType == WallType.Door)
        {
           return wall as DoorSegment; 
        }
        return null;
    }
    public void OnTileExit(Vector2Int coords, Unit unit)
    {
        WallSegment wall = WallHelpers.GetWallAtCoords(coords);
        if (wall.WallType == WallType.Door) {

            DoorSegment ds =wall as DoorSegment;
            if (ds == null)
            {
                return;
            }
            ds.UnitExitDoor(unit);
        }
    }


    public void SetWall(int x, int y, WallTile wallTile, bool value = true)
    {
        WallSegment wall= WallHelpers.GetWallAtCoords(x, y);

        wall.SetHasWall(value) ;
        wall.SetWallType(wallTile);
        GenerateWallCollider(wall);
    }

    public void GenerateWallCollider(WallSegment wall)
    {
        return;
        if (wall.Collider != null||wall.WallType==WallType.None)
        {
            return;
        }
        GameObject col = GameObject.Instantiate(WorldController.Instance.WallCollider, new Vector3(wall.x + .5f, wall.y + .5f, 0), Quaternion.identity);
        col.name = "Wall Collider " + wall.x + "," + wall.y + "||" + wall.localCoords + "|" + wall.WallType.ToString();
        wall.Collider = col;
    }

    public void SetDoor(int x,int y,Tilemap toPlaceOn,WallTile wallType)
    {
        WallSegment newWall = WallHelpers.ChangeWallAtCoords(x,y,toPlaceOn,wallType);
        GenerateWallCollider(newWall);
    }

   

    public void RenderWalls(Tilemap toDrawOn)
    {
        WallSegment wall = null;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                wall = WallHelpers.GetWallAtCoords(x, y);

                if (wall!=null && wall.HasWall)
                {

                    WallHelpers.CalculateTileType(ref wall, this, wall.baseWallType);
                    WorldController.Instance.SetTraversible(x, y, false,WorldTileContents.Wall);
                }
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                wall = WallHelpers.GetWallAtCoords(x, y);
                if (wall != null)
                {
                    toDrawOn.SetTile(new Vector3Int(x, y, 0), wall.ToDraw);
                }
                }
            }
    }

    public void RemoveSingleWall(int x, int y, Tilemap toDrawOn, WallTile toUse,bool alterHealth=true)
    {
        
        SetWall(x, y,toUse,false);
        WallSegment toRemove = WallHelpers.GetWallAtCoords(x, y);
        if (alterHealth)
        {
            toRemove.AdjustHealth(-9999);
        }
        WallHelpers.CalculateTileType(ref toRemove, this, toRemove.baseWallType);

        toDrawOn.SetTile(new Vector3Int(x, y, 0), null);
        WallSegment wall = null;
        for (int x1 = 0; x1 < width; x1++)
        {
            for (int y1 = 0; y1 < height; y1++)
            {
                wall = WallHelpers.GetWallAtCoords(x1, y1);
                if (wall.HasWall)
                {
                    WallHelpers.CalculateTileType(ref wall, this, wall.baseWallType);
                    WorldController.Instance.SetTraversible(x1, y1, !wall.HasWall,WorldTileContents.Wall);
                }
            }
        }
        OnWallRemoved.Invoke(new Vector2Int(x,y));
        for (int x1 = x - 1; x1 <= x + 1; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (!CoordsValid(x1, y1))
                {
                    continue;
                }
                wall = WallHelpers.GetWallAtCoords(x1, y1);

                if (wall.HasWall)
                {
                    toDrawOn.SetTile(new Vector3Int(x1, y1, 0), wall.ToDraw);
                }

            }
        }
    }

    public static Action<Vector2Int> OnWallRemoved, OnWallAdded;


    public List<WallSegment> GetWallSegments(Vector3 low,Vector3 high)
    {
        Vector3 l = low, h = high;
        SelectionUtilities.SetHighAndLowPoints(low, high,ref l,ref h);

        List<WallSegment> retVal = new List<WallSegment>();
        Vector2Int coords = Vector2Int.zero;
        WallSegment wall = null;
        if (Vector3.Distance(l, h) > 1f) 
        { 
            for (float x = l.x; x < h.x; x += 1f)
            {
                for(float y=l.y;y < h.y; y += 1f)
                {
                    
                    coords = WorldController.Instance.ConvertWorldToTileCoords(new Vector3(x,y,0));
                    wall = WallHelpers.GetWallAtCoords(coords);
                    if (wall.HasWall || wall.HasDoor)
                    {
                        retVal.Add(wall);
                    }
                }
            }
        }
        else
        {
            coords = WorldController.Instance.ConvertWorldToTileCoords(l);
            wall = WallHelpers.GetWallAtCoords(coords);
            if (wall.HasWall || wall.HasDoor)
            {
                retVal.Add(wall);
            }
        }


        return retVal;
    }

   public bool CoordsValid(int x,int y)
    {
        return true;
    }

    public void AddSingleDoor(int x,int y,Tilemap toDrawOn, WallTile toUse)
    {

        SetDoor(x, y,toDrawOn,toUse);
        WorldController.Instance.WallManager.GenerateWallCollider(WallHelpers.GetWallAtCoords(x, y));
        Vector2Int asCoords = new Vector2Int(x, y);
        Vector2Int toGetFromCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(asCoords);
        WorldChunk toGetFrom = WorldChunkManager.Instance.GetWorldChunkFromTileCoords(asCoords); //Chunks[toGetFromCoords.x, toGetFromCoords.y];
        if (toGetFrom == null)
        {
            return;
        }
        EnvironmentObjectInstance objAtWall = null;
        if (toGetFrom.DoesAnyObjectExistAtCoords(asCoords, out objAtWall))
        {
            if (EnvironmentObjectHelpers.GetEnvironmentObject(objAtWall.ObjectKey).IsDecoration)
            {
                objAtWall.AdjustHealth(-9999999f);

            }
        }
        WallSegment wall = null;
        for (int x1 = 0; x1 < width; x1++)
        {
            for (int y1 = 0; y1 < height; y1++)
            {
                wall = WallHelpers.GetWallAtCoords(x1, y1);
                if (wall.HasWall)
                {
                    WallHelpers.CalculateTileType(ref wall, this, wall.baseWallType);
                    WorldController.Instance.SetTraversible(x1, y1, !wall.HasWall, WorldTileContents.Door);
                }
            }
        }

        OnWallAdded?.Invoke(asCoords);
        for (int x1 = x - 1; x1 <= x + 1; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (!CoordsValid(x1, y1))
                {
                    continue;
                }
                wall = WallHelpers.GetWallAtCoords(x1, y1);

                if (wall.HasWall)
                {

                    toDrawOn.SetTile(new Vector3Int(x1, y1, 0), wall.ToDraw);
                }
            }
        }
        WorldController.Instance.SetTraversible(x, y, true,WorldTileContents.Door);
        WorldController.Instance.AddPathfindingModifier(x, y, new PathNodeModifier_Door());
    }

    public void AddSingleWall(int x,int y,Tilemap toDrawOn,WallTile toUse)
    {
        SetWall(x, y,toUse);
        // WallHelpers.CalculateTileType(ref WallsInWorld[x, y], this, toUse);
        Vector2Int asCoords = new Vector2Int(x, y);
        Vector2Int toGetFromCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(asCoords);
        WorldChunk toGetFrom = WorldChunkManager.Instance.GetWorldChunkFromTileCoords(asCoords);//Chunks[toGetFromCoords.x, toGetFromCoords.y];
        if (toGetFrom == null)
        {
            return;
        }
        EnvironmentObjectInstance objAtWall = null;
        if (toGetFrom.DoesAnyObjectExistAtCoords(asCoords, out objAtWall))
        {
            if (EnvironmentObjectHelpers.GetEnvironmentObject(objAtWall.ObjectKey).IsDecoration)
            {
                objAtWall.AdjustHealth(-9999999f);
            }
        }
        WallSegment wall = null;

        wall = WallHelpers.GetWallAtCoords(x, y);
        if (wall.WallType == WallType.Wall)
        {
            WorldController.Instance.SetTraversible(x, y,false,WorldTileContents.Door);
        }
        WallHelpers.CalculateTileType(ref wall, this, wall.baseWallType);
        OnWallAdded?.Invoke(asCoords);

        for (int x1 = x - 1; x1 <= x + 1; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (!CoordsValid(x1, y1))
                {
                    continue;
                }
                wall = WallHelpers.GetWallAtCoords(x1, y1);

                if (wall.HasWall)
                {
                    WallHelpers.CalculateTileType(ref wall, this, wall.baseWallType);
                    wall.RenderWall();

                }
            }
        }
    }

   

}
