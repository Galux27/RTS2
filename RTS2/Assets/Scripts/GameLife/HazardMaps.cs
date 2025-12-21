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

    static bool isHostile = false;
    static int xp = 0, yp = 0, speed = 0;
    static float damage = 0;
    static bool isFreindly = false;
    static int dangerRange = 0;

    static int xStart = 0, yStart = 0, xFin = 0, yFin = 0, x = 0, y = 0;
    static List<string> Enemies = null;
    static ALifeFactionGroup group = null;

    public void PopulateHazardMap(Dictionary<string, ALifeFactionGroup> UnitsInTile,Vector2Int coords)
    {
       // EasyStopwatch.StartStopwatch();
        OverworldChunkShown = coords;
        for (int v = 0; v < WorldChunkManager.ChunkBatchSize; v++)
        {
            for (int b = 0; b < WorldChunkManager.ChunkBatchSize; b++)
            {
                HazardMapData[v, b] = 0;
            }
        }
        isHostile = false;
        xp = 0; yp = 0; speed = 0;
        damage = 0;
        isFreindly = false;
        dangerRange = 0;

        xStart = 0; yStart = 0; xFin = 0; yFin = 0; x = 0; y=0;
        Enemies = FactionController.Instance.GetFactionEnemies(FactionID);
        group = null;
        for(int n=0;n<Enemies.Count;n++)
        {
            if (!UnitsInTile.ContainsKey(Enemies[n]))
            {
                continue;
            }
            group = UnitsInTile[Enemies[n]];
            for (int i = 0; i < group.FactionEntities.Count; i++)
            {
                dangerRange = group.FactionEntities[i].GetDangerDistance();
                xp = group.FactionEntities[i].LocalCoords.x;
                yp = group.FactionEntities[i].LocalCoords.y;
                speed = group.FactionEntities[i].MoveSpeed;
                damage = group.FactionEntities[i].AttackDamage;
                xStart = xp - dangerRange;
                xStart = Mathf.Clamp(xStart, 0, WorldChunkManager.ChunkBatchSize - 1);
                yStart = yp - dangerRange;
                yStart = Mathf.Clamp(yStart, 0, WorldChunkManager.ChunkBatchSize - 1);
                x = xStart;
                y = yStart;
                xFin = xp + dangerRange;
                xFin = Mathf.Clamp(xFin, 0, WorldChunkManager.ChunkBatchSize - 1);
                yFin = yp + dangerRange;
                yFin = Mathf.Clamp(yFin, 0, WorldChunkManager.ChunkBatchSize - 1);

                for (; x < xFin; x++)
                {
                    for (; y < yFin; y++)
                    {
                        // if (CoordValid(x, y))
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

        //foreach (KeyValuePair<string, ALifeFactionGroup> factions in UnitsInTile)
        //{
        //    if (factions.Key != FactionID)
        //    {
        //        isHostile = FactionController.Instance.IsHostile(factions.Key, FactionID);
        //        if (isHostile)
        //        {
        //            for (int i = 0; i < factions.Value.FactionEntities.Count; i++)
        //            {
        //                dangerRange = factions.Value.FactionEntities[i].GetDangerDistance();
        //                xp = factions.Value.FactionEntities[i].LocalCoords.x;
        //                yp = factions.Value.FactionEntities[i].LocalCoords.y;
        //                speed = factions.Value.FactionEntities[i].MoveSpeed;
        //                damage = factions.Value.FactionEntities[i].AttackDamage;
        //                xStart = xp - dangerRange;
        //                xStart = Mathf.Clamp(xStart, 0, WorldChunkManager.ChunkBatchSize - 1);
        //                yStart = yp - dangerRange;
        //                yStart = Mathf.Clamp(yStart, 0, WorldChunkManager.ChunkBatchSize - 1);
        //                x = xStart;
        //                y = yStart;
        //                xFin= xp + dangerRange;
        //                xFin = Mathf.Clamp(xFin, 0, WorldChunkManager.ChunkBatchSize - 1);
        //                yFin = yp + dangerRange;
        //                yFin = Mathf.Clamp(yFin, 0, WorldChunkManager.ChunkBatchSize - 1);

        //                for (; x < xFin; x++)
        //                {
        //                    for (; y < yFin; y++)
        //                    {
        //                       // if (CoordValid(x, y))
        //                        {
        //                            if (isHostile)
        //                            {
        //                                HazardMapData[x, y] += damage;
        //                            }
        //                            else
        //                            {
        //                                HazardMapData[x, y] -= damage;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
      //  EasyStopwatch.StopStopwatch();
        //Debug.Log("A Life: Creating hazard map for " + coords+" took "+ EasyStopwatch.GetStopwatchElapsedTime());

    }
    bool CoordValid(int x, int y)
    {
        return x >= 0 && x < WorldChunkManager.ChunkBatchSize && y < WorldChunkManager.ChunkBatchSize && y >= 0;
    }
}
