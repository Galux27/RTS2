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

    public void OnALifeEntityEntersLoadedChunk(ALifeEntity entity,WorldChunkBatch batch)
    {
        if (entity.isDead || entity.isActive)
        {
            return;
        }
        entity.isActive = true;
        int xCoord = -1, yCoord = -1;
        if (entity.PreviousCoords.x > entity.CurrentCoords.x)
        {
            xCoord = WorldChunkManager.ChunksPerBatch - 2;
        }else if (entity.PreviousCoords.x < entity.CurrentCoords.x)
        {
            xCoord = 1;
        }

        if (entity.PreviousCoords.y > entity.CurrentCoords.y)
        {
            yCoord = WorldChunkManager.ChunksPerBatch - 2;
        }
        else if (entity.PreviousCoords.y < entity.CurrentCoords.y)
        {
            yCoord = 1;
        }

        SpawnZombie(batch, xCoord, yCoord); 
    }

    public void OnWorldChunkBatchUnloaded(WorldChunkBatch batch)
    {
        
    }

    GameObject SpawnZombie(WorldChunkBatch batch,int forceChunkX=-1,int forceChunkY=-1)
    {
        return EntitySpawner.SpawnEntity(batch,"Zombie",FactionController.ZOMBIE_FACTION,forceChunkX,forceChunkY);
       
    }

    GameObject SpawnZombie(WorldChunkBatch batch)
    {
        return EntitySpawner.SpawnEntity(batch, "Zombie",FactionController.ZOMBIE_FACTION);
    }
}
