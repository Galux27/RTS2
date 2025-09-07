using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[CreateAssetMenu(fileName = "Overworld Height Type", menuName = "Overworld/Heights Generator", order = 1)]
public class OverworldHeightsGenerator : OverworldFeatureGenerator
{
    public int NumberOfMountains;
    public int HeightSections,SmoothingIterations,smoothingBrushSize;
    public int NumberOfLakes,LakeSize,MountainSize;
    public override void GenerateFeature(OverworldTile[,] world)
    {
        int width = world.GetLength(0);
        int height1 = world.GetLength(1);
        float height = 0f;
        Vector2Int coords = new Vector2Int();
        List<Blob> blobs = BlobGenerator.GenerateBlobsWithFalloff(HeightSections, ref world, 300);
        for (int q = 0; q < blobs.Count; q++)
        {
            height = Random.Range(OverworldGenerator.Instance.SeaLevel + 3, OverworldGenerator.Instance.MaxElevation * Mathf.PerlinNoise(coords.x, coords.y));
            coords = new Vector2Int();
            for (int i = 0; i < blobs[q].PointsInBlob.Count; i++)
            {
                coords = blobs[q].PointsInBlob[i];
                if (world[coords.x, coords.y].Elevation > OverworldGenerator.Instance.SeaLevel)
                {
                    world[coords.x, coords.y].SetElevation(height);
                }
            }
        }

        List<Blob> lakeBlobs = BlobGenerator.GenerateBlobsWithFalloff(NumberOfLakes, ref world, LakeSize);

        for (int q = 0; q < lakeBlobs.Count; q++)
        {
            float maxDistFromCenter = 0;
            float curDist = 0;
            for (int x = 0; x < lakeBlobs[q].PointsInBlob.Count; x++)
            {
                coords = lakeBlobs[q].PointsInBlob[x];
                curDist = Vector2Int.Distance(coords, lakeBlobs[q].GetCenter());
                if (curDist > maxDistFromCenter)
                {
                    maxDistFromCenter = curDist;
                }

            }

            float tileHeight = 0f;
            for (int x = 0; x < lakeBlobs[q].PointsInBlob.Count; x++)
            {
                coords = lakeBlobs[q].PointsInBlob[x];
                curDist = Vector2Int.Distance(coords, lakeBlobs[q].GetCenter());
                tileHeight = Mathf.Lerp(0, OverworldGenerator.Instance.SeaLevel, Mathf.InverseLerp(0f, maxDistFromCenter, curDist));
                world[coords.x, coords.y].AddFeatureToTile(OverworldFeature.LargeWaterBody);
                world[coords.x, coords.y].SetElevation(tileHeight);
            }
        }


        List<Blob> mountainBlobs = BlobGenerator.GenerateBlobsWithFalloff(NumberOfMountains, ref world, MountainSize);

        for (int q = 0; q < mountainBlobs.Count; q++)
        {
            float maxDistFromCenter = 0;
            float curDist = 0;
            for (int x = 0; x < mountainBlobs[q].PointsInBlob.Count; x++)
            {
                coords = mountainBlobs[q].PointsInBlob[x];
                curDist = Vector2Int.Distance(coords, mountainBlobs[q].GetCenter());
                if (curDist > maxDistFromCenter)
                {
                    maxDistFromCenter = curDist;
                }

            }
         
            float tileHeight = 0f;
            for (int x = 0; x < mountainBlobs[q].PointsInBlob.Count; x++)
            {
                coords = mountainBlobs[q].PointsInBlob[x];
                curDist = Vector2Int.Distance(coords, mountainBlobs[q].GetCenter());
                tileHeight = Mathf.Lerp(OverworldGenerator.Instance.MaxElevation, world[coords.x, coords.y].Elevation, Mathf.InverseLerp(0f, maxDistFromCenter, curDist));
                world[coords.x, coords.y].AddFeatureToTile(OverworldFeature.Mountain);
                world[coords.x, coords.y].SetElevation(tileHeight);
            }
        }


        float toSet = 0;
        for (int i = 0; i < SmoothingIterations; i++)
        {
            for (int x = 0 + smoothingBrushSize; x < width - smoothingBrushSize; x += smoothingBrushSize)
            {
                for (int y = 0 + smoothingBrushSize; y < height1 - smoothingBrushSize; y += smoothingBrushSize)
                {
                    height = 0;
                    int count = 0;
                    coords = new Vector2Int();
                    for (int x1 = x - smoothingBrushSize; x1 <= x + smoothingBrushSize; x1++)
                    {
                        for (int y1 = y - smoothingBrushSize; y1 <= y + smoothingBrushSize; y1++)
                        {
                            coords.x = x1;
                            coords.y = y1;
                            
                                height += world[coords.x, coords.y].Elevation;
                                count++;
                            
                        }
                    }
                    if (count > 0)
                    {
                        height /= count;
                    }
                    else
                    {
                        height = 0;
                    }
                        for (int x1 = x - smoothingBrushSize; x1 <= x + smoothingBrushSize; x1++)
                    {
                        for (int y1 = y - smoothingBrushSize; y1 <= y + smoothingBrushSize; y1++)
                        {
                            coords.x = x1;
                            coords.y = y1;
                            toSet = Mathf.Lerp(height, world[coords.x, coords.y].Elevation, .5f);
                            
                                world[coords.x, coords.y].SetElevation(toSet);
                            
                        }
                    }

                }
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
