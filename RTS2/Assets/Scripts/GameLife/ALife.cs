using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALife
{
    const float ALifeUpdateRate=.1f;
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
        if (Input.GetKey(KeyCode.V))
        {
            MultithreadedUpdateALife();
            Debug.Break();
        }
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
            Debug.Log("A Life Update: starting a life update");
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

        }
        else
        {
            CanUpdateUnits = true;
            Debug.Log("A Life Update: fin");
        }
    }
    const int maxPerThread = 10;
    int curX=0,curY=0;
    const float MaxUpdateTimeForThread = .01f;
    void MultithreadedUpdateALife()
    {
        MultithreadingStopwatch.StartStopwatch();
        int count = 0;
        for (; curX < OverworldGenerator.Instance.OverworldWidth; curX++)
        {
            for (; curY < OverworldGenerator.Instance.OverworldHeight; curY++)
            {
                UpdateALifeTile(OverworldGenerator.Instance.OverworldTiles[curX, curY]);
                count++;
                if ( MultithreadingStopwatch.GetStopwatchElapsedTime()> MaxUpdateTimeForThread)
                {
                    Debug.Log("A Life Update: updated " + count+" "+curX+","+curY);

                    MultithreadingStopwatch.StopStopwatch();
                    return;
                }
            }
            curY = 0;
        }
        curX = 0;
        curY = 0;
        //Debug.Log("A Life: finished a life pass");
        RunningALifeUpdate = false;
        OnALifeUpdateFinish();
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
        tile.ALifeChunk.UpdateALifeInChunk();
       
    }

    void GenerateEnemiesForTile(OverworldTile tile)
    {
        int MajorRoads = tile.GetQuantitiyOfFeature(OverworldFeature.MajorRoad);
        int MinorRoads = tile.GetQuantitiyOfFeature(OverworldFeature.MinorRoad);
        int BackRoads = tile.GetQuantitiyOfFeature(OverworldFeature.Backroad);
        int pop = tile.Population / 10;
        int toSpawn = UnityEngine.Random.Range(0, 20);
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
                new Vector2Int(UnityEngine.Random.Range(0, WorldChunkManager.ChunkBatchSize-1), UnityEngine.Random.Range(0, WorldChunkManager.ChunkBatchSize-1))
                , 1, 1, 1);
            spawning.SetUnitDetails(data);
            tile.AddALifeEntity(spawning
                , false);
        }
        GenerateRandomUnitsForTile(toSpawn, tile);
        zombieCount+=toSpawn;
    }

    void GenerateRandomUnitsForTile(int zombiesSpawned, OverworldTile tile)
    {
        int toSpawn = tile.Population / 20;
        ALifeEntity spawning = null;
        Debug.Log("A Life: Spawning " + toSpawn + " rifleman");
        CachedUnitData data = UnitTypesController.Instance.UnitData[UnitTypesController.BaseRilfeman];
        for (int x = 0; x < toSpawn; x++)
        {
            spawning = new ALifeEntity(
                new Vector2Int(tile.X, tile.Y),
                FactionController.USER_FACTION,
                UnitTypesController.BaseRilfeman,
                new Vector2Int(UnityEngine.Random.Range(0, WorldChunkManager.ChunkBatchSize-1), UnityEngine.Random.Range(0, WorldChunkManager.ChunkBatchSize-1))
                , 1, 1, 1);
            spawning.SetUnitDetails(data);
            tile.AddALifeEntity(spawning
                , false);
        }
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
    public bool isActive,isDead,HasID,PerformedCombatAction;
    public ulong ID;
    public string Faction,UnitType;
    public int MoveSpeed;
    public float  AttackRate,AttackMaxRange,AttackMinRange,Health,MaxHealth,RangedDamage,AttackDamage;
    public ALifeCombat CombatImPartOf;
    int attackRangeInt;
    public int GetDangerDistance()
    {
        return attackRangeInt + MoveSpeed + 10;
    }

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
        AttackMaxRange=Mathf.Max(1, data.RangeMax);
        attackRangeInt = (int)AttackMaxRange;
    }

    public void UpdateDetailsFromActiveUnit(Unit unit)
    {
        Health = unit.Health();
    }

    public bool HasRanged()
    {
        return AttackMaxRange > 0;
    }
    public void SetActive(bool val)
    {
        isActive = val;
    }

    public bool IsInCombat()
    {
        return CombatImPartOf != null;
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


    bool CoordValid(int x, int y)
    {
        return x >= 0 && x < WorldChunkManager.ChunkBatchSize && y < WorldChunkManager.ChunkBatchSize && y >= 0;
    }
}

public class ALifeCombat
{
    public Dictionary<string, List<ALifeEntity>> EntitiesInvolved=new Dictionary<string, List<ALifeEntity>>();
    public ALifeCombat()
    {
        EntitiesInvolved = new Dictionary<string, List<ALifeEntity>>();
    }
  
    public void AddEntityToCombat(ALifeEntity entity)
    {
        if (!EntitiesInvolved.ContainsKey(entity.Faction))
        {
            EntitiesInvolved.Add(entity.Faction, new List<ALifeEntity>());
        }
        EntitiesInvolved[entity.Faction].Add(entity);
        entity.CombatImPartOf = this;
    }
    
