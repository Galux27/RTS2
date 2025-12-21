using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HazardMaps 
{
    static Dictionary<string, HazardMap> HazardMapsData;

    public static void UpdateHazardMapData(ALifeChunk toCreateFrom)
    {
        if (HazardMapsData == null)
        {
            HazardMapsData = new Dictionary<string, HazardMap>();
        }
        foreach (KeyValuePair<string, ALifeFactionGroup> kvp in toCreateFrom.UnitsInTile)
        {
            if (!HazardMapsData.ContainsKey(kvp.Key))
            {
                HazardMapsData.Add(kvp.Key, new HazardMap(toCreateFrom.coords, kvp.Key));
            }
           
                if (HazardMapsData[kvp.Key].OverworldChunkShown != toCreateFrom.coords)
                {
                    HazardMapsData[kvp.Key].PopulateHazardMap(toCreateFrom.UnitsInTile, toCreateFrom.coords);
                }

                
        }
    }

    public static float[,] GetHazardMap(string faction)
    {
        if (HazardMapsData.ContainsKey(faction))
        {
            return HazardMapsData[faction].HazardMapData;
        }
        return null;
    }

}

    public class HazardMap
{
    public float[,] HazardMapData;
    public Vector2Int OverworldChunkShown;
    public string FactionID;
    public HazardMap(Vector2Int coords,string FactionID)
    {
        OverworldChunkShown = new Vector2Int(-1, -1);
        HazardMapData=new float[WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize];
        this.FactionID=FactionID;
    }



    public void PopulateHazardMap(Dictionary<string, ALifeFactionGroup> UnitsInTile,Vector2Int coords)
    {
        EasyStopwatch.StartStopwatch();
        OverworldChunkShown = coords;
        for (int x = 0; x < HazardMapData.GetLength(0); x++)
        {
            for (int y = 0; y < HazardMapData.GetLength(1); y++)
            {
                HazardMapData[x, y] = 0;
            }
        }
        bool isHostile = false;
        int xp = 0, yp = 0, speed = 0;
        float damage = 0;
        bool isFreindly = false;
        int dangerRange = 0;
        foreach (KeyValuePair<string, ALifeFactionGroup> factions in UnitsInTile)
        {
            if (factions.Key != FactionID)
            {
                isHostile = FactionController.Instance.IsHostile(factions.Key, FactionID);
                if (isHostile)
                {
                    for (int i = 0; i < factions.Value.FactionEntities.Count; i++)
                    {
                        dangerRange = Mathf.RoundToInt(factions.Value.FactionEntities[i].GetDangerDistance());
                        xp = factions.Value.FactionEntities[i].LocalCoords.x;
                        yp = factions.Value.FactionEntities[i].LocalCoords.y;
                        speed = factions.Value.FactionEntities[i].MoveSpeed;
                        damage = factions.Value.FactionEntities[i].AttackDamage;

                        for (int x = xp - dangerRange; x < xp + dangerRange; x++)
                        {
                            for (int y = yp - dangerRange; y < yp + dangerRange; y++)
                            {
                                if (CoordValid(x, y))
                                {
                                    if (isHostile)
                                    {
                                        HazardMapData[x, y] += damage;
                                    }
                                    else
                                    {
                                        HazardMapData[x, y] -= damage;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        EasyStopwatch.StopStopwatch();
        Debug.Log("A Life: Creating hazard map for " + coords+" took "+ EasyStopwatch.GetStopwatchElapsedTime());

    }
    bool CoordValid(int x, int y)
    {
        return x >= 0 && x < WorldChunkManager.ChunkBatchSize && y < WorldChunkManager.ChunkBatchSize && y >= 0;
    }
}
