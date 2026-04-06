using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public WallTile ElevationWallTiles;
    public Tilemap  ElevationTilemap;
    public const bool UseElevation = false;
  

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

[System.Serializable]
public struct ElevationTile
{
    public List<ElevationTileType> DirectionsForEdge;
    public Vector3Int coords;
    public bool IsCorner, IsEdge,IsGoingUp;
    public float Elevation;
    public ElevationTile(Vector3Int coords, float elevation = 0f)
    {
        this.coords = coords;
        Elevation = elevation;
        DirectionsForEdge = new List<ElevationTileType>();
        IsEdge=false;
        IsCorner = false;
        elevation = 0f;
        isDrawn = false;
        IsGoingUp = true;
        Passible = true;
    }


    public void Init(Vector3Int coords, float elevation = 0f)
    {
        this.coords = coords;
        Elevation = elevation;
        if (DirectionsForEdge == null)
        {
            DirectionsForEdge = new List<ElevationTileType>();
        }
            IsEdge = false;
        IsCorner = false;
        elevation = 0f;
        isDrawn = false;
        IsGoingUp = true;
        Passible = true;
    }



    public void SetIsEdge(bool val,float adjHeight)
    {
        IsEdge = val;
        IsGoingUp = adjHeight > Elevation;
    }
    bool Passible;
    public bool IsPassible()
    {
        return Passible;
    }

    public void SetTileToWalkable(float elevation)
    {
        DirectionsForEdge.Clear();
        SetTile(ElevationTileType.None);
        Passible = true;
        Elevation = elevation;
    }


    public void SetTile(ElevationTileType tile)
    {
        if (!DirectionsForEdge.Contains(tile))
        {
            if (tile != ElevationTileType.None)
            {
                Passible = false;
            }
            DirectionsForEdge.Add(tile);
        }
    }
   public bool isDrawn;
    public void SetElevation(float elevation)
    {
        Elevation = elevation;

    }
    public float GetElevation()
    {
        return Elevation;
    }

    public int DirectionCount()
    {
        return DirectionsForEdge.Count;
    }

    public bool DirectionCheck()
    {
        return DirectionCount() > 0 && !IsCorner;
    }

    public bool IsCornerOrEdge(ElevationTile tile)
    {
        return IsCorner || IsEdge;
    }

    public void Reset()
    {
        
        ElevationController.Instance.ElevationTilemap.SetTile(coords,null);
    }


    /// <summary>
    /// Do one pass over the tiles to mark the ones that could be considered the edge of an elevation change
    /// Do anothe pass over these tiles to check adjacent tiles that are also an elevation change and set tiles based off this
    /// </summary>


