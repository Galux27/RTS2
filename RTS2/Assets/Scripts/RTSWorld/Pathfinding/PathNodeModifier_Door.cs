using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathNodeModifier_Door : PathNodeModifier
{
    public PathNodeModifier_Door()
    {
        modifierKey = "Door";
        Debug.Log("Path node modifier door created one ");
    }


    public override bool IsValid(Unit performing)
    {
        return true;
    }

    public override bool ModifyWalkable(bool originalVal, Unit performing)
    {
        Debug.Log("Path node modifier door " + originalVal);
        if (performing.MySenses.Intelligence > 50)
        {
            return originalVal;
        }

        return false;
    }

}
