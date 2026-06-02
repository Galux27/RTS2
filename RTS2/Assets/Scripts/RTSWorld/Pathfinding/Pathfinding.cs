
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public static class Pathfinding
{

  

    static int worldWidth, worldHeight;

    static Vector2Int wallBatch, wallChunk, wallTile;

    static List<int> AvailableParentChannels= new List<int>() { };
    static List<int> InUseParentChannels = new List<int>();
    static int AllChannels = 0;
    public static int GetParentChannel()
    {
        if (AvailableParentChannels.Count == 0)
        {
            AvailableParentChannels.Add(AllChannels);
            AllChannels++;
            Debug.Log("Increased parent channel count to " + AllChannels);
        }

        int retVal = AvailableParentChannels[0];
        AvailableParentChannels.RemoveAt(0);    
        InUseParentChannels.Add(retVal);
        return retVal;

    }


    public static void ReturnParentChannel(int val)
    {
        InUseParentChannels.Remove(val);
        AvailableParentChannels.Add(val);
    }
    public static void UpdateNodeNeighboursBasedOnWall(int x,int y,bool traversible)
    {
       
            PathfindingNode node = GetNodeFromCoords(x, y);
        if (node == null)
        {
            return;
        }
        WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(x, y, out wallBatch, out wallChunk, out wallTile);
        WallSegment tile = WorldChunkManager.Instance.GetChunkBatch(wallBatch).Chunks[wallChunk.x, wallChunk.y].WallSegments[wallTile.x, wallTile.y];
        int index = -1,neighbourToMeIndex=-1;
        PathfindingNode checking = null;
        WallSegment compWall = null;
   
        if (tile.HasDoor||traversible)
        {
            index = node.GetNeighbourInDireciton(0, -1);
            if (index > -1)
            {
                checking = node.neighbours[index].Node;
                node.neighbours[index].IsAccessable = true;
                neighbourToMeIndex = checking.GetNeighbourInDireciton(0, 1);
                checking.neighbours[neighbourToMeIndex].IsAccessable = true;
            }

            index = node.GetNeighbourInDireciton(0, 1);
            if (index > -1)
            {
                checking = node.neighbours[index].Node;
                node.neighbours[index].IsAccessable = true;
                neighbourToMeIndex = checking.GetNeighbourInDireciton(0, -1);
                checking.neighbours[neighbourToMeIndex].IsAccessable = true;
            }


            index = node.GetNeighbourInDireciton(-1, 0);
            if (index > -1)
            {
                checking = node.neighbours[index].Node;
                node.neighbours[index].IsAccessable = true;
                neighbourToMeIndex = checking.GetNeighbourInDireciton(1, 0);
                checking.neighbours[neighbourToMeIndex].IsAccessable = true;
            }

            index = node.GetNeighbourInDireciton(1, 0);
            if (index > -1)
            {
                checking = node.neighbours[index].Node;
                node.neighbours[index].IsAccessable = true;
                neighbourToMeIndex = checking.GetNeighbourInDireciton(-1, 0);
                checking.neighbours[neighbourToMeIndex].IsAccessable = true;
            }
                return;
        }
        
        if (tile.HasWall&&!traversible)
        {

           
            index = node.GetNeighbourInDireciton(0, -1);
            checking = node.neighbours[index].Node;
            neighbourToMeIndex = checking.GetNeighbourInDireciton(0, 1);
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(x, y - 1, out wallBatch, out wallChunk, out wallTile);
            compWall = WorldChunkManager.Instance.GetChunkBatch(wallBatch).Chunks[wallChunk.x, wallChunk.y ].WallSegments[wallTile.x, wallTile.y];
            if (compWall.HasDoor == false)
            {
                node.neighbours[index].IsAccessable = false;

                checking.neighbours[neighbourToMeIndex].IsAccessable = false;
            }

            
            WorldChunkManager.Instance.ConvertPositionToChunkAndLocalCoords(x - 1, y, out wallBatch, out wallChunk, out wallTile);
            compWall = WorldChunkManager.Instance.GetChunkBatch(wallBatch).Chunks[wallChunk.x, wallChunk.y ].WallSegments[wallTile.x, wallTile.y];
            if (compWall.HasDoor == false) { 
                index = node.GetNeighbourInDireciton(-1, 0);
            checking = node.neighbours[index].Node;
            node.neighbours[index].IsAccessable = false;
            neighbourToMeIndex = checking.GetNeighbourInDireciton(1, 0);
            checking.neighbours[neighbourToMeIndex].IsAccessable = false;
        }
            

             
        }
     
       


    }


    public static void UpdateNodeData(int x,int y,bool traversable)
    {
        PathfindingNode node = GetNodeFromCoords(x, y);
        if (node == null)
        {
           // Debug.LogError("Null node at " + x+"," + y);
            return;
        }
       node.UpdatePassable(traversable);
       
        if (traversable && node.PathNodeGroupID == -1)
        {
            for(int q = 0; q< node.neighbours.Count; q++)
            {
                if (node.neighbours[q].Node.PathNodeGroupID != -1)
                {
                    node.PathNodeGroupID = q;
                    return;
                }
            }
            node.PathNodeGroupID = -2;
        }
    }
    public static void AddPathNodeModifier(int x,int y,PathNodeModifier toAdd)
    {
        GetNodeFromCoords(x, y).AddModifier(toAdd);
    }

    public static void RemovePathModifier(int x,int y,string key)
    {
        GetNodeFromCoords(x, y).RemoveModifier(key);

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
                if (retVal.neighbours[x].Node.PathNodeGroupID != -1 && IsNodeIDValid(retVal.neighbours[x].Node.PathNodeGroupID))
                {
                    dist2 = Vector3.Distance(retVal.neighbours[x].Node.worldPos, Position);
                    if (dist2 < dist )
                    {
                        dist = dist2;
                        retVal = retVal.neighbours[x].Node;
                    }
                }
            }
            found = retVal.PathNodeGroupID != -1;
            if (!found)
            {
                for (int x = 0; x < retVal.neighbours.Count; x++)
                {
                    for (int y = 0; y < retVal.neighbours[x].Node.neighbours.Count; y++)
                    {
                        if (retVal.neighbours[x].Node.neighbours[y].Node.PathNodeGroupID != -1 && IsNodeIDValid(retVal.neighbours[x].Node.neighbours[y].Node.PathNodeGroupID))
                        {
                            dist2 = Vector3.Distance(retVal.neighbours[x].Node.neighbours[y].Node.worldPos, Position);
                            if (dist2 < dist)
                            {
                                dist = dist2;
                                retVal = retVal.neighbours[x].Node.neighbours[y].Node;
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

    public static List<PathfindingNode> FindPath(Vector2Int start,Vector2Int end,int parentChannel)
    {
        return FindPath(new Vector3(start.x, start.y, 0), new Vector3(end.x, end.y, 0),parentChannel);
    }

    const int MaxNodesCanCheck = 3000;

    static bool CanGetPath(PathfindingNode start,PathfindingNode end)
    {
        if(start == null || end == null)
        {
            return false;
        }
        return true;
        if (NodeIDPathing.PathNodeIDs.ContainsKey(start.PathNodeGroupID) == false || NodeIDPathing.PathNodeIDs.ContainsKey(end.PathNodeGroupID) == false)
        {
            Debug.Log("Unit Path: fail due to being stuck on node with no group "+start.PathNodeGroupID+","+end.PathNodeGroupID);
            return false;
        }

        if (start.PathNodeGroupID == -1 || end.PathNodeGroupID == -1)
        {
            Debug.Log("Unit Path: faoil due to being stuck on impassible node " + start.PathNodeGroupID+","+end.PathNodeGroupID);

            return false;
        }
        return NodeIDPathing.GetPath(start, end).Count > 0;
    }

    public static int debugcount = 0;
    /// <summary>
    /// Finds a path without considering the unit performing the path
    /// Used in calculating whether a building is enclosed or not
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static List<PathfindingNode> FindPath(Vector3 startPos, Vector3 targetPos,int parentChannel)
    {
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        List<PathfindingNode> openSet = new List<PathfindingNode>();
        debugcount = 0;
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
                return RetracePath(seekerNode, targetNode, parentChannel);

            }
            debugcount++;
            //adds neighbor nodes to openSet
            foreach (PathfindingNeighbour neighbour in node.neighbours)
            {
                if (neighbour.Node.GetPassable(null) == false || closedSet.Contains(neighbour.Node)||neighbour.IsAccessable==false)
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour.Node);
                if (newCostToNeighbour < neighbour.Node.gCost || !openSet.Contains(neighbour.Node))
                {
                    neighbour.Node.gCost = newCostToNeighbour;
                    neighbour.Node.hCost = GetDistance(neighbour.Node, targetNode);
                    neighbour.Node.SetParent(parentChannel, node);// = node;

                    if (!openSet.Contains(neighbour.Node))
                        openSet.Add(neighbour.Node);
                }
            }
        }
        return null;
    }



    public static List<PathfindingNode> FindPath(Vector3 startPos, Vector3 targetPos,Unit performing,int parentChannel)
    {

        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        List<PathfindingNode> openSet = new List<PathfindingNode>();
        PathfindingNode seekerNode = null;
        List<PathfindingNode> usedNodes = new List<PathfindingNode>();

        if (performing.LastNode != null)
        {
           // Debug.Log("Unit Path: start node is last node at " + performing.LastNode.worldPos);
            seekerNode = performing.LastNode;
            if (seekerNode.worldPos == Vector3.zero)
            {
                seekerNode = GetNodeFromPosition(startPos, performing);

            }
        }
        else
        {
            seekerNode = GetNodeFromCoords(performing.lastCoords.x, performing.lastCoords.y);
            //Debug.Log("Unit Path: start node is found node at " + seekerNode.worldPos);

        }
        //        GetNodeFromPosition(startPos,performing);
        PathfindingNode targetNode = GetNodeFromPosition(targetPos,performing);
        if (!CanGetPath(seekerNode, targetNode))
        {
            Debug.Log("Path Fail: Could not get path between"+seekerNode.PathNodeGroupID+" and " + targetNode.PathNodeGroupID);
            return null;
        }
        //Debug.Log("Getting Path from "+ startPos+" to "+  targetPos+" start node "
        //    +seekerNode.worldPos.ToString()
        //    +" dest node " + targetNode.worldPos.ToString());
        if (/*seekerNode.IsPassable == false ||*/ targetNode.IsPassable == false)
        {
            
            return null;
        }


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
            usedNodes.Add(node);
            //If target found, retrace path
            if (node == targetNode)
            {
               // Debug.Log("Path Count " + count);
               List<PathfindingNode> retVal = RetracePath(seekerNode, targetNode, parentChannel);
                for(int x = 0; x < usedNodes.Count; x++)
                {
                    usedNodes[x].SetParent(parentChannel, null);
                }
                return retVal;
                
            }
            
            //adds neighbor nodes to openSet
            foreach (PathfindingNeighbour neighbour in node.neighbours)
            {
                if (neighbour.Node.GetPassable(performing)==false 
                    || closedSet.Contains(neighbour.Node)
                    ||neighbour.IsAccessable==false
                    ||neighbour.Node==null)
                {
                    continue;
                }

                int newCostToNeighbour = node.GetGCost(performing) + GetDistance(node, neighbour.Node);
                if (newCostToNeighbour < neighbour.Node.GetGCost(performing) || !openSet.Contains(neighbour.Node))
                {
                    neighbour.Node.gCost = newCostToNeighbour;
                    neighbour.Node.hCost = GetDistance(neighbour.Node, targetNode);
                    neighbour.Node.SetParent(parentChannel, node);
                    usedNodes.Add(neighbour.Node);

                    if (!openSet.Contains(neighbour.Node) && neighbour.Node!=null && neighbour.Node.neighbours!=null)
                        openSet.Add(neighbour.Node);
                }
            }
        }
        return null;
    }

    public static List<PathfindingNode> FindPath(Vector3 startPos, PathfindingNode targetNode, Unit performing,int parentChannel)
    {
        HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();
        List<PathfindingNode> openSet = new List<PathfindingNode>();
        List<PathfindingNode> usedNodes = new List<PathfindingNode>();
       
//get player and target position in grid coords
PathfindingNode seekerNode = null;
        if (performing.hasLastNode)
        {
            //Debug.Log("Unit Path: start node is last node at " + performing.LastNode.worldPos+","+performing.LastNode.IsPassable);
            seekerNode = performing.LastNode;
        }
        else
        {
            seekerNode = GetNodeFromPosition(startPos) ;
            //Debug.Log("Unit Path: start node is found node at " + seekerNode.worldPos);

        }
        //Debug.Log("Unit Path: Getting Path from " + startPos + " to " + targetNode.worldPos + " start node "
        //    + seekerNode.worldPos.ToString()
        //    + " dest node " + targetNode.worldPos.ToString());

        if (!CanGetPath(seekerNode, targetNode))
        {
            return null;
        }
        if ( targetNode.IsPassable == false)
        {
         
            return null;
        }
        int count = 0;
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
                if (closedSet.Contains(openSet[i]))
                {
                    continue;
                }
                if (openSet[i].GetFCost(performing) <= node.GetFCost(performing))
                {
                    if (openSet[i].GetHCost(performing) < node.GetHCost(performing))
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);
            usedNodes.Add(node);
            //If target found, retrace path
            if (node == targetNode)
            {
                Debug.Log("Path Count " + count);
                List<PathfindingNode> RetVal = RetracePath(seekerNode, targetNode, parentChannel);
                for(int x = 0; x < usedNodes.Count; x++)
                {
                    usedNodes[x].SetParent(parentChannel, null);
                }
                return RetVal;

            }

            //adds neighbor nodes to openSet
            foreach (PathfindingNeighbour neighbour in node.neighbours)
            {
                if (neighbour.Node.GetPassable(performing) == false 
                    || closedSet.Contains(neighbour.Node)
                    ||neighbour.IsAccessable==false)
                {
                    continue;
                }

                int newCostToNeighbour = node.GetGCost(performing) + GetDistance(node, neighbour.Node);
                if (newCostToNeighbour < neighbour.Node.GetGCost(performing) || !openSet.Contains(neighbour.Node))
                {
                    neighbour.Node.gCost = newCostToNeighbour;
                    neighbour.Node.hCost = GetDistance(neighbour.Node, targetNode);
                    neighbour.Node.SetParent(parentChannel, node);
                    usedNodes.Add(neighbour.Node);

                    if (!openSet.Contains(neighbour.Node))
                        openSet.Add(neighbour.Node);
                }
            }
        }
        return null;
    }


    static List<PathfindingNode> RetracePath(PathfindingNode startNode, PathfindingNode endNode,int parentChannel)
    {
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode currentNode = endNode;
        try
        {
            while (currentNode != startNode)
            {
                if (path.Contains(currentNode))
                {
                    Debug.LogError("Path Length Found:Node added to path more than once, fuck " + currentNode.worldPos+" start " + startNode.worldPos+" end pos " + endNode.worldPos+" parent "+parentChannel);
                    break;
                }
                path.Add(currentNode);
                currentNode = currentNode.GetParent(parentChannel);
            }
        }catch(System.Exception e)
        {
            Debug.LogErrorFormat("Path Length Found:  Error retracing path, path len" + path.Count);
        }
            path.Add(startNode);
        Debug.Log("Path Length Found: " + path.Count);
        
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
