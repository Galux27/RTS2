using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            Buildings[batch.coords].HasChunkBeenGenerated = true;
           /* for(int x = 0; x < Buildings[batch.coords].Count; x++)
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
            Buildings[batch.coords].Clear();*/
        }

    }



    public Dictionary<Vector2Int,ChunkPreGenerationBuildingStore> Buildings=new Dictionary<Vector2Int, ChunkPreGenerationBuildingStore>();
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
            Buildings.Add(coords, new ChunkPreGenerationBuildingStore(coords));
        }
        Buildings[coords].AddBuilding(toAdd);
        coordsAdded.Add(coords);

    }
    Vector2Int batch, chunk, tile;
    Vector2Int GetCoordsOfPosition(Vector2Int coords)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(coords.x, coords.y, out batch, out chunk, out tile);

        return batch ;
    }

    private void Update()
    {
        Debug.Log("Building Gen: total to check " + Buildings.Count);
        foreach(KeyValuePair<Vector2Int,ChunkPreGenerationBuildingStore> kvp in Buildings)
        {
            kvp.Value.CheckToGenerate();
        }
    }
}
public class ChunkPreGenerationBuildingStore
{
    public Vector2Int ChunkCoords;
    public List<BuildingTileArea> AreasForBuildings=new List<BuildingTileArea>();
    public bool HasChunkBeenGenerated = false,FinishedGenerating=false;

    public ChunkPreGenerationBuildingStore(Vector2Int coords)
    {
        ChunkCoords = coords;
    }

    public void AddBuilding(BuildingTileArea building)
    {
        AreasForBuildings.Add(building);
        FinishedGenerating = false;
    }


    public bool DoWeNeedToGenerate()
    {
        return HasChunkBeenGenerated == true && AreasForBuildings.Count > 0;
    }

    public void CheckToGenerate()
    {
        if (FinishedGenerating)
        {
            return;
        }
        Debug.Log("Building Gen: total buildings " + AreasForBuildings.Count);

        if (DoWeNeedToGenerate())
        {
            int toGenerate = GetClosestAreaToPosition(CameraController.Instance.transform.position);
            if (toGenerate > -1)
            {
                Debug.Log("Building Gen: generating building at " + AreasForBuildings[toGenerate].Low);
                BuildingTileArea area = AreasForBuildings[toGenerate];
                AreasForBuildings.RemoveAt(toGenerate);

                BuildingGenerator.Instance.ApplyBuidlingToWorld(area.MyBuilding);
            }
        }
        if (AreasForBuildings.Count == 0)
        {
            {
                Debug.Log("Building Gen: finished generating " + ChunkCoords);

                FinishedGenerating = true;
            }
        }
    }
    int GetClosestAreaToPosition(Vector2 pos)
    {
        int retVal = -1;
        float dist = 9999999f;
        float dist2 = 0f;
        for(int x = 0; x < AreasForBuildings.Count; x++)
        {
            if (AreasForBuildings[x].MyBuilding == null)
            {
                continue;
            }
            dist2 = Vector2.Distance(AreasForBuildings[x].High,pos);
            if (dist2 < dist)
            {
                dist = dist2;
                retVal = x;
            }

            dist2 = Vector2.Distance(AreasForBuildings[x].Low, pos);
            if (dist2 < dist)
            {
                dist = dist2;
                retVal = x;
            }
        }


        return retVal;
    }
        
}