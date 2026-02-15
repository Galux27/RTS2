using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRenderer
{
    public Texture2D Texture = new Texture2D(WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize,TextureFormat.RGBA32,false);

    protected Color[,] Colours =new Color[WorldChunkManager.ChunkBatchSize, WorldChunkManager.ChunkBatchSize];

    public virtual void RefreshData()
    {

    }

    public virtual void RefreshTexture(Color[,] prevColours)
    {
        
    }

    protected void ClearColours()
    {
        for(int x=0;x< WorldChunkManager.ChunkBatchSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkBatchSize; y++)
            {
                Colours[x, y] = Color.clear;
            }
        }
    }


}
