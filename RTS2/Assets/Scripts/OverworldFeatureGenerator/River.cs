using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureRiver : OverworldFeatureToWorldConverter
{
    public OverworldFeature myFeature;
    public string Key;
    public int width;
    public FeatureRiver(OverworldFeature toapply, string key, int width)
    {
        myFeature = toapply;
        Key = key;
        this.width = width;
    }

    int HalfWidth()
    {
        return Mathf.Max(1, width / 2);
    }
    const float MinRiverWidth=2,MaxRiverWidth=32;
    int CalculateRiverWidth(Vector2Int riverData)
    {
        return Mathf.RoundToInt( Mathf.Lerp(MinRiverWidth, MaxRiverWidth, Mathf.InverseLerp(0, riverData.y, riverData.x)));
    }


    public override void GenerateFeature(WorldChunkBatch toGenerateIn)
    {
        OverworldTile myTile = OverworldGenerator.Instance.OverworldTiles[toGenerateIn.OverworldCoords.x,toGenerateIn.OverworldCoords.y];
        width = CalculateRiverWidth(myTile.RiverPoint);
        DrunkWandererResult result = DrunkWandererController.Instance.GetRiverData(toGenerateIn);
        Vector3 pos = new Vector3();
        int tileUpdateCount = 0;
        Vector2Int offset = toGenerateIn.coords - result.BottomCorner;
        for(int x = 0; x < WorldChunkManager.ChunkBatchSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkBatchSize; y++)
            {
                if (result.Data[x+offset.x, y+offset.y])
                {
                    pos.x = x + toGenerateIn.coords.x ;
                    pos.y = y + toGenerateIn.coords.y ;
                    tileUpdateCount += UpdateTile( pos);
                   
                }
            }
        }
        //need to add code to convert the new river method into river objects so they can be used in the city generation process
        Debug.Log("River Gen: tiles updated in " + toGenerateIn.coords + " " + tileUpdateCount);

        toGenerateIn.RefreshGroundTiles();
    }
    WorldTile toEdit;
    int UpdateTile(WorldChunkBatch toGenerateIn, Vector3 pos, string type)
    {
       toEdit = WorldTileHelpers.GetTileNearExisting(toEdit, toGenerateIn, pos);
        if (toEdit != null)
        {
            toEdit.UpdateWaterLevel(10f);
            toEdit.CanPutDecorationsOn = false;
            return 1;
        }
        return 0;
    }
    Vector2Int batch, chunk,tile;
    int UpdateTile(Vector3 pos)
    {
        try
        {
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(pos.x, pos.y, out batch, out chunk, out tile);
            WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[tile.x, tile.y].UpdateWaterLevel(10);
            return 1;

        }
        catch
        {
            Debug.LogError("River Gen: error setting tile at " + pos + " result " + batch + "," + chunk + "," + tile);
            return 0;
        }
    }

    public override OverworldFeature GetFeatureIGenerate()
    {
        return myFeature;
    }
}

public class RiverData
{
    public Vector2Int StartPos, EndPos;
    public Vector2Int LeftStart,RightStart,LeftEnd,RightEnd;
    public int Width;
    public Bounds MyBounds;
    public RiverData(Vector2Int start,Vector2Int end,int width)
    {
        StartPos = start;
        EndPos = end;
        Width = width;
        float halfWidth = width / 2f;
        Vector2 perp = end - start;
        perp = Vector2.Perpendicular(perp).normalized*halfWidth;
        Vector2 LeftStart = StartPos - (perp);
        Vector2 RightStart = StartPos + (perp);
        Vector2 LeftEnd = EndPos - (perp);
        Vector2 RightEnd = EndPos + (perp);
        this.LeftStart = new Vector2Int(Mathf.RoundToInt(LeftStart.x), Mathf.RoundToInt(LeftStart.y));
        this.RightStart = new Vector2Int(Mathf.RoundToInt(RightStart.x), Mathf.RoundToInt(RightStart.y));
        this.LeftEnd = new Vector2Int(Mathf.RoundToInt(LeftEnd.x), Mathf.RoundToInt(LeftEnd.y));
        this.RightEnd = new Vector2Int(Mathf.RoundToInt(RightEnd.x), Mathf.RoundToInt(RightEnd.y));

        MyBounds = new Bounds(Vector2.Lerp(start, end, .5f), Vector3.one) ;
        MyBounds.Encapsulate(LeftStart);
        MyBounds.Encapsulate(RightStart);
        MyBounds.Encapsulate(LeftEnd);
        MyBounds.Encapsulate(RightEnd);

    }

}
