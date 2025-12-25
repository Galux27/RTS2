using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ALifeChunk 
{

    public ALifeChunk(Vector2Int coords)
    {
        this.coords = coords;
        Combat = new ALifeCombat();
    }

    public Vector2Int coords;
    public Dictionary<string, ALifeFactionGroup> UnitsInTile = new Dictionary<string, ALifeFactionGroup>();
    ALifeCombat Combat = null;

    void CheckForEntityWithSameID(ALifeFactionGroup group,ulong id)
    {
        for(int x=0;x<group.FactionEntities.Count;x++)
        {
            if (group.FactionEntities[x].ID == id)
            {
                group.FactionEntities.RemoveAt(x);
                return;
            }
        }
    }

    public void AddALifeEntity(ALifeEntity entity, bool CheckForExisting = true)
    {

        if (!UnitsInTile.ContainsKey(entity.Faction))
        {
            UnitsInTile.Add(entity.Faction, new ALifeFactionGroup(entity.Faction));
        }
        if (entity.Faction == FactionController.USER_FACTION)
        {
        }
        UnitsInTile[entity.Faction].AddEntity(entity);
        if (CheckForExisting)
        {
            if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
            {
                Debug.Log("A Life: moved into existing chunk");
                OverworldGenerator.Instance.ALifeSystem.OnALifeEntityEntersActiveChunk(entity, WorldChunkManager.Instance.ChunkBatches[coords]);
            }
        }
    }

    public void RemoveALifeEntity(ALifeEntity entity)
    {
        if (!UnitsInTile.ContainsKey(entity.Faction))
        {
            UnitsInTile.Add(entity.Faction, new ALifeFactionGroup(entity.Faction));
        }
        UnitsInTile[entity.Faction].RemoveEntity(entity);
    }

    bool NeedsToPerformCombatChecks()
    {
        return UnitsInTile.Count > 1;
    }

    static float Hazard = 0, CombCheck=0, CombExe=0, Decisions = 0;
    public void UpdateALifeInChunk()
    {
        if (UnitsInTile.Count == 0)
        {
            return;
        }
        MakeUnitDecisions();

        if (NeedsToPerformCombatChecks())
        {
            ALifeDebugRenderer.Instance?.AddChunkWithCombat(coords);
          //  UpdateHazardMap();
            CheckForCombat();
            PerformCombat();

        }
    }
    void PerformCombat()
    {
      //  Debug.Log("A Life: starting combat update for " + coords);
      //  EasyStopwatch.StartStopwatch();
        Combat.ProcessCombat(this);
       // EasyStopwatch.StopStopwatch();
        //Debug.Log("A Life: combat update " + EasyStopwatch.GetStopwatchElapsedTime());

    }
    void CheckForCombat()
    {
        EasyStopwatch.StartStopwatch();
        Vector2Int pos = new Vector2Int();

        List<string> factionEnemies = null;
        ALifeEntity target = null;
        foreach (KeyValuePair<string, ALifeFactionGroup> kvp in UnitsInTile)
        {
            factionEnemies = FactionController.Instance.GetFactionEnemies(kvp.Key);
          //  hazardMap = HazardMaps.GetHazardMap(kvp.Key);
            for(int x=0;x<kvp.Value.FactionEntities.Count;x++)
            {
                if (kvp.Value.FactionEntities[x].IsInCombat())
                {
                    continue;
                }
                pos = ClampToArray( kvp.Value.FactionEntities[x].LocalCoords);
                if (CoordValid(pos.x, pos.y))
                {
                   for(int q = 0; q < factionEnemies.Count; q++)
                    {
                        if (UnitsInTile.ContainsKey(factionEnemies[q]) && IsInRangeOfEnemy(UnitsInTile[factionEnemies[q]], kvp.Value.FactionEntities[x],out target))
                        {
                            Combat.AddEntityToCombat(kvp.Value.FactionEntities[x]);
                            Combat.AddEntityToCombat(target);
                            break;
                        }
                    }

                }
                
            }
        }
        EasyStopwatch.StopStopwatch();
        Debug.Log("A Life: checking for combatents took " + EasyStopwatch.GetStopwatchElapsedTime());
    }

    bool IsInRangeOfEnemy(ALifeFactionGroup enemies,ALifeEntity checking,out ALifeEntity target)
    {
        float dist = 0f;
        float maxDist = checking.GetDangerDistance();
        for (int x = 0; x < enemies.FactionEntities.Count; x++)
        {
            dist= Vector2Int.Distance(enemies.FactionEntities[x].LocalCoords,checking.LocalCoords);
            if (dist < maxDist)
            {
                target = enemies.FactionEntities[x];
                return true;
            }
        }
        target = null;
        return false;
    }

    Vector2Int ClampToArray(Vector2Int pos)
    {
        pos.x = Mathf.Clamp(pos.x, 0, WorldChunkManager.ChunkBatchSize - 1);
        pos.y = Mathf.Clamp(pos.y, 0, WorldChunkManager.ChunkBatchSize - 1);
        return pos;
    }
    void MakeUnitDecisions()
    {

        foreach(KeyValuePair<string,ALifeFactionGroup> kvp in UnitsInTile)
        {
            List<ALifeEntity> toUpdate = UnitsInTile[kvp.Key].FactionEntities;

            for (int x = 0; x < toUpdate.Count; x++)
            {
                if (!toUpdate[x].IsInCombat())
                {
                    ALifeDecisionMaker.MakeZombieDecisions(toUpdate[x]);
                }
                }
            }
    }


    void UpdateHazardMap()
    {
        HazardMaps.UpdateHazardMapData(this);
      
    }
    static System.Random ran = new System.Random();
    public Vector2Int GetPositionToRepositionTo(ALifeEntity entity,int dist)
    {
        Vector2Int retVal = entity.LocalCoords;

        retVal.x += ran.Next(-dist, dist);
        retVal.y += ran.Next(-dist, dist);
        retVal.x = Mathf.Clamp(entity.LocalCoords.x, 0, WorldChunkManager.ChunkBatchSize - 1);
        retVal.y = Mathf.Clamp(entity.LocalCoords.y, 0, WorldChunkManager.ChunkBatchSize - 1);

        //float[,] HazardMap = HazardMaps.GetHazardMap(entity.Faction);
        //hazardLevel = HazardMap[retVal.x,retVal.y];

        //if (HazardMap == null)
        //{
        //    return retVal;
        //}
        //float currentHazardLevel = HazardMap[retVal.x, retVal.y];
        //for (int x = retVal.x - entity.MoveSpeed; x < retVal.x + entity.MoveSpeed; x++) 
        //{
        //    for (int y = retVal.y - entity.MoveSpeed; y < retVal.y + entity.MoveSpeed; y++)
        //    {
        //        if (CoordValid(x, y))
        //        {
        //            if(HazardMap[x, y] < currentHazardLevel)
        //            {
        //                retVal.x = x;
        //                retVal.y = y;
        //                currentHazardLevel = HazardMap[x, y];
        //            }
        //        }
        //    }
        //}

        return retVal;
    }


    bool CoordValid(int x, int y)
    {
        return x >= 0 && x < WorldChunkManager.ChunkBatchSize && y < WorldChunkManager.ChunkBatchSize && y >= 0;
    }
}