    public void RemoveEntityFromCombat(ALifeEntity entity)
    {
        entity.CombatImPartOf = null;
        entity.PerformedCombatAction = false;
        EntitiesInvolved[entity.Faction].Remove(entity);
    }
    const float MinAttackRange = 1f;

    void DebugOutCombat()
    {
        Debug.Log("A Life: total factions " + EntitiesInvolved.Count);
        foreach (KeyValuePair<string, List<ALifeEntity>> kvp in EntitiesInvolved)
        {
            Debug.Log("A Life: faction " + kvp.Key +" has "+ kvp.Value.Count);

        }
    }
    void ProcessCombat2(ALifeChunk combatIn)
    {
        foreach (KeyValuePair<string, List<ALifeEntity>> kvp in EntitiesInvolved)
        {
            for (int x = 0; x < kvp.Value.Count; x++)
            {
                kvp.Value[x].PerformedCombatAction = false;
            }
        }
        ALifeEntity target = null;
        System.Random ran = new System.Random();
        float distToTarget = 0f, hazardLevel = 0f, attackChance = 0f;
        List<ALifeEntity> ToRemoveFromCombat = new List<ALifeEntity>();
        foreach (KeyValuePair<string, List<ALifeEntity>> kvp in EntitiesInvolved)
        {
            // Debug.Log("A Life: started unit combat " + kvp.Key+","+kvp.Value.Count);
            for (int x = 0; x < kvp.Value.Count; x++)
            {
                target = GetClosestPotentialCombatTarget(kvp.Value[x]);
                // Debug.Log("A Life: found target " + (target == null) + " " + kvp.Value.Count);
                if (target != null)
                {
                    distToTarget = Vector2Int.Distance(kvp.Value[x].LocalCoords, target.LocalCoords);

                    if (distToTarget < kvp.Value[x].GetDangerDistance())
                    {
                        attackChance = ran.Next(0, (int)kvp.Value[x].AttackMaxRange);
                        if (attackChance < distToTarget)
                        {
                            //  Debug.Log("A Life: action 1");
                            Debug.Log("A Life Combat: attacking target" + "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + " target " + target.Faction
                              + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange + "/" + " dam " + kvp.Value[x].AttackDamage + "," + kvp.Value[x].RangedDamage);

                            ALifeActions.AttackTarget(kvp.Value[x], target, kvp.Value[x].AttackMaxRange > MinAttackRange
                                && distToTarget > MinAttackRange);
                            OnAttack(kvp.Value[x], target, ref ToRemoveFromCombat);

                        }
                        else
                        {
                            Vector2Int pos = combatIn.GetPositionToRepositionTo(kvp.Value[x], kvp.Value[x].MoveSpeed);
                            Debug.Log("A Life Combat: moving to safe location" +
                                "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + "->" + pos + " target " + target.Faction
                             + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange);
                            ALifeActions.MoveTowardsPosition(kvp.Value[x], pos);

                            if (hazardLevel < 0)
                            {
                                ToRemoveFromCombat.Add(kvp.Value[x]);
                            }
                        }
                    }

                    if (distToTarget < kvp.Value[x].AttackMaxRange)
                    {
                        attackChance = ran.Next(0, (int)kvp.Value[x].AttackMaxRange);

                        if (attackChance < distToTarget)
                        {
                            //  Debug.Log("A Life: action 1");
                            //Debug.Log("A Life Combat: attacking target" + "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + " target " + target.Faction
                            //  + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange + "/" + " dam " + kvp.Value[x].AttackDamage + "," + kvp.Value[x].RangedDamage);

                            ALifeActions.AttackTarget(kvp.Value[x], target, kvp.Value[x].AttackMaxRange > MinAttackRange
                                && distToTarget > MinAttackRange);
                            OnAttack(kvp.Value[x], target, ref ToRemoveFromCombat);

                        }
                        else
                        {
                            Vector2Int pos = combatIn.GetPositionToRepositionTo(kvp.Value[x], kvp.Value[x].MoveSpeed);
                            //Debug.Log("A Life Combat: moving to safe location" +
                            //    "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + "->" + pos + " target " + target.Faction
                            // + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange);
                            ALifeActions.MoveTowardsPosition(kvp.Value[x], pos);

                            if (hazardLevel < 0)
                            {
                                ToRemoveFromCombat.Add(kvp.Value[x]);
                            }
                        }

                    }
                    else if (distToTarget > kvp.Value[x].AttackMaxRange)
                    {
                        //   Debug.Log("A Life: action 2");
                        //Debug.Log("A Life Combat: moving towards entity" + "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + " target " + target.Faction
                        //    + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange);

                        ALifeActions.MoveTowardsEntity(kvp.Value[x], target);
                    }
                }
                target = null;
                kvp.Value[x].PerformedCombatAction = true;
            }
            //Debug.Log("A Life: finished unit combat");

        }

        for (int x = 0; x < ToRemoveFromCombat.Count; x++)
        {
            RemoveEntityFromCombat(ToRemoveFromCombat[x]);
        }
    }


