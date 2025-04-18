using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for a behaviour that a unit can perform e.g. move to location, attack other unit...
/// </summary>
public class BehaviourBase
{
    protected Unit unitToMove;

    public Action OnComplete;
    public bool IsUserInstruction = false;

    public virtual void InitBehaviour(Unit toPerform)
    {
        unitToMove= toPerform; 
    }


    public virtual bool CanPerformBehaviour()
    {
        return false;
    }

    public virtual void PerformBehaviour()
    {

    }

    public virtual bool IsBehaviourComplete()
    {
        return false;
    }

    public virtual bool DoWeNullBehaviourOnComplete()
    {
        return false;
    }
}
