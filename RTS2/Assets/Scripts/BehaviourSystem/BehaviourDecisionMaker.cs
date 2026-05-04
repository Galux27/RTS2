using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for logic that decides what behaviour a unit will perform
/// </summary>
[System.Serializable]
public class BehaviourDecisionMaker
{
    BehaviourBase b;

    public BehaviourBase currentBehaviour { get { return b; } set {
            //if (value != null)
            //{
            //    Debug.Log("Setting behaviour: to " + value.BehaviourType()+","+(b == null));
            //}
            //else
            //{
            //    Debug.Log("Setting behaviour: to null" + (b==null));

            //}
            b = value; } }
    bool behaviourOverridden = false;
    public BehaviourState CurrentState;
    public float TimeStateSet = 0f;
    public virtual void PerformBehaivourUpdate(Unit performingBehaviour)
    {

    }

    public virtual void OnUnitAttacked(Unit attackingUnit)
    {

    }

    public virtual void CheckToSeeIfStateShouldChange(Unit toCheck)
    {

    }

    public virtual void SetState(BehaviourState state)
    {
        CurrentState = state;
        currentBehaviour = null;
        TimeStateSet = GameTime.Instance.InGameTime;
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
    public virtual List<string> DecisionMakerDebug(Vector3 pos)
    {
        return null;
    }

}

public enum BehaviourState
{
    Idle,
    Alerted,
    Hostile
}
