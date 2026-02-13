
using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public static class Pathfinding
{

  

    static int worldWidth, worldHeight;
    public static void UpdateNodeData(int x,int y,bool traversable)
    {
        PathfindingNode node = GetNodeFromCoords(x, y);
        if (node == null)
        {
           // Debug.LogError("Null node at " + x+"," + y);
            return;
        }
       node.UpdatePassable(traversable);
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
    static bool IsNodeIDValid(int id)
    {
        if (!NodeIDPathing.PathNodeIDs.ContainsKey(id))
        {
            return false;
        }
        if (NodeIDPathing.PathNodeIDs[id].NeighbouringIDs.Count <= 1)
        {
            if (NodeIDPathing.PathNodeIDs[id].NeighbouringIDs.Contains(-1)==true)
            {
                return false;
            }
        }
        return true;
    }

    public static PathfindingNode GetNodeFromPosition(Vector3 Position,Unit performing=null,bool debug=false)
    {

        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(Mathf.Ceil(Position.x),Mathf.Ceil( Position.y), out batch, out chunk, out local);
        if (!ValidateCoords())
        {
            return null;
        }

        PathfindingNode retVal = WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].PathfindingNodes[local.x, local.y];
        if (retVal.PathNodeGroupID == -1)
        {
            float dist = 22f, dist2 = 0f;
            bool found = false;
            for (int x = 0; x < retVal.neighbours.Count; x++)
            {
                if (retVal.neighbours[x].PathNodeGroupID != -1 && IsNodeIDValid(retVal.neighbours[x].PathNodeGroupID))
                {
                    dist2 = Vector3.Distance(retVal.neighbours[x].worldPos, Position);
                    if (dist2 < dist )
                    {
                        dist = dist2;
                        retVal = retVal.neighbours[x];
                    }
                }
            }
            found = retVal.PathNodeGroupID != -1;
            if (!found)
            {
                for (int x = 0; x < retVal.neighbours.Count; x++)
                {
                    for (int y = 0; y < retVal.neighbours[x].neighbours.Count; y++)
                    {
                        if (retVal.neighbours[x].neighbours[y].PathNodeGroupID != -1 && IsNodeIDValid(retVal.neighbours[x].neighbours[y].PathNodeGroupID))
                        {
                            dist2 = Vector3.Distance(retVal.neighbours[x].neighbours[y].worldPos, Position);
                            if (dist2 < dist)
                            {
                                dist = dist2;
                                retVal = retVal.neighbours[x].neighbours[y];
                            }
                        }
                    }
                    }
                }
        }
        return retVal;
    }


    public static WorldTile GetTileFromPosition(Vector3 Position, Unit performing = null, bool debug = false)
    {

        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords( Position.x, Position.y, out batch, out chunk, out local);
        if (!ValidateCoords())
        {
            return null;
        }
        return WorldChunkManager.Instance.ChunkBatches[batch].Chunks[chunk.x, chunk.y].ChunkTiles[local.x, local.y];
    }

    public static List<PathfindingNode> FindPath(Vector2Int start,Vector2Int end)
    {
        return FindPath(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0));
    }

    const int MaxNodesCanCheck = 3000;

    static bool CanGetPath(PathfindingNode start,PathfindingNode end)
    {
        if (NodeIDPathing.PathNodeIDs.ContainsKey(start.PathNodeGroupID) == false || NodeIDPathing.PathNodeIDs.ContainsKey(end.PathNodeGroupID) == false)
        {
            Debug.Log("Path Fail: due to being stuck on node with no group");
            return false;
        }

        if (start.PathNodeGroupID == -1 || end.PathNodeGroupID == -1)
        {
            Debug.Log("Path Fail: due to being stuck on impassible node");

            return false;
        }
        return NodeIDPathing.GetPath(start, end).Count > 0;
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
        if (!CanGetPath(seekerNode, targetNode))
        {
            return null;
        }
        Debug.Log("Starting path from " + startPos + " to " + targetPos + " start null " + (seekerNode == null) + "," + (targetNode == null));
        int count = 0;
        openSet.Clear();
        closedSet.Clear();

        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0 && count<MaxNodesCanCheck)
        {
            count++;
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
                Debug.Log("Path Count " + count);
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
        if (!CanGetPath(seekerNode, targetNode))
        {
            Debug.Log("Path Fail: Could not get path between"+seekerNode.PathNodeGroupID+" and " + targetNode.PathNodeGroupID);
            return null;
        }
        Debug.Log("Getting Path from "+ startPos+" to "+  targetPos+" start node "
            +seekerNode.worldPos.ToString()
            +" dest node " + targetNode.worldPos.ToString());
        if (/*seekerNode.IsPassable == false ||*/ targetNode.IsPassable == false)
        {
            Debug.Log("Path Failed: Getting path failed " + seekerNode.worldPos + "|" + targetNode.worldPos + 
                "|" + seekerNode.neighbours.Contains(targetNode) + "|" + seekerNode.neighbours.Count + "|" + targetNode.neighbours.Count+"|"+seekerNode.IsPassable+"|"+targetNode.IsPassable);

            return null;
        }
        Debug.Log("Getting path " + seekerNode.worldPos+"|"+targetNode.worldPos+"|"+seekerNode.neighbours.Contains(targetNode)+"|"+seekerNode.neighbours.Count+"|"+targetNode.neighbours.Count);
        openSet.Clear();
        closedSet.Clear();
        openSet.Add(seekerNode);
        int count = 0;
        //calculates path for pathfinding
        while (openSet.Count > 0 && count < MaxNodesCanCheck)
        {
            count++;
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
                Debug.Log("Path Count " + count);

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

    public static List<PathfindingNode> FindPath(Vector3 startPos, PathfindingNode targetNode, Unit performing)
    {
        //get player and target position in grid coords
        PathfindingNode seekerNode = GetNodeFromPosition(startPos, performing);
        Debug.Log("Getting Path from " + startPos + " to " + targetNode.worldPos + " start node "
            + seekerNode.worldPos.ToString()
            + " dest node " + targetNode.worldPos.ToString());
        if (!CanGetPath(seekerNode, targetNode))
        {
            return null;
        }
        if ( targetNode.IsPassable == false)
        {
            Debug.Log("Getting path failed " + seekerNode.worldPos + "|" + targetNode.worldPos +
                "|" + seekerNode.neighbours.Contains(targetNode) + "|" + seekerNode.neighbours.Count + "|" + targetNode.neighbours.Count + "|" + seekerNode.IsPassable + "|" + targetNode.IsPassable);

            return null;
        }
        int count = 0;
        Debug.Log("Getting path " + seekerNode.worldPos + "|" + targetNode.worldPos + "|" + seekerNode.neighbours.Contains(targetNode) + "|" + seekerNode.neighbours.Count + "|" + targetNode.neighbours.Count);
        openSet.Clear();
        closedSet.Clear();
        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0 && count < MaxNodesCanCheck)
        {
            count++;
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
                Debug.Log("Path Count " + count);

                return RetracePath(seekerNode, targetNode);

            }

            //adds neighbor nodes to openSet
            foreach (PathfindingNode neighbour in node.neighbours)
            {
                if (neighbour.GetPassable(performing) == false || closedSet.Contains(neighbour))
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

public static class NodeIDPathing
{
    public static Dictionary<int, PathNodeID> PathNodeIDs = new Dictionary<int, PathNodeID>();
    public static void AddPathfindingIDLink(int id1, int id2, Vector2Int batch1, Vector2Int batch2,Vector2Int batch1Pos,Vector2Int batch2Pos)
    {
        if (!PathNodeIDs.ContainsKey(id1))
        {
            PathNodeIDs.Add(id1, new PathNodeID(id1, batch1));

        }
        if (!PathNodeIDs.ContainsKey(id2))
        {
            PathNodeIDs.Add(id2, new PathNodeID(id2, batch2));
        }
        PathNodeIDs[id1].AddNeighbour(id2);
        PathNodeIDs[id1].AddTile(batch1Pos);
        PathNodeIDs[id2].AddNeighbour(id1);
        PathNodeIDs[id2].AddTile(batch2Pos);
    }
    static float Dist(PathNodeID start,PathNodeID end)
    {

        float dstX = Mathf.Abs(start.AveragePos.x - end.AveragePos.x);
        float dstY = Mathf.Abs(start.AveragePos.y - end.AveragePos.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    public static List<int> GetPath(PathfindingNode start, PathfindingNode end)
    {
        if (start.PathNodeGroupID == -1 || end.PathNodeGroupID == -1)
        {
            return null;
        }
        List<int> retVal = new List<int>();
        if (start.PathNodeGroupID == end.PathNodeGroupID)
        {
            retVal.Add(start.PathNodeGroupID);
            return retVal;
        }
        int count = 0;
        List<int> openSet = new List<int>();
        HashSet<int> closedSet = new HashSet<int>();
        openSet.Add(start.PathNodeGroupID);
        PathNodeID current = PathNodeIDs[ openSet[0]],comparing=null;
        PathNodeID endNodeID = PathNodeIDs[end.PathNodeGroupID];
        while (openSet.Count > 0 && count<600)
        {
            count++;
            current = PathNodeIDs[openSet[0]];
            for (int x = 1; x < openSet.Count; x++)
            {
                comparing = PathNodeIDs[openSet[x]];
                if (comparing.FCost <= current.FCost)
                {
                    if (comparing.HCost < current.HCost)
                    {
                        current = comparing;
                    }
                }
            }
            openSet.Remove(current.NodeID);
            closedSet.Add(current.NodeID);
            if (current.NodeID == end.PathNodeGroupID)
            {
                int curNode = current.NodeID;
                retVal.Add(current.NodeID);
                while (curNode != start.PathNodeGroupID && curNode>-1)
                {
                    retVal.Add(current.Parent);              
                    current = PathNodeIDs[current.Parent];
                    curNode = current.Parent;
                }
                retVal.Add(start.PathNodeGroupID);
                return retVal;
            }

            for(int x=0;x<current.NeighbouringIDs.Count;x++)
            {
                if (closedSet.Contains(current.NeighbouringIDs[x]) 
                    || PathNodeIDs.ContainsKey(current.NeighbouringIDs[x])==false
                    || current.NeighbouringIDs[x]==-1)
                {
                    continue;
                }
                comparing = PathNodeIDs[current.NeighbouringIDs[x]];
                float newCostToNeighbour = current.GCost + Dist(comparing, current);
                if(newCostToNeighbour < comparing.GCost || !openSet.Contains(current.NeighbouringIDs[x]))
                {
                    comparing.GCost = newCostToNeighbour;
                    comparing.HCost = Dist(comparing, endNodeID);
                    comparing.Parent = current.NodeID;
                    if (!openSet.Contains(current.NeighbouringIDs[x]))
                    {
                        openSet.Add(current.NeighbouringIDs[x]);
                    }
                }
            }
        }
        return null;
        
    }

    public static void ClearOutSwaps(Dictionary<int, int> toRemove)
    {
        PathNodeID id = null, neighbour = null;
        foreach (KeyValuePair<int, int> kvp in toRemove)
        {
            id = GetPathNodeID(kvp.Key);
            if (id == null)
            {
                continue;
            }
            for (int x = 0; x < id.NeighbouringIDs.Count; x++)
            {
                neighbour = GetPathNodeID(id.NeighbouringIDs[x]);
                if (neighbour != null)
                {
                    neighbour.RemoveNeighbour(kvp.Key);
                }
            }
        }

        foreach (KeyValuePair<int, int> kvp in toRemove)
        {
            if (PathNodeIDs.ContainsKey(kvp.Key))
            {
                PathNodeIDs.Remove(kvp.Key);
            }
        }
    }

    public static PathNodeID GetPathNodeID(int id)
    {
        if (PathNodeIDs.ContainsKey(id))
        {
            return PathNodeIDs[id];
        }
        return null;
    }
}
[System.Serializable]
public class PathNodeID {
    public int NodeID=-1;
    public Vector2 ChunkBatch,AveragePos;
    public List<int> NeighbouringIDs;
    public float GCost, HCost;
    int count = 0;
    public int Parent=-1;
    public float FCost
    {
        get
        {
            return GCost + HCost;
        }
    }
    public void AddTile(Vector2Int tile)
    {
        AveragePos = tile;
    }


    public PathNodeID(int id,Vector2Int chunkBatch)
    {
        NodeID = id;
        ChunkBatch = chunkBatch;
        NeighbouringIDs= new List<int>();
    }

    public void RemoveNeighbour(int id)
    {
        if (NeighbouringIDs.Contains(id))
        {
            NeighbouringIDs.Remove(id);
        }
    }

    public void AddNeighbour(int id)
    {
        if(NeighbouringIDs.Contains(id))
        {
            return;
        }
        NeighbouringIDs.Add(id);
    }
}
