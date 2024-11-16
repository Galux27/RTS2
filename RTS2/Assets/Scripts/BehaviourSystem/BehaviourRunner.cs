using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourRunner : MonoBehaviour
{
    Unit UnitPerforming;
    BehaviourDecisionMaker myDecisionMaker;
    public void SetDecisionMaker(BehaviourDecisionMaker decisionMaker) 
    {  
        myDecisionMaker = decisionMaker; 
    
    }

    public void SetBehaviour(BehaviourBase toPerform)
    {
        CurrentBehaviour = toPerform; 
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

    private void Update()
    {
        if (myDecisionMaker == null) { return; }

        myDecisionMaker.PerformBehaivourUpdate(UnitPerforming);
        if ( CurrentBehaviour != null)
        {
            if(CurrentBehaviour.CanPerformBehaviour())
            {

                CurrentBehaviour.PerformBehaviour();
            }

            if (CurrentBehaviour.IsBehaviourComplete())
            {

                OnBehaviourComplete();
            }

        }
    }

    void OnBehaviourComplete()
    {
        CurrentBehaviour.OnComplete?.Invoke();
       
    }
}
