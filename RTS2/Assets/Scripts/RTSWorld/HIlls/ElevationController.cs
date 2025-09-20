using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElevationController : MonoBehaviour
{
    static ElevationController instance;
    public static ElevationController Instance 
    { 
        get 
        { 
            if(instance== null)
            {
                instance=FindObjectOfType<ElevationController>();   
            }
            return instance; 
        } 
    }

    public HillTileStore Tiles;

}
[System.Serializable]
public class HillTileStore
{
    public Tile Above,Below,Left,Right,TopLeftOuter,TopRightOuter,BottomLeftOuter, BottomRightOuter,TopLeftInner, TopRightInner, BottomLeftInner, BottomRightInner;

    public Tile GetTile(ElevationTileType type)
    {
        switch (type) 
        {
            case ElevationTileType.None: return null;
            case ElevationTileType.Below: return Below;
            case ElevationTileType.Left: return Left;
            case ElevationTileType.Right: return Right;
            case ElevationTileType.Above: return Above;
            case ElevationTileType.TopLeftOuter: return TopLeftOuter;
            case ElevationTileType.TopRightOuter: return TopRightOuter;
            case ElevationTileType.BottomLeftOuter: return BottomLeftOuter;
            case ElevationTileType.BottomRightOuter: return BottomRightOuter;
            case ElevationTileType.BottomLeftInner: return BottomLeftInner;
            case ElevationTileType.BottomRightInner: return BottomRightInner;
            case ElevationTileType.TopLeftInner: return TopLeftInner;
            case ElevationTileType.TopRightInner: return TopRightInner;

        }
        return null;
    }
}

public enum ElevationTileType
{
    None,
    Above,
    Below,
    Left,
    Right,
    TopLeftOuter,
    TopRightOuter,
    BottomLeftOuter, 
    BottomRightOuter,
    TopLeftInner,
    TopRightInner,
    BottomLeftInner,
    BottomRightInner
}


public class ElevationTile
{
    public ElevationTileType Tile;
    public Vector3Int coords;
    float Elevation = 0f;
    public ElevationTile(Vector3Int coords, float elevation = 0f)
    {
        this.coords = coords;
        Tile = ElevationTileType.None;
        Elevation = elevation;
    }

    public void SetTile(ElevationTileType tile)
    {
        Tile = tile;
       
    }

    public void SetElevation(float elevation)
    {
        Elevation = elevation;
    }
    public float GetElevation()
    {
        return Elevation;
    }
    public void Render()
    {
       
        WorldRenderer.Instance.HillTilemap.SetTile(coords, ElevationController.Instance.Tiles.GetTile(Tile));
    }

    public void Cleanup()
    {
        WorldRenderer.Instance.HillTilemap.SetTile(coords, null);
    }

    public void CalculateElevation(WorldChunk chunk,int x,int y,WorldChunkBatch batch)
    {
        float elevation = Mathf.Round(GetElevation());
        float aboveElevation = GetElevation();
      // float belowElevation =  GetElevation();
       // float leftElevation = GetElevation();
        float rightElevation = GetElevation();




        if (x < chunk.ChunkTiles.GetLength(0) - 1)
        {
            rightElevation = chunk.ChunkTiles[x + 1, y].Elevation.GetElevation();
        }
        else if (chunk.LocalXCoord < WorldChunkManager.ChunksPerBatch - 1 && x == chunk.ChunkTiles.GetLength(0)-1)
        {
            rightElevation = batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.GetElevation();
        }
        else if (chunk.LocalXCoord == WorldChunkManager.ChunksPerBatch - 1 && x == chunk.ChunkTiles.GetLength(0)-1)
        {
            OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords + Vector2Int.right);
            rightElevation = overworldTile.Elevation;
        }

        if (y < chunk.ChunkTiles.GetLength(1) - 1)
        {
           aboveElevation = chunk.ChunkTiles[x, y + 1].Elevation.GetElevation();
        }
        else if (chunk.LocalYCoord < WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1)-1)
        {
            aboveElevation = batch.Chunks[chunk.LocalYCoord , chunk.LocalYCoord + 1].ChunkTiles[x, 0].Elevation.GetElevation();
        }
        else if (chunk.LocalYCoord == WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1)-1)
        {
            OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords + Vector2Int.up);
            aboveElevation = overworldTile.Elevation;
        }
    

        float AboveDiff = (elevation - Mathf.Round(aboveElevation));
        float RightDiff = (elevation - Mathf.Round(rightElevation));
        if ( RightDiff + AboveDiff  != 0)
        {
            Debug.Log("Tile Elevation: " + coords.ToString() + "," + GetElevation() + " diff " + aboveElevation + "," + rightElevation);
        }

        if (AboveDiff > 0 )
        {
            SetTile(ElevationTileType.Above);
        }else if (AboveDiff < 0f)
        {
            SetTile(ElevationTileType.Below);

        }
        else if (RightDiff > 0)
        {
            SetTile(ElevationTileType.Right);
        }
        else if (RightDiff > 0f)
        {
            SetTile(ElevationTileType.Left);
        }
        else
        {
            SetTile(ElevationTileType.None);
        }
        //else if(BelowDiff>0)
        //{
        //    SetTile(ElevationTileType.Below);
        //}else if(LeftDiff > 0)
        //{
        //    SetTile(ElevationTileType.Left);
        //}


    }
}