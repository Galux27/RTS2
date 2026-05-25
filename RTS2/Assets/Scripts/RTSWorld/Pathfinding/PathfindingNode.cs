using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PathfindingNode
{
    //The cumulative cost from the starting point to the current point
    public int gCost;
   //The estimated cost from the current point to the end of the current traversal
    public int hCost;
    public bool obstacle;

    public int X, Y,localX,localY;
    public Dictionary<int, PathfindingNode> parent=new Dictionary<int, PathfindingNode>();

    public PathfindingNode GetParent(int index)
    {
        if (parent.ContainsKey(index) == false)
        {
            parent.Add(index, null);
        }
        return parent[index];
    }

    public void SetParent(int index, PathfindingNode node)
    {
        if (parent.ContainsKey(index) == false)
        {
            parent.Add(index, null);
        }
      
        parent[index] = node ;
    }


    public bool IsPassable = true;

    public List<PathfindingNeighbour> neighbours;
    public Vector3 worldPos;
    //id to identify different sub groups in an id
    public int PathNodeGroupID = -1;
    //represents the highest path node used to make sure they're all unique
    public static int CurrentPathNodeID=0;
    static Dictionary<int, Color> PathGroupColours = new Dictionary<int, Color>();
    public static Color GetPathGroupColour(int id)
    {
        if (!PathGroupColours.ContainsKey(id))
        {
            PathGroupColours.Add(id, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }
        return PathGroupColours[id];
    }

    public PathfindingNode(int x, int y, bool passable)
    {
        Init(x, y, passable);

    }

    public int GetNeighbourInDireciton(int xMod,int yMod)
    {
        Vector3 worldPos = this.worldPos+new Vector3(xMod,yMod);
        for(int x = 0; x < neighbours.Count; x++)
        {
            if (neighbours[x].Node.worldPos == worldPos)
            {
                return x;
            }
        }


        return -1;
    }

    public void Init(int x, int y, bool passable)
    {
        X = x;
        Y = y;
        IsPassable = passable;
        if (neighbours == null)
        {
            neighbours = new List<PathfindingNeighbour>(4);
        }
    }

    public void InitData(PathfindingNode[,] myGrid,int localX,int localY)
    {
        worldPos = new Vector3(X+.5f, Y+.5f);
       this.localX= localX;
        this.localY = localY;
        
        if (localX > 0)
        {
            AddNeighbour(myGrid[localX-1,localY]);
        }
        
        if(localX < WorldChunkManager.ChunkSize-1)
        {
            AddNeighbour(myGrid[localX + 1, localY]);
        }

        if (localY > 0)
        {
            AddNeighbour(myGrid[localX , localY - 1]);
        }
        
        if (localY < WorldChunkManager.ChunkSize - 1)
        {
            AddNeighbour(myGrid[localX , localY + 1]);
        }

    }

    public bool DoWeHaveLinkToNode(PathfindingNode node, out int index)
    {
        index = -1;
        for(int x = 0; x < neighbours.Count; x++)
        {
            if (neighbours[x].Node== node)
            {
                index = x;
                return true;
            }
        }
        return false;
    }


    void AddNeighbour(PathfindingNode node)
    {
        if (DoWeHaveLinkToNode(node,out int ind))
        {
            return;
        }
        neighbours.Add(new PathfindingNeighbour( node));
    }

    public void UpdatePassable(bool val)
    {
        if (PathNodeGroupID == -1 && val && BuildingGenerator.Instance.IsGenerating)
        {
            for(int x=0;x<neighbours.Count;x++)
            {
                if (neighbours[x].Node.PathNodeGroupID != -1 && neighbours[x].Node.IsPassable && neighbours[x].IsAccessable)
                {
                    PathNodeGroupID = neighbours[x].Node.PathNodeGroupID;
                    break;
                }
            }
        }
        IsPassable = val;
    }



    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }


    Dictionary<string, PathNodeModifier> modifiers;

    public void AddModifier(PathNodeModifier modifier)
    {
        if (modifiers == null)
        {
            modifiers= new Dictionary<string, PathNodeModifier>();
        }
        if(!modifiers.ContainsKey(modifier.modifierKey))
        {
            modifiers.Add(modifier.modifierKey, modifier);
        }
    }

    public void RemoveModifier(string key)
    {
        if (modifiers == null)
        {
            return;
        }
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
            if (modifiers != null)
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
            }
            return Cache;
    }

    public int GetFCost(Unit performing)
    {
        int Cache = FCost;
        if (modifiers != null)
        {
            foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
            {
                if (kvp.Value.IsValid(performing))
                {
                    Cache = kvp.Value.ModifyFCost(Cache, performing);
                }
            }
        }
        return Cache;
    }

    public int GetHCost(Unit performing)
    {
        int Cache = hCost;
        if (modifiers != null)
        {
            foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
            {
                if (kvp.Value.IsValid(performing))
                {
                    Cache = kvp.Value.ModifyHCost(Cache, performing);
                }
            }

        }
            return Cache;
    }

    public int GetGCost(Unit performing)
    {
        int Cache = gCost;
        if (modifiers != null)
        {
            foreach (KeyValuePair<string, PathNodeModifier> kvp in modifiers)
            {
                if (kvp.Value.IsValid(performing))
                {
                    Cache = kvp.Value.ModifyGCost(Cache, performing);
                }
            }
        }
            return Cache;
    }
    public void ManuallyRemoveNeighbour(PathfindingNode toRemove)
    {
        int index = -1;
        if (neighbours == null || !DoWeHaveLinkToNode(toRemove, out index))
        {
            return;
        }
        neighbours.RemoveAt(index);
    }


    public void ManuallyAddNeighbour(PathfindingNode toAdd)
    {
        if (neighbours == null)
        {
            neighbours = new List<PathfindingNeighbour>(4);
        }
       
        AddNeighbour(toAdd);
       
    }
}

public class PathfindingNeighbour
{
    public PathfindingNode Node;
    public bool IsAccessable = true;
    public PathfindingNeighbour(PathfindingNode node)
    {
        Node = node;
    }

    public void SetLinkAccessable(bool val)
    {
        IsAccessable = val;
    }
}
