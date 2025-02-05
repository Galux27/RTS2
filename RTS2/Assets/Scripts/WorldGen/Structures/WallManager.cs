using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallManager
{
    public WallSegment[,] WallsInWorld;
    int width, height;
   public WallManager(int width,int height)
   {
        this.width = width;
        this.height = height;
        WallsInWorld = new WallSegment[width, height];
        for(int x=0;x< width; x++)
        {
            for(int y=0;y< height; y++)
            {
                WallsInWorld[x,y] = new WallSegment(x,y);
            }
        }
        WorldController.Instance.OnTileEnterAction += OnTileEnter;
        WorldController.Instance.OnTileExitAction += OnTileExit;
    }

    public void OnTileEnter(Vector2Int coords,Unit unit)
    {
        if (WallsInWorld[coords.x, coords.y].WallType == WallType.Door) 
        {
            DoorSegment ds = WallsInWorld[coords.x, coords.y] as DoorSegment;
            if (ds==null)
            {
                return;
            }
            ds.UnitEnterDoor(unit);
        }

    }

    public bool DoesSomethingExistAtCoords(Vector2Int coords)
    {
        if (WallsInWorld[coords.x,coords.y].WallType == WallType.Door|| WallsInWorld[coords.x, coords.y].WallType == WallType.Wall)
        {
            return true;
        }
        return false;
    }

    public DoorSegment IsThereADoorAtCoords(int x, int y)
    {
        if (WallsInWorld[x, y].WallType == WallType.Door)
        {
           return WallsInWorld[x, y] as DoorSegment; 
        }
        return null;
    }
    public void OnTileExit(Vector2Int coords, Unit unit)
    {
        if (WallsInWorld[coords.x,coords.y].WallType==WallType.Door) {
            DoorSegment ds = WallsInWorld[coords.x, coords.y] as DoorSegment;
            if (ds == null)
            {
                return;
            }
            ds.UnitExitDoor(unit);
        }
    }


    public void SetWall(int x, int y, bool value = true)
    {
        WallsInWorld[x, y].SetHasWall(value) ;
        GenerateWallCollider(x, y);
    }

    public void GenerateWallCollider(int x,int y)
    {
        GameObject col = GameObject.Instantiate(WorldController.Instance.WallCollider, new Vector3(x+.5f, y+.5f, 0), Quaternion.identity);
        WallsInWorld[x,y].Collider= col;
    }

    public void SetDoor(int x,int y,Tilemap toPlaceOn)
    {
        WallsInWorld[x,y]=new DoorSegment(x,y,toPlaceOn);
        GenerateWallCollider(x,y);
    }

    public void DrawSomeRandomWalls()
    {
        for(int q = 0; q < 10; q++)
        {
            int x = Random.Range(5, width - 5);
            int y=Random.Range(5, height - 5);

            for(int x1=x; x1 < x + 5; x1++)
            {
                for( int y1=y; y1 < y + 5; y1++)
                {
                    WallsInWorld[x1, y1].SetHasWall(true);
                }
            }
        }
    }

    public void RenderWalls(Tilemap toDrawOn, WallTile toUse)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (WallsInWorld[x, y].HasWall)
                {
                    WallHelpers.CalculateTileType(ref WallsInWorld[x, y], this, toUse);
                    WorldController.Instance.SetTraversible(x, y, false);
                }
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                toDrawOn.SetTile(new Vector3Int(x, y, 0), WallsInWorld[x, y].ToDraw);
            }
        }
    }

    public void RemoveSingleWall(int x, int y, Tilemap toDrawOn, WallTile toUse)
    {
        
        SetWall(x, y,false);
        WallHelpers.CalculateTileType(ref WallsInWorld[x, y], this, toUse);

        toDrawOn.SetTile(new Vector3Int(x, y, 0), null);

        for (int x1 = 0; x1 < width; x1++)
        {
            for (int y1 = 0; y1 < height; y1++)
            {
                if (WallsInWorld[x1, y1].HasWall)
                {
                    WallHelpers.CalculateTileType(ref WallsInWorld[x1, y1], this, toUse);
                    WorldController.Instance.SetTraversible(x1, y1, !WallsInWorld[x1, y1].HasWall);
                }
            }
        }

        for (int x1 = x - 1; x1 <= x + 1; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (!CoordsValid(x1, y1))
                {
                    continue;
                }

                if (WallsInWorld[x1, y1].HasWall)
                {
                    toDrawOn.SetTile(new Vector3Int(x1, y1, 0), WallsInWorld[x1, y1].ToDraw);
                }

            }
        }
    }

    public List<WallSegment> GetWallSegments(Vector3 low,Vector3 high)
    {
        Vector3 l = low, h = high;
        SelectionUtilities.SetHighAndLowPoints(low, high,ref l,ref h);

        List<WallSegment> retVal = new List<WallSegment>();
        Vector2Int coords = Vector2Int.zero;
        Debug.Log("Wall Check: " + l + "|" + h);
        if (Vector3.Distance(l, h) > 1f) { 
            for (float x = l.x; x < h.x; x += 1f)
            {
                for(float y=l.y;y < h.y; y += 1f)
                {
                    coords = WorldController.Instance.ConvertWorldToTileCoords(new Vector3(x,y,0));
                    if (WallsInWorld[coords.x, coords.y].HasWall || WallsInWorld[coords.x, coords.y].HasDoor)
                    {
                        retVal.Add(WallsInWorld[coords.x, coords.y]);
                    }
                }
            }
        }
        else
        {
            coords = WorldController.Instance.ConvertWorldToTileCoords(Vector3.Lerp(l,h,.5f));
            if (WallsInWorld[coords.x, coords.y].HasWall || WallsInWorld[coords.x, coords.y].HasDoor)
            {
                retVal.Add(WallsInWorld[coords.x, coords.y]);
            }
        }


        return retVal;
    }

    bool CoordsValid(int x,int y)
    {
        return x>0&&y>0 &&x<width&&y<height;
    }

    public void AddSingleDoor(int x,int y,Tilemap toDrawOn, WallTile toUse)
    {
        SetDoor(x, y,toDrawOn);
        // WallHelpers.CalculateTileType(ref WallsInWorld[x, y], this, toUse);

        for (int x1 = 0; x1 < width; x1++)
        {
            for (int y1 = 0; y1 < height; y1++)
            {
                if (WallsInWorld[x1, y1].HasWall)
                {
                    WallHelpers.CalculateTileType(ref WallsInWorld[x1, y1], this, toUse);
                    WorldController.Instance.SetTraversible(x1, y1, !WallsInWorld[x1, y1].HasWall);
                }
            }
        }


        for (int x1 = x - 1; x1 <= x + 1; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (!CoordsValid(x1, y1))
                {
                    continue;
                }

                if (WallsInWorld[x1, y1].HasWall)
                {

                    toDrawOn.SetTile(new Vector3Int(x1, y1, 0), WallsInWorld[x1, y1].ToDraw);
                }
            }
        }
        WorldController.Instance.SetTraversible(x, y, true);
        WorldController.Instance.AddPathfindingModifier(x, y, new PathNodeModifier_Door());
    }

    public void AddSingleWall(int x,int y,Tilemap toDrawOn,WallTile toUse)
    {
        SetWall(x, y);
       // WallHelpers.CalculateTileType(ref WallsInWorld[x, y], this, toUse);


        for (int x1 = 0; x1 < width; x1++)
        {
            for (int y1 = 0; y1 < height; y1++)
            {
                if (WallsInWorld[x1, y1].HasWall)
                {
                    WallHelpers.CalculateTileType(ref WallsInWorld[x1, y1], this, toUse);
                    WorldController.Instance.SetTraversible(x1, y1, !WallsInWorld[x1, y1].HasWall);
                }
            }
        }


        for (int x1 = x - 1; x1 <= x + 1; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (!CoordsValid(x1, y1))
                {
                    continue;
                }

                if (WallsInWorld[x1, y1].HasWall)
                {

                    toDrawOn.SetTile(new Vector3Int(x1, y1, 0), WallsInWorld[x1, y1].ToDraw);
                }
            }
        }
    }

}
