using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourRunner : MonoBehaviour, Updater
{
    Unit UnitPerforming;
    public BehaviourDecisionMaker myDecisionMaker;
    public Action<BehaviourBase> OnBehaviourChange;
    public void SetDecisionMaker(BehaviourDecisionMaker decisionMaker) 
    {  
        myDecisionMaker = decisionMaker; 
    
    }
    public Vector3 GetPosition()
    {
        return UnitPerforming.Position();
    }

    public void SetBehaviour(BehaviourBase toPerform)
    {
        if (CurrentBehaviour != null)
        {
            CurrentBehaviour.OnDestroy();
        }
        CurrentBehaviour = toPerform;
        OnBehaviourChange?.Invoke(toPerform);
        LastBehaviourSet = GameTime.Instance.InGameTime;
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
    public bool IsBehaviourNull = true,breakpointBeforeRun=false;
    public string behaviourName;
    public List<string> BehaviourDebug,BehaviourDecisionDebug;
    public float LastUpdate, LastBehaviourSet,CurTime,LastLimitedUpdate;
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
    public bool DebugOutBehaviourDetails = false;
    public void OnEveryFrame()
    {
        IsBehaviourNull = CurrentBehaviour == null;
        if (breakpointBeforeRun)
        {
            Debug.Log("Breakpoint trigger");
        }
        CurTime= GameTime.Instance.InGameTime;
        if (CurrentBehaviour != null)
        {
            if (CurrentBehaviour != null && CurrentBehaviour.CanPerformBehaviour())
            {

                CurrentBehaviour.PerformBehaviour();
                LastUpdate = GameTime.Instance.InGameTime;
            }

            if (CurrentBehaviour != null && CurrentBehaviour.IsBehaviourComplete())
            {

                OnBehaviourComplete();
            }
#if UNITY_EDITOR
            if (DebugOutBehaviourDetails || DebugCheats.Instance.DoWeLogBehaviourDetails())
            {
                if (CurrentBehaviour != null)
                {
                    BehaviourDebug = CurrentBehaviour.GetDebugData();
                    BehaviourDebug.Add("Can Perform: " + CurrentBehaviour.CanPerformBehaviour());
                }
                else
                {
                    BehaviourDebug = new List<string> { "Null" };
                }
            }
#endif
            //if(myDecisionMaker != null)
            //{
            //    BehaviourDecisionDebug = myDecisionMaker.DecisionMakerDebug(this.transform.position);
            //}
            //else
            //{
            //    BehaviourDecisionDebug = new List<string> { "Null" };
            //}
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
        if (breakpointBeforeRun)
        {
            Debug.Log("Breakpoint trigger");
        }
        {
            LastLimitedUpdate= GameTime.Instance.InGameTime;
            myDecisionMaker.PerformBehaivourUpdate(UnitPerforming);
        }
    }

    private void OnDestroy()
    {
        if (ManualUpdater.Instance != null)
        {
            ManualUpdater.Instance.RemoveUpdater(this);
        }
    }
}
