using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLifeManager : MonoBehaviour
{
    static GameLifeManager instance;
    public static GameLifeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameLifeManager>();
                instance.InitGameLifeManager();
            }
            return instance;
        }
        
    }
    public ZombieSpawner ZombieSpawner;

    void InitGameLifeManager()
    {
        ZombieSpawner=this.GetComponent<ZombieSpawner>();
    }


    public void OnChunkBatchGenerated(WorldChunkBatch toGenerate)
    {
        SpawnUnitsForGeneratedChunkBatch(toGenerate);
        //ZombieSpawner.OnWorldChunkBatchGenerated(toGenerate);
    }

    public void OnChunkBatchUnloaded(WorldChunkBatch unloading)
    {
        for (int x=0;x< unloading.Chunks.GetLength(0);x++)
        {
            for (int y = 0; y < unloading.Chunks.GetLength(1); y++)
            {
                for(int i =0;i< unloading.Chunks[x, y].UnitsInChunk.Count; i++)
                {
                    ConvertUnitToALifeEntity(unloading.Chunks[x, y].UnitsInChunk[i]);
                }
            }
        }
    }
    public bool SpawnedInitialUserUnits = false;
    public void OnNewGameStarted()
    {
        //spawn 5 engineers
        Vector2Int startChunk = OverworldGenerator.Instance.GetOverworldStartingCoords();

        for (int x = 0; x < 5; x++)
        {
            OverworldGenerator.Instance.OverworldTiles[startChunk.x, startChunk.y].AddALifeEntity(new ALifeEntity(startChunk,
           FactionController.USER_FACTION, "Engineer"));
        }
        SpawnedInitialUserUnits = true;
    }

    public void SpawnUnitFromALifeEntity(ALifeEntity entity, WorldChunkBatch batch)
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
        }
        else if (entity.PreviousCoords.x < entity.CurrentCoords.x)
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
        EntitySpawner.SpawnEntity(batch, entity.UnitType, entity.Faction, xCoord, yCoord);
    }


    void SpawnUnitsForGeneratedChunkBatch(WorldChunkBatch batch)
    {
        OverworldTile tile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords);
        GameObject enemy = null;
        int failCount = 0;

        foreach(KeyValuePair<string,ALifeFactionGroup> kvp in tile.UnitsInTile) 
        {

            for (int x = 0; x < tile.UnitsInTile[kvp.Key].FactionEntities.Count; x++)
            {

                if (tile.UnitsInTile[kvp.Key].FactionEntities[x].isActive == false
                    && tile.UnitsInTile[kvp.Key].FactionEntities[x].isDead == false)
                {
                    while (enemy == null && failCount < 50)
                    {
                        enemy = EntitySpawner.SpawnEntity(batch, tile.UnitsInTile[kvp.Key].FactionEntities[x].UnitType, tile.UnitsInTile[kvp.Key].FactionEntities[x].Faction);
                        if (enemy != null)
                        {
                            tile.UnitsInTile[kvp.Key].FactionEntities[x].SetActive(true);
                            tile.UnitsInTile[kvp.Key].FactionEntities[x].SetID(enemy.GetComponent<Unit>().GetMyUID().Value);
                        }
                        failCount++;
                    }
                    enemy = null;
                    failCount = 0;
                }
            }

        }
    }


    void ConvertUnitToALifeEntity(Unit u)
    {

    }

}
