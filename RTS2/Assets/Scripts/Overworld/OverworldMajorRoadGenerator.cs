using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Overworld Major Road Generator", menuName = "Overworld/Major Road Generator", order = 1)]
public class OverworldMajorRoadGenerator : OverworldFeatureGenerator
{
    public override void GenerateFeature(OverworldTile[,] world)
    {
        OverworldPathfinding.Init(world);


      
        List<OverworldPathfindingNode> path=null;
        List<OverworldSettlement> MajorSettlements = new List<OverworldSettlement>(), MinorSettlements = new List<OverworldSettlement>() ;
        int averagePop = 0;
        for (int x = 0; x < OverworldGenerator.Instance.Settlements.Length - 1; x++)
        {
            averagePop += OverworldGenerator.Instance.Settlements[x].TotalPopulation;
        }
        averagePop /= OverworldGenerator.Instance.Settlements.Length;

        for (int x = 0; x < OverworldGenerator.Instance.Settlements.Length - 1; x++)
        {
            if(averagePop< OverworldGenerator.Instance.Settlements[x].TotalPopulation)
            {
                MajorSettlements.Add(OverworldGenerator.Instance.Settlements[x]);
            }
            else
            {
                MinorSettlements.Add(OverworldGenerator.Instance.Settlements[x]);
            }
        }
        OverworldBasicPathfinding.InitOverworldBasicPathfinding(world);
        float dist3 = 0f;
        for (int x = 0; x < MajorSettlements.Count - 1; x++)
        {
            if (MajorSettlements[x].pointsInSettlement.Count == 0 || MajorSettlements[x + 1].pointsInSettlement.Count == 0)
            {
                continue;
            }
            path = OverworldPathfinding.FindPathUsingBasic(MajorSettlements[x].pointsInSettlement[0], MajorSettlements[x + 1].pointsInSettlement[0], world);
            if (path!=null&& path.Count > 0)
            {
                for (int q = 0; q < path.Count; q++)
                {
                    if (q > 0)
                    {
                        dist3 = Vector2.Distance(path[q].coords, path[q - 1].coords);
                        if (dist3 > 1)
                        {
                            world[path[q - 1].coords.x, path[q].coords.y].AddFeatureToTile(OverworldFeature.MajorRoad);
                        }
                    }
                    world[path[q].coords.x, path[q].coords.y].AddFeatureToTile(OverworldFeature.MajorRoad);
                }
            }
            OverworldBasicPathfinding.UpdateBasicWeightings();
        }
        OverworldSettlement closestMajor = null;
        float dist = 9999999f;
        float dist2 = 0f;
        for (int x = 0; x < MinorSettlements.Count; x++)
        {
            for (int q = 0; q < MajorSettlements.Count; q++)
            {
                dist2 = Vector2Int.Distance(MinorSettlements[x].pointsInSettlement[0], MajorSettlements[q].pointsInSettlement[0]);
                if (dist2 < dist)
                {
                    dist = dist2;
                    closestMajor = MajorSettlements[q];
                }
            }
            path = OverworldPathfinding.FindPathUsingBasic(closestMajor.pointsInSettlement[0], MinorSettlements[x].pointsInSettlement[0], world);
            if (path!=null&& path.Count > 0)
            {
                for (int q = 0; q < path.Count; q++)
                {

                    world[path[q].coords.x, path[q].coords.y].AddFeatureToTile(OverworldFeature.MinorRoad);
                    if (q > 0)
                    {
                        dist3 = Vector2.Distance(path[q].coords, path[q - 1].coords);
                        if (dist3 > 1)
                        {
                            world[path[q - 1].coords.x, path[q].coords.y].AddFeatureToTile(OverworldFeature.MajorRoad);
                        }
                    }
                }
            }
            OverworldBasicPathfinding.UpdateBasicWeightings();

        }

    }
}
