using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode
{
    //The cumulative cost from the starting point to the current point
    public int gCost;
   //The estimated cost from the current point to the end of the current traversal
    public int hCost;
    public bool obstacle;

    public int x, y;
    public PathfindingNode parent;
    public bool IsPassable = true;

    public List<PathfindingNode> neighbours=new List<PathfindingNode>();
    public Vector3 worldPos;




    public PathfindingNode(int x, int y, bool passable)
    {
        this.x = x;
        this.y = y;
        IsPassable= passable;

    }

    public void InitData(PathfindingNode[,] myGrid,int localX,int localY)
    {
        worldPos = new Vector3(x+.5f, y+.5f);
        neighbours = new List<PathfindingNode>();
        if (localX > 0)
        {
            neighbours.Add(myGrid[localX-1,localY]);
        }
        
        if(localX < WorldChunkManager.ChunkSize-1)
        {
            neighbours.Add(myGrid[localX + 1, localY]);
        }

        if (localY > 0)
        {
            neighbours.Add(myGrid[localX , localY - 1]);
        }
        
        if (localY < WorldChunkManager.ChunkSize - 1)
        {
            neighbours.Add(myGrid[localX , localY + 1]);
        }



        Pathfinding.GetNeighbours(this);
    }

    public void UpdatePassable(bool val)
    {
        IsPassable = val;
    }



    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }


    Dictionary<string, PathNodeModifier> modifiers = new Dictionary<string, PathNodeModifier>();

    public void AddModifier(PathNodeModifier modifier)
    {
       
        if(!modifiers.ContainsKey(modifier.modifierKey))
        {
            modifiers.Add(modifier.modifierKey, modifier);
        }
    }

    public void RemoveModifier(string key)
    {
        if (!modifiers.ContainsKey(key))
        {
            modifiers.Remove(key);
        }
    }

    public bool GetPassable(Unit performing,bool useModifiers=true)
    {
        
        bool Cache = IsPassable;
        if (performing==null)
        {
            return Cache;
        }
        if (useModifiers)
        {
            foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
            {
                if (kvp.Value.IsValid(performing))
                {
                    Cache = kvp.Value.ModifyWalkable(Cache, performing);
                    if (Cache == false)
                    {
                        return Cache;
                    }
                }
            }
        }
            return Cache;
    }

    public int GetFCost(Unit performing)
    {
        int Cache = FCost;
        foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
        {
            if (kvp.Value.IsValid(performing))
            {
                Cache = kvp.Value.ModifyFCost(Cache, performing);
            }
        }
        return Cache;
    }

    public int GetHCost(Unit performing)
    {
        int Cache = hCost;
        foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
        {
            if (kvp.Value.IsValid(performing))
            {
                Cache = kvp.Value.ModifyHCost(Cache, performing);
            }
        }
        return Cache;
    }

    public int GetGCost(Unit performing)
    {
        int Cache = gCost;
        foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
        {
            if (kvp.Value.IsValid(performing))
            {
                Cache = kvp.Value.ModifyGCost(Cache, performing);
            }
        }
        return Cache;
    }

    public void ManuallyAddNeighbour(PathfindingNode toAdd)
    {
        neighbours.Add(toAdd);
    }
}
