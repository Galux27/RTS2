using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public static class OverworldPathfinding
{
    static int Height, Width;
    public static void Init(OverworldTile[,] world)
    {
        Width= world.GetLength(0);
        Height= world.GetLength(1);
        for(int x=0;x< Width; x++)
        {
            for(int y=0;y< Height;y++)
            {
                world[x, y].SetNode(new OverworldPathfindingNode(x, y));
                if (world[x, y].Elevation <= OverworldGenerator.Instance.SeaLevel)
                {
                    world[x, y].Node.IsPassible = false;
                }
            }
        }
        Vector2Int coords = new Vector2Int();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                coords.x = x;
                coords.y = y;
                Neighbours(coords, world);
                for(int q = 0; q < neighbourCache.Count; q++)
                {
                    world[x, y].Node.Neighbours.Add(world[neighbourCache[q].x, neighbourCache[q].y]);
                }
            }
        }
    }
    static List<Vector2Int> neighbourCache;

    static void Neighbours(Vector2Int coords, OverworldTile[,] world)
    {
        neighbourCache = new List<Vector2Int>();
        if (validCoords(coords.x + 1, coords.y, world) )
        {
            neighbourCache.Add(coords + new Vector2Int(1, 0));
        }
        if (validCoords(coords.x - 1, coords.y, world))
        {
            neighbourCache.Add(coords + new Vector2Int(-1, 0));
        }
        if (validCoords(coords.x, coords.y + 1, world))
        {
            neighbourCache.Add(coords + new Vector2Int(0, 1));
        }
        if (validCoords(coords.x, coords.y - 1, world))
        {
            neighbourCache.Add(coords + new Vector2Int(0, -1));
        }
    }
    static bool validCoords(int x, int y, OverworldTile[,] world)
    {
        if (x < 0 || y < 0 || y >= Height || x >= Width)
        {
            return false;
        }
      
        return true;
    }
   
    public static List<OverworldPathfindingNode> FindPathUsingBasic(Vector2Int startPos,Vector2Int targetPos, OverworldTile[,] world)
    {
        Vector2Int posInStart=startPos, posInEnd=targetPos;
        List<BasicOverworldPathfindingNode> basicPath = OverworldBasicPathfinding.GetPathFromSimplified(startPos, targetPos, out posInStart, out posInEnd);
        if (basicPath == null||basicPath.Count==0)
        {
            //Debug.LogError("Could not get simplifeid path between " + startPos + " " + targetPos);
            return null;
        }
        List<OverworldPathfindingNode> retVal = new List<OverworldPathfindingNode>();
        Vector2Int localStart= posInStart, localEnd= posInEnd;
        Vector2Int offset = Vector2Int.zero;

        List<OverworldPathfindingNode> cache = new List<OverworldPathfindingNode>();
        if (basicPath.Count >1) { 
            for (int x = 0; x < basicPath.Count; x++)
            {
            if (cache != null)
            {
                cache.Clear();

            }
            else
            {
                cache = new List<OverworldPathfindingNode>();
            }

                offset = new Vector2Int(basicPath[x].coords.x * OverworldBasicPathfinding.SimplifySize, basicPath[x].coords.y * OverworldBasicPathfinding.SimplifySize);
                if (x == 0)
                {
                    localStart = posInStart;
                    localEnd = OverworldBasicPathfinding.GetTargetPosToGetToNeighbour(basicPath[x], basicPath[x + 1]);

                }
                else if (x == basicPath.Count - 1)
                {
                    localStart = ConvertToNeighbouringCoords(localEnd, basicPath[x]);
                    localEnd = posInEnd;

                }
                else
                {
                    localStart = ConvertToNeighbouringCoords(localEnd, basicPath[x]);
                    localEnd = OverworldBasicPathfinding.GetTargetPosToGetToNeighbour(basicPath[x], basicPath[x + 1]);
                }
                cache = FindPath(localStart, localEnd, basicPath[x].TilesISimplify, offset);
                if (cache != null)
                {
                    retVal.AddRange(cache);
                }
                else
                {
                    Debug.LogError("Error on section " + x + " between " + localStart + " to " + localEnd + " in chunk " + basicPath[x].coords);
                }
            }
        }
        else
        {
            retVal = FindPath(startPos, targetPos, world,false);
        }
        return retVal;
    }

    static Vector2Int ConvertToNeighbouringCoords(Vector2Int pos, BasicOverworldPathfindingNode neighbour)
    {
        if (pos.x == neighbour.TilesISimplify.GetLength(0) - 1)
        {
            pos.x = 0;
        }else if (pos.x == 0)
        {
            pos.x = neighbour.TilesISimplify.GetLength(0) - 1;
        }

        if (pos.y == neighbour.TilesISimplify.GetLength(1) - 1)
        {
            pos.y = 0;
        }
        else if (pos.y == 0)
        {
            pos.y = neighbour.TilesISimplify.GetLength(1) - 1;
        }
        return pos;
    }



    public static List<OverworldPathfindingNode> FindPath(Vector2Int startPos, Vector2Int targetPos, OverworldTile[,] world, Vector2Int offset, bool ImpassibleStopsPath = true)
    {
        //get player and target position in grid coords
        OverworldPathfindingNode seekerNode = world[startPos.x, startPos.y].Node;
        OverworldPathfindingNode targetNode = world[targetPos.x, targetPos.y].Node;
        openSet.Clear();
        closedSet.Clear();

        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            OverworldPathfindingNode node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //If target found, retrace path
            if (node == targetNode)
            {

                return RetracePath(seekerNode, targetNode);

            }

            //adds neighbor nodes to openSet
            foreach (OverworldTile neighbour in node.Neighbours)
            {
                if (neighbour.Node.IsPassible == false && ImpassibleStopsPath || closedSet.Contains(neighbour.Node)||neighbour.Node.coords.x-offset.x<0||neighbour.Node.coords.y-offset.y<0||
                    neighbour.Node.coords.x - offset.x > world.GetLength(0)-1 || neighbour.Node.coords.y - offset.y > world.GetLength(1)-1)
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour.Node) + CalculateCostBetweenNodes(world[node.coords.x-offset.x, node.coords.y-offset.y], neighbour);
                if (newCostToNeighbour < neighbour.Node.gCost || !openSet.Contains(neighbour.Node))
                {
                    neighbour.Node.gCost = newCostToNeighbour;
                    neighbour.Node.hCost = GetDistance(neighbour.Node, targetNode);
                    neighbour.Node.parent = node;

                    if (!openSet.Contains(neighbour.Node))
                        openSet.Add(neighbour.Node);
                }
            }
        }

        return null;
    }


    public static List<OverworldPathfindingNode> FindPath(Vector2Int startPos, Vector2Int targetPos, OverworldTile[,] world,bool ImpassibleStopsPath=true)
    {
        //get player and target position in grid coords
        OverworldPathfindingNode seekerNode = world[startPos.x,startPos.y].Node;
        OverworldPathfindingNode targetNode = world[targetPos.x, targetPos.y].Node;
        openSet.Clear();
        closedSet.Clear();

        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            OverworldPathfindingNode node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //If target found, retrace path
            if (node == targetNode)
            {

                return RetracePath(seekerNode, targetNode);

            }

            //adds neighbor nodes to openSet
            foreach (OverworldTile neighbour in node.Neighbours)
            {
                if (neighbour.Node.IsPassible == false && ImpassibleStopsPath || closedSet.Contains(neighbour.Node))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour.Node) + CalculateCostBetweenNodes(world[node.coords.x,node.coords.y],neighbour);
                if (newCostToNeighbour < neighbour.Node.gCost || !openSet.Contains(neighbour.Node))
                {
                    neighbour.Node.gCost = newCostToNeighbour;
                    neighbour.Node.hCost = GetDistance(neighbour.Node, targetNode);
                    neighbour.Node.parent = node;

                    if (!openSet.Contains(neighbour.Node))
                        openSet.Add(neighbour.Node);
                }
            }
        }

        return null;
    }

   
    
    
    
    public static int CalculateCostBetweenNodes(OverworldTile start,OverworldTile end)
    {
        float retVal = 0f;
        retVal += Mathf.Abs( start.Elevation- end.Elevation);
        retVal += GetNodeWeight(end);
        retVal += Random.Range(-1f, 1f) * 5f;

        return Mathf.RoundToInt( retVal);
    }

    public static float GetNodeWeight(OverworldTile node)
    {
        if (node == null)
        {
            return 9999999999999f;
        }
        float retVal = 0f;
        retVal += node.Elevation;

        if (node.Features.Contains(OverworldFeature.MajorRoad) || node.Features.Contains(OverworldFeature.Settlement))
        {
            retVal *= .5f;
        }
        if (node.Features.Contains(OverworldFeature.River))
        {
            retVal *= 1.25f;
        }
        if (node.Features.Contains(OverworldFeature.MinorRoad))
        {
            retVal *= .8f;
        }
        if (node.Features.Contains(OverworldFeature.Backroad))
        {
            retVal *= .9f;
        }
        return Mathf.RoundToInt(retVal);

    }

    static HashSet<OverworldPathfindingNode> closedSet = new HashSet<OverworldPathfindingNode>();
    static List<OverworldPathfindingNode> openSet = new List<OverworldPathfindingNode>();

    static List<OverworldPathfindingNode> RetracePath(OverworldPathfindingNode startNode, OverworldPathfindingNode endNode)
    {
        List<OverworldPathfindingNode> path = new List<OverworldPathfindingNode>();
        OverworldPathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        path.Reverse();

        return path;

    }

    static int GetDistance(OverworldPathfindingNode nodeA, OverworldPathfindingNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.coords.x - nodeB.coords.x);
        int dstY = Mathf.Abs(nodeA.coords.y - nodeB.coords.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

public class OverworldPathfindingNode
{
    public Vector2Int coords;
    public List<OverworldTile> Neighbours;
    //The cumulative cost from the starting point to the current point
    public int gCost;
    //The estimated cost from the current point to the end of the current traversal
    public int hCost;
    public bool IsPassible = true;
    public OverworldPathfindingNode parent;
    public float ManualWeighting = 0;
    public OverworldPathfindingNode(int x,int y)
    {
        coords = new Vector2Int(x, y);
        Neighbours = new List<OverworldTile>();
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}


