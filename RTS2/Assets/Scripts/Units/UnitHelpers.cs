using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitHelpers 
{
    public static List<Vector3> GetRelativePositionsForUnitsToMoveTo(List<Selectable> toMove,Vector3 target)
    {
        List<Vector3> retVal = new List<Vector3>();
        Vector3 centerOfExisting = Vector3.zero;
        for(int x = 0; x < toMove.Count; x++)
        {
            centerOfExisting +=  ((Unit)toMove[x]).Position();
        }
        centerOfExisting /= toMove.Count;

        for(int x = 0; x < toMove.Count; x++)
        {
            retVal.Add(target+ (((Unit)toMove[x]).Position() - centerOfExisting));
        }


        return retVal;
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
