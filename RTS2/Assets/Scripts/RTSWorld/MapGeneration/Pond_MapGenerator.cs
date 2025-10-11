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
        return;


        OnStartGenerate();
        Vector2Int startCoords = toGenerateIn.coords + new Vector2Int(Random.Range(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkBatchSize - WorldChunkManager.ChunkSize), Random.Range(WorldChunkManager.ChunkSize, WorldChunkManager.ChunkBatchSize - WorldChunkManager.ChunkSize));
        Vector2Int originalStart = startCoords;
        Dictionary<Vector2Int,PotentialPondTile> tiles=new Dictionary<Vector2Int, PotentialPondTile>();
        WorldTile toAdd = null;
       for(int i = 0; i < Iterations; i++)
        {
            Vector2Int curCoords = new Vector2Int();
            float dist = 0f;
            for (int x=startCoords.x-MaxDistForWater ;x<startCoords.x+MaxDistForWater; x++)
            {
                curCoords.x = x;
                for(int y= startCoords.y - MaxDistForWater; y < startCoords.y + MaxDistForWater; y++)
                {
                    curCoords.y= y;
                    dist = Vector2Int.Distance(curCoords, startCoords);
                    if (dist < MaxDistForWater)
                    {
                        toAdd = WorldTileHelpers.GetTileNearExisting(toAdd, toGenerateIn, curCoords);//toGenerateIn.GetWorldTileFromVec2Int(curCoords);
                        if (toAdd != null)
                        {
                            if (toAdd.CanPutDecorationsOn == false)
                            {
                                return;
                            }
                            if (!tiles.ContainsKey(curCoords))
                            {

                                tiles.Add(curCoords, new PotentialPondTile(toAdd, MaxDistForWater - dist));
                            }
                            else
                            {
                                tiles[curCoords].waterLevel += MaxDistForWater - dist;
                            }
                        }
                        }
                    }
            }

            startCoords = originalStart + new Vector2Int(Random.Range(-MaxDistForWater, MaxDistForWater), Random.Range(-MaxDistForWater, MaxDistForWater));
            EnvironmentObjectInstance objectToClear = null;
            foreach(KeyValuePair<Vector2Int,PotentialPondTile> kvp in tiles)
            {
                kvp.Value.tile.UpdateWaterLevel(kvp.Value.waterLevel,false);
                //chunkCoords = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(kvp.Key);
                if (toGenerateIn.Chunks[kvp.Value.tile.Chunk.x, kvp.Value.tile.Chunk.y].DoesAnyObjectExistAtCoords(kvp.Key, out objectToClear))
                {
                    objectToClear.AdjustHealth(-99999999f);
                }
            }

            List<Vector2Int> chunkCoordsUpdated = new List<Vector2Int>();
            foreach (KeyValuePair<Vector2Int, PotentialPondTile> kvp in tiles)
            {
               // kvp.Value.tile.UpdateWaterLevel(kvp.Value.waterLevel, false);
                if (!chunkCoordsUpdated.Contains(kvp.Value.tile.Chunk))
                {
                    for(int x = 0; x < WorldChunkManager.ChunkSize; x++)
                    {
                        for (int y = 0; y < WorldChunkManager.ChunkSize; y++)
                        {
                            toGenerateIn.Chunks[kvp.Value.tile.Chunk.x, kvp.Value.tile.Chunk.y].PathfindingNodes[x, y].UpdatePassable(toGenerateIn.Chunks[kvp.Value.tile.Chunk.x, kvp.Value.tile.Chunk.y].ChunkTiles[x, y].TileTraversable());
                        }
                    }
                    chunkCoordsUpdated.Add(kvp.Value.tile.Chunk);
                }
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

public class PotentialPondTile
{
    public WorldTile tile;
    public float waterLevel;

    public PotentialPondTile(WorldTile tile,float waterLevel)
    {
        this.tile = tile;
        this.waterLevel = waterLevel;
    }
}
