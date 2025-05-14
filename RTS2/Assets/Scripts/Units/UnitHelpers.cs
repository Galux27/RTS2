using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitHelpers 
{
    public static List<Vector3> GetWalkablePositionsNearTarget(List<Selectable> toMove,Vector3 target)
    {
     
        List<Vector3> closeResults = new List<Vector3>();

        HashSet<PathfindingNode> checkedNodes = new HashSet<PathfindingNode>();
        List<PathfindingNode> toCheck = new List<PathfindingNode>();
        toCheck.Add(Pathfinding.GetNodeFromPosition(target));
        while (closeResults.Count < toMove.Count)
        {
            List<PathfindingNode> newToCheck = new List<PathfindingNode>();
            for(int x = 0; x < toCheck.Count; x++)
            {
                if (toCheck[x].IsPassable)
                {
                    closeResults.Add(toCheck[x].worldPos);
                    checkedNodes.Add(toCheck[x]);
                }
                for(int q = 0; q < toCheck[x].neighbours.Count; q++)
                {
                    if (checkedNodes.Contains(toCheck[x].neighbours[q])==false) {
                        newToCheck.Add(toCheck[x].neighbours[q]);
                    }
                }
            }
            toCheck = newToCheck;
        }

        return closeResults;
    }

    public static void OnUnitCollision(Unit unit1,Unit unit2)
    {
        if (CanSwap(unit1, unit2))
        {
            unit2.SetPassable();

            unit1.HasBeenSwapped = true;
            unit2.HasBeenSwapped = true;
        }
    }

    static bool CanSwap(Unit unit1,Unit unit2)
    {
        if (unit1.HasBeenSwapped || unit2.HasBeenSwapped)
        {
            return false;
        }
        if (unit1.MyFaction.MyFactionID != unit2.MyFaction.MyFactionID)
        {
            return false;
        }

        return true;
    }
}
