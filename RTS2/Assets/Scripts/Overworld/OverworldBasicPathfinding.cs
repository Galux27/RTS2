using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OverworldBasicPathfinding 
{
    public const int SimplifySize = 25;
    public static BasicOverworldPathfindingNode[,] SimplifiedWorld;
    public static void InitOverworldBasicPathfinding(OverworldTile[,] world)
    {
        SimplifiedWorld = new BasicOverworldPathfindingNode[Mathf.CeilToInt( world.GetLength(0)/ SimplifySize), Mathf.CeilToInt(world.GetLength(1) / SimplifySize)];
        for (int x = 0; x < SimplifiedWorld.GetLength(0); x++)
        {
            for (int y = 0; y < SimplifiedWorld.GetLength(0); y++)
            {
                SimplifiedWorld[x,y] = new BasicOverworldPathfindingNode(x, y,SimplifySize);
            }
        }

      

        Vector2Int parentCoords = new Vector2Int(),LocalCoords=new Vector2Int();
        for (int x = 0; x < world.GetLength(0); x++)
        {
            for (int y = 0; y < world.GetLength(0); y++)
            {
                parentCoords.x = x / SimplifySize;
                parentCoords.y = y / SimplifySize;
                LocalCoords.x = x % SimplifySize;
                LocalCoords.y = y % SimplifySize;
                SimplifiedWorld[parentCoords.x, parentCoords.y].AddLocalCoord(world[x, y], LocalCoords);
            }
        }

        UpdateBasicWeightings();
        for (int x = 0; x < SimplifiedWorld.GetLength(0) ; x++)
        {
            for (int y = 0; y < SimplifiedWorld.GetLength(0) ; y++)
            {
                if (x > 0)
                {
                    SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x - 1, y]);
                    if (y > 0)
                    {
                        SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x-1, y - 1]);

                    }
                    if (y < SimplifiedWorld.GetLength(1) - 1)
                    {
                        SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x-1, y + 1]);
                    }
                }
                if (x < SimplifiedWorld.GetLength(0) - 1)
                {
                    SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x + 1, y]);
                    if (y > 0)
                    {
                        SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x+1, y - 1]);

                    }
                    if (y < SimplifiedWorld.GetLength(1) - 1)
                    {
                        SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x+1, y + 1]);
                    }
                }
                if (y > 0)
                {
                    SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x, y - 1]);

                }
                if (y < SimplifiedWorld.GetLength(1) - 1)
                {
                    SimplifiedWorld[x, y].AddNeighbour(SimplifiedWorld[x, y + 1]);
                }

                }
            }

        for (int x = 0; x < SimplifiedWorld.GetLength(0); x++)
        {
            for (int y = 0; y < SimplifiedWorld.GetLength(0); y++)
            {
                
                 if(SimplifiedWorld[x, y].Neighbours.Count == 0)
                {
                    SimplifiedWorld[x, y].IsPassible = false;
                }
                 

            }
        }
        Debug.Log("Finished simplifying pathfinding");
    }

    public static void UpdateBasicWeightings()
    {
        for (int x = 0; x < SimplifiedWorld.GetLength(0); x++)
        {
            for (int y = 0; y < SimplifiedWorld.GetLength(0); y++)
            {
                SimplifiedWorld[x, y].RefreshWeight();
            }
        }

    }

    public static Vector2Int GetTargetPosToGetToNeighbour(BasicOverworldPathfindingNode chunk1, BasicOverworldPathfindingNode chunk2)
    {
        Vector2Int Target = new Vector2Int(chunk1.TilesISimplify.GetLength(0) / 2, chunk1.TilesISimplify.GetLength(1) / 2);
        if (chunk2.coords.x > chunk1.coords.x)
        {
            Target.x = chunk1.TilesISimplify.GetLength(0) - 1;
        }
        else if (chunk2.coords.x < chunk1.coords.x)
        {
            Target.x = 0;
        }

        if (chunk2.coords.y > chunk1.coords.y)
        {
            Target.y = chunk1.TilesISimplify.GetLength(1) - 1;
        }
        else if (chunk2.coords.y < chunk1.coords.y)
        {
            Target.y = 0;
        }
        return Target;
    }


    public static bool CanConnectSimplifiedChunk(BasicOverworldPathfindingNode chunk1, BasicOverworldPathfindingNode chunk2)
    {
        Vector2Int offset = new Vector2Int(chunk1.coords.x * SimplifySize, chunk1.coords.y * SimplifySize);
        Vector2Int Start = new Vector2Int(chunk1.TilesISimplify.GetLength(0) / 2, chunk1.TilesISimplify.GetLength(1) / 2);
        Vector2Int Target = new Vector2Int(chunk1.TilesISimplify.GetLength(0) / 2, chunk1.TilesISimplify.GetLength(1) / 2);
        if (chunk2.coords.x > chunk1.coords.x)
        {
            Target.x = chunk1.TilesISimplify.GetLength(0) - 1;
        }
        else if(chunk2.coords.x < chunk1.coords.x)
        {
            Target.x = 0;
        }

        if (chunk2.coords.y > chunk1.coords.y)
        {
            Target.y = chunk1.TilesISimplify.GetLength(1) - 1;
        }
        else if (chunk2.coords.y < chunk1.coords.y)
        {
            Target.y = 0;
        }
        if (Start == Target)
        {
            return false;
        }
        List<OverworldPathfindingNode> path = OverworldPathfinding.FindPath(Start, Target, chunk1.TilesISimplify, offset);
        if (path!=null)
        {
            return true;
        }

        return false;
    }

    static HashSet<BasicOverworldPathfindingNode> closedSet = new HashSet<BasicOverworldPathfindingNode>();
    static List<BasicOverworldPathfindingNode> openSet = new List<BasicOverworldPathfindingNode>();

    public static List<BasicOverworldPathfindingNode> GetPathFromSimplified(Vector2Int startPos, Vector2Int targetPos,out Vector2Int posInStart,out Vector2Int posInEnd)
    {
        Vector2Int convertedStartCoords = startPos;
        Vector2Int convertedEndCoords = targetPos;
        Vector2Int parentCoords = new Vector2Int(), LocalCoords = new Vector2Int();
        parentCoords.x = convertedStartCoords.x / SimplifySize;
        parentCoords.y = convertedStartCoords.y / SimplifySize;
        LocalCoords.x = convertedStartCoords.x % SimplifySize;
        LocalCoords.y = convertedStartCoords.y % SimplifySize;
        posInStart = LocalCoords;
        convertedStartCoords = parentCoords;

        parentCoords.x = convertedEndCoords.x / SimplifySize;
        parentCoords.y = convertedEndCoords.y / SimplifySize;
        LocalCoords.x = convertedEndCoords.x % SimplifySize;
        LocalCoords.y = convertedEndCoords.y % SimplifySize;
        posInEnd = LocalCoords;
        convertedEndCoords = parentCoords;

        return FindPath(convertedStartCoords, convertedEndCoords, SimplifiedWorld);
    }


    static List<BasicOverworldPathfindingNode> FindPath(Vector2Int startPos, Vector2Int targetPos, BasicOverworldPathfindingNode[,] world)
    {
        //get player and target position in grid coords
        BasicOverworldPathfindingNode seekerNode = world[startPos.x, startPos.y];
        BasicOverworldPathfindingNode targetNode = world[targetPos.x, targetPos.y];
        openSet.Clear();
        closedSet.Clear();

        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            BasicOverworldPathfindingNode node = openSet[0];
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
            foreach (BasicOverworldPathfindingNode neighbour in node.Neighbours)
            {
                if (neighbour.IsPassible == false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + CalculateCostBetweenNodes(world[node.coords.x, node.coords.y], neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
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

    public static int CalculateCostBetweenNodes(BasicOverworldPathfindingNode start, BasicOverworldPathfindingNode end)
    {
        float retVal = 0f;
        retVal += end.TraversalWeight;

        return Mathf.RoundToInt(retVal);
    }

    static List<BasicOverworldPathfindingNode> RetracePath(BasicOverworldPathfindingNode startNode, BasicOverworldPathfindingNode endNode)
    {
        List<BasicOverworldPathfindingNode> path = new List<BasicOverworldPathfindingNode>();
        BasicOverworldPathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        path.Reverse();

        return path;

    }

    static int GetDistance(BasicOverworldPathfindingNode nodeA, BasicOverworldPathfindingNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.coords.x - nodeB.coords.x);
        int dstY = Mathf.Abs(nodeA.coords.y - nodeB.coords.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

public class BasicOverworldPathfindingNode
{
    public Vector2Int coords;
    public List<BasicOverworldPathfindingNode> Neighbours;
    //The cumulative cost from the starting point to the current point
    public int gCost;
    //The estimated cost from the current point to the end of the current traversal
    public int hCost;
    public bool IsPassible = true;
    public float TraversalWeight = 0f;
    public OverworldTile[,] TilesISimplify;
    public BasicOverworldPathfindingNode parent;
    public BasicOverworldPathfindingNode(int x, int y,int size)
    {
        coords = new Vector2Int(x, y);
        Neighbours = new List<BasicOverworldPathfindingNode>();
        TilesISimplify = new OverworldTile[size, size];
        Neighbours = new List<BasicOverworldPathfindingNode>();
    }
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public void AddLocalCoord(OverworldTile toAdd,Vector2Int coords)
    {
        TilesISimplify[coords.x, coords.y] = toAdd;
    }

    public void AddNeighbour(BasicOverworldPathfindingNode neighbour,bool addInReverse = true)
    {
        if (!OverworldBasicPathfinding.CanConnectSimplifiedChunk(this, neighbour))
        {
            return;
        }
        if (!Neighbours.Contains(neighbour))
        {
            Neighbours.Add(neighbour);
            neighbour.AddNeighbour(neighbour,false);
        }
    }

    public void RefreshWeight()
    {
        float total = 0;
        for(int x = 0; x < TilesISimplify.GetLength(0); x++)
        {
            for (int y = 0; y < TilesISimplify.GetLength(1); y++)
            {
                total += OverworldPathfinding.GetNodeWeight(TilesISimplify[x, y]);
            }
        }
        TraversalWeight = total / (TilesISimplify.GetLength(0) * TilesISimplify.GetLength(1));
    }
}
