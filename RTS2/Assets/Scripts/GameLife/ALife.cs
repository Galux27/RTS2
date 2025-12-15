using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALife
{
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
        for(int x = 0; x < toSpawn; x++)
        {
            tile.AddALifeEntity(new ALifeEntity(new Vector2Int(tile.X, tile.Y), ALifeEntityType.Zombie));
        }
        zombieCount+=toSpawn;
    }
}

public struct ALifeEntity
{
    public Vector2Int CurrentCoords,PreviousCoords;
    public ALifeEntityType EntityType;
    public bool isActive,isDead,HasID;
    public ulong ID;
    public ALifeEntity(Vector2Int startCoords,ALifeEntityType entityType)
    {
        CurrentCoords = startCoords;
        EntityType = entityType;
        PreviousCoords = startCoords;
        isActive = false;
        isDead = false;
        HasID = false;
        ID = 0;
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

public class ALifeAction
{
    public virtual void PerformAction(ALifeEntity performing)
    {

    }
}
