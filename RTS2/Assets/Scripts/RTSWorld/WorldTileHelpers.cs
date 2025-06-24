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
     //   Vector2Int chunkForNode = WorldChunkManager.Instance.GetChunkCoordsFromTileCoords(coords);
        //if (WorldChunkManager.Instance.CoordsValid(chunkForNode.x,chunkForNode.y))
        {
            WorldChunk toGetFrom = WorldChunkManager.Instance.GetWorldChunkFromTileCoords(coords,false);
            if (toGetFrom == null)
            {
                Debug.Log("Furniture Click: no world chunk at " + coords + " to get tile from "+coordsCache);
                return null;
            }
            Vector2Int zeroCoords = toGetFrom.ChunkTiles[0, 0].Coords();
            Vector2Int topRightCoords = toGetFrom.ChunkTiles[WorldChunkManager.ChunkSize - 1, WorldChunkManager.ChunkSize - 1].Coords();
            coordsCache = coords-zeroCoords;

            float xP = Mathf.InverseLerp(zeroCoords.x, topRightCoords.x, coords.x);
            float yP=Mathf.InverseLerp(zeroCoords.y, topRightCoords.y, coords.y);
            coordsCache.x = Mathf.RoundToInt(Mathf.Lerp(0, toGetFrom.ChunkTiles.GetLength(0)-1, xP));
            coordsCache.y = Mathf.RoundToInt(Mathf.Lerp(0,toGetFrom.ChunkTiles.GetLength(1)-1, yP));

            //make this code work by lerping between the two points to get the right coordinates
            //  if (coordsCache.x < 0 || coordsCache.y < 0 || coordsCache.x >= WorldChunkManager.ChunkSize || coordsCache.y >= WorldChunkManager.ChunkSize) { return null; }
            try
            {
                return toGetFrom.ChunkTiles[coordsCache.x, coordsCache.y];
            }
            catch
            {
                Debug.Log("Furniture Click: trying to get" + coordsCache + " out of array " + zeroCoords+" to " + topRightCoords);
            }
            }
        return null;
    }

    public static void UpdateTileTraversible(int x,int y,bool val)
    {
        GetTileFromCoords(x,y).traversable = val;
    }

}
