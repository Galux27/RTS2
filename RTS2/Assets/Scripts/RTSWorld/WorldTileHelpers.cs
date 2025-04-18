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

    public static WorldTile GetTileFromCoords(Vector2Int coords)
    {
        Vector2Int chunkForNode = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(coords);
        if (WorldChunkManager.Instance.CoordsValid(chunkForNode.x,chunkForNode.y))
        {
            WorldChunk toGetFrom = WorldChunkManager.Instance.Chunks[chunkForNode.x, chunkForNode.y];
            coordsCache = coords - toGetFrom.WorldCoords;
            if (coordsCache.x < 0 || coordsCache.y < 0 || coordsCache.x >= WorldChunkManager.ChunkSize || coordsCache.y >= WorldChunkManager.ChunkSize) { return null; }
            return toGetFrom.ChunkTiles[coordsCache.x, coordsCache.y];
        }
        return null;
    }

    public static void UpdateTileTraversible(int x,int y,bool val)
    {
        GetTileFromCoords(x,y).traversable = val;
    }

}
