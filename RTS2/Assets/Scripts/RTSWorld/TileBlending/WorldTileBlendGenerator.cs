using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldTileBlendGenerator
{
   public virtual void GenerateBlend(WorldChunkBatch batch)
    {

    }


   public virtual void GenerateBlendData(WorldTileBlend toBlend,WorldChunkBatch batch)
    {

    }

    public virtual WorldTileBlendType TypeIGenerate()
    {
        return WorldTileBlendType.None;
    }

    public virtual void GenerateBlendDataForChunk(WorldChunk chunk,WorldTileBlend blend, out int endPoint, int StartOverride = -1, int forceEnd = -1)
    {
        endPoint = -1;
    }

    public virtual void GenerateBlendDataForCorner(WorldChunk chunk, int xMin,int xMax,int yMin,int yMax,Vector2Int corner,Vector2Int direction)
    {

    }

    public virtual bool ShouldUse(WorldChunkBatch batch)
    {
        return false;
    }
}

public class LandToMountainGenerator : WorldTileBlendGenerator
{
    public override WorldTileBlendType TypeIGenerate()
    {
        return WorldTileBlendType.LandToMountain;
    }
    public override void GenerateBlend(WorldChunkBatch batch)
    {
        for (int x = 0; x < batch.BlendList.Count; x++)
        {
            if (batch.BlendList[x].BlendType == TypeIGenerate())
            {
                GenerateBlendData(batch.BlendList[x], batch);
            }
        }
    }
    public override void GenerateBlendData(WorldTileBlend toBlend, WorldChunkBatch batch)
    {
        int output = 0;
        for(int x = 0; x < batch.Chunks.GetLength(0); x++)
        {
            for(int y = 0; y < batch.Chunks.GetLength(1); y++)
            {
                GenerateBlendDataForChunk(batch.Chunks[x, y], toBlend, out output);
            }
        }
    }
    public override void GenerateBlendDataForChunk(WorldChunk chunk, WorldTileBlend blend, out int endPoint, int StartOverride = -1, int forceEnd = -1)
    {
        endPoint = 0;
        int rand = 0;

        Vector2Int target = Vector2Int.zero;
        float lowX = 0f,lowY=0f,highX=0f,highY=0f;
        if (blend.Direction.x > 0)
        {
            target.x = 15;
            lowX = 0;
            highX = target.x;
        }else if (blend.Direction.x < 0)
        {
            target.x = 0;
            highX = 0;
            lowX = 15;
        }

        if (blend.Direction.y > 0)
        {
            target.y = 15;
            lowY = 0;
            highY = target.y;
        }
        else if (blend.Direction.y < 0)
        {
            target.y = 0;
            highY = 0;
            lowY = 15;
        }




       int oddsX=0,oddsY=0;
        oddsX = (int)Mathf.Lerp(0f, 70f, Mathf.InverseLerp(lowX, highX, chunk.LocalXCoord));
        oddsY = (int)Mathf.Lerp(0f, 70f, Mathf.InverseLerp(lowY, highY, chunk.LocalYCoord));
        string type = "Mountain";
        uint ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(type);
        for (int x=0;x<chunk.ChunkTiles.GetLength(0); x++)
        {
            for(int y = 0; y < chunk.ChunkTiles.GetLength(1); y++)
            {
                rand = (oddsX + oddsY) / 2;
                if (Random.Range(0, 100) < rand)
                {
                    chunk.UpdateTile(x, y, type, ID) ;
                }
               
            }
        }
    }
}


public class LandToWaterBlendGenerator : WorldTileBlendGenerator
{
    public override void GenerateBlend(WorldChunkBatch batch)
    {
        for(int x = 0; x < batch.BlendList.Count; x++)
        {
            if (batch.BlendList[x].BlendType == TypeIGenerate())
            {
                GenerateBlendData(batch.BlendList[x], batch);
            }
        }
    }