    public void ProcessCombat(ALifeChunk combatIn)
    {
        ProcessCombat2(combatIn);
        return;
       // DebugOutCombat();
        foreach(KeyValuePair<string,List<ALifeEntity>> kvp in  EntitiesInvolved)
        {
            for(int x=0;x<kvp.Value.Count;x++)
            {
                kvp.Value[x].PerformedCombatAction = false;
            }
        }
        ALifeEntity target = null;
        System.Random ran = new System.Random();
        float distToTarget = 0f, hazardLevel = 0f, attackChance = 0f;
        List<ALifeEntity> ToRemoveFromCombat = new List<ALifeEntity>();
        foreach (KeyValuePair<string, List<ALifeEntity>> kvp in EntitiesInvolved)
        {
           // Debug.Log("A Life: started unit combat " + kvp.Key+","+kvp.Value.Count);
            for (int x = 0; x < kvp.Value.Count; x++)
            {
                target = GetClosestPotentialCombatTarget(kvp.Value[x]);
               // Debug.Log("A Life: found target " + (target == null) + " " + kvp.Value.Count);
                if (target != null)
                {
                    distToTarget = Vector2Int.Distance(kvp.Value[x].LocalCoords, target.LocalCoords);
                    if (distToTarget < kvp.Value[x].AttackMaxRange)
                    {
                        attackChance = ran.Next(0, (int)kvp.Value[x].AttackMaxRange);

                        if (attackChance < distToTarget)
                        {
                            //  Debug.Log("A Life: action 1");
                            Debug.Log("A Life Combat: attacking target" + "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + " target " + target.Faction
                              + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange + "/" + " dam " + kvp.Value[x].AttackDamage + "," + kvp.Value[x].RangedDamage);

                            ALifeActions.AttackTarget(kvp.Value[x], target, kvp.Value[x].AttackMaxRange > MinAttackRange
                                && distToTarget > MinAttackRange);
                            OnAttack(kvp.Value[x], target, ref ToRemoveFromCombat);

                        }
                        else
                        {
                            Vector2Int pos = combatIn.GetPositionToRepositionTo(kvp.Value[x], kvp.Value[x].MoveSpeed);
                            Debug.Log("A Life Combat: moving to safe location" +
                                "OVC:" + combatIn.coords + kvp.Value[x].Faction + " " + kvp.Value[x].LocalCoords + "->" + pos + " target " + target.Faction
                             + " " + target.LocalCoords + " dist " + distToTarget + "/" + kvp.Value[x].AttackMaxRange);
                            ALifeActions.MoveTowardsPosition(kvp.Value[x], pos);

                            if (hazardLevel < 0)
                            {
                                ToRemoveFromCombat.Add(kvp.Value[x]);
                            }
                        }
                       
                    }
                    else if(distToTarget > kvp.Value[x].AttackMaxRange)
                    {
                        //   Debug.Log("A Life: action 2");
                        Debug.Log("A Life Combat: moving towards entity" + "OVC:" + combatIn.coords + kvp.Value[x].Faction+" " + kvp.Value[x].LocalCoords+" target " +target.Faction
                            +" "+ target.LocalCoords+" dist "+ distToTarget+"/"+ kvp.Value[x].AttackMaxRange);
  
                          ALifeActions.MoveTowardsEntity(kvp.Value[x], target);
                    }    
                }
                target = null;
                kvp.Value[x].PerformedCombatAction = true;
            }
            //Debug.Log("A Life: finished unit combat");

        }

        for (int x = 0; x < ToRemoveFromCombat.Count; x++)
        {
            RemoveEntityFromCombat(ToRemoveFromCombat[x]);
        }
    }

    void OnAttack(ALifeEntity performing,ALifeEntity target,ref List<ALifeEntity> toRemove)
    {
        Debug.Log("A Life Combat: type "+performing.UnitType+" pos "+performing.LocalCoords+" hp " +performing.Health 
            +" target type"+ target.UnitType + " pos " + target.LocalCoords+" hp " + target.Health);
        if (performing.isDead &&!toRemove.Contains(performing))
        {
            toRemove.Add(performing);
        }
        if(target.isDead&& !toRemove.Contains(target))
        {
            toRemove.Add(target);
        }
    }

    ALifeEntity GetClosestPotentialCombatTarget(ALifeEntity performing)
    {
        ALifeEntity retVal = null;
        float closest = 999999f, curDist = 999999f ;
        foreach (KeyValuePair<string, List<ALifeEntity>> kvp in EntitiesInvolved)
        {
            if(kvp.Key==performing.Faction|| FactionController.Instance.IsHostile(kvp.Key, performing.Faction)==false) {
                continue;
            
            }
            for(int x=0;x<kvp.Value.Count;x++)
            {
                if (kvp.Value[x].isDead || kvp.Value[x].isActive)
                {
                    continue;
                }
                curDist = Vector2Int.Distance(kvp.Value[x].LocalCoords, performing.LocalCoords);
                if(curDist < closest)
                {
                    closest = curDist;
                    retVal = kvp.Value[x];
                }
            }
        }
        return retVal;
    }
}
