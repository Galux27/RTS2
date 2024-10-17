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
                WallsInWorld[x,y] = new WallSegment(x,y,false);
            }
        }
    }

    public void SetWall(int x,int y)
    {
        WallsInWorld[x, y].SetHasWall(true);

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


}
