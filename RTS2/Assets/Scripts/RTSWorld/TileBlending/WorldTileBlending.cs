using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldTileBlending
{

    public static void OnWorldChunkBatchGenerated(WorldChunkBatch batch)
    {
        OverworldTile myTile = OverworldGenerator.Instance.OverworldTiles[batch.coords.x,batch.coords.y];
        List<OverworldTile> neighbours = OverworldGenerator.Instance.GetNeighbours(batch.OverworldCoords,true);
        for(int x=0;x<neighbours.Count;x++)
        {
            if (neighbours[x].Features.Contains(OverworldFeature.Mountain) && myTile.Features.Contains(OverworldFeature.Mountain) == false)
            {
                batch.AddWorldBlend(new WorldTileBlend(GetDirectionBetweenOverworldTiles(neighbours[x], myTile), WorldTileBlendType.LandToMountain));
            }

            if (neighbours[x].Features.Contains(OverworldFeature.LargeWaterBody) && myTile.Features.Contains(OverworldFeature.LargeWaterBody) == false)
            {
                batch.AddWorldBlend(new WorldTileBlend(GetDirectionBetweenOverworldTiles(neighbours[x], myTile), WorldTileBlendType.LandToWater));
            }
        }
    }

    static Vector2Int GetDirectionBetweenOverworldTiles(OverworldTile dest,OverworldTile cur)
    {
        return new Vector2Int(dest.X-cur.X, dest.Y-cur.Y);
    }

}

/// <summary>
/// Stores data on the tiles on the edge of a chunk used for blending so they can be lined up with adjacent chunk
/// </summary>
public class WorldTileBlendCoordData
{
    public Vector2Int EdgeOn;
    public List<Vector2Int> EdgeTiles;
}

/// <summary>
/// Data on the feature the given chunk needs to 
/// </summary>
public class WorldTileBlend
{
    public Vector2Int Direction;
    public WorldTileBlendType BlendType;
    public WorldTileBlend(Vector2Int direction, WorldTileBlendType blendType)
    {
        Direction = direction;
        BlendType = blendType;
    }
}

public enum WorldTileBlendType
{
    None,
    LandToWater,
    LandToMountain
}

