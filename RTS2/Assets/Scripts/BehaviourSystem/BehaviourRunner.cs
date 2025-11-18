using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourRunner : MonoBehaviour
{
    Unit UnitPerforming;
    BehaviourDecisionMaker myDecisionMaker;
    public Action<BehaviourBase> OnBehaviourChange;
    public void SetDecisionMaker(BehaviourDecisionMaker decisionMaker) 
    {  
        myDecisionMaker = decisionMaker; 
    
    }

    public void SetBehaviour(BehaviourBase toPerform)
    {
        CurrentBehaviour = toPerform;
        OnBehaviourChange?.Invoke(toPerform);
    }

    public string GetBehaviourDisplayText()
    {
        if (CurrentBehaviour != null)
        {
            return CurrentBehaviour.BehaviourType();
        }
        return "Idle";
    }


    public void SetUnitPerforming(Unit toPerform)
    {
        UnitPerforming = toPerform;
        toPerform.OnAttacked += myDecisionMaker.OnUnitAttacked;
    }


    public BehaviourBase CurrentBehaviour
    {
        get
        {
            return myDecisionMaker.currentBehaviour;
        }
        set
        {
            myDecisionMaker.OverrideBehaviour(value);
        }
    }
    public bool IsBehaviourNull = true;
    public string behaviourName;
    private void Update()
    {
       

        IsBehaviourNull = CurrentBehaviour == null;
        if (!IsBehaviourNull)
        {
            behaviourName = CurrentBehaviour.GetType().ToString();
        }
        if (myDecisionMaker == null) { return; }
        if (CurrentBehaviour == null)
        {
            myDecisionMaker.PerformBehaivourUpdate(UnitPerforming);
        }
        if ( CurrentBehaviour != null)
        {
            if(CurrentBehaviour!=null && CurrentBehaviour.CanPerformBehaviour())
            {

                CurrentBehaviour.PerformBehaviour();
            }

            if (CurrentBehaviour!=null && CurrentBehaviour.IsBehaviourComplete())
            {

                OnBehaviourComplete();
            }

        }
    }

    void OnBehaviourComplete()
    {

        CurrentBehaviour.OnComplete?.Invoke();
        if (CurrentBehaviour.DoWeNullBehaviourOnComplete())
        {
            CurrentBehaviour = null;
        }
    }
}
