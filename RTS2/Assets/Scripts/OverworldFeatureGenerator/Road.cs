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
                target = center + new Vector2Int(WorldChunkManager.ChunkSize + (WorldChunkManager.ChunkBatchSize / 2), 0);
            }
            else if (AdjacentWithSameFeature[x].X < toGenerateIn.OverworldCoords.x)
            {
                target = center - new Vector2Int(WorldChunkManager.ChunkSize + (WorldChunkManager.ChunkBatchSize / 2), 0);

            }
            else if (AdjacentWithSameFeature[x].Y > toGenerateIn.OverworldCoords.y)
            {
                target = center + new Vector2Int(0,WorldChunkManager.ChunkSize+( WorldChunkManager.ChunkBatchSize / 2));

            }
            else if (AdjacentWithSameFeature[x].Y < toGenerateIn.OverworldCoords.y)
            {
                target = center - new Vector2Int(0, WorldChunkManager.ChunkSize + (WorldChunkManager.ChunkBatchSize / 2));

            }
            Vector2 pos = center;
            float dist = Vector2.Distance(pos, new Vector2(target.x,target.y));
            Vector2 dir = target - pos;
            dir = dir.normalized;
            Vector2 perpDir = Vector2.Perpendicular(target - pos).normalized * HalfWidth();
            float inc = 1f / dist;
            Vector2 leftEdge = Vector2.zero;
            Vector2 rightEdge = Vector2.zero;
            Vector2 curPos = new Vector2();
            Vector2 finalPos = new Vector2();
            int count = 0, success = 0;
            for (float f = 0f; f < 1f; f += inc)
            {
                curPos = Vector2.Lerp(pos, target, f);
                leftEdge = curPos + perpDir;
                rightEdge=curPos + (perpDir*-1f);
                //if(UpdateTile(toGenerateIn, curPos, Key))
                //{
                //    success++;
                //}
                //count++;
                for (float a = 0f; a < 1f; a += (1f / (float)width))
                {
                    finalPos = Vector2.Lerp(leftEdge, rightEdge, a);
                    UpdateTile(toGenerateIn, finalPos, Key);
                }
            }
            Debug.Log("Generating road from " + pos + " to " + target + " in batch " + toGenerateIn.coords + " inc " + inc+" count "+ count+"/"+success+" dir "+ dir+" perp "+ perpDir);


            //if (center.x < target.x)
            //{
            //    for (int y1 = center.y - HalfWidth(); y1 < center.y + HalfWidth(); y1++)
            //    {
            //        pos.y = y1;
            //        for (int x1 = center.x; x1 < target.x; x1++)
            //        {
            //            pos.x = x1;

            //            UpdateTile(toGenerateIn, pos, Key);
            //        }
            //    }
            //}
            //else if (center.x > target.x)
            //{
            //    for (int y1 = center.y - HalfWidth(); y1 < center.y + HalfWidth(); y1++)
            //    {
            //        pos.y = y1;
            //        for (int x1 = target.x; x1 < center.x; x1++)
            //        {
            //            pos.x = x1;

            //            UpdateTile(toGenerateIn, pos, Key);
            //        }
            //    }
            //}
            //else
            //{
            //    if (center.y < target.y)
            //    {
            //        for (int x1 = center.x - HalfWidth(); x1 < center.x + HalfWidth(); x1++)
            //        {
            //            pos.x = x1;

            //            for (int y1 = center.y; y1 < target.y; y1++)
            //            {
            //                pos.y = y1;
            //                UpdateTile(toGenerateIn, pos, Key);

            //            }
            //        }
            //    }
            //    else if (center.y > target.y)
            //    {
            //        for (int x1 = center.x - HalfWidth(); x1 < center.x + HalfWidth(); x1++)
            //        {
            //            pos.x = x1;

            //            for (int y1 = target.y; y1 < center.y; y1++)
            //            {
            //                pos.y = y1;
            //                UpdateTile(toGenerateIn, pos, Key);

            //            }
            //        }
            //        }
            //    }
        }
        toGenerateIn.RefreshGroundTiles();
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
