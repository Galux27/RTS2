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
          
            b = value; } }
    bool behaviourOverridden = false;
    public BehaviourState CurrentState;
    public float TimeStateSet = 0f;
    public Unit LinkedUnit;
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
        if (state != CurrentState)
        {
            CurrentState = state;
            currentBehaviour = null;
            TimeStateSet = GameTime.Instance.InGameTime;
            if (state != BehaviourState.Idle)
            {
                StopFollowingUnit();
            }
        }
     }

        public void OverrideBehaviour(BehaviourBase toOverrideWith){
        currentBehaviour = toOverrideWith;
        behaviourOverridden = true;
    }

    void StopFollowingUnit()
    {
        if (LinkedUnit != null)
        {
            LinkedUnit.BehaviourRunner.myDecisionMaker.OnUnlinkUnit(performing);
        }
    }

    public bool init = false;
    Unit performing = null;
    public virtual void InitBehaviourMaker(Unit performing)
    {
        this.performing = performing;
        init = true;
    }
    public virtual List<string> DecisionMakerDebug(Vector3 pos)
    {
        return null;
    }
    public virtual void OnUnlinkUnit(Unit unlinking)
    {

    }
}

public enum BehaviourState
{
    Idle,
    Alerted,
    Hostile
}
