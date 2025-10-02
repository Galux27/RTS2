using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public static class Pathfinding
{
    static int worldWidth, worldHeight;
       public static void UpdateNodeData(int x,int y,bool traversable)
    {
        if (GetNodeFromCoords(x, y) == null)
        {
            Debug.LogError("Null node at " + x+"," + y);
        }


       GetNodeFromCoords(x,y).UpdatePassable(traversable);
    }
    public static void AddPathNodeModifier(int x,int y,PathNodeModifier toAdd)
    {
        GetNodeFromCoords(x, y).AddModifier(toAdd);
    }

    public static void RemovePathModifier(int x,int y,string key)
    {
        GetNodeFromCoords(x, y).RemoveModifier(key);

    }

    public static List<PathfindingNode> GetNeighbours(PathfindingNode node)
    {
        List<PathfindingNode> retVal = new List<PathfindingNode>();
        PathfindingNode toAdd = null;
        //if (node.x == 0)
        {
            toAdd = GetNodeFromCoords(node.X - 1, node.Y);
            if (toAdd != null)
            {
                retVal.Add(toAdd);
                if (!toAdd.neighbours.Contains(node))
                {
                    toAdd.neighbours.Add(node);
                }
            }
        }

        //if (node.x == WorldChunkManager.ChunkSize - 1)
        {
            toAdd = GetNodeFromCoords(node.X + 1, node.Y);
            if (toAdd != null)
            {
                retVal.Add(toAdd);
                if (!toAdd.neighbours.Contains(node))
                {
                    toAdd.neighbours.Add(node);
                }
            }
        }
       // if (node.y == 0)
        {
            toAdd = GetNodeFromCoords(node.X, node.Y - 1);
            if (toAdd != null)
            {
                retVal.Add(toAdd);
                if (!toAdd.neighbours.Contains(node))
                {
                    toAdd.neighbours.Add(node);
                }
            }
        }

       // if (node.y == WorldChunkManager.ChunkSize - 1)
        {
            toAdd = GetNodeFromCoords(node.X, node.Y + 1);
            if (toAdd != null)
            {
                retVal.Add(toAdd);
                if (!toAdd.neighbours.Contains(node))
                {
                    toAdd.neighbours.Add(node);
                }
            }
        }
        return retVal;
    }


   public static Vector2Int GetCoordsFromPosition (Vector3 Position)
    {
        int x = Mathf.RoundToInt(Position.x);
     

        int y = Mathf.RoundToInt(Position.y);
      
        return new Vector2Int(x, y);
    }

    static Vector2Int coordsCache;

    public static PathfindingNode GetNodeFromCoords(int x, int y)
    {
        coordsCache = new Vector2Int(x, y);
        return GetNodeFromCoords(coordsCache);
    }
   static Vector2Int batch = new Vector2Int(), chunk = new Vector2Int(), local = new Vector2Int();


    public static string GetLastCoordsFound()
    {
        return batch.ToString() + " " + chunk.ToString() + " " + local.ToString();
    }

    public static PathfindingNode GetNodeFromCoords(Vector2Int coords)
    {
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(coords.x,coords.y,out batch,out chunk, out local);
        if (!ValidateCoords())
        {
            return null;
        }
        return WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x,chunk.y].PathfindingNodes[local.x,local.y];   
    }

    static bool ValidateCoords()
    {
        if (WorldChunkManager.Instance.ChunkBatches.ContainsKey(batch) == false)
        {
            return false;
        }
        return true;
    }


    public static PathfindingNode GetNodeFromPosition(Vector3 Position,Unit performing=null,bool debug=false)
    {

        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(Position.x, Position.y, out batch, out chunk, out local);
        if (!ValidateCoords())
        {
            return null;
        }
        return WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].PathfindingNodes[local.x, local.y];
    }


    public static List<PathfindingNode> FindPath(Vector2Int start,Vector2Int end)
    {
        return FindPath(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0));
    }

    /// <summary>
    /// Finds a path without considering the unit performing the path
    /// Used in calculating whether a building is enclosed or not
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static List<PathfindingNode> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //get player and target position in grid coords
        PathfindingNode seekerNode = GetNodeFromPosition(startPos);
        PathfindingNode targetNode = GetNodeFromPosition(targetPos);
        Debug.Log("Starting path from " + startPos + " to " + targetPos + " start null " + (seekerNode == null) + "," + (targetNode == null));

        openSet.Clear();
        closedSet.Clear();

        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            PathfindingNode node = openSet[0];
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
            foreach (PathfindingNode neighbour in node.neighbours)
            {
                if (neighbour.GetPassable(null) == false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
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


    static HashSet<PathfindingNode> closedSet=new HashSet<PathfindingNode>();
    static List<PathfindingNode> openSet=new List<PathfindingNode>();
    public static List<PathfindingNode> FindPath(Vector3 startPos, Vector3 targetPos,Unit performing)
    {
        //get player and target position in grid coords
        PathfindingNode seekerNode = GetNodeFromPosition(startPos,performing);
        PathfindingNode targetNode = GetNodeFromPosition(targetPos,performing);
        Debug.Log("Getting Path from "+ startPos+" to "+  targetPos+" start node "+seekerNode.worldPos.ToString()+" dest node " + targetNode.worldPos.ToString());
        if (seekerNode.IsPassable == false || targetNode.IsPassable == false)
        {
            return null;
        }
        Debug.Log("Getting path " + seekerNode.worldPos+"|"+targetNode.worldPos+"|"+seekerNode.neighbours.Contains(targetNode)+"|"+seekerNode.neighbours.Count+"|"+targetNode.neighbours.Count);
        openSet.Clear();
        closedSet.Clear();
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
        path.Add(startNode);
        path.Reverse();

        return path;

    }

    static int GetDistance(PathfindingNode nodeA, PathfindingNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.X- nodeB.X);
        int dstY = Mathf.Abs(nodeA.Y - nodeB.Y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
