using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldTileHelpers
{
    static Vector2Int coordsCache;

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
            Debug.LogError("Error getting tile from coords " + coords + " " + batch.ToString() + "/" + chunk.ToString() + "/" + local.ToString());
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
    public static void UpdateTileTraversible(int x,int y,bool val)
    {
        GetTileFromCoords(x,y).traversable = val;
    }

}
