using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Road : OverworldFeatureToWorldConverter
{
    public OverworldFeature myFeature;
    public string Key;
    public int width;
    public Road(OverworldFeature toapply,string key,int width) {
        myFeature = toapply;
        Key = key;
        this.width= width;
    }

    int HalfWidth()
    {
        return Mathf.Max(1, width / 2);
    }

    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        List<OverworldTile> AdjacentTiles = OverworldGenerator.Instance.GetNeighbours(toGenerateIn.OverworldCoords);
        OverworldTile myTile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);
        List<OverworldTile> AdjacentWithSameFeature = new List<OverworldTile>();
        for(int x = 0; x < AdjacentTiles.Count; x++)
        {
            if (AdjacentTiles[x].Features.Contains(GetFeatureIGenerate()))
            {
                AdjacentWithSameFeature.Add(AdjacentTiles[x]);
            }
        }
        Dictionary<RoadType, List<Vector2Int>> Roads = new Dictionary<RoadType, List<Vector2Int>>();
        //WorldChunk centerChunk = toGenerateIn.Chunks[toGenerateIn.Chunks.GetLength(0) / 2, toGenerateIn.Chunks.GetLength(1) / 2];
        Vector2Int center = toGenerateIn.Center();//centerChunk.ChunkTiles[centerChunk.ChunkTiles.GetLength(0)/2,centerChunk.ChunkTiles.GetLength(1)/2].Coords();
        List<PathfindingNode> path = null;
        Vector2Int target = Vector2Int.zero;
        Vector2Int offCenter = center;
        int mod = 0;
        if (GetFeatureIGenerate() != OverworldFeature.MajorRoad)
        {
            mod = -1;
        }

        for(int x = 0; x < AdjacentWithSameFeature.Count; x++)
        {
            if (!ShouldWeSkipRoad(myTile, AdjacentWithSameFeature[x]))
            {
                
            
                if (AdjacentWithSameFeature[x].X > toGenerateIn.OverworldCoords.x)
                {
                    target = center + new Vector2Int( (WorldChunkManager.ChunkBatchSize / 2)+mod, 0);
                }
                else if (AdjacentWithSameFeature[x].X < toGenerateIn.OverworldCoords.x)
                {
                    target = center - new Vector2Int( (WorldChunkManager.ChunkBatchSize / 2) + mod, 0);

                }
                else if (AdjacentWithSameFeature[x].Y > toGenerateIn.OverworldCoords.y)
                {
                    target = center + new Vector2Int(0,( WorldChunkManager.ChunkBatchSize / 2) + mod);

                }
                else if (AdjacentWithSameFeature[x].Y < toGenerateIn.OverworldCoords.y)
                {
                    target = center - new Vector2Int(0,  (WorldChunkManager.ChunkBatchSize / 2) + mod);

                }
                offCenter = new Vector2Int((int)Mathf.Lerp(center.x, target.x, .25f), (int)Mathf.Lerp(center.y, target.y, .25f));
                Vector2 dir = target - center;
                dir = dir.normalized;
                Vector2Int centerOffset = new Vector2Int(Mathf.RoundToInt(dir.x * (width / 2)), Mathf.RoundToInt(dir.y * (width / 2)));
                AddRoad(toGenerateIn, GetRoadTypeFromOverworldFeature(myFeature), center- centerOffset, target, width);
                Debug.Log("Generating road from " + center + " to " + target + " in batch " + toGenerateIn.coords);
            }
        }
        toGenerateIn.RefreshGroundTiles();
    }

    bool ShouldWeSkipRoad(OverworldTile tile,OverworldTile checking)
    {
        switch (GetFeatureIGenerate())
        {
            case OverworldFeature.MajorRoad:
                break;
            case OverworldFeature.MinorRoad:
                if (checking.Features.Contains(OverworldFeature.MajorRoad)&&tile.Features.Contains(OverworldFeature.MajorRoad))
                {
                    return true;
                }
                break;
            case OverworldFeature.Backroad:
                if (checking.Features.Contains(OverworldFeature.MajorRoad) 
                    && tile.Features.Contains(OverworldFeature.MajorRoad)|| 
                    checking.Features.Contains(OverworldFeature.MinorRoad)
                    && tile.Features.Contains(OverworldFeature.MinorRoad))
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }


    bool CheckIfRoadExists(WorldChunkBatch toAddTo,RoadType type,Vector2 end)
    {
        //for(int x = 0; x < toAddTo.Roads.Count; x++)
        //{
        //    if (toAddTo.Roads[x].Type == type)
        //    {
        //        if (toAddTo.Roads[x].RoadEnd == end)
        //        {
        //            return true;
        //        }
        //    }
        //}
        return false;
    }

    void AddRoad(WorldChunkBatch toAddTo,RoadType type,Vector2Int start,Vector2Int end,int width)
    {
        if (CheckIfRoadExists(toAddTo, type, end))
        {
            return;
        }
        switch (type)
        {
            case RoadType.None:
                break;
            case RoadType.MajorRoad:
                toAddTo.AddRoad(new RoadData(start, end, width, type));
                break;
            case RoadType.MinorRoad:
                toAddTo.AddRoad(new RoadData(start, end, width, type));
                break;
            case RoadType.Backroad:
                toAddTo.AddRoad(new RoadData(start, end, width, type));
                break;
            default:
                break;
        }
    }


    RoadType GetRoadTypeFromOverworldFeature(OverworldFeature overworldFeature)
    {
        if (overworldFeature == OverworldFeature.MajorRoad)
        {
            return RoadType.MajorRoad;
        }else if (overworldFeature == OverworldFeature.MinorRoad)
        {
            return RoadType.MinorRoad;

        }
        else if (overworldFeature == OverworldFeature.Backroad)
        {
            return RoadType.Backroad;

        }
        else
        {
            return RoadType.None;
        }
    }

    

    public override OverworldFeature GetFeatureIGenerate()
    {
        return myFeature;
    }
}
