using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class UnitsMinimapRenderer : MinimapRenderer
{
    public WorldChunkBatch BatchImIn;
    Vector3 bottomLeftCorner = Vector3.zero,topRightCorner=Vector3.zero;
    int xc = 0, yc = 0;
    Vector2Int coords, curCoords;
    bool updating = false;
    public bool IsDataUpdateDone()
    {
        return !updating;
    }

    public void StartRefresh()
    {
        updating = true;
        ClearColours();
        Vector3 cameraPos = CameraController.Instance.transform.position;
        bottomLeftCorner = cameraPos - new Vector3(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2, 0);
        topRightCorner = cameraPos + new Vector3(WorldChunkManager.ChunkBatchSize / 2, WorldChunkManager.ChunkBatchSize / 2, 0);
        coords = new Vector2Int(Mathf.CeilToInt(cameraPos.x), Mathf.CeilToInt(cameraPos.y));
        curCoords = coords;
        xc = coords.x - WorldChunkManager.ChunkBatchSize;
        yc = coords.y - WorldChunkManager.ChunkBatchSize;
    }

    int count = 0;
    public override void RefreshData()
    {
        curCoords.x = xc;
        curCoords.y = yc;
        BatchImIn = WorldChunkManager.Instance.GetWorldChunkBatchFromPosition(curCoords);

        if (BatchImIn != null)
        {
            for (int x = 0; x < WorldChunkManager.ChunksPerBatch; x++)
            {
                for (int y = 0; y < WorldChunkManager.ChunksPerBatch; y++)
                {
                    for (int q = 0; q < BatchImIn.Chunks[x, y].UnitsInChunk.Count; q++)
                    {
                        if (BatchImIn.Chunks[x, y].UnitsInChunk[q] != null)
                        {
                            if (IsPointInRange(BatchImIn.Chunks[x, y].UnitsInChunk[q].Position()))
                            {
                                DrawUnit(BatchImIn.Chunks[x, y].UnitsInChunk[q]);
                            }
                        }
                    }
                }
            }
        }

       
        yc += WorldChunkManager.ChunkBatchSize;
        if (yc > coords.y + WorldChunkManager.ChunkBatchSize)
        {
            yc = coords.y - WorldChunkManager.ChunkBatchSize;
            xc += WorldChunkManager.ChunkBatchSize;

            if (xc > coords.x + WorldChunkManager.ChunkBatchSize)
            {

                xc = xc = coords.x - WorldChunkManager.ChunkBatchSize;
            }
        }
        count++;
        if (count >= 9)
        {
            updating = false;
            count = 0;
        }
     
    }

    public override void RefreshTexture(Color[,] prevColours)
    {
        for(int x = 0; x < WorldChunkManager.ChunkBatchSize; x++)
        {
            for (int y = 0; y < WorldChunkManager.ChunkBatchSize; y++)
            {
                if (Colours[x, y].a > 0)
                {
                    Texture.SetPixel(x, y, Colours[x, y]);
                }
                else
                {
                    Texture.SetPixel(x, y, prevColours[x, y]);
                }
            }
        }
        Texture.filterMode = FilterMode.Point;
        Texture.Apply();
    }
    const int UnitSize = 2;
    void DrawUnit(Unit u)
    {
        Color c = Color.white;
        int xp =  Mathf.FloorToInt( Mathf.Lerp(0, WorldChunkManager.ChunkBatchSize, Mathf.InverseLerp(bottomLeftCorner.x, topRightCorner.x, u.Position().x)));
        int yp = Mathf.FloorToInt(Mathf.Lerp(0, WorldChunkManager.ChunkBatchSize, Mathf.InverseLerp(bottomLeftCorner.y, topRightCorner.y, u.Position().y)));

        if (u.MyFaction.MyFactionID == FactionController.USER_FACTION)
        {
            c = Color.green;
        }else if (u.MyFaction.MyFactionID == FactionController.ZOMBIE_FACTION)
        {
            c = Color.red;
        }
        for(int x = xp - UnitSize; x < xp + UnitSize; x++)
        {
            for (int y = yp - UnitSize; y < yp + UnitSize; y++)
            {
                Colours[Mathf.Clamp(x, 0, WorldChunkManager.ChunkBatchSize - 1), Mathf.Clamp(y, 0, WorldChunkManager.ChunkBatchSize - 1)] = c;

            }
        }

    }

    bool IsPointInRange(Vector3 point)
    {
        if(point.x>=bottomLeftCorner.x && point.x <= topRightCorner.x)
        {
            if(point.y>=bottomLeftCorner.y && point.y <= topRightCorner.y)
            {
                return true;
            }
        }
        return false;
    }
}
