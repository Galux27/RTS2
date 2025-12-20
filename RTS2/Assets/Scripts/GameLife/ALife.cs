using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALife
{
    const float ALifeUpdateRate=30;
    float UpdateTimer = 0f;
    const int ZombiesPerMajorRoad = 25, ZombiesPerMinorRoad = 10, ZombiesPerBackroad = 5;
    public int zombieCount = 0;
    public void GenerateEntitiesForOverworldTile(OverworldTile tile)
    {
        if (tile.GetQuantitiyOfFeature(OverworldFeature.MiscFeature) > 0)
        {

        }else
        {
            GenerateEnemiesForTile(tile);
        }
    }
    static bool RunningALifeUpdate = false,CanStartNewThread=true,CanUpdateUnits=false;
    public void Update()
    {

        if (RunningALifeUpdate)
        {
            if (CanStartNewThread)
            {
                StartUpdateThread();
            }
            return;
        }

        if (CanUpdateUnits)
        {
            GenerateUnitsFromMovingChunks();
        }
        UpdateTimer += DeltaTimeWrapper.GameplayDelta;
        if(UpdateTimer > ALifeUpdateRate)
        {
            RunningALifeUpdate = true;
            Debug.Log("A Life: starting a life update");
            StartUpdateThread();
            //MultiThreadedManager.Instance.AddAction(()=> { MultithreadedUpdateALife(); },()=> { OnALifeUpdateFinish(); });
            //OverworldGenerator.Instance.StartCoroutine(UpdateALife());
            UpdateTimer = 0f;
        }
    }
    void StartUpdateThread()
    {
        CanStartNewThread = false;
        MultiThreadedManager.Instance.AddAction(() => { MultithreadedUpdateALife(); }, () => { OnALifeUpdateFinish(); });
    
    }
    void OnALifeUpdateFinish()
    {
        CanStartNewThread = true;

        if (RunningALifeUpdate)
        {
            Debug.Log("A Life: next batch" +curX+","+curY);

        }
        else
        {
            CanUpdateUnits = true;
            Debug.Log("A Life: fin");
        }
    }
    const int maxPerThread = 500;
    int curX=0,curY=0;
    void MultithreadedUpdateALife()
    {
        int count = 0;
        for (; curX < OverworldGenerator.Instance.OverworldWidth; curX++)
        {
            for (; curY < OverworldGenerator.Instance.OverworldHeight; curY++)
            {
                UpdateALifeTile(OverworldGenerator.Instance.OverworldTiles[curX, curY]);
                count++;
                if (count > maxPerThread)
                {
                    return;
                }
            }
            curY = 0;
        }
        curX = 0;
        curY = 0;
        //Debug.Log("A Life: finished a life pass");
        RunningALifeUpdate = false;
    }

    const int ALifeUpdatesPerFrame = 30;
    IEnumerator UpdateALife()
    {
        yield return new WaitForEndOfFrame();
        int updateCount = 0;
        for(int x = 0; x < OverworldGenerator.Instance.OverworldWidth; x++)
        {
            for (int y = 0; y < OverworldGenerator.Instance.OverworldHeight; y++)
            {
                UpdateALifeTile(OverworldGenerator.Instance.OverworldTiles[x,y]);
                updateCount++;
                if (updateCount > ALifeUpdatesPerFrame)
                {
                    updateCount = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        Debug.Log("A Life: finished a life pass");
        RunningALifeUpdate=false;
    }

    void UpdateALifeTile(OverworldTile tile)
    {
        if ( tile.UnitsInTile.ContainsKey(FactionController.ZOMBIE_FACTION))
        {
            List<ALifeEntity> toUpdate = tile.UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities;

            for (int x = 0; x < toUpdate.Count; x++)
            {
                ALifeDecisionMaker.MakeZombieDecisions(toUpdate[x]);
            }
        }
    }

    void GenerateEnemiesForTile(OverworldTile tile)
    {
        int MajorRoads = tile.GetQuantitiyOfFeature(OverworldFeature.MajorRoad);
        int MinorRoads = tile.GetQuantitiyOfFeature(OverworldFeature.MinorRoad);
        int BackRoads = tile.GetQuantitiyOfFeature(OverworldFeature.Backroad);
        int pop = tile.Population / 10;
        int toSpawn = Random.Range(0, 20);
        toSpawn += pop;
        toSpawn += MajorRoads * ZombiesPerMajorRoad;
        toSpawn += MinorRoads * ZombiesPerMinorRoad;
        toSpawn += BackRoads * ZombiesPerBackroad;
        int MajorWater = tile.GetQuantitiyOfFeature(OverworldFeature.LargeWaterBody);
        if (MajorWater > 0)
        {
            toSpawn = 0;
        }
        ALifeEntity spawning = null;
        CachedUnitData data = UnitTypesController.Instance.UnitData["Zombie"];
        for(int x = 0; x < toSpawn; x++)
        {
            spawning = new ALifeEntity(
                new Vector2Int(tile.X, tile.Y),
                FactionController.ZOMBIE_FACTION,
                UnitTypesController.BaseZombie,
                new Vector2Int(Random.Range(0, WorldChunkManager.ChunkBatchSize), Random.Range(0, WorldChunkManager.ChunkBatchSize))
                , 1, 1, 1);
            spawning.SetUnitDetails(data);
            tile.AddALifeEntity(spawning
                , false);
        }
        zombieCount+=toSpawn;
    }




    public void OnALifeEntityEntersActiveChunk(ALifeEntity ent, WorldChunkBatch entering)
    {
        if (ent.isActive || ent.isDead)
        {
            return;
        }
        if (!ToGenerate.ContainsKey(entering))
        {
            ToGenerate.Add(entering, new List<ALifeEntity>());
        }
        ToGenerate[entering].Add(ent);
    }
    Dictionary<WorldChunkBatch, List<ALifeEntity>> ToGenerate = new Dictionary<WorldChunkBatch, List<ALifeEntity>>();
    void GenerateUnitsFromMovingChunks()
    {
        foreach(KeyValuePair<WorldChunkBatch,List<ALifeEntity>> kvp in ToGenerate)
        {
            for(int x=0;x< kvp.Value.Count; x++)
            {

                GameLifeManager.Instance.SpawnUnitFromALifeEntity(kvp.Value[x], kvp.Key);//ZombieSpawner.OnALifeEntityEntersLoadedChunk(kvp.Value[x], kvp.Key);


            }
        }
        ToGenerate.Clear();
        CanUpdateUnits = false;
    }
}

public class ALifeEntity
{
    public Vector2Int CurrentBatchCoords,PreviousBatchCoords,LocalCoords;
    public bool isActive,isDead,HasID;
    public ulong ID;
    public string Faction,UnitType;
    public int MoveSpeed;
    public float  AttackRate,AttackMaxRange,AttackMinRange,Health,MaxHealth,RangedDamage,AttackDamage;
    public ALifeEntity(Vector2Int startCoords,string faction,string type,Vector2Int localCoords,float moveSpeed,float attackRate,float attackRange)
    {
        CurrentBatchCoords = startCoords;
        PreviousBatchCoords = startCoords;
        isActive = false;
        isDead = false;
        HasID = false;
        ID = 0;
        Faction = faction;
        UnitType=type;
        LocalCoords = localCoords;
        MoveSpeed = Mathf.RoundToInt( moveSpeed);
        AttackRate = attackRate;
        AttackMaxRange = attackRange;
        AttackMinRange = 0;

    }

    public void SetUnitDetails(CachedUnitData data)
    {
        MoveSpeed =Mathf.RoundToInt( data.MoveSpeed);
        AttackRate = data.AttackRate;
        AttackDamage = data.MeleeDamage;
        Health = data.Health;
        MaxHealth = data.MaxHealth;
        RangedDamage=data.RangedDamage;
        AttackMinRange = data.RangeMin;
        AttackMaxRange=data.RangeMax;
    }
    public bool HasRanged()
    {
        return AttackMaxRange > 0;
    }
    public void SetActive(bool val)
    {
        isActive = val;
    }

    public void SetID(ulong id) { 
        ID = id;
        HasID = true;
    }
}

public enum ALifeEntityType
{
    Zombie,
    UserUnit,
    AIUnit
}

public class ALifeFactionGroup
{
    public string FactionID;
    public List<ALifeEntity> FactionEntities;
    public ALifeFactionGroup(string faction)
    {
        FactionID = faction;
        FactionEntities=new List<ALifeEntity>();
    }

    public void AddEntity(ALifeEntity entity) { 
        FactionEntities.Add(entity);
    }

    public void RemoveEntity(ALifeEntity entity)
    {
        FactionEntities.Remove(entity);
    }
}
