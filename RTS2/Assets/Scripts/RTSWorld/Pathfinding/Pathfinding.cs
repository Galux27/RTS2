using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static PathfindingNode[,] pathfindingNodes;
    static int worldWidth, worldHeight;
    public static void CreateNodesFromWorld(WorldTile[,] world)
    {
        worldWidth = world.GetLength(0);
        worldHeight = world.GetLength(1);
        pathfindingNodes = new PathfindingNode[worldWidth, worldHeight];

        for(int x = 0; x < worldWidth; x++)
        {
            for(int y = 0; y < worldHeight; y++)
            {
                pathfindingNodes[x, y] = new PathfindingNode(x, y, world[x, y].traversable);
            }
        }

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                pathfindingNodes[x, y].InitData();
            }
        }
    }

    public static void UpdateNodeData(int x,int y,bool traversable)
    {
        pathfindingNodes[x, y].UpdatePassable(traversable);
    }
    public static void AddPathNodeModifier(int x,int y,PathNodeModifier toAdd)
    {
        Debug.Log("Path node modifier door adding " + toAdd.modifierKey);
        pathfindingNodes[x, y].AddModifier(toAdd);
    }

    public static void RemovePathModifier(int x,int y,string key)
    {
        pathfindingNodes[x, y].RemoveModifier(key);

    }

    public static List<PathfindingNode> GetNeighbours(PathfindingNode node)
    {
        List<PathfindingNode> retVal = new List<PathfindingNode>();
        if (node.x > 0)
        {
            retVal.Add(pathfindingNodes[node.x-1,node.y]);
        }

        if (node.x < pathfindingNodes.GetLength(0)-1)
        {
            retVal.Add(pathfindingNodes[node.x + 1, node.y]);
        }
        if (node.y > 0)
        {
            retVal.Add(pathfindingNodes[node.x , node.y - 1]);
        }

        if (node.y < pathfindingNodes.GetLength(1) - 1)
        {
            retVal.Add(pathfindingNodes[node.x , node.y + 1]);
        }


        return retVal;
    }


    public static PathfindingNode GetNodeFromPosition(Vector3 Position)
    {
        int x= Mathf.RoundToInt(Position.x);
        x = Mathf.Max(0, x);
        x = Mathf.Min(worldWidth-1, x);

        int y = Mathf.RoundToInt(Position.y);
        y = Mathf.Max(0, y);
        y = Mathf.Min(worldHeight-1, y);

        return pathfindingNodes[x,y];
    }

    

    public static List<PathfindingNode> FindPath(Vector3 startPos, Vector3 targetPos,Unit performing)
    {
        //get player and target position in grid coords
        PathfindingNode seekerNode = GetNodeFromPosition(startPos);
        PathfindingNode targetNode = GetNodeFromPosition(targetPos);

        List<PathfindingNode> openSet = new List<PathfindingNode>();
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            PathfindingNode node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].GetFCost(performing) <= node.GetFCost(performing))
                {
                    if (openSet[i].GetHCost(performing) < node.GetHCost(performing))
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
            foreach (PathfindingNode neighbour in node.neighbours)
            {
                if (neighbour.GetPassable(performing)==false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.GetGCost(performing) + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.GetGCost(performing) || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    static List<PathfindingNode> RetracePath(PathfindingNode startNode, PathfindingNode endNode)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;

    }

    static int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x- nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
