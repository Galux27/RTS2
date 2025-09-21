using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Feature", menuName = "Map Features/Platau", order = 1)]
public class Platau_MapGeneratorFeature : MapFeatureBase
{
    public int Iterations = 6;
    public int MaxDistForPlatau = 5;

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
        if (!PointsToUseNextIteration.Contains(point) && !PointsUsed.Contains(point))
        {
            PointsToUseNextIteration.Add(point);
        }
    }

    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        OnStartGenerate();
        Vector2Int startCoords = toGenerateIn.coords + new Vector2Int(Random.Range(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkBatchSize - WorldChunkManager.ChunkSize), Random.Range(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkBatchSize - WorldChunkManager.ChunkSize));
        Vector2Int originalStart = startCoords;
        Dictionary<Vector2Int, PotentialPondTile> tiles = new Dictionary<Vector2Int, PotentialPondTile>();
        WorldTile toAdd = null;
        float elevationAdjust = Random.Range(MinHeight, MaxHeight) ;
        for (int i = 0; i < Iterations; i++)
        {

            Vector2Int curCoords = new Vector2Int();
            float dist = 0f;
            for (int x = startCoords.x - MaxDistForPlatau; x < startCoords.x + MaxDistForPlatau; x++)
            {
                curCoords.x = x;
                for (int y = startCoords.y - MaxDistForPlatau; y < startCoords.y + MaxDistForPlatau; y++)
                {
                    curCoords.y = y;
                    dist = Vector2Int.Distance(curCoords, startCoords);
                    if (dist < MaxDistForPlatau)
                    {
                        toAdd = toGenerateIn.GetWorldTileFromVec2Int(curCoords);
                        if (toAdd != null)
                        {
                            if (toAdd.CanPutDecorationsOn == false)
                            {
                                return;
                            }
                            if (!tiles.ContainsKey(curCoords))
                            {
                                toAdd.SetElevation(toAdd.Elevation.GetElevation() + elevationAdjust);
                                tiles.Add(curCoords, new PotentialPondTile(toAdd, MaxDistForPlatau - dist));
                            }
                           
                        }
                    }
                }
            }

            //startCoords = originalStart + new Vector2Int(Random.Range(-MaxDistForPlatau, MaxDistForPlatau), Random.Range(-MaxDistForPlatau, MaxDistForPlatau));
            //Vector2Int chunkCoords = new Vector2Int();
            //EnvironmentObjectInstance objectToClear = null;
            //foreach (KeyValuePair<Vector2Int, PotentialPondTile> kvp in tiles)
            //{
            //    kvp.Value.tile.Elevation.SetElevation(kvp.Value.tile.Elevation.GetElevation() + elevationAdjust);              
            //}

        }

    }

    public List<Vector2Int> RemoveRandomElements(List<Vector2Int> toSampleFrom)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        float r = Random.Range(0f, 100f);
        for (int x = 0; x < toSampleFrom.Count; x++)
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
