using System.Collections.Generic;
using System.Linq;
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
            newPosition = new Vector2Int(Mathf.RoundToInt(currentCenterPosition.x), Mathf.RoundToInt(currentCenterPosition.y));
            //  if (roundedCurrentCenterPosition != newPosition || count == 0)
            {
                GenerateRoadSegmentInterior(roundedCurrentCenterPosition, data, PerpDirection);

                roundedCurrentCenterPosition = newPosition;
            }
            if (Vector2.Distance(currentCenterPosition, data.EndPos) < Vector2.Distance(currentCenterPosition + Direction, data.EndPos))
            {
                hitEnd = true;
            }
            currentCenterPosition += Direction;
            count++;
            if (count > maxIterations)
            {
                hitEnd = true;
            }
        }
        
        currentCenterPosition = data.StartPos + Direction;
        hitEnd = false;
        count = 0;
        roundedCurrentCenterPosition = data.StartPos;
        newPosition = Vector2Int.zero;

        while (!hitEnd)
        {
            newPosition = new Vector2Int(Mathf.RoundToInt(currentCenterPosition.x), Mathf.RoundToInt(currentCenterPosition.y));
            //  if (roundedCurrentCenterPosition != newPosition || count == 0)
            {
                GenerateRoadSegmentEdges(newPosition, data, PerpDirection);
            }
            if (Vector2.Distance(currentCenterPosition, data.EndPos) < Vector2.Distance(currentCenterPosition + Direction, data.EndPos))
            {
                hitEnd = true;
            }
            currentCenterPosition += Direction;
            count++;
            if (count > maxIterations)
            {
                hitEnd = true;
            }
        }
    }
    static WorldTile currentTile;
    static HashSet<Vector2Int> updatedTiles = new HashSet<Vector2Int>();
    static void GenerateRoadSegmentEdges(Vector2Int startCoords,RoadData data,Vector2 direction)
    {
        Vector2 startPos = startCoords-direction*(data.Width/2);
        Vector2 endPos = startCoords + direction* (data.Width / 2);
        uint edgeID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.EdgeTile);
        uint roadID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.RoadTile);
        updatedTiles = new HashSet<Vector2Int>();
        Vector2Int globalCoords = new Vector2Int();
        WorldChunkBatch batch = null;
        currentTile = null;

        if (data.HasEdge)
        {

            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {

                currentTile = WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                UpdateTile(data.EdgeTile, edgeID, currentTile, Vector2Int.zero);
              
            }
           
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(endPos.x, endPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {  
                currentTile = WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                UpdateTile(data.EdgeTile, edgeID, currentTile, Vector2Int.zero);
         
            }
        }


   
   
    }
    static void GenerateRoadSegmentInterior(Vector2Int startCoords, RoadData data, Vector2 direction)
    {
        Vector2 startPos = startCoords - direction * (data.Width / 2);
        Vector2 endPos = startCoords + direction * (data.Width / 2);
        uint edgeID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.EdgeTile);
        uint roadID = WorldRenderer.Instance.WorldTilesManager.GetTileID(data.RoadTile);
        updatedTiles = new HashSet<Vector2Int>();
        Vector2Int globalCoords = new Vector2Int();
        WorldChunkBatch batch = null;
        currentTile = null;

        startPos += direction;
        while (Vector2.Distance(startPos, endPos) > Vector2.Distance(startPos + direction, endPos))
        {
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out Batch, out Chunk, out Coords);
            batch = WorldChunkManager.Instance.GetChunkBatch(Batch);
            if (batch != null)
            {
                currentTile = batch.Chunks[Chunk.x, Chunk.y].ChunkTiles[Coords.x, Coords.y];
                globalCoords.x = currentTile.x;
                globalCoords.y = currentTile.y;
               
                    UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.zero, false);
                    UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.up, false);
                    UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.down, false);
                    UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.left, false);
                    UpdateTile(data.RoadTile, roadID, currentTile, Vector2Int.right, false);
                
            }
            startPos += direction;

        }

    }

    static void UpdateTile(string type, uint id, WorldTile tile, Vector2Int offset, bool canSetTile = true)
    {
        Vector2Int globalCoords = new Vector2Int(tile.x, tile.y)+offset;
        if (updatedTiles.Contains(globalCoords))
        {
            return;
        }
        Vector2Int myCoords = Coords + offset;
        if(!WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].CoordsValid(myCoords.x, myCoords.y))
        {
            return;
        }
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].UpdateTile(myCoords.x, myCoords.y, type,id);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[myCoords.x, myCoords.y].SetElevation(OverworldGenerator.Instance.SeaLevel+1);
        WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y]
            .ChunkTiles[myCoords.x, myCoords.y].UpdateWaterLevel(WorldChunkManager.Instance.ChunkBatches[Batch].Chunks[Chunk.x, Chunk.y].ChunkTiles[myCoords.x, myCoords.y].WaterData.WaterLevel * -1f);

        EnvironmentObjectInstance obj = WorldChunkManager.Instance.GetChunkBatch(Batch).Chunks[Chunk.x, Chunk.y].GetEnvObjectNearPoint(new Vector2(currentTile.x,currentTile.y), 2f);
        if (obj != null)
        {
            obj.DestroyInstance();
        }
        if (canSetTile)
        {
            updatedTiles.Add(globalCoords);
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
