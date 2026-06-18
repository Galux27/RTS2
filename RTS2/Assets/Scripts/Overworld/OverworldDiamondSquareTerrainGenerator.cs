using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
[CreateAssetMenu(fileName = "Diamond Generator Heights", menuName = "Overworld/Diamond Heights Generator", order = 1)]
public class OverworldDiamondSquareTerrainGenerator : OverworldFeatureGenerator
{
    public int argSize,seed;
    public float roughness;
    public float scale;
    public int Increments;
    public override void GenerateFeature(OverworldTile[,] world)
    {
        Debug.Log("Generating overworld diamond heightmap");
        size=new Vector2Int(world.GetLength(0),world.GetLength(1));
        argSize = size.x;
        float[,] data = SquareDiamond.GenerateHeightmap(argSize, scale, roughness, seed,Increments);

        //GenerateTerrain(world, argSize, roughness, seed);
        float low = 99999f, high = -999999f;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                world[x, y].Elevation = data[x, y] + OverworldGenerator.Instance.SeaLevel - 20;
                
                if (world[x,y].Elevation< low)
                {
                    low = world[x,y].Elevation;
                }
                if (world[x, y].Elevation > high)
                {
                    high = world[x, y].Elevation;
                }
            }
        }
        Debug.Log("Borders were " + low+","+ high);
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y< size.y; y++)
            {
                world[x, y].SetElevation(Mathf.Lerp(0,OverworldGenerator.Instance.MaxElevation, Mathf.InverseLerp(low,high, world[x, y].Elevation)));
            }
        }
        OverworldRenderer.Instance.RenderWorld();
    }

    float RandomHeight()
    {
        return Random.Range(-1f, 1f);
    }

    void GenerateTerrain(OverworldTile[,] world,int size, float roughness,int seed)
    {
        if (size % 2 != 1)
        {
            Debug.LogError("Size must be 2^n + 1");
            return;
        }

        //Random.InitState(seed);
        world[0,0].Elevation=RandomHeight();
        world[size-1, 0].Elevation = RandomHeight();
        world[0, size-1].Elevation = RandomHeight();
        world[size-1, size-1].Elevation = RandomHeight();

        int sideLength, halfSide, x, y;
        float range = .5f;
        // While the side length is greater than 1
        for (sideLength = size - 1; sideLength > 1; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            // Run Diamond Step
            for (x = 0; x < size - 1; x += sideLength)
            {
                for (y = 0; y < size - 1; y += sideLength)
                {
                    // Get the average of the corners
                    DiamondStep(x, y, sideLength, range, world);
                }
            }

            // Run Square Step
            for (x = 0; x < size - 1; x += halfSide)
            {
                for (y = (x + halfSide) % sideLength; y < size - 1; y += sideLength)
                {
                    SquareStep(x, y, sideLength,range, world);

                }
            }

            // Lower the random value range
            range -= range * 0.5f * roughness;
        }
    }



    float average = 0f;
    Vector2Int size;
    void DiamondStep(int x,int y,int step,float scale, OverworldTile[,] world)
    {
        
        int halfStep = step / 2;
        average = GetHeight(x , y , ref world);
        average += GetHeight(x +step,y,ref world);
        average += GetHeight(x, y + step, ref world);
        average += GetHeight(x + step, y + step, ref world);
        average /= 4f;
        average += RandomHeight() * scale;
        world[x + halfStep, y + halfStep].Elevation = average;//,ref  world);
    }

    void SquareStep(int x, int y, int step, float scale, OverworldTile[,] world)
    {

        int halfStep = step / 2;
        average = GetHeight((x - halfStep), y, ref world);
        average += GetHeight((x+halfStep), y , ref world);
        average += GetHeight(x, (y + halfStep), ref world);
        average += GetHeight(x, (y - halfStep ) , ref world);

        average /= 4f;
        average += RandomHeight() * scale;
        world[x, y].Elevation = average;//,ref  world);
        
    }


    float GetHeight(int x,int y,ref OverworldTile[,] world)
    {
        if (x >= size.x)
        {
            x = size.x - 1;
        }
        else if (x < 0)
        {
            x = 0;
        }

        if (y >= size.y)
        {
            y = size.y - 1;
        }
        else if (y < 0)
        {
            y = 0;
        }
        return world[x, y].Elevation;
    }

  
    void SetHeight(int x,int y, float height,ref OverworldTile[,] world)
    {
        world[x, y].Elevation = height;
    }


}

