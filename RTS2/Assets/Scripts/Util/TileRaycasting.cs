using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileRaycasting 
{
    public static List<WorldTileContents> AllFilter = new List<WorldTileContents>() { WorldTileContents.EnvObject, WorldTileContents.Wall, WorldTileContents.Door };
    public static List<WorldTileContents> WallDoorFilter = new List<WorldTileContents>() {  WorldTileContents.Wall, WorldTileContents.Door };
    public static List<WorldTileContents> NothingFilter = new List<WorldTileContents>() {  };

}

public class TileRaycast
{
    public WorldTile currentTile;
    public Vector2 increment;
    Vector2Int batch = new Vector2Int(), chunk = new Vector2Int(), coords = new Vector2Int();
    Vector2 CoordsAsVector;
    Vector2Int roundedCoords = new Vector2Int();
    public List<WorldTile> TilesHit = new List<WorldTile>();
    int maxIterations = 0;
    Vector3 startPosCache, endPosCache;
    public TileRaycast(Vector3 startPos, Vector3 endPos)
    {
        SetFilter(TileRaycasting.AllFilter);
        InitRaycast(startPos, endPos);

    }
    int filterLength = 0;
    public void SetFilter(List<WorldTileContents> Filter)
    {
        this.Filter = Filter;
        filterLength = Filter.Count;
    }

    public void LineOfTilesToTarget(Vector3 startPos,Vector3 endPos)
    {
        List<WorldTileContents> Filter = this.Filter;
        SetFilter(TileRaycasting.NothingFilter);

        InitRaycast(startPos, endPos);
        PerformRaycast();
        SetFilter(Filter);

    }

    public void InitRaycast(Vector3 startPos, Vector3 endPos)
    {
        startPosCache = startPos; 
        endPosCache = endPos;
        currentTile = Pathfinding.GetTileFromPosition(startPos);
        batch = currentTile.Batch;
        chunk = currentTile.Chunk;
        coords = currentTile.Local;
        TilesHit.Add(currentTile);
        increment = endPos - startPos;
        increment = increment.normalized;
        CoordsAsVector = new Vector2(coords.x, coords.y);
        maxIterations = Mathf.RoundToInt(Vector3.Distance(startPos, endPos))*8;
    }

    public bool DoesRaycastNeedReinitializing(Vector3 startPos, Vector3 endPos)
    {
        if (startPosCache != startPos || endPosCache != endPos)
        {
            return true;
        }
        return false;
    }


    public void RaycastCheck(Vector3 startPos, Vector3 endPos)
    {
        if (DoesRaycastNeedReinitializing(startPos, endPos))
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
        while (maxIterations > 0&& !DidRaycastHitEnd(endPosCache))
        {
            ProgressRaycast();
            maxIterations--;
            if (!IsLastTileValid(GetFurthestTile()))
            {
                break;
            }
        }
    }

  

    Vector2 PathcastHorizontal()
    {
        if (endPosCache.x > GetFurthestTilePos().x)
        {
            return Vector2.right;
        }
        //else if (endPosCache.x == GetFurthestTilePos().x)
        //{
        //    return Vector2.zero;
        //}
        else
        {
            return Vector2.left;
        }
    }

    Vector2 PathcastVertical()
    {
        if(endPosCache.y> GetFurthestTilePos().y)
        {
            return Vector2.up;
        }
        //else if (endPosCache.y == GetFurthestTilePos().y)
        //{
        //    return Vector2.zero;
        //}
        else
        {
            return Vector2.down;
        }
    }
 
    bool DidGetCloser()
    {
        return Vector3.Distance(CoordsAsVector, endPosCache) < Vector3.Distance(GetSecondFurthestTilePos(), endPosCache);
    }

    Vector2 HorPathcastDir, VerPathcastDir;
    public void PerformPathCast()
    {
        LastPathcastIncrement = increment;
        List<Vector2> ValidDirecitons = new List<Vector2>();
        while (maxIterations > 0 && !DidRaycastHitEnd(endPosCache))
        {
            HorPathcastDir = PathcastHorizontal();
            VerPathcastDir = PathcastVertical();
            maxIterations--;
             ProgressPathcast(increment.normalized/2);
            if (IsLastTileValid(GetFurthestTile()) == false || !DidGetCloser())
            {
                ReversePathcastStep();
                ProgressPathcast(HorPathcastDir.normalized/2);
                if (IsLastTileValid(GetFurthestTile()) == false)
                {
                    ReversePathcastStep();
                    ProgressPathcast(VerPathcastDir.normalized/2);
                    if (IsLastTileValid(GetFurthestTile()) == false)
                    { 
                        return;
                    }
                }
            }
            
          
        }
    }

    public bool IsValid()
    {
        return TilesHit.Count > 0;
    }
    public Vector3 GetSecondFurthestTilePos()
    {
        if (TilesHit.Count <= 1)
        {
            return startPosCache;
        }
        return TilesHit[TilesHit.Count - 2].WorldPos();
    }
    public Vector3 GetFurthestTilePos()
    {
        if (TilesHit.Count == 0)
        {
            return startPosCache;
        }
        return TilesHit[TilesHit.Count - 1].WorldPos();
    }
    public WorldTile GetFurthestTile()
    {
        if(TilesHit.Count == 0)
        {
            return null;
        }
        return TilesHit[TilesHit.Count - 1];
    }


    List<WorldTileContents> Filter;
    public bool IsLastTileValid(WorldTile tile)
    {
        if (tile == null)
        {
            return true;
        }
        if (Filter != null)
        {
            for(int x = 0; x < filterLength; x++)
            {
                if (tile.ContainsContents(Filter[x]))
                {
                    return false;
                }
            }
        }
        //check for world tile
        //if (tile.ContainsContents(WorldTileContents.EnvObject)|| tile.ContainsContents(WorldTileContents.Wall)|| tile.ContainsContents(WorldTileContents.Door))
        //{
        //    return false;
        //}
        return true;
    }

    Vector2 GetPathcastOffset()
    {
        return (endPosCache - GetFurthestTilePos()).normalized;
    }

    void ReversePathcastStep()
    {
        CoordsAsVector -= increment;
        roundedCoords.x = Mathf.FloorToInt(CoordsAsVector.x);
        roundedCoords.y = Mathf.FloorToInt(CoordsAsVector.y);
        if (TilesHit.Count == 0)
        {
            return;
        }
        TilesHit.RemoveAt(TilesHit.Count - 1);
    }

    WorldTile pathCastTile = null;
    Vector2 LastPathcastIncrement;
    public void ProgressPathcast(Vector2 increment)
    {
        CoordsAsVector += increment;

        roundedCoords.x = Mathf.FloorToInt(CoordsAsVector.x);
        roundedCoords.y = Mathf.FloorToInt(CoordsAsVector.y);
        if(roundedCoords.x!=coords.x && roundedCoords.y!=coords.y)
        {
            if (Random.Range(0, 100) < 50)
            {
                roundedCoords.x = coords.x;
                CoordsAsVector.x -= increment.x;
            }
            else
            {
                roundedCoords.y = coords.y;
                CoordsAsVector.y -= increment.y;
            }
        }
        if (roundedCoords != coords)
        {
            coords = roundedCoords;
            if (IsChunkOutOfBounds(coords))
            {
                IncrementCoords(ref coords, ref chunk, ref CoordsAsVector, 1);
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
