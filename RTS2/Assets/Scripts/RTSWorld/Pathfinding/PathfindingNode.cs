using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode
{
    public int gCost, hCost;
    public bool obstacle;

    public int x, y;
    public PathfindingNode parent;
    public bool IsPassable = true;



    public List<PathfindingNode> neighbours;
    public Vector3 worldPos;
    public PathfindingNode(int x, int y, bool passable)
    {
        this.x = x;
        this.y = y;
        IsPassable= passable;

    }

    public void InitData()
    {
        worldPos = new Vector3(x, y);
        neighbours = Pathfinding.GetNeighbours(this);
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



}
