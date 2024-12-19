using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for modifying how nodes affect paths e.g. 
/// having a flooring tile to reduce weight, if a tile was on fire massivly increase the weight etc....
/// </summary>
public class PathNodeModifier
{
    public string modifierKey = "None";

    public virtual bool IsValid(Unit performing)
    {
        return false;
    }


    public virtual bool ModifyWalkable(bool originalVal,Unit performing)
    {
        return originalVal;
    }

    public virtual int ModifyFCost(int originalCost,Unit performing)
    {
        return originalCost;
    }

    public virtual int ModifyHCost(int originalCost, Unit performing)
    {
        return originalCost;
    }

    public virtual int ModifyGCost(int originalCost,Unit performing)
    {
        return originalCost;
    }
}
