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
        int toSpawn = 0;
        OverworldTile tile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords);
        GameObject enemy = null;
        int failCount = 0;
        for (int x = 0; x < tile.EntitiesInTile.Count; x++)
        {

            if (tile.EntitiesInTile[x].isActive == false && tile.EntitiesInTile[x].isDead==false && tile.EntitiesInTile[x].EntityType==ALifeEntityType.Zombie)
            {
                while (enemy == null&&failCount<50)
                {
                    enemy = SpawnZombie(batch);
                    if (enemy != null)
                    {
                        tile.EntitiesInTile[x].SetActive(true);
                        tile.EntitiesInTile[x].SetID(enemy.GetComponent<Unit>().GetMyUID().Value);
                    }
                    failCount++;
                }
                enemy = null;
                failCount = 0;
            }
        }    
    }

    public void OnWorldChunkBatchUnloaded(WorldChunkBatch batch)
    {

    }

    GameObject SpawnZombie(WorldChunkBatch batch)
    {
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch.coords) == false)
        {
            return null;
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
            return g;
        }
        return null;
    }
}
