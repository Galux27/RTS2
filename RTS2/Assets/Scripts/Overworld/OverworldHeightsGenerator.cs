using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Overworld Height Type", menuName = "Overworld/Heights Generator", order = 1)]
public class OverworldHeightsGenerator : OverworldFeatureGenerator
{
    public int NumberOfHeightNodes, MinPaths, MaxPaths,MaxPathLength;

    public override void GenerateFeature(ref OverworldTile[,] world)
    {
        int width = world.GetLength(0);
        int height = world.GetLength(1);
        int x = 0, y = 0;
        HeightChunk[] chunks = new HeightChunk[NumberOfHeightNodes];
        for (int i = 0; i < NumberOfHeightNodes; i++)
        {
            x = Random.Range(0, width - 1);
            y = Random.Range(0, height - 1);
            chunks[i] = new HeightChunk(x, y, Random.Range(OverworldGenerator.Instance.SeaLevel+1,OverworldGenerator.Instance.MaxElevation));
            GenerateHeightmaps(ref world,chunks[i]);
        }
    }

    void GenerateHeightmaps(ref OverworldTile[,] world,HeightChunk toRender)
    {
        int count = Random.Range(MinPaths, MaxPaths);
        int width = world.GetLength(0);
        int height = world.GetLength(1);
        float tileHeight = toRender.Height;
        for(int x = 0; x < count; x++)
        {
            Vector2Int currentCoords = toRender.Center;
            int nodesHit = 0;
            while (nodesHit < MaxPathLength && validCoords(currentCoords,width,height)&&tileHeight>OverworldGenerator.Instance.SeaLevel)
            {
                tileHeight = toRender.Height - Vector2.Distance(currentCoords, toRender.Center);
                world[currentCoords.x, currentCoords.y].SetElevation(Mathf.Max( tileHeight, world[currentCoords.x, currentCoords.y].Elevation));

                nodesHit++;
                currentCoords.x += Random.Range(-1, 2);
                currentCoords.y += Random.Range(-1, 2);
            }
        }
    }

    bool validCoords(Vector2Int coords, int width, int height)
    {
        if (coords.x < 0 || coords.y < 0 || coords.y >= height || coords.x >= width)
        {
            return false;
        }
        return true;
    }
}

public struct HeightChunk
{
    public float Height;
    public Vector2Int Center;
    
    public HeightChunk(int x,int y,float height)
    {
        Center=new Vector2Int(x,y);
        Height = height;
        Debug.Log("Created height chunk " + x + "," + y + " height " + height);
    }
}
