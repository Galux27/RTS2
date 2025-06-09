using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinding
{
    static int worldWidth, worldHeight;
    public static void CreateNodesFromWorld(WorldTile[,] world)
    {
        worldWidth = world.GetLength(0);
        worldHeight = world.GetLength(1);
        for(int x = 0; x < worldWidth; x++)
        {
            for(int y = 0; y < worldHeight; y++)
            {
                GetNodeFromCoords(x, y).UpdatePassable(world[x, y].traversable);
            }
        }

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                GetNodeFromCoords(x, y).InitData();
            }
        }
    }

    public static void UpdateNodeData(int x,int y,bool traversable)
    {
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
        toAdd = GetNodeFromCoords(node.x - 1, node.y);
        if (toAdd != null)
        {
            retVal.Add(toAdd);
            if (!toAdd.neighbours.Contains(node))
            {
                toAdd.neighbours.Add(node);
            }
        }


        toAdd = GetNodeFromCoords(node.x + 1, node.y);
        if (toAdd != null)
        {
            retVal.Add(toAdd);
            if (!toAdd.neighbours.Contains(node))
            {
                toAdd.neighbours.Add(node);
            }
        }


        toAdd = GetNodeFromCoords(node.x, node.y - 1);
        if (toAdd != null)
        {
            retVal.Add(toAdd);
            if (!toAdd.neighbours.Contains(node))
            {
                toAdd.neighbours.Add(node);
            }
        }
        toAdd = GetNodeFromCoords(node.x, node.y + 1);
        if (toAdd != null)
        {
            retVal.Add(toAdd);
            if (!toAdd.neighbours.Contains(node))
            {
                toAdd.neighbours.Add(node);
            }
        }
        return retVal;
    }


   public static Vector2Int GetCoordsFromPosition (Vector3 Position)
    {
        int x = Mathf.RoundToInt(Position.x);
        x = Mathf.Max(0, x);
        x = Mathf.Min(worldWidth - 1, x);

        int y = Mathf.RoundToInt(Position.y);
        y = Mathf.Max(0, y);
        y = Mathf.Min(worldHeight - 1, y);

        return new Vector2Int(x, y);
    }

    static bool ValidCoords(int x,int y)
    {
        return x>=0&&y>=0&&x<worldWidth&&y<worldHeight;
    }


    static Vector2Int coordsCache;

    public static PathfindingNode GetNodeFromCoords(int x, int y)
    {
        coordsCache = new Vector2Int(x, y);
        return GetNodeFromCoords(coordsCache);
    }

    public static PathfindingNode GetNodeFromCoords(Vector2Int coords)
    {
        WorldChunk toGetFrom = WorldChunkManager.Instance.GetWorldChunkFromTileCoords(coords);
        if (toGetFrom == null)
        {
            return null;
        }
        coordsCache = coords - toGetFrom.WorldCoords;
        if (coordsCache.x < 0 || coordsCache.y < 0||coordsCache.x >=WorldChunkManager.ChunkSize||coordsCache.y>=WorldChunkManager.ChunkSize) { return null; }
        return toGetFrom.PathfindingNodes[coordsCache.x, coordsCache.y];
    }


    public static PathfindingNode GetNodeFromPosition(Vector3 Position,Unit performing=null)
    {
        WorldChunk toGetFrom = WorldChunkManager.Instance.GetWorldChunkFromPos(Position + new Vector3(.5f, .5f, 0f));//.Chunks[chunkForNode.x, chunkForNode.y];

        int xC = 0, yC = 0;
        for(int x = 0; x < toGetFrom.PathfindingNodes.GetLength(0); x++)
        {
            if (toGetFrom.PathfindingNodes[x,0].worldPos.x > Position.x)
            {
                xC = Mathf.Max( x-1,0);

                break;
            }
            
        }
        for (int y = 0; y < toGetFrom.PathfindingNodes.GetLength(1); y++)
        {
            if (toGetFrom.PathfindingNodes[0, y].worldPos.y > Position.y)
            {
                yC = Mathf.Max(y -1,0);
                break;
            }
        }

      
        return toGetFrom.PathfindingNodes[xC, yC];

     
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
        int dstX = Mathf.Abs(nodeA.x- nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
