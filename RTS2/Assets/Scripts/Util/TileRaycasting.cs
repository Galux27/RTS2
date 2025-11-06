using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileRaycasting 
{
    
}

public class TileRaycast
{
    public WorldTile currentTile;
    public Vector2 increment;
    Vector2Int batch = new Vector2Int(), chunk = new Vector2Int(), coords = new Vector2Int();
    Vector2 CoordsAsVector;
    Vector2Int roundedCoords = new Vector2Int();
    List<WorldTile> TilesHit = new List<WorldTile>();
    int maxIterations = 0;
    public TileRaycast(Vector3 startPos,Vector3 endPos)
    {
        currentTile = Pathfinding.GetTileFromPosition(startPos);
        // WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(startPos.x, startPos.y, out batch, out chunk, out coords);
        batch = currentTile.Batch;
        chunk = currentTile.Chunk;
        coords = currentTile.Local;
        Debug.Log("Starting tile raycast at " + batch + "," + chunk + "," + coords);
        TilesHit.Add(currentTile);
        increment = endPos - startPos;
        increment = increment.normalized;
        CoordsAsVector = new Vector2(coords.x, coords.y);
        maxIterations = Mathf.RoundToInt( Vector3.Distance(startPos, endPos));
    }

    public void PerformRaycast()
    {
        while (maxIterations > 0)
        {
            ProgressRaycast();
            maxIterations--;
        }
    }

    public void ProgressRaycast()
    {
        CoordsAsVector += increment;
        roundedCoords.x = Mathf.FloorToInt(CoordsAsVector.x);
        roundedCoords.y = Mathf.FloorToInt(CoordsAsVector.y);
        bool MovedToNew = false;
        Debug.Log("tile raycast iteration " + batch + "," + chunk + "," + coords+"("+CoordsAsVector+")");

        if (roundedCoords != coords)
        {
            MovedToNew = true;
            coords = roundedCoords;
            if (IsChunkOutOfBounds(coords))
            {
                IncrementCoords(ref coords, ref chunk,1);
                if (IsChunkOutOfBounds(chunk))
                {
                    IncrementCoords(ref chunk, ref batch, WorldChunkManager.ChunkBatchSize);
                }
                CoordsAsVector.x = coords.x;
                CoordsAsVector.y = coords.y;
            }

        }
        if (MovedToNew)
        {
            try
            {
                TilesHit.Add(WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[coords.x, coords.y]);
            }
            catch(System.Exception e)
            {
                Debug.LogError("Error on tile raycast " + batch + "," + chunk + "," + coords+","+increment+","+e.ToString());
            }
        }
    }

    public void DrawPath()
    {
        for(int x = 0; x < TilesHit.Count-1; x++)
        {
            Debug.DrawLine(TilesHit[x].WorldPos(), TilesHit[x + 1].WorldPos(), Color.cyan);
        }
    }


    void IncrementCoords(ref Vector2Int coordsToCheckAgainst,ref Vector2Int coordsToAlter,int inc)
    {
        if (coordsToCheckAgainst.x < 0)
        {
            coordsToAlter.x-=inc;
            coordsToCheckAgainst.x = WorldChunkManager.ChunkSize-1;
        }
        else if (coordsToCheckAgainst.x >= WorldChunkManager.ChunkSize)
        {
            coordsToAlter.x+=inc;
            coordsToCheckAgainst.x = 0;
        }

        if (coordsToCheckAgainst.y < 0)
        {
            coordsToAlter.y-=inc;
            coordsToCheckAgainst.y=WorldChunkManager.ChunkSize-1;
        }
        else if (coordsToCheckAgainst.y >= WorldChunkManager.ChunkSize)
        {
            coordsToAlter.y+=inc;
            coordsToCheckAgainst.y = 0;

        }
    }

    bool IsChunkOutOfBounds(Vector2Int coords)
    {
        if(coords.x>=WorldChunkManager.ChunkSize||coords.y>=WorldChunkManager.ChunkSize||coords.x<0||coords.y<0) return true;

        return false;
    }


}
