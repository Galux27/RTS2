using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Zombie_BehaviourDecisionMaker : BehaviourDecisionMaker
{

    Unit UnitThatAttacked;

    bool gotObjectToAttack = false;
    ObjectInfo ObjectAttacking;
    const float DistToLoseAttacker = 150f;
    ListeningEvent triggeredEvent = null;
    public override void OnUnitAttacked(Unit attackingUnit)
    {
        if(UnitThatAttacked == null)
        {
            UnitThatAttacked = attackingUnit;
        }
    }
    public void OnNewTile(Vector2Int coords)
    {
        //if (ObjectAttacking == null)
        //{
        //    PathfindingNode node = Pathfinding.GetNodeFromCoords(coords);
        //    if (node != null)
        //    {
        //        currentBehaviour = null;
        //    }
        //}
    }

 
    public override void CheckToSeeIfStateShouldChange(Unit toCheck)
    {
        if (!CanChangeState(toCheck))
        {
            return;
        }
        Unit UnitNearMe = BehaviourUtilities.GetClosestTargetThatsHostile(toCheck, toCheck.MySenses.Sight);
        if (UnitNearMe != null)
        {
            UnitThatAttacked = UnitNearMe;
            SetState(BehaviourState.Hostile);
            return;
        }
        triggeredEvent = ListeningEventController.Instance.GetEventInRange(toCheck.MySenses.Hearing, toCheck.transform.position);
        if(triggeredEvent != null)
        {
            SetState(BehaviourState.Alerted);
            return;
        }
        
        if (CurrentState==BehaviourState.Hostile)
        {
            if(UnitThatAttacked==null|| Vector3.Distance(toCheck.transform.position, UnitThatAttacked.transform.position) > toCheck.MySenses.Sight*2)
            {
                UnitThatAttacked = null;
                SetState(BehaviourState.Idle);
                return;
            }
        }
        else if(CurrentState == BehaviourState.Alerted)
        {
            if (TimeStateSet + toCheck.MySenses.Memory > GameTime.Instance.InGameTime)
            {
                SetState(BehaviourState.Idle);
                return;
            }
        }
        else
        {
            SetState(BehaviourState.Idle);
            return;
        }
    }

    bool CanChangeState(Unit toCheck) {
        if (currentBehaviour == null)
        {
            return true;
        }
        switch (CurrentState)
        {
            case BehaviourState.Idle:
                return true;
                break;
            case BehaviourState.Alerted:
                return ShouldChangeFromAlerted(toCheck);
                break;
            case BehaviourState.Hostile:
                return ShouldChangeFromHostile(toCheck);
                break;
            default:
                break;
        }
        return true;
    }

    public override List<string> DecisionMakerDebug(Vector3 pos)
    {
        List<string> retVal = new List<string>();
        retVal.Add("State: " + CurrentState.ToString());
        retVal.Add("Set at " + TimeStateSet);
        retVal.Add("Cur time " + GameTime.Instance.InGameTime);
        if (UnitThatAttacked != null)
        {
            retVal.Add("Dist: " + Vector3.Distance(UnitThatAttacked.transform.position, pos));
        }
        return retVal;
    }

    bool ShouldChangeFromHostile(Unit toCheck)
    {
        if (UnitThatAttacked == null)
        {
            return true;
        }
        float distToTarget = Vector3.Distance(toCheck.transform.position, UnitThatAttacked.transform.position);
        if (distToTarget > toCheck.MySenses.Sight * 2)
        {
            return true;
        }
        if(TimeStateSet + toCheck.MySenses.Memory > GameTime.Instance.InGameTime)
        {
            if (distToTarget > toCheck.MySenses.Sight)
            {
                return true;
            }
        }
        return false;
    }

    bool ShouldChangeFromAlerted(Unit toCheck)
    {
        if (TimeStateSet + toCheck.MySenses.Memory > GameTime.Instance.InGameTime||ObjectAttacking==null)
        {
            return true;
        }else if (triggeredEvent != null && Vector3.Distance(triggeredEvent.Position, toCheck.transform.position) < 2f)
        {
            return true;
        }
        return false;
    }

    void PerformIdleBehaviour(Unit toCheck) 
    {

        if (currentBehaviour == null || currentBehaviour.GetType() != typeof(ZombieRoam_Behaviour))
        {
            ZombieRoam_Behaviour zombieRoam_Behaviour = new ZombieRoam_Behaviour();
            zombieRoam_Behaviour.InitRoamBehaviour((Zombie)toCheck);
            currentBehaviour = zombieRoam_Behaviour;
        }

    }

    void PerformAlertedBehaviour(Unit toCheck)
    {
        if (triggeredEvent != null)
        {
            if (Vector3.Distance(triggeredEvent.Position, toCheck.transform.position) > 3f)
            {
                if (currentBehaviour==null||currentBehaviour.GetType() != typeof(MoveTo_Behaviour) 
                    || currentBehaviour.GetType() == typeof(MoveTo_Behaviour)&&Vector3.Distance( (currentBehaviour as MoveTo_Behaviour).TargetPosition, triggeredEvent.Position)>5)
                {
                    MoveTo_Behaviour moveTo = new MoveTo_Behaviour();
                    moveTo.InitBehaviour(toCheck, triggeredEvent.Position, true);
                    currentBehaviour = moveTo;
                }

            }
            else
            {
                triggeredEvent = null;
            }
        }

        ObjectAttacking = BehaviourUtilities.GetNearbyWallSegmentToAttack(toCheck, out gotObjectToAttack);
        if (gotObjectToAttack)
        {
            ZombieAttackObject_Behaviour zombieAttackObject_Behaviour = new ZombieAttackObject_Behaviour();
            zombieAttackObject_Behaviour.InitBehaviour(ObjectAttacking, toCheck);
            currentBehaviour = zombieAttackObject_Behaviour;

        }
        else
        {
            ObjectAttacking = BehaviourUtilities.GetNearbyObjectToAttack(toCheck, out gotObjectToAttack);
            if (gotObjectToAttack)
            {
                ZombieAttackObject_Behaviour zombieAttackObject_Behaviour = new ZombieAttackObject_Behaviour();
                zombieAttackObject_Behaviour.InitBehaviour(ObjectAttacking, toCheck);
                currentBehaviour = zombieAttackObject_Behaviour;

            }
            else
            {

            } 
        }
    }

    void PerformHostileBehaviour(Unit toCheck)
    {
        if (currentBehaviour == null || currentBehaviour.GetType() != typeof(ZombieAttackTarget_Behaviour))
        {
            ZombieAttackTarget_Behaviour zombieFollowTarget_Behaviour = new ZombieAttackTarget_Behaviour();
            zombieFollowTarget_Behaviour.InitBehaviour(UnitThatAttacked, toCheck);
            currentBehaviour = zombieFollowTarget_Behaviour;

        }
    }


    public override void InitBehaviourMaker(Unit performing)
    {
        base.InitBehaviourMaker(performing);
        performing.OnEnterNewTile += OnNewTile;
    }

    public override void PerformBehaivourUpdate(Unit toCheck)
    {
        if (!init)
        {
            InitBehaviourMaker(toCheck);
        }
        CheckToSeeIfStateShouldChange(toCheck);
        switch (CurrentState)
        {
            case BehaviourState.Idle:
                PerformIdleBehaviour(toCheck);
                break;
            case BehaviourState.Alerted:
                PerformAlertedBehaviour(toCheck);
                break;
            case BehaviourState.Hostile:
                PerformHostileBehaviour(toCheck);
                break;
            default:
                break;
        }
    }
}


