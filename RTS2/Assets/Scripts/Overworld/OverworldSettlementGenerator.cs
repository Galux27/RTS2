using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
[CreateAssetMenu(fileName = "Overworld Settlement Generator", menuName = "Overworld/Settlement Generator", order = 1)]
public class OverworldSettlementGenerator : OverworldFeatureGenerator
{
    public int SettlementPopulationMinimum,SettlementPopulationMaximum;
    public int NumberOfSettlements,ExpandChance;
    public int MinSettlementElemevation,MaxSettlementElemevation;
    int width, height;

    public override void GenerateFeature(OverworldTile[,] world)
    {
        OverworldSettlement[] settlements=new OverworldSettlement[NumberOfSettlements];
        Vector2Int coords = new Vector2Int();
        width = world.GetLength(0);
        height = world.GetLength(1);
        coords.x = Random.Range(0, width);
        coords.y = Random.Range(0, height);
   
        List<Vector2Int> coordsUsed = new List<Vector2Int>();
        for (int x = 0; x < NumberOfSettlements; x++)
        {
            while (!validCoords(coords, world,coordsUsed) && coordsUsed.Contains(coords)==false)
            {
                coords.x = Random.Range(0, width);
                coords.y = Random.Range(0, height);
            }
            settlements[x] = new OverworldSettlement(Random.Range(SettlementPopulationMinimum, SettlementPopulationMaximum));
            settlements[x].AddTile(coords,ref world);
            Neighbours(coords, world);
            for(int q = 0; q < neighbourCache.Count; q++)
            {
                settlements[x].AddToWaitingRoom(neighbourCache[q]);
            }
            coordsUsed.Add(coords);
            coords.x = Random.Range(0, width);
            coords.y = Random.Range(0, height);
        }

        bool CanContinue = true;
        while (CanContinue)
        {
            bool hitAnything = false;
            for(int x = 0; x < NumberOfSettlements; x++)
            {
                if (settlements[x].CanExpand())
                {
                    hitAnything = true;
                    ExpandSettlement(settlements[x], ref world);
                }
            }
            
            CanContinue = hitAnything;
            
        }
        OverworldGenerator.Instance.Settlements = settlements;
    }

    void ExpandSettlement(OverworldSettlement toExpand,ref OverworldTile[,] world)
    {
        List<Vector2Int> coordsToAdd = new List<Vector2Int>();

        while (toExpand.waitingRoom.Count > 0)
        {
            int x = Random.Range(0, toExpand.waitingRoom.Count);
            Neighbours(toExpand.waitingRoom[x], world);
            toExpand.AddTile(toExpand.waitingRoom[x], ref world);
            if (!toExpand.CanExpand())
            {
                return;
            }
            for (int q = 0; q < neighbourCache.Count; q++)
            {
                if (coordsToAdd.Contains(neighbourCache[q]) == false
                    && toExpand.pointsInSettlement.Contains(neighbourCache[q]) == false
                    && toExpand.waitingRoom.Contains(neighbourCache[q]) == false)
                {
                    coordsToAdd.Add(neighbourCache[q]);
                }
            }
            toExpand.waitingRoom.RemoveAt(x);
        }
       
        toExpand.waitingRoom = coordsToAdd;
    }

    List<Vector2Int> neighbourCache;
    void Neighbours(Vector2Int coords, OverworldTile[,] world)
    {
        neighbourCache = new List<Vector2Int>();
        if (validCoords(coords.x + 1, coords.y,world) && Random.Range(0, 100) < ExpandChance)
        {
            neighbourCache.Add(coords + new Vector2Int(1, 0));
        }
        if (validCoords(coords.x - 1, coords.y, world) && Random.Range(0, 100) < ExpandChance)
        {
            neighbourCache.Add(coords + new Vector2Int(-1, 0));
        }
        if (validCoords(coords.x, coords.y + 1, world) && Random.Range(0, 100) < ExpandChance)
        {
            neighbourCache.Add(coords + new Vector2Int(0, 1));
        }
        if (validCoords(coords.x, coords.y - 1, world) && Random.Range(0,100)< ExpandChance)
        {
            neighbourCache.Add(coords + new Vector2Int(0, -1));
        }
    }
    bool validCoords(int x, int y, OverworldTile[,] world)
    {
        if (x < 0 || y < 0 || y >= height || x >= width)
        {
            return false;
        }
        if (world[x, y].Elevation >= MinSettlementElemevation && world[x, y].Elevation <= MaxSettlementElemevation)
        {
            return true;
        }

     
        return false;
    }
    bool validCoords(Vector2Int coords, OverworldTile[,] world,List<Vector2Int> existing = null)
    {
        if (coords.x < 0 || coords.y < 0 || coords.y >= height || coords.x >= width)
        {
            return false;
        }
        //for (int q = 0; q < existing.Count; q++)
        //{
        //    if (Vector2Int.Distance(coords, existing[q]) < 100)
        //    {
        //        return false;
        //    }
        //}


        if (world[coords.x, coords.y].Elevation >= MinSettlementElemevation && world[coords.x, coords.y].Elevation <= MaxSettlementElemevation)
        {
            return true;
        }
       
        

        return false;
    }
    bool validCoords(Vector2Int coords, int width, int height, OverworldTile[,] world)
    {
        if (coords.x < 0 || coords.y < 0 || coords.y >= height || coords.x >= width)
        {
            return false;
        }


        if (world[coords.x,coords.y].Elevation >=MinSettlementElemevation
            && world[coords.x, coords.y].Elevation <= MaxSettlementElemevation)
        {
            return true;
        }

        
        return false;
    }
}

