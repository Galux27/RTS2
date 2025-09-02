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

    public virtual void GenerateBlendDataForChunk(WorldChunk chunk,WorldTileBlend blend)
    {

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
        if (toBlend.Direction.x != 0 && toBlend.Direction.y != 0)
        {

        }
        else
        {
            if (toBlend.Direction.x > 0)
            {
                yMin = 0;
                yMax = batch.Chunks.GetLength(1)-1;
                xMin = batch.Chunks.GetLength(0) - 1;
                xMax = xMin;
            }else if (toBlend.Direction.x < 0)
            {
                yMin = 0;
                yMax = batch.Chunks.GetLength(1) - 1;
                xMin = 0;
                xMax = xMin;
            }else if (toBlend.Direction.y > 0)
            {
                yMin = batch.Chunks.GetLength(1) - 1;
                yMax = yMin;
                xMin = 0;
                xMax = batch.Chunks.GetLength(0) - 1;

            }else if (toBlend.Direction.y < 0)
            {
                yMin = batch.Chunks.GetLength(1) - 1;
                yMax = yMin;
                xMin = 0;
                xMax = batch.Chunks.GetLength(0) - 1;
            }
        }

        for(int x=xMin;x<=xMax; x++)
        {
            for(int y=yMin;y<=yMax; y++)
            {
                GenerateBlendDataForChunk(batch.Chunks[x,y],toBlend);
            }
        }
    }

    public override void GenerateBlendDataForChunk(WorldChunk chunk, WorldTileBlend toBlend)
    {

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
                yMin = chunk.ChunkTiles.GetLength(1) - 1;
                yMax = yMin;
                xMin = 0;
                xMax = chunk.ChunkTiles.GetLength(0) - 1;
            }
        if (useX)
        {
            int xStart = 0;
            int rand = 0;
            for(int y = yMin; y< yMax; y++)
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
                    if (positive)
                    {
                        for (int x = xStart; x < chunk.ChunkTiles.GetLength(0); x++)
                        {
                            //set tiles to be blend
                        }
                    }
                    else
                    {
                        for (int x = xStart; x > 0; x--)
                        {
                            //set tiles to be blend
                        }
                    }
                }
            }
        else
        {
            int yStart = 0;
            int rand = 0;

            for (int x = xMin; x < xMax; x++)
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
                if (positive)
                {
                    for (int y = yStart; y < chunk.ChunkTiles.GetLength(1); y++)
                    {
                        //set tiles to be blend
                    }
                }
                else
                {
                    for (int y = yStart; y > 0; y--)
                    {
                        //set tiles to be blend
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
