using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinding
{
    static int worldWidth, worldHeight;
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


    public static PathfindingNode GetNodeFromPosition(Vector3 Position,Unit performing=null,bool debug=false)
    {
        WorldChunk toGetFrom = WorldChunkManager.Instance.GetWorldChunkFromPos(Position );//.Chunks[chunkForNode.x, chunkForNode.y];
        if(toGetFrom == null)
        {
            
            return null;
        }
      
        int xC = 0, yC = 0;
        Vector2Int bottom = toGetFrom.ChunkTiles[0, 0].Coords();
        Vector2Int top = toGetFrom.ChunkTiles[WorldChunkManager.ChunkSize-1, WorldChunkManager.ChunkSize - 1].Coords();

        float lX = Mathf.InverseLerp(bottom.x, top.x, Position.x);
        float lY = Mathf.InverseLerp(bottom.y, top.y, Position.y);

        xC = Mathf.RoundToInt(Mathf.Lerp(0, WorldChunkManager.ChunkSize - 1, lX));
        yC = Mathf.RoundToInt(Mathf.Lerp(0, WorldChunkManager.ChunkSize - 1, lY));



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
        int dstX = Mathf.Abs(nodeA.X- nodeB.X);
        int dstY = Mathf.Abs(nodeA.Y - nodeB.Y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
