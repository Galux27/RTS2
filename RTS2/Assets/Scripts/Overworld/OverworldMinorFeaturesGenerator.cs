using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
[CreateAssetMenu(fileName = "Overworld Minor Feature Generator", menuName = "Overworld/Minor Feature Generator", order = 1)]
public class OverworldMinorFeaturesGenerator : OverworldFeatureGenerator
{
    public int MinorFeaturesToGenerate;
    int width, height;

    public override void GenerateFeature(OverworldTile[,] world)
    {
        Vector2Int coords = Vector2Int.zero;
        width = world.GetLength(0);
        height = world.GetLength(1);
        coords.x = Random.Range(0, width);
        coords.y = Random.Range(0, height);
        List<Vector2Int> coordsUsed = new List<Vector2Int>();

        for (int i=0;i<MinorFeaturesToGenerate;i++)
        {
            while (!validCoords(coords, width, height, world) && coordsUsed.Contains(coords) == false)
            {
                coords.x = Random.Range(0, width);
                coords.y = Random.Range(0, height);
            }

            world[coords.x, coords.y].AddFeatureToTile(OverworldFeature.MiscFeature);

            coordsUsed.Add(coords);
            coords.x = Random.Range(0, width);
            coords.y = Random.Range(0, height);
        }
        float dist = 99999999f;
        float workingDist = 0f;
        Vector2Int targetToMakeRoadTo = Vector2Int.zero;
        List<OverworldPathfindingNode> path = null;

        for (int i = 0; i < coordsUsed.Count; i++)
        {
            for (int x = 0; x < OverworldGenerator.Instance.Settlements.Length - 1; x++)
            {
                workingDist = Vector2Int.Distance(coords,OverworldGenerator.Instance.Settlements[x].pointsInSettlement[0]);
                if (workingDist < dist)
                {
                    dist = workingDist;
                    targetToMakeRoadTo = OverworldGenerator.Instance.Settlements[x].pointsInSettlement[0];
                }
            }

            path = OverworldPathfinding.FindPathUsingBasic(coordsUsed[i], targetToMakeRoadTo, world);
            if (path != null)
            {
                for(int x=0;x< path.Count; x++)
                {

                    if (world[path[x].coords.x, path[x].coords.y].Features.Contains(OverworldFeature.Settlement)||
                        world[path[x].coords.x, path[x].coords.y].Features.Contains(OverworldFeature.MajorRoad)
                        || world[path[x].coords.x, path[x].coords.y].Features.Contains(OverworldFeature.MinorRoad) 
                        || world[path[x].coords.x, path[x].coords.y].Features.Contains(OverworldFeature.Backroad))
                    {
                        world[path[x].coords.x, path[x].coords.y].Features.Contains(OverworldFeature.Backroad);

                        break;
                    }
                    else if (world[path[x].coords.x, path[x].coords.y].Features.Contains(OverworldFeature.Backroad))
                    {
                        break;
                    }
                    world[path[x].coords.x, path[x].coords.y].AddFeatureToTile(OverworldFeature.Backroad);

                }
            }
            OverworldBasicPathfinding.UpdateBasicWeightings();

        }
    }

    bool validCoords(Vector2Int coords, int width, int height, OverworldTile[,] world)
    {
        if (coords.x < 0 || coords.y < 0 || coords.y >= height || coords.x >= width)
        {
            return false;
        }
        if (world[coords.x, coords.y].Elevation > OverworldGenerator.Instance.SeaLevel)
        {
            return true;
        }


        return false;
    }
}