    public void Render()
    {
        isDrawn = true;
        if ((IsEdge||IsCorner)==false)
        {
            return;
        }
        bool up = DirectionsForEdge.Contains(ElevationTileType.Above), down = DirectionsForEdge.Contains(ElevationTileType.Below),
            left = DirectionsForEdge.Contains(ElevationTileType.Left), right = DirectionsForEdge.Contains(ElevationTileType.Right);
           Tile toUse = null;

        
        if (up && down && left && right)
        {
            toUse = ElevationController.Instance.ElevationWallTiles.Cross;
        }
        else
        {
            if (up && down && left)
            {
                toUse = ElevationController.Instance.ElevationWallTiles.TopBottomLeft;
            }
            else if (up && down && right)
            {
                toUse = ElevationController.Instance.ElevationWallTiles.TopBottomRight;
            }
            else if (left && right && down)
            {
                toUse = ElevationController.Instance.ElevationWallTiles.LeftRightBelow;
            }
            else if (left && right && up)
            {
                toUse = ElevationController.Instance.ElevationWallTiles.LeftRightAbove;
            }
            else
            {
                if (left && down)
                {
                    toUse = ElevationController.Instance.ElevationWallTiles.LeftBelow;
                }
                else if (left && up)
                {
                    toUse = ElevationController.Instance.ElevationWallTiles.LeftAbove;
                }
                else if (right && down)
                {
                    toUse = ElevationController.Instance.ElevationWallTiles.RightBelow;
                }
                else if (right && up)
                {
                    toUse = ElevationController.Instance.ElevationWallTiles.RightAbove;
                }
                else if (left && right)
                {
                    toUse = ElevationController.Instance.ElevationWallTiles.LeftRight;
                }
                else if (up && down)
                {
                    toUse = ElevationController.Instance.ElevationWallTiles.UpDown;
                }
                else
                {
                    if (left)
                    {
                        toUse = ElevationController.Instance.ElevationWallTiles.Left;
                    }
                    else if (right)
                    {
                        toUse = ElevationController.Instance.ElevationWallTiles.Right;
                    }
                    else if (up)
                    {
                        toUse = ElevationController.Instance.ElevationWallTiles.Above;
                    }
                    else if (down)
                    {
                        toUse = ElevationController.Instance.ElevationWallTiles.Below;
                    }
                    else
                    {
                        return;
                        //toUse = ElevationController.Instance.ElevationWallTiles.NoNeighbours;
                    }
                }
            }
        }
        isDrawn = true;
        ElevationController.Instance.ElevationTilemap.SetTile(coords, toUse);

      
    }

    public void Cleanup()
    {
       
           
        ElevationController.Instance.ElevationTilemap.SetTile(coords, null);
            
       
    }




    public void WorkOutStartingEdges(WorldChunk chunk,int x,int y,WorldChunkBatch batch)
    {
        float elevation = Mathf.Round( GetElevation());
        float aboveElevation = GetElevation();
        float belowElevation =  GetElevation();
        float leftElevation = GetElevation();
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
            //Vector2Int newBatch = batch.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
            //WorldChunkBatch neighbour = null;
            //bool RightCorner = false;
            //if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            //{
            //    neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
            //    rightElevation = neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.GetElevation();
            //    RightCorner = neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.IsCornerOrEdge();
            //}
            //if (RightCorner)
            //{
            //    SetTile(ElevationTileType.Right);
            //    UpdateExistingNeighbour(neighbour.Chunks[0, chunk.LocalYCoord], ElevationTileType.Left, neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation);
            //    neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.SetTile(ElevationTileType.Left);
            //}
        }


        if (x > 0)
        {
            leftElevation = chunk.ChunkTiles[x - 1, y].Elevation.GetElevation();
        }
        else if (chunk.LocalXCoord > 0 && x == 0)
        {
            leftElevation = batch.Chunks[chunk.LocalXCoord - 1, chunk.LocalYCoord].ChunkTiles[chunk.ChunkTiles.GetLength(0)-1, y].Elevation.GetElevation();
        }
        else if (chunk.LocalXCoord == 0 && x == 0)
        {
            //Vector2Int newBatch = batch.coords - new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
            //WorldChunkBatch neighbour = null;
            //bool LeftCorner = false;
            //if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            //{
            //    neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
            //    leftElevation = neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord].ChunkTiles[WorldChunkManager.ChunkSize - 1, y].Elevation.GetElevation();
            //    LeftCorner = neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord].ChunkTiles[WorldChunkManager.ChunkSize - 1, y].Elevation.IsCornerOrEdge();
            //}

            //if (LeftCorner)
            //{
            //    SetTile(ElevationTileType.Left);
            //    UpdateExistingNeighbour(neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord], ElevationTileType.Right, neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord].ChunkTiles[WorldChunkManager.ChunkSize - 1, y].Elevation);
            //}
        }



