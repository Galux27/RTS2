using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
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
        Settlement[] settlements=new Settlement[NumberOfSettlements];
        Vector2Int coords = new Vector2Int();
        width = world.GetLength(0);
        height = world.GetLength(1);
        coords.x = Random.Range(0, width);
        coords.y = Random.Range(0, height);
   
        List<Vector2Int> coordsUsed = new List<Vector2Int>();
        for (int x = 0; x < NumberOfSettlements; x++)
        {
            while (!validCoords(coords, width, height, world) && coordsUsed.Contains(coords)==false)
            {
                coords.x = Random.Range(0, width);
                coords.y = Random.Range(0, height);
            }
            settlements[x] = new Settlement(Random.Range(SettlementPopulationMinimum, SettlementPopulationMaximum));
            settlements[x].AddTile(coords,ref world);
            Neighbours(coords, world);
            for(int q = 0; q < neighbourCache.Count; q++)
            {
                settlements[x].AddToWaitingRoom(neighbourCache[q]);
            }
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

    void ExpandSettlement(Settlement toExpand,ref OverworldTile[,] world)
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
    bool validCoords(Vector2Int coords, int width, int height, OverworldTile[,] world)
    {
        if (coords.x < 0 || coords.y < 0 || coords.y >= height || coords.x >= width)
        {
            return false;
        }
        if (world[coords.x,coords.y].Elevation >=MinSettlementElemevation && world[coords.x, coords.y].Elevation <= MaxSettlementElemevation)
        {
            return true;
        }

        
        return false;
    }
}

public class Settlement
{
    static int BaseSettlmentID = 0;
    public int Id;
    public List<Vector2Int> pointsInSettlement,waitingRoom;
    public int RemainingPopulationToDistribute,TotalPopulation;

    public Settlement(int pop)
    {
        Id=BaseSettlmentID;
        BaseSettlmentID++;
        waitingRoom = new List<Vector2Int>();
        RemainingPopulationToDistribute = pop;
        TotalPopulation = pop;
        pointsInSettlement = new List<Vector2Int>();
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
        world[coords.x, coords.y].Population += toAdd;
        RemainingPopulationToDistribute -= toAdd;
        pointsInSettlement.Add(coords);
    }

    public bool CanExpand()
    {
        return RemainingPopulationToDistribute > 0&&waitingRoom.Count>0;
    }
}
