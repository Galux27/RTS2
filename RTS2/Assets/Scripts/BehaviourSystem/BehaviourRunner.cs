using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourRunner : MonoBehaviour, Updater
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
        if (CurrentBehaviour != null)
        {
            CurrentBehaviour.OnDestroy();
        }
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
        Init();
        
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
    public List<string> BehaviourDebug;
    void OnBehaviourComplete()
    {

        CurrentBehaviour.OnComplete?.Invoke();
        if (CurrentBehaviour.DoWeNullBehaviourOnComplete())
        {
            SetBehaviour( null);
        }
    }
    UpdaterType MyType;
    public UpdaterType GetUpdaterType()
    { 
        return MyType;
    }
    public void Init()
    {
        if (UnitPerforming.MyFaction.MyFactionID == FactionController.USER_FACTION)
        {
            MyType = UpdaterType.User;
        }
        else
        {
            MyType = UpdaterType.AI;
        }
        ManualUpdater.Instance.AddUpdater(this);

    }

    public void OnEveryFrame()
    {
        IsBehaviourNull = CurrentBehaviour == null;
      
        if (CurrentBehaviour != null)
        {
            if (CurrentBehaviour != null && CurrentBehaviour.CanPerformBehaviour())
            {

                CurrentBehaviour.PerformBehaviour();
            }

            if (CurrentBehaviour != null && CurrentBehaviour.IsBehaviourComplete())
            {

                OnBehaviourComplete();
            }
            if (CurrentBehaviour != null)
            {
                BehaviourDebug = CurrentBehaviour.GetDebugData();
            }
            else
            {
                BehaviourDebug = new List<string> {"Null" };
            }
        }
    }

    public void LimitedUpdate()
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
    }

    private void OnDestroy()
    {
        ManualUpdater.Instance.RemoveUpdater(this);
    }
}
