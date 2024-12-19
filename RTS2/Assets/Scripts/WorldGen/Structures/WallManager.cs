using System.Collections;
using System.Collections.Generic;
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
    }

    public void SetWall(int x, int y, bool value = true)
    {
        WallsInWorld[x, y].SetHasWall(value) ;

    }

    public void SetDoor(int x,int y,Tilemap toPlaceOn)
    {
        WallsInWorld[x,y]=new DoorSegment(x,y,toPlaceOn);
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
