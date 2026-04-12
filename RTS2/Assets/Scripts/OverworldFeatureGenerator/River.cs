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
            RiverData data = new RiverData(center, target, width);
            toGenerateIn.AddRiver(data);
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
    WorldTile toEdit;
    void UpdateTile(WorldChunkBatch toGenerateIn, Vector3 pos, string type)
    {
       toEdit = WorldTileHelpers.GetTileNearExisting(toEdit, toGenerateIn, pos);
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

public class RiverData
{
    public Vector2Int StartPos, EndPos;
    public Vector2Int LeftStart,RightStart,LeftEnd,RightEnd;
    public int Width;
    public Bounds MyBounds;
    public RiverData(Vector2Int start,Vector2Int end,int width)
    {
        StartPos = start;
        EndPos = end;
        Width = width;
        float halfWidth = width / 2f;
        Vector2 perp = end - start;
        perp = Vector2.Perpendicular(perp).normalized*halfWidth;
        Vector2 LeftStart = StartPos - (perp);
        Vector2 RightStart = StartPos + (perp);
        Vector2 LeftEnd = EndPos - (perp);
        Vector2 RightEnd = EndPos + (perp);
        this.LeftStart = new Vector2Int(Mathf.RoundToInt(LeftStart.x), Mathf.RoundToInt(LeftStart.y));
        this.RightStart = new Vector2Int(Mathf.RoundToInt(RightStart.x), Mathf.RoundToInt(RightStart.y));
        this.LeftEnd = new Vector2Int(Mathf.RoundToInt(LeftEnd.x), Mathf.RoundToInt(LeftEnd.y));
        this.RightEnd = new Vector2Int(Mathf.RoundToInt(RightEnd.x), Mathf.RoundToInt(RightEnd.y));

        MyBounds = new Bounds(Vector2.Lerp(start, end, .5f), Vector3.one) ;
        MyBounds.Encapsulate(LeftStart);
        MyBounds.Encapsulate(RightStart);
        MyBounds.Encapsulate(LeftEnd);
        MyBounds.Encapsulate(RightEnd);

    }

}
