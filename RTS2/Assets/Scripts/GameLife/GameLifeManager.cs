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
        Debug.Log("Chunk Loading: generating units from ALife " + toGenerate.coords);
        SpawnUnitsFromALife(toGenerate);
        //ZombieSpawner.OnWorldChunkBatchGenerated(toGenerate);
    }

    public void OnChunkBatchUnloaded(WorldChunkBatch unloading,bool clearExistingUnits=true)
    {
        Debug.Log("Chunk Loading: Unloading chunk units at" + unloading.coords);
        for (int x=0;x< unloading.Chunks.GetLength(0);x++)
        {
            for (int y = 0; y < unloading.Chunks.GetLength(1); y++)
            {
                for(int q=0;q< unloading.Chunks[x, y].UnitsInChunk.Count; q++)
                {
                    ConvertUnitToALifeEntity(unloading.Chunks[x, y].UnitsInChunk[q], unloading,clearExistingUnits);
                }
                if (clearExistingUnits)
                {
                    unloading.Chunks[x, y].UnitsInChunk.Clear();
                }
                }
            }
        Debug.Log("Chunk Loading Units: units found in "+unloading.coords+","+unloading.OverworldCoords+"," + OverworldGenerator.Instance.OverworldTiles[unloading.OverworldCoords.x,unloading.OverworldCoords.y].ALifeChunk.UnitsInTile.Count);

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


    public void SpawnUnitsFromALife(WorldChunkBatch batch)
    {
        OverworldTile tile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords);
        GameObject enemy = null;
        int failCount = 0;
        Vector2Int tileCoords = new Vector2Int(),chunkCoords= new Vector2Int();
        Debug.Log("Chunk Loading Units: generating units from ALife " +batch.coords+","+batch.OverworldCoords+" "+tile.UnitsInTile.Count);
        Unit u = null;
        Dictionary<ALifeEntity, Unit> deserializedALifeEntites = new Dictionary<ALifeEntity, Unit>();
        foreach (KeyValuePair<string,ALifeFactionGroup> kvp in tile.UnitsInTile) 
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
                            u = enemy.GetComponent<Unit>();
                            tile.UnitsInTile[kvp.Key].FactionEntities[x].SetActive(true);
                            tile.UnitsInTile[kvp.Key].FactionEntities[x].SetID(enemy.GetComponent<Unit>().GetMyUID().Value);
                            deserializedALifeEntites.Add(tile.UnitsInTile[kvp.Key].FactionEntities[x], u);
                          //  tile.UnitsInTile[kvp.Key].FactionEntities[x].LoadBehaviourData(u);
                         //   tile.UnitsInTile[kvp.Key].FactionEntities[x].LoadOrderData(u);
                        }
                        failCount++;
                    }
                    enemy = null;
                    failCount = 0;
                }
            }

        }
        foreach(KeyValuePair<ALifeEntity,Unit> kvp in deserializedALifeEntites)
        {
            kvp.Key.LoadBehaviourData(kvp.Value);
            kvp.Key.LoadOrderData(kvp.Value);
        }
        deserializedALifeEntites.Clear();
        tile.UnitsInTile.Clear();
    }
    void ConvertALifeEntityCoordsToChunkAndTile(Vector2Int input,out Vector2Int chunk, out Vector2Int tile)
    {
        tile = new Vector2Int(input.x % WorldChunkManager.ChunkSize, input.y % WorldChunkManager.ChunkSize);
        chunk = new Vector2Int(Mathf.FloorToInt(input.x / WorldChunkManager.ChunkSize), Mathf.FloorToInt(input.y / WorldChunkManager.ChunkSize));
        
    }

    Vector2Int batch = new Vector2Int(), chunk = new Vector2Int(), local = new Vector2Int();
    public void ConvertUnitToALifeEntity(Unit u,WorldChunkBatch chunkImIn,bool destroyUnits=true)
    {
        if(u==null) return;
        CachedUnitData data = UnitTypesController.Instance.UnitData[u.MyType.ToString()];
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(u.transform.position.x,u.transform.position.y,out batch,out chunk,out local);
        Vector2Int aLifePos = new Vector2Int(local.x + (chunk.x * WorldChunkManager.ChunkSize), local.y + (chunk.y * WorldChunkManager.ChunkSize));

        ALifeEntity entity = new ALifeEntity(WorldChunkManager.Instance.ChunkBatches[batch].OverworldCoords, u.MyFaction.MyFactionID, u.MyType.ToString(), aLifePos, data.MoveSpeed, data.AttackRate, data.RangeMax);
        entity.SetUnitDetails(data);
        entity.UpdateDetailsFromActiveUnit(u);
        entity.SetID(u.MyUID().Value);
        entity.SetBehaviourDetails(u.BehaviourRunner.CurrentBehaviour);
        entity.SetOrdersData(u.MyOrders);
        OverworldGenerator.Instance.OverworldTiles[chunkImIn.OverworldCoords.x, chunkImIn.OverworldCoords.y].AddALifeEntity(entity);
        if (destroyUnits)
        {
            u.DestroyUnit();
        }
        }

    }
