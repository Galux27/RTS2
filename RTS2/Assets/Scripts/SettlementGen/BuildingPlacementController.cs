using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class BuildingPlacementController : MonoBehaviour
{
    static BuildingPlacementController instance;
    public static BuildingPlacementController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<BuildingPlacementController>();
            }
            return instance;
        }
    }
    public List<WorldChunkBatch> BatchesWithBuildings = new List<WorldChunkBatch>();

    // Update is called once per frame
    void Update()
    {
        Vector2 camPos = CameraController.Instance.transform.position;
        for(int x = 0; x < BatchesWithBuildings.Count; x++)
        {
            float dist = 9999999f;
            float dist2 = 0;
            int zoneTouse = -1, indexToUse = -1;
            for(int y= 0; y < BatchesWithBuildings[x].Zones.Count; y++)
            {
                for (int z = 0; z < BatchesWithBuildings[x].Zones[y].Buildings.Count; z++)
                {
                    if (BatchesWithBuildings[x].Zones[y].Buildings[z].Drawn)
                    {
                        continue;
                    }
                    dist2 = Vector2.Distance(BatchesWithBuildings[x].Zones[y].Buildings[z].Position, camPos);
                    if (dist2 < dist)
                    {
                        dist = dist2;
                        zoneTouse = y;
                        indexToUse = z;
                    }
                }
            }
            if (zoneTouse>-1)
            {
                BuildingGenerator.Instance.GenerateBuilding(BatchesWithBuildings[x].Zones[zoneTouse].Buildings[indexToUse]);
                BatchesWithBuildings[x].Zones[zoneTouse].Buildings[indexToUse].Drawn = true;
                return;
            }
        }
    }
}
