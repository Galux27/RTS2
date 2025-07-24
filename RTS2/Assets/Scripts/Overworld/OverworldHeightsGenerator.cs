using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
[CreateAssetMenu(fileName = "Overworld Height Type", menuName = "Overworld/Heights Generator", order = 1)]
public class OverworldHeightsGenerator : OverworldFeatureGenerator
{
    public int NumberOfHeightNodes, MinPaths, MaxPaths,MaxPathLength;
    public int NumberOfFaultLines;
    public override void GenerateFeature(ref OverworldTile[,] world)
    {
        int width = world.GetLength(0);
        int height = world.GetLength(1);
        //int x = 0, y = 0;
        //HeightChunk[] chunks = new HeightChunk[NumberOfHeightNodes];
        //for (int i = 0; i < NumberOfHeightNodes; i++)
        //{
        //    x = Random.Range(0, width - 1);
        //    y = Random.Range(0, height - 1);
        //    chunks[i] = new HeightChunk(x, y, RandomHeight());
        //    GenerateHeightmaps(ref world,chunks[i]);
        //}
        Vector2Int coords = new Vector2Int();
        FaultLine[] faultLines = new FaultLine[NumberOfFaultLines];
        for(int i=0;i<NumberOfFaultLines; i++)
        {
            coords.x = Random.Range(0, width - 1);
            coords.y = Random.Range(0, height - 1);
            faultLines[i] = new FaultLine(coords);
        }
        for (int i = 0; i < NumberOfFaultLines; i++)
        {
            GenerateFaultLinePoints(ref faultLines[i], world);
        }

        for (int i = 0; i < NumberOfFaultLines; i++)
        {
            ExpandFaultLine(faultLines[i], world);
        }
    }


    void GenerateFaultLinePoints(ref FaultLine toGenerate, OverworldTile[,] world)
    {
        int count = Random.Range(MinPaths, MaxPaths);
        int width = world.GetLength(0);
        int height = world.GetLength(1);

        Vector2Int currentCoords = toGenerate.StartCoords;
        toGenerate.AddCoords(currentCoords);
        int xAxis = 1;
        int yAxis = 1;
        float r = Random.Range(0, 100f);
        if (r < 50)
        {
            xAxis *= -1;

        }
        r = Random.Range(0, 100f); 
        if (r < 50)
        {
            yAxis *= -1;

        }
        toGenerate.Axis.x = xAxis;
        toGenerate.Axis.y = yAxis;
        while (
                validCoords(currentCoords, width, height) 
                )
            {
            if(world[currentCoords.x, currentCoords.y].Elevation > OverworldGenerator.Instance.SeaLevel)
            {
                world[currentCoords.x, currentCoords.y].SetElevation(OverworldGenerator.Instance.MaxElevation);
            }

            currentCoords.x += Random.Range(0, 2)*xAxis;
                currentCoords.y += Random.Range(0, 2)*yAxis;
                toGenerate.AddCoords(currentCoords);
        }

    }

    void ExpandFaultLine(FaultLine toGenerate, OverworldTile[,] world)
    {
        Vector2 axis = Vector2.Perpendicular( toGenerate.Axis);
        Vector2Int ConvertedAxis = new Vector2Int(Mathf.RoundToInt(axis.x),Mathf.RoundToInt(axis.y));
        Debug.Log("axis was " + ConvertedAxis);
        int width = world.GetLength(0);
        int height = world.GetLength(1);
        Vector2Int currentCoords=new Vector2Int();
        for(int q = 0; q < toGenerate.Coords.Count; q++)
        {
            currentCoords = toGenerate.Coords[q];
            int count = 0;
            while (count < MaxPathLength &&
                validCoords(currentCoords, width, height)
                && world[currentCoords.x, currentCoords.y].Elevation > OverworldGenerator.Instance.SeaLevel
               )
            {
                world[currentCoords.x, currentCoords.y].SetElevation(
     Mathf.Max(world[currentCoords.x, currentCoords.y].Elevation,
   Mathf.Lerp(OverworldGenerator.Instance.MaxElevation, OverworldGenerator.Instance.SeaLevel + 1, Mathf.InverseLerp(0, MaxPathLength, count))));
                currentCoords.x += ConvertedAxis.x * Random.Range(0, 2);
                currentCoords.y += ConvertedAxis.y * Random.Range(0, 2);

                count++;
            }
        }
        ConvertedAxis *= -1;
        for (int q = 0; q < toGenerate.Coords.Count; q++)
        {
            currentCoords = toGenerate.Coords[q];
            int count = 0;
            while (count < MaxPathLength &&
               validCoords(currentCoords, width, height)
               && world[currentCoords.x, currentCoords.y].Elevation > OverworldGenerator.Instance.SeaLevel
              )
            {
                world[currentCoords.x, currentCoords.y].SetElevation(
                    Mathf.Max(world[currentCoords.x, currentCoords.y].Elevation,
                  Mathf.Lerp(OverworldGenerator.Instance.MaxElevation, OverworldGenerator.Instance.SeaLevel + 1, Mathf.InverseLerp(0, MaxPathLength, count))));
                currentCoords.x += ConvertedAxis.x * Random.Range(0, 2);
                currentCoords.y += ConvertedAxis.y * Random.Range(0, 2);

                count++;
            }
        }
    }

    float RandomHeight()
    {
        float f = Random.Range(0f, 100f);
        if (f < 25f)
        {
            return Random.Range(OverworldGenerator.Instance.SeaLevel + 1, OverworldGenerator.Instance.SeaLevel + 10f);
        }else if (f < 85f)
        {
            return Random.Range(OverworldGenerator.Instance.SeaLevel + 10, OverworldGenerator.Instance.SeaLevel + 30f);

        }
        else
        {
            return Random.Range(OverworldGenerator.Instance.SeaLevel + 30, OverworldGenerator.Instance.MaxElevation);

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
            while (nodesHit < MaxPathLength && 
                validCoords(currentCoords,width,height)&&
                tileHeight>OverworldGenerator.Instance.SeaLevel && 
                world[currentCoords.x, currentCoords.y].Elevation > OverworldGenerator.Instance.SeaLevel)
            {
                tileHeight = toRender.Height - Vector2.Distance(currentCoords, toRender.Center);
                world[currentCoords.x, currentCoords.y].SetElevation(Mathf.Lerp( tileHeight, world[currentCoords.x, currentCoords.y].Elevation,.5f));

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

public struct FaultLine
{
    public Vector2Int StartCoords;
    public List<Vector2Int> Coords;
    public Vector2Int Axis;
    public FaultLine(Vector2Int start)
    {
        StartCoords = start;
        Coords = new List<Vector2Int>();
        Axis = new Vector2Int();
    }
   public void AddCoords(Vector2Int coords)
    {
        Coords.Add(coords);
    }


}
