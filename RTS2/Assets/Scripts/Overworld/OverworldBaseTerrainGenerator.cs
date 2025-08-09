using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Overworld Generator Type", menuName = "Overworld/Base Terrain Generator", order = 1)]
public class OverworldBaseTerrainGenerator : OverworldFeatureGenerator
{
    public int NumberOfNodesToStartWith, NumberOfNodesToExpandFrom;


    public override void GenerateFeature(OverworldTile[,] world)
    {
        tileHeight =  OverworldGenerator.Instance.SeaLevel + 1; 
        int width = world.GetLength(0);
        int height = world.GetLength(1);    
        int x = width/2, y = height/2;

        for(int i = 0; i < width; i++)
        {
            for(int q = 0; q < height; q++)
            {
                world[i, q].SetElevation(tileHeight);
            }
        }

        //GenerateTerrain( x, y, world,width,height);
        //for(int i=0; i<NumberOfNodesToStartWith-1; i++)
        //{
        //    x = Random.Range(0, width - 1);
        //    y = Random.Range(0, height - 1);
        //    GenerateTerrain(x, y, world, width, height);
        //}

       
    }

    Vector2Int currentCoords = new Vector2Int();
    float tileHeight;
    void GenerateTerrain(int x,int y, OverworldTile[,] world,int width,int height)
    {
        
        
        world[x, y].SetElevation(tileHeight);
        currentCoords.x = x; 
        currentCoords.y = y;

        for(int i=0;i< NumberOfNodesToExpandFrom; i++)
        {
            currentCoords.x += Random.Range(-1, 2);
            currentCoords.y += Random.Range(-1, 2);

            if (!validCoords(currentCoords,width,height))
            {
                return;
            }
            SetHeight(currentCoords, tileHeight, world);
        }
     
    }
 
    bool validCoords(Vector2Int coords,int width,int height)
    {
        if (coords.x < 0 || coords.y < 0 || coords.y >= height || coords.x >= width)
        {
            return false;
        }
        return true;
    }
    void SetHeight(Vector2Int coords, float height,OverworldTile[,] world)
    {
        world[coords.x, coords.y].SetElevation(height);
    }

   
}

public static class FalloffGenerator
{

    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 4f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
