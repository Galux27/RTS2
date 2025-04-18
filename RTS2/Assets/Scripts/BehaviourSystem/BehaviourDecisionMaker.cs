using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for logic that decides what behaviour a unit will perform
/// </summary>
public class BehaviourDecisionMaker
{
    BehaviourBase b;

    public BehaviourBase currentBehaviour { get { return b; } set { Debug.Log("Setting behaviour to " + (value==null)); b = value; } }
    bool behaviourOverridden = false;
    public virtual void PerformBehaivourUpdate(Unit performingBehaviour)
    {

    }

    public virtual void OnUnitAttacked(Unit attackingUnit)
    {

    }

    public void OverrideBehaviour(BehaviourBase toOverrideWith){
        currentBehaviour = toOverrideWith;
        behaviourOverridden = true;
    }

    public bool init = false;

    public virtual void InitBehaviourMaker(Unit performing)
    {
        init = true;
    }

}
