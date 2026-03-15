using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class WorldTileHelpers
{
    static Vector2Int coordsCache;
    static bool InChunkRange(Vector2 coords)
    {
        if (coords.x >= 0 && coords.y >= 0 && coords.x < WorldChunkManager.ChunkSize && coords.y < WorldChunkManager.ChunkSize)
        {
            return true;
        }
        return false;
    }
    public static WorldTile GetTileNearExisting(WorldTile current,WorldChunkBatch toGetFrom,Vector2 pos)
    {
        if (current != null)
        {
            Vector2 dir = new Vector2(pos.x - current.x, pos.y - current.y);
            Vector2 local = current.Local + dir;
            int localX = Mathf.FloorToInt(local.x);
            int localY = Mathf.FloorToInt(local.y);
            if (InChunkRange(local))
            {
                return WorldChunkManager.Instance.ChunkBatches[current.Batch].Chunks[current.Chunk.x, current.Chunk.y].ChunkTiles[localX, localY];
            }
        }
        return toGetFrom.GetTileFromPosition(pos);
    }


    public static WorldTile GetTileFromCoords(int x, int y)
    {
        coordsCache = new Vector2Int(x, y);
        return GetTileFromCoords(coordsCache);
    }

    static Vector2Int batch = new Vector2Int(), chunk = new Vector2Int(), local = new Vector2Int();

    public static WorldTile GetTileFromCoords(Vector2Int coords)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(coords.x, coords.y, out batch, out chunk, out local);
        if (!ValidateCoords())
        {
            return null;
        }
        try
        {
            return WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[local.x, local.y];
        }
        catch
        {
          //  Debug.LogError("Error getting tile from coords " + coords + " " + batch.ToString() + "/" + chunk.ToString() + "/" + local.ToString());
            return null;
        }
    }
    static bool ValidateCoords()
    {
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch) == false)
        {
            return false;
        }
        return true;
    }
    public static bool UpdateTileTraversible(int x,int y,bool val,WorldTileContents toAdd=WorldTileContents.None)
    {
        WorldTile worldTile= GetTileFromCoords(x, y);
        
        if (worldTile != null)
        {
            if(WorldChunkManager.Instance.ChunkBatches[worldTile.Batch].Chunks[worldTile.Chunk.x, worldTile.Chunk.y].WallSegments[worldTile.Local.x, worldTile.Local.y].HasWall)
            {
                if (val == true)
                {
                    return false;
                }
            }
            worldTile.traversable = val;
            if (toAdd != WorldTileContents.None)
            {
                if (!val)
                {
                    worldTile.AddContents(toAdd);
                }
                else
                {
                    
                    worldTile.RemoveContents(toAdd);
                }

            }
            return true;
        }
        else
        {
            //Debug.LogError("No node found at " + x + "," + y);
        }
        return false;
    }
    
}
