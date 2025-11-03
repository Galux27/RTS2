using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class ZombieSpawner:MonoBehaviour
{
    const float PopulationMultiplier = .1f;
    const int ZombiesPerMajorRoad = 25, ZombiesPerMinorRoad = 10, ZombiesPerBackroad = 5;
    public GameObject ZombiePrefab;
    public void OnWorldChunkBatchGenerated(WorldChunkBatch batch)
    {
        OverworldTile tile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords);
        int MajorRoads = tile.GetQuantitiyOfFeature(OverworldFeature.MajorRoad);
        int MinorRoads = tile.GetQuantitiyOfFeature(OverworldFeature.MinorRoad);
        int BackRoads = tile.GetQuantitiyOfFeature(OverworldFeature.Backroad);
        int pop = tile.Population / 10;
        int toSpawn = Random.Range(0,20);
        toSpawn += pop;
        toSpawn += MajorRoads * ZombiesPerMajorRoad;
        toSpawn += MinorRoads * ZombiesPerMinorRoad;
        toSpawn += BackRoads * ZombiesPerBackroad;
        int MajorWater = tile.GetQuantitiyOfFeature(OverworldFeature.LargeWaterBody);
        if (MajorWater > 0)
        {
            toSpawn = 0;
        }
        Debug.Log("Spawner: spawning zombies " + batch.coords + " quantity " + toSpawn);
        for(int x = 0; x < toSpawn; x++)
        {
            SpawnZombie(batch);
        }
    }

    void SpawnZombie(WorldChunkBatch batch)
    {
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch.coords) == false)
        {
            return;
        }

        Vector2Int chunk = new Vector2Int(Random.Range(1, WorldChunkManager.ChunkSize-1), Random.Range(1, WorldChunkManager.ChunkSize-1));
        Vector2Int tile = new Vector2Int(Random.Range(0, WorldChunkManager.ChunkSize), Random.Range(0, WorldChunkManager.ChunkSize));

        int xCoord = 0;
        int yCoord = 0;

        WorldTile toSpawnOn = WorldChunkManager.Instance.ChunkBatches[batch.coords].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y];
        if (toSpawnOn.TileTraversable())
        {
            Vector3 worldPos = new Vector3(toSpawnOn.Coords().x, toSpawnOn.Coords().y);
            UnitTypeSO zombie = UnitTypesController.Instance.Units["Zombie"];
            GameObject g = Instantiate(zombie.Prefab, worldPos, Quaternion.identity);
        }
    }
}
