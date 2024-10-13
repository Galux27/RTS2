using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourDecisionMaker
{
    BehaviourBase b;
    public BehaviourBase currentBehaviour { get { return b; } set { Debug.Log("Setting behaviour to " + (value==null)); b = value; } }
    bool behaviourOverridden = false;
    public virtual void PerformBehaivourUpdate(Unit performingBehaviour)
    {

    }

    public void OverrideBehaviour(BehaviourBase toOverrideWith){
        currentBehaviour = toOverrideWith;
        behaviourOverridden = true;
    }
}
