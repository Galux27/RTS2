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
        if(tile.UnitsInTile.ContainsKey(FactionController.ZOMBIE_FACTION))
        {
            
            for (int x = 0; x < tile.UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities.Count; x++)
            {

                if (tile.UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities[x].isActive == false 
                    && tile.UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities[x].isDead == false )
                {
                    while (enemy == null && failCount < 50)
                    {
                        enemy = SpawnZombie(batch);
                        if (enemy != null)
                        {
                            tile.UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities[x].SetActive(true);
                            tile.UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities[x].SetID(enemy.GetComponent<Unit>().GetMyUID().Value);
                        }
                        failCount++;
                    }
                    enemy = null;
                    failCount = 0;
                }
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
