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


    RoadType ConvertToRoadType(OverworldFeature applying)
    {
        if (applying == OverworldFeature.MajorRoad)
        {
            return RoadType.MajorRoad;
        }else if(applying== OverworldFeature.MinorRoad)
        {
            return RoadType.MinorRoad;
        }else if (applying == OverworldFeature.MinorRoad)
        {
            return RoadType.MinorRoad;
        }
        return RoadType.None;
    }


    void GenerateRoadToExistingSettlement(WorldChunkBatch toGenerateIn,OverworldTile myTile,List<OverworldTile> AdjacentTiles)
    {
        List<OverworldTile> AdjacentWithSameFeature = new List<OverworldTile>();
        for (int x = 0; x < AdjacentTiles.Count; x++)
        {
            if (AdjacentTiles[x].Features.Contains(GetFeatureIGenerate()) 
                && !AdjacentTiles[x].Features.Contains(OverworldFeature.Settlement))
            {
                AdjacentWithSameFeature.Add(AdjacentTiles[x]);
            }
        }
        if (AdjacentWithSameFeature.Count == 0)
        {
            return;
        }
        Dictionary<RoadType, List<Vector2Int>> Roads = new Dictionary<RoadType, List<Vector2Int>>();
        RoadType myType = GetRoadTypeFromOverworldFeature(myFeature);
        Vector2Int roadStart = toGenerateIn.Center();//centerChunk.ChunkTiles[centerChunk.ChunkTiles.GetLength(0)/2,centerChunk.ChunkTiles.GetLength(1)/2].Coords();
      
            List<PathfindingNode> path = null;
        Vector2Int target = Vector2Int.zero;
        Vector2Int offCenter = roadStart;

        
        int mod = 0;
        if (GetFeatureIGenerate() != OverworldFeature.MajorRoad)
        {
            mod = -1;
        }

        for (int x = 0; x < AdjacentWithSameFeature.Count; x++)
        {
           // if (!ShouldWeSkipRoad(myTile, AdjacentWithSameFeature[x]))
            {
                if (AdjacentWithSameFeature[x].X > toGenerateIn.OverworldCoords.x)
                {
                    target = roadStart + new Vector2Int((WorldChunkManager.ChunkBatchSize / 2) + mod, 0);
                }
                else if (AdjacentWithSameFeature[x].X < toGenerateIn.OverworldCoords.x)
                {
                    target = roadStart - new Vector2Int((WorldChunkManager.ChunkBatchSize / 2) + mod, 0);

                }
                else if (AdjacentWithSameFeature[x].Y > toGenerateIn.OverworldCoords.y)
                {
                    target = roadStart + new Vector2Int(0, (WorldChunkManager.ChunkBatchSize / 2) + mod);

                }
                else if (AdjacentWithSameFeature[x].Y < toGenerateIn.OverworldCoords.y)
                {
                    target = roadStart - new Vector2Int(0, (WorldChunkManager.ChunkBatchSize / 2) + mod);

                }

                bool gotStart = false;
                float dist = 9999999f, dist2 = 0;
                Vector2 closestPoint = roadStart;
                for (int q = 0; q < toGenerateIn.Roads.Count; q++)
                {
                    if (toGenerateIn.Roads[q].Type == myType)
                    {

                        dist2 = Vector2.Distance(toGenerateIn.Roads[q].StartPos, target);
                        if (dist2 < dist)
                        {
                            dist = dist2;
                            closestPoint = toGenerateIn.Roads[q].StartPos;
                        }

                        dist2 = Vector2.Distance(toGenerateIn.Roads[q].EndPos, target);
                        if (dist2 < dist)
                        {
                            dist = dist2;
                            closestPoint = toGenerateIn.Roads[q].EndPos;
                        }
                    }
                }
                roadStart = new Vector2Int( Mathf.RoundToInt( closestPoint.x),Mathf.RoundToInt(closestPoint.y));


                offCenter = new Vector2Int((int)Mathf.Lerp(roadStart.x, target.x, .25f), (int)Mathf.Lerp(roadStart.y, target.y, .25f));
                Vector2 dir = target - roadStart;
                dir = dir.normalized;
                Vector2Int centerOffset = new Vector2Int(Mathf.RoundToInt(dir.x * (width / 2)), Mathf.RoundToInt(dir.y * (width / 2)));
                AddRoad(toGenerateIn, GetRoadTypeFromOverworldFeature(myFeature), roadStart, target, width);
                Debug.Log("Generating road settlement connection from " + roadStart + " to " + target + " in batch " + toGenerateIn.coords);
            }
        }
    }


    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        if (ConvertToRoadType(myFeature) == RoadType.None)
        {
            return;
        }
      
        List<OverworldTile> AdjacentTiles = OverworldGenerator.Instance.GetNeighbours(toGenerateIn.OverworldCoords);
        OverworldTile myTile = OverworldGenerator.Instance.GetOverworldTile(toGenerateIn.OverworldCoords);

        if (myTile.Features.Contains(OverworldFeature.Settlement))
        {
            return;
        }
        width = RoadTypeManager.Instance.AllRoadDetails[ConvertToRoadType(myFeature).ToString()].RoadWidth;

        //if (myTile.Features.Contains(OverworldFeature.Settlement))
        //{
        //    GenerateRoadToExistingSettlement(toGenerateIn, myTile, AdjacentTiles);

        //}
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
                if (checking.Features.Contains(OverworldFeature.MajorRoad)
                    &&tile.Features.Contains(OverworldFeature.MajorRoad))
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
