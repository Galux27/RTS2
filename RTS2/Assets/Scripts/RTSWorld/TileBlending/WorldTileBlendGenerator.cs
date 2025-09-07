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

    public virtual void GenerateBlendDataForChunk(WorldChunk chunk,WorldTileBlend blend, out int endPoint, int StartOverride = -1)
    {
        endPoint = -1;
    }

    public virtual bool ShouldUse(WorldChunkBatch batch)
    {
        return false;
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
        if (toBlend.Direction.x != 0 && toBlend.Direction.y != 0)
        {

        }
        else
        {
            Debug.Log("Generating blend data for " + batch.coords + "/" + batch.OverworldCoords + "/" + toBlend.BlendType.ToSafeString() + " dir " + toBlend.Direction);
            
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
        WorldTileBlendCoordData data=null;
        if (Generate)
        {
            if (yMin == yMax)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    GenerateBlendDataForChunk(batch.Chunks[x, yMin], toBlend, out LastStart, PrevStart);
                    data = batch.Chunks[x, yMin].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                    PrevStart = LastStart;
                }
            }
            else if (xMin == xMax)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    GenerateBlendDataForChunk(batch.Chunks[xMin, y], toBlend, out LastStart, PrevStart);
                    data = batch.Chunks[xMin, y].TileBlends[TypeIGenerate()].GetBlendData(toBlend.Direction);
                    PrevStart = LastStart;
                }
            }

            
        }
    }

    public override void GenerateBlendDataForChunk(WorldChunk chunk, WorldTileBlend toBlend, out int endPoint, int StartOverride=-1)
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
                rand = Random.Range(0, chunk.ChunkTiles.GetLength(0));
                if (rand > xStart)
                {
                    xStart++;
                }
                else
                {
                    xStart--;
                }
                xStart=Mathf.Clamp(xStart,0,chunk.ChunkTiles.GetLength(0));
                if (StartOverride > -1)
                {
                    xStart = StartOverride;
                    StartOverride = -1;
                }
                endPoint = xStart;
                chunk.AddTileBlends(toBlend.Direction, new Vector2Int(xStart, y), toBlend.BlendType, xStart, false);
                float waterLevel = 0f;
                    if (positive)
                    {
                        for (int x = xStart; x < chunk.ChunkTiles.GetLength(0); x++)
                        {
                            //set tiles to be blend
                            if (x == xStart)
                            {
                                chunk.UpdateTile(x, y, "Sand");
                            }
                            else
                            {
                            waterLevel = Mathf.Lerp(0,5, Mathf.InverseLerp(xStart, chunk.ChunkTiles.GetLength(0), x));
                                chunk.UpdateWaterLevel(x,y, waterLevel);
                            }
                        }
                    }
                    else
                    {
                        for (int x = xStart; x >= 0; x--)
                        {
                        //set tiles to be blend
                        if (x == xStart)
                        {
                            chunk.UpdateTile(x, y, "Sand");
                        }
                        else
                        {
                            waterLevel = Mathf.Lerp(0, 5, Mathf.InverseLerp(xStart, 0, x));
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
                rand = Random.Range(0, chunk.ChunkTiles.GetLength(0));
                if (rand > yStart)
                {
                    yStart++;
                }
                else
                {
                    yStart--;
                }
                yStart = Mathf.Clamp(yStart, 0, chunk.ChunkTiles.GetLength(0));

                if (StartOverride > -1)
                {
                    yStart = StartOverride;
                    StartOverride = -1;
                }
                endPoint = yStart;

                chunk.AddTileBlends(toBlend.Direction, new Vector2Int(x, yStart), toBlend.BlendType, yStart, true);

                float waterLevel = 0f;
                if (positive)
                {
                    for (int y = yStart; y < chunk.ChunkTiles.GetLength(1); y++)
                    {
                        //set tiles to be blend
                        if (y == yStart)
                        {
                            chunk.UpdateTile(x, y, "Sand");
                        }
                        else
                        {
                            waterLevel = Mathf.Lerp(0, 5, Mathf.InverseLerp(yStart, chunk.ChunkTiles.GetLength(1), y));
                            chunk.UpdateWaterLevel(x, y, waterLevel);
                        }
                    }
                }
                else
                {
                    for (int y = yStart; y >= 0; y--)
                    {
                        //set tiles to be blend
                        if (y == yStart)
                        {
                            chunk.UpdateTile(x, y, "Sand");
                        }
                        else
                        {
                            waterLevel = Mathf.Lerp(0, 5, Mathf.InverseLerp(yStart, 0, y));
                            chunk.UpdateWaterLevel(x, y, waterLevel);
                        }
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
