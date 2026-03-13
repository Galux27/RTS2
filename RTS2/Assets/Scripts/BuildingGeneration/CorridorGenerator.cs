using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator
{
    public virtual void GenerateCorridor(Vector2Int size,GeneratedBuilding toPopulate,int width)
    {

    }
}

public class TShapeCorridorGenerator : CorridorGenerator
{
    public override void GenerateCorridor(Vector2Int size, GeneratedBuilding toPopulate, int width)
    {
        Vector2Int axis = new Vector2Int();
        int r = Random.Range(0, 100);
        if (r < 50)
        {
            axis.x = 0;
        }
        else
        {
            axis.x = 1;
        }

        r = Random.Range(0, 100);
        if (r < 50)
        {
            axis.y = 0;
        }
        else
        {
            axis.y = 1;
        }

        Vector2Int startCoord = new Vector2Int(axis.x*(toPopulate.Tiles.GetLength(0)-1), axis.y * (toPopulate.Tiles.GetLength(1) - 1));

        r = Random.Range(0, 100);
        if (r < 50)
        {
            startCoord.x = Random.Range(width, (toPopulate.Tiles.GetLength(0) - (width + 1)));
        }
        else
        {
            startCoord.y = Random.Range(width, (toPopulate.Tiles.GetLength(1) - (width + 1)));
        }

        Vector2Int mod = axis;
        if (mod.x >0)
        {
            mod.x = -1;
        }
        else
        {
            mod.x = 1;
        }

        if (mod.y>0)
        {
            mod.y = -1;
        }
        else
        {
            mod.y = 1;
        }
        Vector2Int curCoords = startCoord;
        Debug.Log("Corridor: start coords " + startCoord + "," + size + "," + axis + "," + mod+","+ toPopulate.Tiles.GetLength(0)+","+ toPopulate.Tiles.GetLength(1));
        for(int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < width; y++)
            {
               
                toPopulate.SetTileAsCorridor(curCoords);
                curCoords.y += mod.y;
            }
            curCoords.y = startCoord.y;
            curCoords.x += mod.x;
        }

        curCoords.x = startCoord.x + (mod.x * (size.x/2));
        curCoords.y = startCoord.y;
        Debug.Log("Corridor: second pass " + curCoords+ "," + size + "," + axis + "," + mod + "," + toPopulate.Tiles.GetLength(0) + "," + toPopulate.Tiles.GetLength(1));

        for (int y = 0; y < size.y; y++)
        {
            for(int x=0; x < width; x++)
            {
                toPopulate.SetTileAsCorridor(curCoords);
                curCoords.x += mod.x*-1;
            }
            curCoords.x = startCoord.x + (mod.x  * (size.x / 2));
            curCoords.y += mod.y;
        }
    }
}
