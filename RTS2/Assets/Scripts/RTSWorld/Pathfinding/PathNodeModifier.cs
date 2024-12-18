using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for modifying how nodes affect paths e.g. 
/// having a flooring tile to reduce weight, if a tile was on fire massivly increase the weight etc....
/// </summary>
public class PathNodeModifier
{
    List<string> FactionsIsValidFor = new List<string>();

    public bool IsValid()
    {
        return false;
    }


    public virtual bool ModifyWalkable(bool originalVal)
    {
        return originalVal;
    }

    public virtual int ModifyFCost(int originalCost)
    {
        return originalCost;
    }
}