        if (y < chunk.ChunkTiles.GetLength(1) - 1)
        {
           aboveElevation = chunk.ChunkTiles[x, y + 1].Elevation.GetElevation();
        }
        else if (chunk.LocalYCoord < WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1)-1)
        {
            aboveElevation = batch.Chunks[chunk.LocalXCoord , chunk.LocalYCoord + 1].ChunkTiles[x, 0].Elevation.GetElevation();
        }
        else if (chunk.LocalYCoord == WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1)-1)
        {
            //Vector2Int newBatch = batch.coords + new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
            //WorldChunkBatch neighbour = null;
            //bool AboveCorner = false;
            //if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            //{
            //    neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
            //    leftElevation = neighbour.Chunks[chunk.LocalXCoord, 0].ChunkTiles[x, 0].Elevation.GetElevation();
            //    AboveCorner = neighbour.Chunks[chunk.LocalXCoord, 0].ChunkTiles[x, 0].Elevation.IsCornerOrEdge();
            //}

            //if (AboveCorner)
            //{
            //    SetTile(ElevationTileType.Above);
            //    UpdateExistingNeighbour(neighbour.Chunks[chunk.LocalXCoord, 0], ElevationTileType.Below, neighbour.Chunks[chunk.LocalXCoord, 0].ChunkTiles[x, 0].Elevation);
            //}
        }


        if (y > 0)
        {
            belowElevation = chunk.ChunkTiles[x , y - 1].Elevation.GetElevation();
        }
        else if (chunk.LocalYCoord > 0 && y == 0)
        {
            belowElevation = batch.Chunks[chunk.LocalXCoord, chunk.LocalYCoord - 1].ChunkTiles[x,chunk.ChunkTiles.GetLength(1)-1].Elevation.GetElevation();
        }
        else if (chunk.LocalYCoord == 0 && y == 0)
        {
            //Vector2Int newBatch = batch.coords - new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
            //WorldChunkBatch neighbour = null;
            //bool BelowCorner = false;
            //if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            //{
            //    neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
            //    belowElevation = neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.GetElevation();
            //    BelowCorner = neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.IsCornerOrEdge();


            //}
            //if (BelowCorner)
            //{
            //    SetTile(ElevationTileType.Below);
            //    UpdateExistingNeighbour(neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1], ElevationTileType.Above, neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation);
            //}
        }


       

       
            if (Mathf.Round(aboveElevation) != elevation)
            {
            //SetTile(ElevationTileType.Above);
            SetIsEdge(true, aboveElevation);

            }

        if (Mathf.Round(belowElevation) != elevation)
            {
            //SetTile(ElevationTileType.Below);
            SetIsEdge(true, GetElevation());

        }

        if (Mathf.Round(rightElevation) != elevation)
            {
            //SetTile(ElevationTileType.Right);
            IsEdge = true;
            SetIsEdge(true, GetElevation());
        }

        if (Mathf.Round(leftElevation) != elevation)
        {
            //SetTile(ElevationTileType.Left);
            IsEdge = true;
            SetIsEdge(true, GetElevation());
        }
    }

    public void FinalBlend(WorldChunk chunk, int x, int y, WorldChunkBatch batch)
    {

        if (!IsEdge && !IsCorner)
        {
            return;
        }

        float elevation = Mathf.Round(GetElevation());
        float aboveElevation = GetElevation();
        float belowElevation = GetElevation();
        float leftElevation = GetElevation();
        float rightElevation = GetElevation();
        bool LeftCorner = false,RightCorner = false,AboveCorner=false,BelowCorner=false;


        if (x < chunk.ChunkTiles.GetLength(0) - 1)
        {
            rightElevation = chunk.ChunkTiles[x + 1, y].Elevation.GetElevation();
            RightCorner = chunk.ChunkTiles[x + 1, y].Elevation.IsCornerOrEdge(this);
            if ( RightCorner)
            {
                SetTile(ElevationTileType.Right);
//                UpdateExistingNeighbour(chunk,ElevationTileType.Left, chunk.ChunkTiles[x + 1, y].Elevation);
            }
        }
        else if (chunk.LocalXCoord < WorldChunkManager.ChunksPerBatch - 1 && x == chunk.ChunkTiles.GetLength(0) - 1)
        {
            rightElevation = batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.GetElevation();
            RightCorner = batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.IsCornerOrEdge(this);
            if ( RightCorner)
            {
                SetTile(ElevationTileType.Right);
                batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.SetTile(ElevationTileType.Left);
                UpdateExistingNeighbour(batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord], ElevationTileType.Left, batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord].ChunkTiles[0, y].Elevation);
            }
        }
        else if (chunk.LocalXCoord == WorldChunkManager.ChunksPerBatch - 1 && x == chunk.ChunkTiles.GetLength(0) - 1)
        {
            Vector2Int newBatch = batch.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize,0);
            WorldChunkBatch neighbour = null;
            if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            {
                neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
                rightElevation = neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.GetElevation();
                RightCorner = neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.IsCornerOrEdge(this) ;
            }
            if ( RightCorner)
            {
                SetTile(ElevationTileType.Right);
                UpdateExistingNeighbour(neighbour.Chunks[0, chunk.LocalYCoord], ElevationTileType.Left, neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation);
                neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.SetTile(ElevationTileType.Left);
                //if (neighbour.Chunks[0, chunk.LocalYCoord].IsRendered)
                //{
                //    neighbour.Chunks[0, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.Render();
                //}
            }
        }


        if (x > 0)
        {
            leftElevation = chunk.ChunkTiles[x - 1, y].Elevation.GetElevation();
            LeftCorner = chunk.ChunkTiles[x - 1, y].Elevation.IsCornerOrEdge(this);

            if ( LeftCorner)
            {
                SetTile(ElevationTileType.Left);
            }
        }
        else if (chunk.LocalXCoord > 0 && x == 0)
        {
            leftElevation = batch.Chunks[chunk.LocalXCoord - 1, chunk.LocalYCoord].ChunkTiles[chunk.ChunkTiles.GetLength(0) - 1, y].Elevation.GetElevation();
            LeftCorner = batch.Chunks[chunk.LocalXCoord - 1, chunk.LocalYCoord].ChunkTiles[chunk.ChunkTiles.GetLength(0) - 1, y].Elevation.IsCornerOrEdge(this);

            if (LeftCorner)
            {
                SetTile(ElevationTileType.Left);
                UpdateExistingNeighbour(batch.Chunks[chunk.LocalXCoord - 1, chunk.LocalYCoord], ElevationTileType.Right, batch.Chunks[chunk.LocalXCoord - 1, chunk.LocalYCoord].ChunkTiles[chunk.ChunkTiles.GetLength(0) - 1, y].Elevation);
            }
        }
        else if (chunk.LocalXCoord == 0 && x == 0)
        {
            Vector2Int newBatch = batch.coords - new Vector2Int( WorldChunkManager.ChunkBatchSize,0);
            WorldChunkBatch neighbour = null;
            if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            {
                neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
                leftElevation = neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord].ChunkTiles[WorldChunkManager.ChunkSize - 1, y].Elevation.GetElevation();
                LeftCorner = neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord].ChunkTiles[WorldChunkManager.ChunkSize - 1, y].Elevation.IsCornerOrEdge(this);
            }

            if ( LeftCorner)
            {
                SetTile(ElevationTileType.Left);            
                UpdateExistingNeighbour(neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord], ElevationTileType.Right, neighbour.Chunks[neighbour.Chunks.GetLength(0) - 1, chunk.LocalYCoord].ChunkTiles[WorldChunkManager.ChunkSize - 1, y].Elevation);
            }
        }



        if (y < chunk.ChunkTiles.GetLength(1) - 1)
        {
            aboveElevation = chunk.ChunkTiles[x, y + 1].Elevation.GetElevation();
            AboveCorner = chunk.ChunkTiles[x, y + 1].Elevation.IsCornerOrEdge(this);

            if ( AboveCorner)
            {
                SetTile(ElevationTileType.Above);
            }
        }
        else if (chunk.LocalYCoord < WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1) - 1)
        {
            aboveElevation = batch.Chunks[chunk.LocalXCoord, chunk.LocalYCoord + 1].ChunkTiles[x, 0].Elevation.GetElevation();
            AboveCorner = batch.Chunks[chunk.LocalXCoord, chunk.LocalYCoord + 1].ChunkTiles[x, 0].Elevation.IsCornerOrEdge(this);

            if (AboveCorner)
            {
                SetTile(ElevationTileType.Above);
                UpdateExistingNeighbour(batch.Chunks[chunk.LocalYCoord, chunk.LocalYCoord + 1], ElevationTileType.Below, batch.Chunks[chunk.LocalYCoord, chunk.LocalYCoord + 1].ChunkTiles[x, 0].Elevation);
            }
        }
        else if (chunk.LocalYCoord == WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1) - 1)
        {
            Vector2Int newBatch = batch.coords + new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
            WorldChunkBatch neighbour = null;
            if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            {
                neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
                leftElevation = neighbour.Chunks[chunk.LocalXCoord, 0].ChunkTiles[x, 0].Elevation.GetElevation();
                AboveCorner = neighbour.Chunks[chunk.LocalXCoord, 0].ChunkTiles[x, 0].Elevation.IsCornerOrEdge(this);
            }

            if ( AboveCorner)
            {
                SetTile(ElevationTileType.Above);
                UpdateExistingNeighbour(neighbour.Chunks[chunk.LocalXCoord, 0], ElevationTileType.Below, neighbour.Chunks[chunk.LocalXCoord, 0].ChunkTiles[x, 0].Elevation);         
            }
        }


        if (y > 0)
        {
            belowElevation = chunk.ChunkTiles[x, y - 1].Elevation.GetElevation();
            BelowCorner = chunk.ChunkTiles[x, y - 1].Elevation.IsCornerOrEdge(this);
            if ( BelowCorner)
            {
                SetTile(ElevationTileType.Below);

            }

        }
        else if (chunk.LocalYCoord > 0 && y == 0)
        {
            belowElevation = batch.Chunks[chunk.LocalXCoord, chunk.LocalYCoord - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.GetElevation();
            BelowCorner = batch.Chunks[chunk.LocalXCoord, chunk.LocalYCoord - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.IsCornerOrEdge(this);
            if ( BelowCorner)
            {
                SetTile(ElevationTileType.Below);
                UpdateExistingNeighbour(batch.Chunks[chunk.LocalXCoord , chunk.LocalYCoord - 1],ElevationTileType.Above, batch.Chunks[chunk.LocalXCoord , chunk.LocalYCoord - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation);
            }

        }
        else if (chunk.LocalYCoord == 0 && y == 0)
        {
            Vector2Int newBatch = batch.coords - new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
            WorldChunkBatch neighbour = null;
            if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(newBatch))
            {
                neighbour = WorldChunkManager.Instance.ChunkBatches[newBatch];
               belowElevation = neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.GetElevation();
                BelowCorner = neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.IsCornerOrEdge(this);
            
                
            }
            if ( BelowCorner)
            {
                SetTile(ElevationTileType.Below);
                UpdateExistingNeighbour(neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1], ElevationTileType.Above, neighbour.Chunks[chunk.LocalXCoord, neighbour.Chunks.GetLength(1) - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation);
            }
        }









       

    }

    void UpdateExistingNeighbour(WorldChunk chunkItsIn,ElevationTileType typeToAdd,ElevationTile toAddTo)
    {
        toAddTo.SetTile(typeToAdd);
        if (chunkItsIn.IsRendered)
        {
            toAddTo.Render();
        }
    }

    public void WorkOutCorners(WorldChunk chunk, int x, int y, WorldChunkBatch batch)
    {

        if (IsEdge)
        {
            return;
        }

        bool hasLeft = false;
        bool hasRight = false;
        bool hasAbove = false;
        bool hasBelow = false;




        if (x < chunk.ChunkTiles.GetLength(0) - 1)
        {
            hasRight = chunk.ChunkTiles[x + 1, y].Elevation.IsEdge;
        }
        else if (chunk.LocalXCoord < WorldChunkManager.ChunksPerBatch - 1 && x == chunk.ChunkTiles.GetLength(0) - 1)
        {
            hasRight = batch.Chunks[chunk.LocalXCoord + 1, chunk.LocalYCoord].ChunkTiles[0, y].Elevation.IsEdge;
        }
        //else if (chunk.LocalXCoord == WorldChunkManager.ChunksPerBatch - 1 && x == chunk.ChunkTiles.GetLength(0) - 1)
        //{
        //    OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords + Vector2Int.right);
        //    hasRight = overworldTile.Elevation.DirectionCount() > 0;
        //}


        if (x > 0)
        {
            hasLeft = chunk.ChunkTiles[x - 1, y].Elevation.IsEdge;
        }
        else if (chunk.LocalXCoord > 0 && x == 0)
        {
            hasLeft = batch.Chunks[chunk.LocalXCoord - 1, chunk.LocalYCoord].ChunkTiles[chunk.ChunkTiles.GetLength(0) - 1, y].Elevation.IsEdge;
        }
        //else if (chunk.LocalXCoord == 0 && x == 0)
        //{
        //    OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords + Vector2Int.left);
        //    hasLeft = overworldTile.Elevation;
        //}



        if (y < chunk.ChunkTiles.GetLength(1) - 1)
        {
            hasAbove = chunk.ChunkTiles[x, y + 1].Elevation.IsEdge;
        }
        else if (chunk.LocalYCoord < WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1) - 1)
        {
            hasAbove = batch.Chunks[chunk.LocalXCoord, chunk.LocalYCoord + 1].ChunkTiles[x, 0].Elevation.IsEdge;
        }
        //else if (chunk.LocalYCoord == WorldChunkManager.ChunksPerBatch - 1 && y == chunk.ChunkTiles.GetLength(1) - 1)
        //{
        //    OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords + Vector2Int.up);
        //    aboveElevation = overworldTile.Elevation;
        //}


        if (y > 0)
        {
           hasBelow = chunk.ChunkTiles[x, y - 1].Elevation.IsEdge;
        }
        else if (chunk.LocalYCoord > 0 && y == 0)
        {
            hasBelow = batch.Chunks[chunk.LocalXCoord , chunk.LocalYCoord - 1].ChunkTiles[x, chunk.ChunkTiles.GetLength(1) - 1].Elevation.IsEdge;
        }
        //else if (chunk.LocalYCoord == 0 && y == 0)
        //{
        //    OverworldTile overworldTile = OverworldGenerator.Instance.GetOverworldTile(batch.OverworldCoords + Vector2Int.down);
        //    belowElevation = overworldTile.Elevation;
        //}

        int edgeCount = 0;
        if (hasAbove)
        {
            edgeCount++;
        }

        if (hasBelow)
        {
            edgeCount++;
        }
        if (hasLeft)
        {
            edgeCount++;
        }
        if (hasRight)
        {
            edgeCount++;
        }
       //if (edgeCount <4)
       // {
       //     if (hasAbove)
       //     {
       //         IsCorner = true;
       //        // SetTile(ElevationTileType.Above);
       //     }

       //     if (hasBelow)
       //     {
       //         IsCorner = true;

       //       //  SetTile(ElevationTileType.Below);

       //     }

       //     if (hasLeft)
       //     {
       //         IsCorner = true;

       //       //  SetTile(ElevationTileType.Left);
       //     }

       //     if (hasRight)
       //     {
       //         IsCorner = true;

       //       //  SetTile(ElevationTileType.Right);
       //     }
       // }
        }
    }