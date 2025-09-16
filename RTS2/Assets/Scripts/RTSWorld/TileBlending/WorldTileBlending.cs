using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldTileBlending
{
    static Dictionary<WorldTileBlendType,WorldTileBlendGenerator> Generators;
    public static void OnWorldChunkBatchGenerated(WorldChunkBatch batch)
    {
        if (Generators == null)
        {
            Generators = new Dictionary<WorldTileBlendType, WorldTileBlendGenerator>();
            Generators.Add(WorldTileBlendType.LandToWater,new LandToWaterBlendGenerator());
        }
        Debug.Log("Batch overworld coords " + batch.OverworldCoords);
        OverworldTile myTile = OverworldGenerator.Instance.OverworldTiles[batch.OverworldCoords.x,batch.OverworldCoords.y];
        List<OverworldTile> neighbours = OverworldGenerator.Instance.GetNeighbours(batch.OverworldCoords,true);
        for(int x=0;x<neighbours.Count;x++)
        {
            if (neighbours[x].Features.Contains(OverworldFeature.Mountain) && myTile.Features.Contains(OverworldFeature.Mountain) == false)
            {
              
                batch.AddWorldBlend(new WorldTileBlend(GetDirectionBetweenOverworldTiles(neighbours[x], myTile), WorldTileBlendType.LandToMountain));
            }

            if ( myTile.Features.Contains(OverworldFeature.LargeWaterBody) && neighbours[x].Features.Contains(OverworldFeature.LargeWaterBody) ==false)
            {
                batch.AddWorldBlend(new WorldTileBlend(GetDirectionBetweenOverworldTiles(neighbours[x], myTile), WorldTileBlendType.LandToWater));
            }
        }

       for(int x = 0; x < batch.BlendList.Count; x++)
        {
            if (Generators.ContainsKey(batch.BlendList[x].BlendType))
            {
                Generators[batch.BlendList[x].BlendType].GenerateBlendData(batch.BlendList[x], batch);
            }
        }
    }

    static Vector2Int GetDirectionBetweenOverworldTiles(OverworldTile dest,OverworldTile cur)
    {
        return new Vector2Int(dest.X-cur.X, dest.Y-cur.Y);
    }

}
/// <summary>
/// Stores instances of world tile blend coord data together with other blends of the same type
/// </summary>
public class WorldTileBlendCoordDataStore
{
    public WorldTileBlendType MyType;
    public Dictionary<Vector2Int, WorldTileBlendCoordData> Data;

    public WorldTileBlendCoordDataStore(WorldTileBlendType myType)
    {
        MyType = myType;
        Data = new Dictionary<Vector2Int, WorldTileBlendCoordData>();
    }
    public void AddBlend(Vector2Int edge, int coord, Vector2Int coords, bool isHorizontal)
    {
        if (!Data.ContainsKey(edge))
        {
            Data.Add(edge,new WorldTileBlendCoordData(edge, coord, coords, isHorizontal));
        }
        else
        {
            Data[edge].UpdateBlendData(coord, coords);
        }
    }


    public WorldTileBlendCoordData GetBlendData(Vector2Int edge)
    {
        if(Data.ContainsKey(edge))
        {
            return Data[edge];
        }
        return null;
    }
}

/// <summary>
/// Stores data on the tiles on the edge of a chunk used for blending so they can be lined up with adjacent chunk
/// </summary>
public class WorldTileBlendCoordData
{
    public Vector2Int EdgeOn;

    //Edge is the coord on the other axis that the blend starts at
    public int LowEdgeStart, HighEdgeStart;
    //Coords of the value on the axis
    public int LowEdgeCoord=9999999,HighEdgeCoord=-999999;
    bool IsHorizontal = false;
    public WorldTileBlendCoordData( Vector2Int edge,int coord,Vector2Int coords,bool isHorizontal)
    {
        IsHorizontal = isHorizontal;
        EdgeOn = edge;
        UpdateBlendData(coord, coords);
      
    }

    public int GetEdge(int xMin,int xMax,Vector2Int worldCoords)
    {
        if (xMin == xMax)
        {
            if (LowEdgeCoord > worldCoords.y)
            {
                return LowEdgeStart;
            }
            else
            {
                return HighEdgeStart;
            }
        }
        else
        {
            if (LowEdgeCoord > worldCoords.x)
            {
                return LowEdgeStart;
            }
            else
            {
                return HighEdgeStart;
            }

        }
    }


    public void UpdateBlendData( int coord, Vector2Int coords)
    {
       
        if (IsHorizontal)
        {
            if (coords.x < LowEdgeCoord)
            {
                LowEdgeCoord = coords.x;
                LowEdgeStart = coord;
            }
            if (coords.x > HighEdgeCoord)
            {
                HighEdgeCoord = coords.x;
                HighEdgeStart = coord;
            }

        }
        else
        {
            if (coords.y < LowEdgeCoord)
            {
                LowEdgeCoord = coords.y;
                LowEdgeStart = coord;
            }
            if (coords.y > HighEdgeCoord)
            {
                HighEdgeCoord = coords.y;
                HighEdgeStart = coord;
            }
        }
    }
}

/// <summary>
/// Data on the feature the given chunk needs to 
/// </summary>
[System.Serializable]
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

