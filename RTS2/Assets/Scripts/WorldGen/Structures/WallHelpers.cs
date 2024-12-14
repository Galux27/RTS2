using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallHelpers
{

    public static Sprite GetSpriteForWallType(WallSegment segment, WallManager wallManager, WallTile toGetTileFrom)
    {
        bool up = false, down = false, left = false, right = false;

        if (segment.x > 0)
        {
            if (wallManager.WallsInWorld[segment.x - 1, segment.y].HasWall)
            {
                left = true;
            }
        }

        if (segment.x < wallManager.WallsInWorld.GetLength(0) - 1)
        {
            if (wallManager.WallsInWorld[segment.x + 1, segment.y].HasWall)
            {
                right = true;
            }
        }

        if (segment.y > 0)
        {
            if (wallManager.WallsInWorld[segment.x, segment.y - 1].HasWall)
            {
                down = true;
            }
        }

        if (segment.y < wallManager.WallsInWorld.GetLength(1) - 1)
        {
            if (wallManager.WallsInWorld[segment.x, segment.y + 1].HasWall)
            {
                up = true;
            }
        }



        if (up && down && left && right)
        {
            return (toGetTileFrom.Cross.sprite);
        }
        else
        {
            if (up && down && left)
            {
                return (toGetTileFrom.TopBottomLeft.sprite);
            }
            else if (up && down && right)
            {
                return (toGetTileFrom.TopBottomRight.sprite);
            }
            else if (left && right && down)
            {
                return (toGetTileFrom.LeftRightBelow.sprite);
            }
            else if (left && right && up)
            {
                return (toGetTileFrom.LeftRightAbove.sprite);
            }
            else
            {
                if (left && down)
                {
                    return (toGetTileFrom.LeftBelow.sprite);
                }
                else if (left && up)
                {
                    return (toGetTileFrom.LeftAbove.sprite);
                }
                else if (right && down)
                {
                    return (toGetTileFrom.RightBelow.sprite);
                }
                else if (right && up)
                {
                    return (toGetTileFrom.RightAbove.sprite);
                }
                else if (left && right)
                {
                    return (toGetTileFrom.LeftRight.sprite);
                }
                else if (up && down)
                {
                    return (toGetTileFrom.UpDown.sprite);
                }
                else
                {
                    if (left)
                    {
                        return (toGetTileFrom.Left.sprite);
                    }
                    else if (right)
                    {
                        return (toGetTileFrom.Right.sprite);
                    }
                    else if (up)
                    {
                        return (toGetTileFrom.Above.sprite);
                    }
                    else if (down)
                    {
                        return (toGetTileFrom.Below.sprite);
                    }
                    else
                    {
                        return (toGetTileFrom.NoNeighbours.sprite);
                    }
                }
            }
        }
    }

    public static void CalculateTileType(ref WallSegment segment, WallManager wallManager,WallTile toGetTileFrom)
    {
        bool up=false,down=false,left=false,right=false;

        if (segment.x > 0)
        {
            if (wallManager.WallsInWorld[segment.x - 1, segment.y].HasWall)
            {
                left = true;
            }
        }

        if (segment.x < wallManager.WallsInWorld.GetLength(0) - 1)
        {
            if (wallManager.WallsInWorld[segment.x + 1, segment.y].HasWall)
            {
                right = true;
            }
        }

         if (segment.y > 0)
        {
            if (wallManager.WallsInWorld[segment.x , segment.y - 1].HasWall)
            {
                down = true;
            }
        }

        if (segment.y < wallManager.WallsInWorld.GetLength(1) - 1)
        {
            if (wallManager.WallsInWorld[segment.x , segment.y + 1].HasWall)
            {
                up = true;
            }
        }
        


        if (up && down && left && right)
        {
            segment.SetTile(toGetTileFrom.Cross);
        }
        else
        {
            if (up && down && left)
            {
                segment.SetTile(toGetTileFrom.TopBottomLeft);
            }
            else if (up && down && right)
            {
                segment.SetTile(toGetTileFrom.TopBottomRight);
            }
            else if (left && right && down)
            {
                segment.SetTile(toGetTileFrom.LeftRightBelow);
            }
            else if (left && right && up)
            {
                segment.SetTile(toGetTileFrom.LeftRightAbove);
            }
            else
            {
                if (left && down)
                {
                    segment.SetTile(toGetTileFrom.LeftBelow);
                }
                else if (left && up)
                {
                    segment.SetTile(toGetTileFrom.LeftAbove);
                }
                else if(right && down)
                {
                    segment.SetTile(toGetTileFrom.RightBelow);
                }
                else if(right && up)
                {
                    segment.SetTile(toGetTileFrom.RightAbove);
                }
                else if(left && right)
                {
                    segment.SetTile(toGetTileFrom.LeftRight);
                }
                else if(up && down)
                {
                    segment.SetTile(toGetTileFrom.UpDown);
                }
                else
                {
                    if (left)
                    {
                        segment.SetTile(toGetTileFrom.Left) ;
                    }
                    else if (right)
                    {
                        segment.SetTile(toGetTileFrom.Right);
                    }
                    else if (up)
                    {
                        segment.SetTile(toGetTileFrom.Above);
                    }
                    else if (down)
                    {
                        segment.SetTile(toGetTileFrom.Below);
                    }
                    else
                    {
                        segment.SetTile(toGetTileFrom.NoNeighbours);
                    }
                }
            }
        }
    
    
    }


    public static void CreateWallBuildableStructure(int x, int y,Tilemap toDrawOn,WallTile toUse,Vector3 worldPos,Vector3 offset=default)
    {
        Action OnBuilt = () => { WallHelpers.CreateWallObject(x, y, toDrawOn, toUse); };
        BuildableStructure bs = new BuildableStructure(x, y, 1f, false, OnBuilt, Vector3.one,offset);
        Vector2Int coords = WorldChunkManager.Instance.GetChunkCoordsFromWorldPos(worldPos);
        WorldChunkManager.Instance.Chunks[coords.x,coords.y].AddConstructable(bs);
    }

    public static void CreateWallObject(int x, int y, Tilemap toDrawOn, WallTile toUse)
    {
        WorldController.Instance.WallManager.AddSingleWall(x, y, toDrawOn, toUse);
    }

}
