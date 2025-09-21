using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureRiver : OverworldFeatureToWorldConverter
{
    public OverworldFeature myFeature;
    public string Key;
    public int width;
    public FeatureRiver(OverworldFeature toapply, string key, int width)
    {
        myFeature = toapply;
        Key = key;
        this.width = width;
    }

    int HalfWidth()
    {
        return Mathf.Max(1, width / 2);
    }
    const float MinRiverWidth=2,MaxRiverWidth=32;
    int CalculateRiverWidth(Vector2Int riverData)
    {
        return Mathf.RoundToInt( Mathf.Lerp(MinRiverWidth, MaxRiverWidth, Mathf.InverseLerp(0, riverData.y, riverData.x)));
    }


    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        OverworldTile myTile = OverworldGenerator.Instance.OverworldTiles[toGenerateIn.OverworldCoords.x,toGenerateIn.OverworldCoords.y];
        width = CalculateRiverWidth(myTile.RiverPoint);


        List<OverworldTile> AdjacentTiles = OverworldGenerator.Instance.GetNeighbours(toGenerateIn.OverworldCoords);

        List<OverworldTile> AdjacentWithSameFeature = new List<OverworldTile>();
        for (int x = 0; x < AdjacentTiles.Count; x++)
        {
            if (AdjacentTiles[x].Features.Contains(GetFeatureIGenerate()))
            {
                AdjacentWithSameFeature.Add(AdjacentTiles[x]);
            }
        }

        WorldChunk centerChunk = toGenerateIn.Chunks[toGenerateIn.Chunks.GetLength(0) / 2, toGenerateIn.Chunks.GetLength(1) / 2];
        Vector2Int center = centerChunk.ChunkTiles[centerChunk.ChunkTiles.GetLength(0) / 2, centerChunk.ChunkTiles.GetLength(1) / 2].Coords();
        List<PathfindingNode> path = null;
        Vector2Int target = Vector2Int.zero;
        for (int x = 0; x < AdjacentWithSameFeature.Count; x++)
        {
            if (AdjacentWithSameFeature[x].X > toGenerateIn.OverworldCoords.x)
            {
                target = center + new Vector2Int(16 + WorldChunkManager.ChunkBatchSize / 2, 0);
            }
            else if (AdjacentWithSameFeature[x].X < toGenerateIn.OverworldCoords.x)
            {
                target = center - new Vector2Int(16 + WorldChunkManager.ChunkBatchSize / 2, 0);

            }
            else if (AdjacentWithSameFeature[x].Y > toGenerateIn.OverworldCoords.y)
            {
                target = center + new Vector2Int(0, 16 + WorldChunkManager.ChunkBatchSize / 2);

            }
            else if (AdjacentWithSameFeature[x].Y < toGenerateIn.OverworldCoords.y)
            {
                target = center - new Vector2Int(0, 16 + WorldChunkManager.ChunkBatchSize / 2);

            }
            Vector3 pos = new Vector3();

            if (center.x < target.x)
            {
                for (int y1 = center.y - HalfWidth(); y1 < center.y + HalfWidth(); y1++)
                {
                    pos.y = y1;
                    for (int x1 = center.x; x1 < target.x; x1++)
                    {
                        pos.x = x1;

                        UpdateTile(toGenerateIn, pos, Key);
                    }
                }
            }
            else if (center.x > target.x)
            {
                for (int y1 = center.y - HalfWidth(); y1 < center.y + HalfWidth(); y1++)
                {
                    pos.y = y1;
                    for (int x1 = target.x; x1 < center.x; x1++)
                    {
                        pos.x = x1;

                        UpdateTile(toGenerateIn, pos, Key);
                    }
                }
            }
            else
            {
                if (center.y < target.y)
                {
                    for (int x1 = center.x - HalfWidth(); x1 < center.x + HalfWidth(); x1++)
                    {
                        pos.x = x1;

                        for (int y1 = center.y; y1 < target.y; y1++)
                        {
                            pos.y = y1;
                            UpdateTile(toGenerateIn, pos, Key);

                        }
                    }
                }
                else if (center.y > target.y)
                {
                    for (int x1 = center.x - HalfWidth(); x1 < center.x + HalfWidth(); x1++)
                    {
                        pos.x = x1;

                        for (int y1 = target.y; y1 < center.y; y1++)
                        {
                            pos.y = y1;
                            UpdateTile(toGenerateIn, pos, Key);

                        }
                    }
                }
            }
        }
        toGenerateIn.RefreshGroundTiles();
    }

    void UpdateTile(WorldChunkBatch toGenerateIn, Vector3 pos, string type)
    {
        WorldTile toEdit = toGenerateIn.GetTileFromPosition(pos);
        if (toEdit != null)
        {
            toEdit.UpdateWaterLevel(10f);
            toEdit.CanPutDecorationsOn = false;
        }
    }

    public override OverworldFeature GetFeatureIGenerate()
    {
        return myFeature;
    }
}
