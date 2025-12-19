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
        CachedUnitData data=  UnitTypesController.Instance.UnitData["Engineer"];
        ALifeEntity ae = null;
        for (int x = 0; x < 5; x++)
        {
            ae = new ALifeEntity(startChunk,
           FactionController.USER_FACTION, "Engineer",
           new Vector2Int(Random.Range(0, WorldChunkManager.ChunkSize), Random.Range(0, WorldChunkManager.ChunkSize)), 1, 1, 1);
            ae.SetUnitDetails(data);
            OverworldGenerator.Instance.OverworldTiles[startChunk.x, startChunk.y].AddALifeEntity(ae,false);
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
        if (entity.PreviousBatchCoords.x > entity.CurrentBatchCoords.x)
        {
            xCoord = WorldChunkManager.ChunksPerBatch - 2;
        }
        else if (entity.PreviousBatchCoords.x < entity.CurrentBatchCoords.x)
        {
            xCoord = 1;
        }

        if (entity.PreviousBatchCoords.y > entity.CurrentBatchCoords.y)
        {
            yCoord = WorldChunkManager.ChunksPerBatch - 2;
        }
        else if (entity.PreviousBatchCoords.y < entity.CurrentBatchCoords.y)
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
        Vector2Int tileCoords = new Vector2Int(),chunkCoords= new Vector2Int();
        foreach(KeyValuePair<string,ALifeFactionGroup> kvp in tile.UnitsInTile) 
        {
            for (int x = 0; x < tile.UnitsInTile[kvp.Key].FactionEntities.Count; x++)
            {
                ConvertALifeEntityCoordsToChunkAndTile(kvp.Value.FactionEntities[x].LocalCoords,out chunkCoords,out tileCoords);
                if (tile.UnitsInTile[kvp.Key].FactionEntities[x].isActive == false
                    && tile.UnitsInTile[kvp.Key].FactionEntities[x].isDead == false)
                {
                    while (enemy == null && failCount < 50)
                    {
                        enemy = EntitySpawner.SpawnEntity(batch, 
                            tile.UnitsInTile[kvp.Key].FactionEntities[x].UnitType, tile.UnitsInTile[kvp.Key].FactionEntities[x].Faction
                            ,chunkCoords.x,chunkCoords.y,tileCoords.x,tileCoords.y);
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
    void ConvertALifeEntityCoordsToChunkAndTile(Vector2Int input,out Vector2Int chunk, out Vector2Int tile)
    {
        tile = new Vector2Int(input.x % WorldChunkManager.ChunkSize, input.y % WorldChunkManager.ChunkSize);
        chunk = new Vector2Int(Mathf.FloorToInt(input.x / WorldChunkManager.ChunkSize), Mathf.FloorToInt(input.y / WorldChunkManager.ChunkSize));
        
    }


    void ConvertUnitToALifeEntity(Unit u)
    {

    }

}
