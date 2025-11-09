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
    Vector3 startPosCache, endPosCache;
    public TileRaycast(Vector3 startPos,Vector3 endPos)
    {
       InitRaycast(startPos, endPos);
    }

    public void InitRaycast(Vector3 startPos,Vector3 endPos)
    {
        startPosCache = startPos; endPosCache=endPos;
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
        maxIterations = Mathf.RoundToInt(Vector3.Distance(startPos, endPos));
    }

    public bool DoesRaycastNeedReinitializing(Vector3 startPos, Vector3 endPos)
    {
        if (startPosCache != startPos || endPosCache != endPos)
        {
            return true;
        }
        return false;
    }


    public void RaycastCheck(Vector3 startPos,Vector3 endPos)
    {
        if(DoesRaycastNeedReinitializing(startPos, endPos))
        {
            InitRaycast(startPos, endPos);
            PerformRaycast();
        }
    }

    public bool DidRaycastHitEnd(Vector3 endPos)
    {
        return Vector3.Distance(GetFurthestTile().WorldPos(), endPos) < 1.5f;
    }

    public void PerformRaycast()
    {
        EasyStopwatch.StartStopwatch();
        while (maxIterations > 0)
        {
            ProgressRaycast();
            maxIterations--;
            if (!IsLastTileValid(GetFurthestTile()))
            {
                break;
            }
        }
        EasyStopwatch.StopStopwatch();
        Debug.Log("Tile Raycast Took " + EasyStopwatch.GetStopwatchElapsedTime());
    }

    public bool IsValid()
    {
        return TilesHit.Count > 0;
    }

    public WorldTile GetFurthestTile()
    {
        return TilesHit[TilesHit.Count - 1];
    }

    public bool IsLastTileValid(WorldTile tile)
    {
        //check for world tile
        if (tile.ContainsContents(WorldTileContents.EnvObject))
        {
            return false;
        }
        return true;
    }


    public void ProgressRaycast()
    {
        CoordsAsVector += increment;
        roundedCoords.x = Mathf.FloorToInt(CoordsAsVector.x);
        roundedCoords.y = Mathf.FloorToInt(CoordsAsVector.y);

        if (roundedCoords != coords)
        {
            coords = roundedCoords;
            if (IsChunkOutOfBounds(coords))
            {
                IncrementCoords(ref coords, ref chunk,ref CoordsAsVector,1);
                if (IsChunkOutOfBounds(chunk))
                {
                    IncrementCoords(ref chunk, ref batch, WorldChunkManager.ChunkBatchSize);
                }
               /// CoordsAsVector.x = coords.x;
              //  CoordsAsVector.y = coords.y;
            }
            TilesHit.Add(WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[coords.x, coords.y]);

        }
       
    }

    public void DrawPath()
    {
        for(int x = 0; x < TilesHit.Count-1; x++)
        {
            Debug.DrawLine(TilesHit[x].WorldPos(), TilesHit[x + 1].WorldPos(), Color.cyan);
        }
    }
    Vector2Int lastCoords = new Vector2Int();
    void IncrementCoords(ref Vector2Int coordsToCheckAgainst, ref Vector2Int coordsToAlter,ref Vector2 actualPos, int inc)
    {
        lastCoords = coordsToCheckAgainst;
        if (coordsToCheckAgainst.x < 0)
        {
            coordsToAlter.x -= inc;
            coordsToCheckAgainst.x = WorldChunkManager.ChunkSize - 1;

            
        }
        else if (coordsToCheckAgainst.x >= WorldChunkManager.ChunkSize)
        {
            coordsToAlter.x += inc;
            coordsToCheckAgainst.x = 0;
        }

        if (coordsToCheckAgainst.y < 0)
        {
            coordsToAlter.y -= inc;
            coordsToCheckAgainst.y = WorldChunkManager.ChunkSize - 1;
        }
        else if (coordsToCheckAgainst.y >= WorldChunkManager.ChunkSize)
        {
            coordsToAlter.y += inc;
            coordsToCheckAgainst.y = 0;

        }

        lastCoords =  coordsToCheckAgainst-lastCoords;
        actualPos += lastCoords;
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
