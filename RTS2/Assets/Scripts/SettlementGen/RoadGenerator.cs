using System.Collections.Generic;
using System.Net;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

public static class RoadGenerator
{
    const int maxIterations = 500;
    static HashSet<Vector2Int> Edges=new HashSet<Vector2Int>();
    static Vector2Int Batch, Chunk, Coords;
   public static void GenerateRoad(RoadData data)
    {
        batchesChanged = new List<Vector2Int>();
        Edges = new HashSet<Vector2Int>();
        Vector2 Direction = data.EndPos - data.StartPos;
        Direction = Direction.normalized;
        Vector2 PerpDirection=Vector2.Perpendicular(Direction);
        PerpDirection = PerpDirection.normalized;
        Vector2 currentCenterPosition = data.StartPos;
        Vector2Int roundedCurrentCenterPosition = data.StartPos;
        Vector2Int newPosition = Vector2Int.zero;
        bool hitEnd = false;
        int count = 0;

        while (!hitEnd)
        {
            newPosition=new Vector2Int(Mathf.RoundToInt(currentCenterPosition.x),Mathf.RoundToInt(currentCenterPosition.y));
          //  if (roundedCurrentCenterPosition != newPosition || count == 0)
            {
                GenerateRoadSegment(roundedCurrentCenterPosition, data, PerpDirection);
                roundedCurrentCenterPosition=newPosition;
            }
            if (Vector2.Distance(currentCenterPosition, data.EndPos) < Vector2.Distance(currentCenterPosition+Direction, data.EndPos))
            {
                hitEnd = true;
            }
            currentCenterPosition += Direction;
            count++;
            if(count>maxIterations)
            {
                hitEnd = true;
            }
        }
    }
    static WorldTile currentTile;
    static List<Vector2Int> batchesChanged = new List<Vector2Int>();
    static void GenerateRoadSegment(Vector2Int startCoords,RoadData data,Vector2 direction)
    {
        Vector2 startPos = startCoords-direction*(data.Width/2);
        Vector2 endPos = startCoords + direction* (data.Width / 2);
        uint edgeID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.EdgeTile);
        uint roadID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.RoadTile);
        HashSet<Vector2Int> updatedTiles = new HashSet<Vector2Int>();
        Vector2Int globalCoords = new Vector2Int();
        WorldChunkBatch batch = null;
        currentTile = null;
        while(Vector2.Distance(startPos,endPos)>Vector2.Distance(startPos+direction, endPos))
        {
            startPos += direction;
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {
                if (!batchesChanged.Contains(Batch))
                {
                    batchesChanged.Add(Batch);
                }
                currentTile = batch.Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                globalCoords.x = currentTile.x;
                globalCoords.y = currentTile.y;
                if (!updatedTiles.Contains(globalCoords))
                {
                    UpdateTile(data.RoadTile, roadID);
                    updatedTiles.Add(globalCoords);

                    if (!updatedTiles.Contains(globalCoords + Vector2Int.right))
                    {
                        WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x + 1, Coords.y, data.RoadTile, roadID);
                    }
                    if (!updatedTiles.Contains(globalCoords + Vector2Int.left))
                    {
                        WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x - 1, Coords.y, data.RoadTile, roadID);
                    }
                    if (!updatedTiles.Contains(globalCoords + Vector2Int.up))
                    {
                        WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y + 1, data.RoadTile, roadID);
                    }
                    if (!updatedTiles.Contains(globalCoords + Vector2Int.down))
                    {
                        WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y - 1, data.RoadTile, roadID);
                    }
                }
            }
        }
        if (data.HasEdge)
        {
            startPos += direction;

            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {
                if (!batchesChanged.Contains(Batch))
                {
                    batchesChanged.Add(Batch);
                }
                currentTile = WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                UpdateTile(data.EdgeTile, edgeID);
                globalCoords = new Vector2Int(currentTile.x, currentTile.y);

                if (!updatedTiles.Contains(globalCoords + Vector2Int.right))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x + 1, Coords.y, data.EdgeTile, edgeID);
                }
                if (!updatedTiles.Contains(globalCoords + Vector2Int.left))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x - 1, Coords.y, data.EdgeTile, edgeID);
                }
                if (!updatedTiles.Contains(globalCoords + Vector2Int.up))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y + 1, data.EdgeTile, edgeID);
                }
                if (!updatedTiles.Contains(globalCoords + Vector2Int.down))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y - 1, data.EdgeTile, edgeID);
                }
                Edges.Add(globalCoords);
                updatedTiles.Add(globalCoords);
            }
            startPos = startCoords - direction * (data.Width / 2);
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {
                if (!batchesChanged.Contains(Batch))
                {
                    batchesChanged.Add(Batch);
                }
                currentTile = WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
              UpdateTile(data.EdgeTile, edgeID);
                globalCoords.x = currentTile.x;
                globalCoords.y = currentTile.y;
                if (!updatedTiles.Contains(globalCoords + Vector2Int.right))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x + 1, Coords.y, data.EdgeTile, edgeID);
                }
                if (!updatedTiles.Contains(globalCoords + Vector2Int.left))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x - 1, Coords.y, data.EdgeTile, edgeID);
                }
                if (!updatedTiles.Contains(globalCoords + Vector2Int.up))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y + 1, data.EdgeTile, edgeID);
                }
                if (!updatedTiles.Contains(globalCoords + Vector2Int.down))
                {
                    WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y - 1, data.EdgeTile, edgeID);
                }
                Edges.Add(globalCoords);
                updatedTiles.Add(globalCoords);
            }
        }

        //for (int x = 0; x < batchesChanged.Count; x++)
        //{
        //    WorldChunkManager.Instance.ChunkBatches[batchesChanged[x]].UpdateElevations();
        //    WorldChunkManager.Instance.ChunkBatches[batchesChanged[x]].RefreshElevationTiles();
        //}
    }


    static void UpdateTile(string type,uint id)
    {
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].UpdateTile(Coords.x, Coords.y, type,id);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y].SetElevation(OverworldGenerator.Instance.SeaLevel+1);
        OverworldTile tile = OverworldGenerator.Instance.GetOverworldTile(WorldChunkManager.Instance.ChunkBatches[Batch].OverworldCoords);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y].Elevation.SetTileToWalkable(tile.Elevation);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y]
            .ChunkTiles[Coords.x, Coords.y].UpdateWaterLevel(WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y].WaterData.WaterLevel * -1f);

        EnvironmentObjectInstance obj = WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].GetEnvObjectNearPoint(new Vector2(currentTile.x,currentTile.y), 2f);
        if (obj != null)
        {
            obj.DestroyInstance();
        }
    }
}

public class RoadData
{
    public Vector2Int StartPos, EndPos;
    public int Width;
    public bool HasEdge = false;
    public string RoadTile, EdgeTile;
}