    public override void GenerateBlendData(WorldTileBlend toBlend, WorldChunkBatch batch)
    {
        int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
        bool Generate = false;
        Vector2Int cornerCoords = new Vector2Int();
        Vector2Int Dir=new Vector2Int();
        if (toBlend.Direction.x != 0 && toBlend.Direction.y != 0)
        {
            if (toBlend.Direction.x < 0)
            {
                if (toBlend.Direction.y < 0)
                {
                    //bottom left
                    yMin = -1;
                    xMin = -1;
                    Vector2Int coords = batch.coords + new Vector2Int(0, -WorldChunkManager.ChunkBatchSize);
                    WorldChunkBatch neighbor = null;
                    WorldChunk toCheck=null;
                    WorldTileBlendCoordDataStore blend = null; 
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor= WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[0, neighbor.Chunks.GetLength(1) - 1];
                        if (toCheck.TileBlends!=null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            if (blend.Data.ContainsKey(Vector2Int.left))
                            {
                                xMax = blend.Data[Vector2Int.left].HighEdgeStart;
                            }
                            else
                            {
                                xMax = 8;
                            }
                        }
                        else
                        {
                            xMax = 8;

                        }
                    }
                    else
                    {
                        xMax = 8;
                    }

                    coords = batch.coords + new Vector2Int(-WorldChunkManager.ChunkBatchSize, 0);
                    neighbor = null;
                    toCheck = null;
                    blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[neighbor.Chunks.GetLength(0) - 1,0];
                        if (toCheck.TileBlends!=null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            if (blend.Data.ContainsKey(Vector2Int.down))
                            {
                                yMax = blend.Data[Vector2Int.down].HighEdgeStart;
                            }
                            else
                            {
                                yMax = 8;
                            }
                        }
                        else
                        {
                            yMax = 8;
                        }
                    }
                    else
                    {
                        yMax =8;
                    }
                    cornerCoords = new Vector2Int(0, 0);
                    Generate = true;
                }
                else
                {
                    //top right
                   yMax = WorldChunkManager.ChunkSize-1;
                    xMin = -1;
                    Vector2Int coords = batch.coords + new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
                    WorldChunkBatch neighbor = null;
                    WorldChunk toCheck = null;
                    WorldTileBlendCoordDataStore blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[0, 0];
                        if (toCheck.TileBlends != null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            if (blend.Data.ContainsKey(Vector2Int.left))
                            {
                                xMax = blend.Data[Vector2Int.left].LowEdgeStart;
                            }
                            else
                            {
                                xMax = 8;
                            }
                        }
                        else
                        {
                            xMax = 8;
                        }
                    }
                    else
                    {
                        xMax = 8;
                    }

                    coords = batch.coords + new Vector2Int(-WorldChunkManager.ChunkBatchSize, 0);
                    neighbor = null;
                    toCheck = null;
                    blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[neighbor.Chunks.GetLength(0)-1, 0];
                        if (toCheck.TileBlends != null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            yMin = blend.Data[Vector2Int.up].HighEdgeStart;
                        }
                        else
                        {
                            yMin = 8;
                        }
                    }
                    else
                    {
                        yMin = 8;
                    }
                    cornerCoords = new Vector2Int(0, WorldChunkManager.ChunkSize - 1);
                    Generate = true;
                }
            }
            else
            {
                if (toBlend.Direction.y < 0)
                {
                    //top left
                    yMin = -1;
                    xMax = WorldChunkManager.ChunkSize-1;
                    Vector2Int coords = batch.coords + new Vector2Int(0, -WorldChunkManager.ChunkBatchSize);
                    WorldChunkBatch neighbor = null;
                    WorldChunk toCheck = null;
                    WorldTileBlendCoordDataStore blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[neighbor.Chunks.GetLength(0) - 1, neighbor.Chunks.GetLength(1) - 1];
                        if (toCheck.TileBlends != null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            xMin = blend.Data[Vector2Int.right].HighEdgeStart;
                        }
                        else
                        {
                            xMin = 8;
                        }
                    }
                    else
                    {
                        xMin = 8;
                    }

                    coords = batch.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
                    neighbor = null;
                    toCheck = null;
                    blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[0, 0];
                        if (toCheck.TileBlends != null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            yMax = blend.Data[Vector2Int.down].LowEdgeStart;
                        }
                        else
                        {
                            yMax = 8;
                        }
                    }
                    else
                    {
                        yMax = 8;
                    }
                    cornerCoords = new Vector2Int(WorldChunkManager.ChunkSize - 1, 0);
                    Generate = true;
                }
                else
                {
                    //top right
                    yMax = WorldChunkManager.ChunkSize;
                    xMax = WorldChunkManager.ChunkSize;
                    Vector2Int coords = batch.coords + new Vector2Int(0, WorldChunkManager.ChunkBatchSize);
                    WorldChunkBatch neighbor = null;
                    WorldChunk toCheck = null;
                    WorldTileBlendCoordDataStore blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[neighbor.Chunks.GetLength(0) - 1, 0];
                        if (toCheck.TileBlends != null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            xMin= blend.Data[Vector2Int.right].HighEdgeStart;
                        }
                        else
                        {
                            xMin = 8;
                        }
                    }
                    else
                    {
                        xMin = 8;
                    }

                    coords = batch.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
                    neighbor = null;
                    toCheck = null;
                    blend = null;
                    if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                    {
                        neighbor = WorldChunkManager.Instance.ChunkBatches[coords];
                    }
                    if (neighbor != null)
                    {
                        toCheck = neighbor.Chunks[0, neighbor.Chunks.GetLength(1)-1];
                        if (toCheck.TileBlends != null && toCheck.TileBlends.ContainsKey(WorldTileBlendType.LandToWater))
                        {
                            blend = toCheck.TileBlends[WorldTileBlendType.LandToWater];
                            if (blend.Data.ContainsKey(Vector2Int.up))
                            {
                                yMin = blend.Data[Vector2Int.up].LowEdgeStart;
                            }
                        }
                        else
                        {
                            yMin = 8;
                        }
                    }
                    else
                    {
                        yMin = 8;
                    }
                    cornerCoords = new Vector2Int(WorldChunkManager.ChunkSize-1, WorldChunkManager.ChunkSize - 1);
                    Dir = new Vector2Int(-1, 1);
                    Generate = true;
                }
            }
        }
        else
        {            
            if (toBlend.Direction.x > 0)
            {
                yMin = 0;
                yMax = batch.Chunks.GetLength(1)-1;
                xMin = batch.Chunks.GetLength(0) - 1;
                xMax = xMin;
                Generate = true;

            }
            else if (toBlend.Direction.x < 0)
            {
                yMin = 0;
                yMax = batch.Chunks.GetLength(1) - 1;
                xMin = 0;
                xMax = xMin;
                Generate = true;

            }
            else if (toBlend.Direction.y > 0)
            {
                yMin = batch.Chunks.GetLength(1) - 1;
                yMax = yMin;
                xMin = 0;
                xMax = batch.Chunks.GetLength(0) - 1;
                Generate = true;

            }
            else if (toBlend.Direction.y < 0)
            {
                yMin = 0;
                yMax = yMin;
                xMin = 0;
                xMax = batch.Chunks.GetLength(0) - 1;
                Generate = true;
            }
        }
        int PrevStart = -1;
        int LastStart = -1;
        int forceEnd = -1;
        WorldTileBlendCoordData data=null;
           
        
        if (Generate)
        {
            if (yMin == yMax)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (x == xMax)
                    {
                        Vector2Int coords = batch.coords + new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
                        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords)) 
                        {
                            WorldChunkBatch adj = WorldChunkManager.Instance.ChunkBatches[coords];
                            if (adj != null)
                            {

                                if (adj.Chunks[0, yMin].TileBlends!=null && adj.Chunks[0, yMin].TileBlends.ContainsKey(TypeIGenerate()))
                                {
                                    WorldTileBlendCoordData neighbourBlend = adj.Chunks[ 0,yMin].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                                    if (neighbourBlend != null)
                                    {
                                        forceEnd = neighbourBlend.GetEdge(xMin, xMax, batch.Chunks[x, yMin].WorldCoords);
                                    }

                                }
                            }
                        }
                    }
                    else if (x == xMin)
                    {
                        Vector2Int coords = batch.coords - new Vector2Int(WorldChunkManager.ChunkBatchSize, 0);
                        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                        {
                            WorldChunkBatch adj = WorldChunkManager.Instance.ChunkBatches[coords];
                            if (adj != null)
                            {
                                if (adj.Chunks[WorldChunkManager.ChunkSize - 1, yMin].TileBlends!=null && adj.Chunks[WorldChunkManager.ChunkSize - 1,yMin].TileBlends.ContainsKey(TypeIGenerate()))
                                {
                                    WorldTileBlendCoordData neighbourBlend = adj.Chunks[WorldChunkManager.ChunkSize - 1, yMin].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                                    if (neighbourBlend != null)
                                    {
                                        PrevStart = neighbourBlend.GetEdge(xMin, xMax, batch.Chunks[x, yMin].WorldCoords);
                                    }
                                }
                            }
                        }
                    }


                    GenerateBlendDataForChunk(batch.Chunks[x, yMin], toBlend, out LastStart, PrevStart,forceEnd);                 
                }
            }
            else if (xMin == xMax)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (y == yMax)
                    {
                        Vector2Int coords = batch.coords + new Vector2Int(0,WorldChunkManager.ChunkBatchSize);
                        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                        {
                            WorldChunkBatch adj = WorldChunkManager.Instance.ChunkBatches[coords];
                            if (adj != null)
                            {
                                if (adj.Chunks[xMin, 0].TileBlends != null && adj.Chunks[xMin, 0].TileBlends.ContainsKey(TypeIGenerate()))
                                {
                                    WorldTileBlendCoordData neighbourBlend = adj.Chunks[yMin, 0].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                                    if (neighbourBlend != null)
                                    {
                                        forceEnd = neighbourBlend.GetEdge(yMin, yMax, batch.Chunks[xMin, y].WorldCoords);
                                    }
                                    }
                                }
                        }
                    }
                    else if (y == yMin)
                    {
                        Vector2Int coords = batch.coords - new Vector2Int( 0, WorldChunkManager.ChunkBatchSize);
                        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(coords))
                        {
                            WorldChunkBatch adj = WorldChunkManager.Instance.ChunkBatches[coords];
                            if (adj != null)
                            {
                                if (adj.Chunks[xMin, WorldChunkManager.ChunkSize - 1].TileBlends!=null && adj.Chunks[xMin, WorldChunkManager.ChunkSize-1].TileBlends.ContainsKey(TypeIGenerate()))
                                {
                                    WorldTileBlendCoordData neighbourBlend = adj.Chunks[xMin, WorldChunkManager.ChunkSize - 1].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                                    if (neighbourBlend != null)
                                    {
                                        PrevStart = neighbourBlend.GetEdge(xMin, xMax, batch.Chunks[xMin, y].WorldCoords);
                                    }
                                    }
                                }
                        }
                    }



                    GenerateBlendDataForChunk(batch.Chunks[xMin, y], toBlend, out LastStart, PrevStart);
                   // data = batch.Chunks[xMin, y].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                  //  PrevStart = batch.Chunks[xMin, y].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction).GetEdge(xMin, xMax, batch.Chunks[xMin, y].WorldCoords);

                    // PrevStart = LastStart;
                }
            }
            else
            {
                GenerateBlendDataForCorner(batch.Chunks[cornerCoords.x, cornerCoords.y], xMin, xMax, yMin, yMax, cornerCoords,toBlend.Direction);
            }
            
        }
    }
    const float DistFromCornerForBlend = 6f;
    public override void GenerateBlendDataForCorner(WorldChunk chunk, int xMin, int xMax, int yMin, int yMax,Vector2Int corner,Vector2Int direction)
    {
        Vector2Int coords = new Vector2Int();
        float dist = 9999f;
        string type = "Sand";
        uint ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(type);
        for (int x=0;x<chunk.ChunkTiles.GetLength(0);x++)
        {
            for (int y = 0; y < chunk.ChunkTiles.GetLength(0); y++)
            {
                coords.x = x;
                coords.y = y;
                dist = Vector2Int.Distance(coords, corner);
                if (dist < DistFromCornerForBlend)
                {
                    chunk.UpdateTile(x, y,type,ID );
                    chunk.UpdateWaterLevel(x, y, -99);
                }
            }
        }
    }

    public override void GenerateBlendDataForChunk(WorldChunk chunk, WorldTileBlend toBlend, out int endPoint, int StartOverride = -1, int forceEnd = -1)
    {
        endPoint = -1;
        //todo add data to capture the edge of the blends so that they could be lined up in the future
        int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
        bool useX = false;
        bool positive = false;
            if (toBlend.Direction.x > 0)
            {
                yMin = 0;
                yMax = chunk.ChunkTiles.GetLength(1) - 1;
                xMin = chunk.ChunkTiles.GetLength(0) - 1;
                xMax = xMin;
            positive = true;
            useX = true;
            }
            else if (toBlend.Direction.x < 0)
            {
                yMin = 0;
                yMax = chunk.ChunkTiles.GetLength(1) - 1;
                xMin = 0;
                xMax = xMin;
            useX = true;
            }
            else if (toBlend.Direction.y > 0)
            {
                yMin = chunk.ChunkTiles.GetLength(1) - 1;
                yMax = yMin;
                xMin = 0;
                xMax = chunk.ChunkTiles.GetLength(0) - 1;
            positive = true;
            }
            else if (toBlend.Direction.y < 0)
            {
                yMin = 0;
                yMax = yMin;
                xMin = 0;
                xMax = chunk.ChunkTiles.GetLength(0) - 1;
            }
        if (useX)
        {
            int xStart = 0;
            int rand = 0;
            for(int y = yMin; y<= yMax; y++)
            {
               
                xStart = Random.Range(5, 8);
                if (!positive)
                {
                    xStart = Random.Range(8, 12);
                }
                xStart =Mathf.Clamp(xStart,0,chunk.ChunkTiles.GetLength(0));
                int xEnd = chunk.ChunkTiles.GetLength(0) - Random.Range(0, 3);
                if (!positive)
                {
                    xEnd = Random.Range(0, 3);
                   
                }
              
                if (StartOverride > -1)
                {
                    xStart = StartOverride;
                    StartOverride = -1;
                }
                endPoint = xStart;
                float waterLevel = 0f;
                    if (positive)
                    {
                    string type = "Sand";
                    uint ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(type);
                    for (int x = xStart; x < chunk.ChunkTiles.GetLength(0); x++)
                        {
                            //set tiles to be blend
                            //if (x == chunk.ChunkTiles.GetLength(0)-1)
                            {
                            if (x <= xEnd)
                            {
                                chunk.UpdateTile(x, y,type,ID);
                            }
                             waterLevel =  Mathf.Lerp(0,-50, Mathf.InverseLerp(xStart, chunk.ChunkTiles.GetLength(0), x));
                                chunk.UpdateWaterLevel(x,y, waterLevel);
                            }
                        }
                    }
                    else
                    {
                    string type = "Sand";
                    uint ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(type);
                    for (int x = xStart; x >= 0; x--)
                        {
                        
                        {
                            if (x > xEnd)
                            {
                                chunk.UpdateTile(x, y, type,ID);
                            }
                            waterLevel = Mathf.Lerp(0, -50, Mathf.InverseLerp(xStart, 0, x));
                            chunk.UpdateWaterLevel(x, y, waterLevel);
                        }
                    }
                    }
                }
            }
        else
        {
            int yStart = 0;
            int rand = 0;

            for (int x = xMin; x <= xMax; x++)
            {
                yStart = Random.Range(5, 8);

                if (!positive)
                {
                    yStart = Random.Range(8, 12);
                }

                yStart = Mathf.Clamp(yStart, 0, chunk.ChunkTiles.GetLength(0));
                int yEnd = chunk.ChunkTiles.GetLength(1) - Random.Range(0, 3);
                if (!positive)
                {
                    yEnd = Random.Range(0, 3);
                   
                }

                endPoint = yStart;
                

                float waterLevel = 0f;
                if (positive)
                {
                    string type = "Sand";
                    uint ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(type);
                    for (int y = yStart; y < chunk.ChunkTiles.GetLength(1); y++)
                    {

                        if (y < yEnd)
                        {
                            chunk.UpdateTile(x, y, type, ID);

                        }

                      
                            waterLevel = Mathf.Lerp(0, -50, Mathf.InverseLerp(yStart, chunk.ChunkTiles.GetLength(1), y)); 
                            chunk.UpdateWaterLevel(x, y, waterLevel);
                        
                    }
                }
                else
                {
                    string type = "Sand";
                    uint ID = WorldRenderer.Instance.WorldTilesManager.GetTileID(type);
                    for (int y = yStart; y >= 0; y--)
                    {
                        //set tiles to be blend
                        if (y > yEnd)
                        {
                            chunk.UpdateTile(x, y, type,ID);

                        }
                        waterLevel = Mathf.Lerp(0, -50, Mathf.InverseLerp(yStart, 0, y));
                        chunk.UpdateWaterLevel(x, y, waterLevel);

                    }
                }
            }
        }
    }


    public override WorldTileBlendType TypeIGenerate()
    {
        return WorldTileBlendType.LandToWater;
    }
    public override bool ShouldUse(WorldChunkBatch batch)
    {
        for(int x=0;x<batch.BlendList.Count;x++)
        {
            if (batch.BlendList[x].BlendType == WorldTileBlendType.LandToWater)
            {
                return true;
            }
        }
        return false;
    }
}
