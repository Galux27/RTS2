using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Map Feature", menuName = "Map Features/Pond", order = 1)]
public class Pond_MapGenerator : MapFeatureBase
{
    public int Iterations = 6;
    public int MaxDistForWater = 5;
    
    List<Vector2Int> GetNeighboursOfPoint(Vector2Int vector2Int)
    {
        List<Vector2Int> retVal = new List<Vector2Int>
        {
            vector2Int + new Vector2Int(-1, 0),
            vector2Int + new Vector2Int(1, 0),
            vector2Int + new Vector2Int(0, 1),
            vector2Int + new Vector2Int(0, -1)
        };
        return retVal;
    }

    List<Vector2Int> PointsUsed = new List<Vector2Int>();
    List<Vector2Int> PointsToUseNextIteration = new List<Vector2Int>();


    public override void OnStartGenerate()
    {
        PointsUsed.Clear();
        PointsToUseNextIteration.Clear();
    }


    public void AddPointToNextIteration(Vector2Int point)
    {
        if(!PointsToUseNextIteration.Contains(point) && !PointsUsed.Contains(point))
        {
            PointsToUseNextIteration.Add(point);
        }
    }

    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        OnStartGenerate();
        Vector2Int startCoords = toGenerateIn.coords + new Vector2Int(Random.Range(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkBatchSize - WorldChunkManager.ChunkSize), Random.Range(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkBatchSize - WorldChunkManager.ChunkSize));
        float dist = 0f;
        Vector2Int curCoords = new Vector2Int();
        WorldTile tileChecking = null;
        Vector2Int chunkCoords = new Vector2Int(0, 0);
        EnvironmentObjectInstance objectToClear = null;

        for (int q = 0; q < Iterations; q++)
        {
            for (int x = startCoords.x - MaxDistForWater; x < startCoords.x + MaxDistForWater; x++)
            {
                for (int y = startCoords.y - MaxDistForWater; y < startCoords.y + MaxDistForWater; y++)
                {
                    curCoords.x = x;
                    curCoords.y = y;
                    dist = Vector2Int.Distance(curCoords, startCoords);
                    if (dist <= MaxDistForWater)
                    {
                        tileChecking = WorldTileHelpers.GetTileFromCoords(x, y);
                        if (tileChecking != null)
                        {
                            chunkCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(curCoords);
                            if (toGenerateIn.Chunks[chunkCoords.x, chunkCoords.y].DoesAnyObjectExistAtCoords(curCoords, out objectToClear))
                            {
                                objectToClear.AdjustHealth(-99999999f);
                            }
                            tileChecking.tileType = "Mud";
                            tileChecking.UpdateWaterLevel(MaxDistForWater - dist);
                            PointsUsed.Add(curCoords);
                        }
                    }
                }
            }
            startCoords = PointsUsed[Random.Range(0, PointsUsed.Count)];

        }

        //for (int r = 0; r < Iterations; r++)
        {
            Vector2Int center = startCoords + new Vector2Int(Random.Range(MinWidth, MaxWidth), Random.Range(MinHeight, MaxHeight));
            List<Vector2Int> pointsToCheck = new List<Vector2Int>();
            pointsToCheck.Add(center);
            List<Vector2Int> neighbours = null;


            for (int x = 0; x < Iterations; x++)
            {
                for (int q = 0; q < pointsToCheck.Count; q++)
                {
                    tileChecking = WorldTileHelpers.GetTileFromCoords(pointsToCheck[q].x, pointsToCheck[q].y);
                    if (tileChecking != null)
                    {
                        chunkCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(pointsToCheck[q]);
                        if (toGenerateIn.Chunks[chunkCoords.x, chunkCoords.y].DoesAnyObjectExistAtCoords(pointsToCheck[q], out objectToClear))
                        {
                            objectToClear.AdjustHealth(-99999999f);
                        }

                        tileChecking.tileType = "Mud";
                        tileChecking.WaterData.UpdateWaterLevel(2f - (2f / (Iterations / (x + 1))));
                    }
                    neighbours = GetNeighboursOfPoint(pointsToCheck[q]);
                    PointsUsed.Add(pointsToCheck[q]);
                    for (int y = 0; y < neighbours.Count; y++)
                    {
                        AddPointToNextIteration(neighbours[y]);
                    }
                }
                pointsToCheck = PointsToUseNextIteration;
                PointsToUseNextIteration = new List<Vector2Int>();
            }
        }

    }
    
    public List<Vector2Int> RemoveRandomElements(List<Vector2Int> toSampleFrom)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        float r = Random.Range(0f, 100f);
        for(int x = 0; x < toSampleFrom.Count; x++)
        {
            if (r > 50f)
            {
                retVal.Add(toSampleFrom[x]);
            }
            r = Random.Range(0f, 100f);
        }

        return toSampleFrom;
    }


    public List<Vector2Int> ShuffleList(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Vector2Int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }

}
