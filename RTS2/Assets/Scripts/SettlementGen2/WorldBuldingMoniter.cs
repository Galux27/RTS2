using System.Collections.Generic;
using UnityEngine;

public class WorldBuldingMoniter : MonoBehaviour
{
    static WorldBuldingMoniter instance;
    public static WorldBuldingMoniter Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<WorldBuldingMoniter>(true);
            }
            return instance;
        }
    }

    public void OnWorldChunkBatchGenerated(WorldChunkBatch batch)
    {
        if (Buildings.ContainsKey(batch.coords))
        {
            for(int x = 0; x < Buildings[batch.coords].Count; x++)
            {
                try
                {
                    BuildingGenerator.Instance.ApplyBuidlingToWorld(Buildings[batch.coords][x].MyBuilding);
                    Buildings[batch.coords][x].DebugColor = Color.green;

                }
                catch (System.Exception e)
                {
                    Buildings[batch.coords][x].DebugColor = Color.red;
                    Debug.LogError("error creating building " + e.ToString());
                }
            }
            Buildings[batch.coords].Clear();
        }

    }

    public Dictionary<Vector2Int,List< BuildingTileArea>> Buildings=new Dictionary<Vector2Int, List<BuildingTileArea>>();
    List<Vector2Int> coordsAdded = new List<Vector2Int>();

    public void AddBuildingZones(List<BuildingTileArea> buildings)
    {
        for(int x=0;x<buildings.Count;x++)
        {
            AddBuildingZone(buildings[x]);
        }
    }

    public void AddBuildingZone(BuildingTileArea area)
    {
        coordsAdded.Clear();
        Vector2Int coords = GetCoordsOfPosition(area.Low);
        AddToBuildingsDictionary(coords, area);

        coords = GetCoordsOfPosition(area.High);
        if (coordsAdded.Contains(coords) == false)
        {
            AddToBuildingsDictionary(coords, area);
            
        }
        Vector2Int c1 = new Vector2Int(area.Low.x, area.High.y);
        coords = GetCoordsOfPosition(c1);
        if (coordsAdded.Contains(coords) == false)
        {
            AddToBuildingsDictionary(coords, area);
        }
        Vector2Int c2 = new Vector2Int(area.High.x, area.Low.y);
        coords = GetCoordsOfPosition(c2);
        if (coordsAdded.Contains(coords) == false)
        {
            AddToBuildingsDictionary(coords, area);
        }
    }

    void AddToBuildingsDictionary(Vector2Int coords,BuildingTileArea toAdd)
    {
        if (!Buildings.ContainsKey(coords))
        {
            Buildings.Add(coords, new List<BuildingTileArea>());
        }
        Buildings[coords].Add(toAdd);
        coordsAdded.Add(coords);

    }
    Vector2Int batch, chunk, tile;
    Vector2Int GetCoordsOfPosition(Vector2Int coords)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(coords.x, coords.y, out batch, out chunk, out tile);

        return batch ;
    }

}