public class OverworldSettlement
{
    static int BaseSettlmentID = 0;
    public int Id;
    public List<Vector2Int> pointsInSettlement,waitingRoom;
    public int RemainingPopulationToDistribute,TotalPopulation;
    public Color DebugColour;
    public GeneratedSettlement GeneratedInstance;


    public OverworldSettlement(int pop)
    {
        Id=BaseSettlmentID;
        BaseSettlmentID++;
        waitingRoom = new List<Vector2Int>();
        RemainingPopulationToDistribute = pop;
        TotalPopulation = pop;
        pointsInSettlement = new List<Vector2Int>();
        DebugColour = new Color(Random.value, Random.value, Random.value);
    }

    public void AddToWaitingRoom(Vector2Int tiles)
    {
        if (!pointsInSettlement.Contains(tiles) && !waitingRoom.Contains(tiles))
        {
            waitingRoom.Add(tiles);
        }
    }


    public void AddTile(Vector2Int coords,ref OverworldTile[,] world)
    {
        int toAdd = Random.Range(50, 200);
        toAdd=Mathf.Min(RemainingPopulationToDistribute, toAdd);
        world[coords.x, coords.y].AddFeatureToTile(OverworldFeature.Settlement);
        world[coords.x, coords.y].SetPopulation(this, toAdd);
        RemainingPopulationToDistribute -= toAdd;
        pointsInSettlement.Add(coords);
    }

    public bool CanExpand()
    {
        return RemainingPopulationToDistribute > 0&&waitingRoom.Count>0;
    }
    

    public void GenerateSettlement()
    {
        Settlement_Settings settings = GetSettingsForSettlement();
        GeneratedInstance = new GeneratedSettlement();
        GeneratedInstance.GenerateSettlementAreas(settings,WorldChunkManager.ChunkBatchSize);

        SettlementGenerator.GenerateSettlement(GeneratedInstance, settings);
        Debug.LogError("Generated settlement ID " + Id + "," + settings.Center + "," + settings.Size+","+GeneratedInstance.highways.Count+","+GeneratedInstance.avenues.Count+","+GeneratedInstance.roads.Count);

        GeneratedInstance.PopulateAreas(settings, WorldChunkManager.ChunkBatchSize);

        SettlementTileArea area = new SettlementTileArea(GeneratedInstance, settings);
        GeneratedInstance.AssignBuildingsToArea(area, 64,settings);

        SettlementGenerator.DebugDrawSettlementRoads(GeneratedInstance, 100f);

#if UNITY_EDITOR
        DebugCheats.Instance.LastSettlement = GeneratedInstance;
        DebugCheats.Instance.LastArea = area;
        DebugCheats.Instance.LastSettings = settings;
#endif

    }


    public Settlement_Settings GetSettingsForSettlement()
    {
        Settlement_Settings settings = SettlementGeneratorSettingsController.Instance.BaseSettings;

        Vector2Int Min = new Vector2Int(9999999, 999999), Max = new Vector2Int(-9999999, -9999999);
        Vector2Int cp = pointsInSettlement[0];
        Vector2Int wp = cp* WorldChunkManager.ChunkBatchSize;
        Vector2Int overworldOffset = OverworldGenerator.Instance.GetOverworldStartingCoords() * WorldChunkManager.ChunkBatchSize;
        OverworldTile curTile = null;
        for(int x = 0; x < pointsInSettlement.Count; x++)
        {
            cp = pointsInSettlement[x];
            wp = cp * WorldChunkManager.ChunkBatchSize;
            if (wp.x < Min.x)
            {
                Min.x = wp.x;
            }
            if (wp.x > Max.x)
            {
                Max.x = wp.x;
            }

            if (wp.y < Min.y)
            {
                Min.y = wp.y;
            }
            if (wp.y> Max.y)
            {
                Max.y = wp.y;
            }
            curTile = OverworldGenerator.Instance.OverworldTiles[cp.x, cp.y];
            if (curTile.Features.Contains(OverworldFeature.MajorRoad))
            {
                settings.ManualHighwayPoints.Add(wp);

            }
            if (curTile.Features.Contains(OverworldFeature.MinorRoad))
            {
                settings.ManualAvenuePoints.Add(wp);
            }
            if (curTile.Features.Contains(OverworldFeature.Backroad))
            {
                settings.ManualRoadPoints.Add(wp);
            }
            if (curTile.Features.Contains(OverworldFeature.River))
            {
                settings.ManualRiverPoints.Add(wp);
            }
        }
        settings.GenerateHighwayStarts = true;
        //settings.StartingHighwayCount = 5;
       
        settings.RiverPoints = 0;
        settings.Center = Vector2.Lerp(Min, Max, .5f);
        settings.Size = Max - Min;
        Debug.LogError("Settlement Settings " + settings.Center + "," + settings.Size);
        settings.DistBetweenAvenues = 45;
        settings.DistBetweenRoads = 10;
        settings.MaxAvenuePasses = 22;
        settings.MaxRoadPasses = 22;
        settings.AvenueLength = 50;
        settings.RoadLength = 25;
        settings.HighwayLength = 75;
        settings.GenerateHighwayStarts = false;
        settings.RiverWidth = 7;
        settings.RiverSectionLength = 11;
        
        return settings;
    }

}
