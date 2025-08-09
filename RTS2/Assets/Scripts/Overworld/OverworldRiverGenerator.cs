using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Overworld River Generator", menuName = "Overworld/River Generator", order = 1)]
public class OverworldRiverGenerator : OverworldFeatureGenerator
{
    public int NumberOfRiversToGenerate;
    int width, height;
    public override void GenerateFeature(OverworldTile[,] world)
    {
        if(NumberOfRiversToGenerate<=0) return;
        width = world.GetLength(0);
        height = world.GetLength(1);

        List<Vector2Int> PositionsForRiverStart = new List<Vector2Int>();
        for(int x = 0; x < width; x++)
        {
            for(int y=0; y < height; y++)
            {
                if (world[x, y].Elevation >OverworldGenerator.Instance.MaxElevation * .7f)
                {
                    PositionsForRiverStart.Add(new Vector2Int(x, y));
                }
            }
        }



        List<River> rivers = new List<River>();
        Vector2Int coords = new Vector2Int();

        for(int x=0;x<NumberOfRiversToGenerate;x++) 
        {
            coords = PositionsForRiverStart[Random.Range(0,PositionsForRiverStart.Count)];
            rivers.Add(new River(coords, world[coords.x,coords.y].Elevation));
        }



        bool PerformAnotherLoop = true;
        while (PerformAnotherLoop )
        {
            bool hasExpandedRiver = false;
            int index = 0;
            int count = rivers.Count;
            while(index<count)
            {
                if (ExpandRiver(rivers[index],world,rivers))
                {
                    hasExpandedRiver = true;
                }
                index++;
            }
            if (!hasExpandedRiver)
            {
                PerformAnotherLoop = false;
            }
        }
        for (int x = 0; x < NumberOfRiversToGenerate; x++)
        {
           for(int q = 0; q < rivers[x].TilesInRiver.Count; q++)
            {
                coords = rivers[x].TilesInRiver[q];
                world[coords.x, coords.y].AddFeatureToTile(OverworldFeature.River);
            }
        }

    }

    bool ExpandRiver(River r,OverworldTile[,] world,List<River> rivers )
    {
        if (!CanRiverProgress(r,rivers,world))
        {
            return false;
        }
        List<Vector2Int> nextPoints = CanRiverExpand(r, world);
        if (nextPoints!=null && nextPoints.Count>0)
        {
            int index = Random.Range(0, nextPoints.Count);  
            r.ProgressRiver(nextPoints[index], world[nextPoints[index].x, nextPoints[index].y].Elevation);
            return true;
            
        }
        return false;
    }

    bool CanRiverProgress(River r,  List<River> rivers,OverworldTile[,] world)
    {
        if (r.currentCoords.x == 0 || r.currentCoords.y == 0 || r.currentCoords.x == width - 1 || r.currentCoords.y == height - 1)
        {
            return false;
        }

        if (world[r.currentCoords.x, r.currentCoords.y].Elevation < OverworldGenerator.Instance.SeaLevel)
        {
            return false;
        }
        
        for(int x = 0; x < rivers.Count; x++)
        {
            if (rivers[x] == r)
            {
                continue;
            }else if (rivers[x].TilesInRiver.Contains(r.currentCoords))
            {
                return false;
            }
        }
        return true;
    }

    List<Vector2Int> CanRiverExpand(River r, OverworldTile[,] world)
    {
        Neighbours(r.currentCoords);
        if (neighbourCache.Count > 0)
        {
            float lowestNeighbours = r.CurrentHeight*1.5f;
            float curHeight = 0;
            List<Vector2Int> expanding = new List<Vector2Int>();
            for(int x = 0; x < neighbourCache.Count; x++)
            {
                curHeight = world[neighbourCache[x].x, neighbourCache[x].y].Elevation;
                if (curHeight < lowestNeighbours)
                {
                    lowestNeighbours = curHeight;
                    expanding.Add(neighbourCache[x]);
                }else if (curHeight == lowestNeighbours)
                {
                    if (r.TilesInRiver.Contains(neighbourCache[x]) == false)
                    {
                        expanding.Add(neighbourCache[x]);
                    }
                }
            }
            return expanding;
        }
        return null;
    }
    List<Vector2Int> neighbourCache;
    void Neighbours(Vector2Int coords)
    {
        neighbourCache = new List<Vector2Int>();
        if (validCoords(coords.x + 1, coords.y))
        {
            neighbourCache.Add(coords + new Vector2Int(1, 0));
        }
        if (validCoords(coords.x - 1, coords.y))
        {
            neighbourCache.Add(coords + new Vector2Int(-1, 0));
        }
        if (validCoords(coords.x , coords.y + 1))
        {
            neighbourCache.Add(coords + new Vector2Int(0, 1));
        }
        if (validCoords(coords.x, coords.y - 1))
        {
            neighbourCache.Add(coords + new Vector2Int(0, -1));
        }
    }
    bool validCoords(int x,int y)
    {
        if (x < 0 || y < 0 ||y >= height || x >= width)
        {
            return false;
        }
        return true;
    }
}


public class River
{
    public Vector2Int currentCoords;
    public float CurrentHeight;
    public List<Vector2Int> TilesInRiver=new List<Vector2Int>();
    public bool CanExpand = true;
    public River(Vector2Int coords,float height)
    {
        currentCoords = coords;
        CurrentHeight = height;
    }

    public void ProgressRiver(Vector2Int newPos,float newHeight)
    {
        TilesInRiver.Add(currentCoords);
        currentCoords = newPos;
        CurrentHeight = newHeight;
    }
}
