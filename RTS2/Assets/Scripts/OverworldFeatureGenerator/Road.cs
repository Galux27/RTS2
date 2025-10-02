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

        List<OverworldTile> AdjacentWithSameFeature = new List<OverworldTile>();
        for(int x = 0; x < AdjacentTiles.Count; x++)
        {
            if (AdjacentTiles[x].Features.Contains(GetFeatureIGenerate()))
            {
                AdjacentWithSameFeature.Add(AdjacentTiles[x]);
            }
        }

        //WorldChunk centerChunk = toGenerateIn.Chunks[toGenerateIn.Chunks.GetLength(0) / 2, toGenerateIn.Chunks.GetLength(1) / 2];
        Vector2Int center = toGenerateIn.Center();//centerChunk.ChunkTiles[centerChunk.ChunkTiles.GetLength(0)/2,centerChunk.ChunkTiles.GetLength(1)/2].Coords();
        List<PathfindingNode> path = null;
        Vector2Int target = Vector2Int.zero;
        for(int x = 0; x < AdjacentWithSameFeature.Count; x++)
        {
            
            if (AdjacentWithSameFeature[x].X > toGenerateIn.OverworldCoords.x)
            {
                target = center + new Vector2Int( (WorldChunkManager.ChunkBatchSize / 2), 0);
            }
            else if (AdjacentWithSameFeature[x].X < toGenerateIn.OverworldCoords.x)
            {
                target = center - new Vector2Int( (WorldChunkManager.ChunkBatchSize / 2), 0);

            }
            else if (AdjacentWithSameFeature[x].Y > toGenerateIn.OverworldCoords.y)
            {
                target = center + new Vector2Int(0,( WorldChunkManager.ChunkBatchSize / 2));

            }
            else if (AdjacentWithSameFeature[x].Y < toGenerateIn.OverworldCoords.y)
            {
                target = center - new Vector2Int(0,  (WorldChunkManager.ChunkBatchSize / 2));

            }
            Vector2 pos = center;
            AddRoad(toGenerateIn, GetRoadTypeFromOverworldFeature(myFeature), pos, target, Key, width);
            Debug.Log("Generating road from " + pos + " to " + target + " in batch " + toGenerateIn.coords);

        }
        toGenerateIn.RefreshGroundTiles();
    }

    void AddRoad(WorldChunkBatch toAddTo,RoadType type,Vector2 start,Vector2 end,string key,int width)
    {
        switch (type)
        {
            case RoadType.None:
                break;
            case RoadType.MajorRoad:
                toAddTo.AddRoad(new MajorRoad(type, start, end, Key, width));

                break;
            case RoadType.MinorRoad:
                toAddTo.AddRoad(new MinorRoad(type, start, end, Key, width));

                break;
            case RoadType.Backroad:
                toAddTo.AddRoad(new Backroad(type, start, end, Key, width));

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

    bool UpdateTile(WorldChunkBatch toGenerateIn,Vector3 pos, string type)
    {
        WorldTile toEdit = toGenerateIn.GetTileFromPosition(pos);
        if (toEdit != null)
        {
            Debug.Log("Road: setting tile at " + toEdit.Coords() + " original pos " + pos + " " + toGenerateIn.GetDebugOut()+" to " + type) ;
            toEdit.UpdateTileType(type);
            toEdit.CanPutDecorationsOn = false;
            return true;
        }
        return false;
    }

    public override OverworldFeature GetFeatureIGenerate()
    {
        return myFeature;
    }
}
