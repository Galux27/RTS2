using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human_BehaviourDecisionMaker : BehaviourDecisionMaker
{
    Unit UnitThatAttacked;
    public override void OnUnitAttacked(Unit attackingUnit)
    {
        if (UnitThatAttacked == null)
        {
            UnitThatAttacked = attackingUnit;
        }
        Debug.Log("Human attacked by " + attackingUnit.gameObject.name + "|" + attackingUnit.gameObject.transform.position);
    }


    void CheckForHostilesNearby(Unit performingBehaviour)
    {
        if (performingBehaviour.GetOrderVal(OrderConstants.ORDER_ATTACK_NEARBY_ENEMIES)==false)
        {
            return;
        }

        Unit target = BehaviourUtilities.GetClosestTargetThatsHostile(performingBehaviour, performingBehaviour.GetComponent<UnitSenses>().Sight);

        if (target != null)
        {
            HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
            attack.InitBehaviour(target, performingBehaviour);
            performingBehaviour.BehaviourRunner.SetBehaviour(attack);
        }
    
    }

    public override void PerformBehaivourUpdate(Unit performingBehaviour)
    {
        if(UnitThatAttacked != null)
        {
            bool canRetaliate = false;
            if (performingBehaviour.BehaviourRunner.CurrentBehaviour == null)
            {
                canRetaliate = true;
            }

            if (performingBehaviour.ItemHolder.IsHoldingWeapon()==false)
            {
                canRetaliate = false;
            }


            if (currentBehaviour!=null && currentBehaviour.IsUserInstruction)
            {
                canRetaliate = false;
            }

            if (canRetaliate)
            {
                if (performingBehaviour.GetOrderVal(OrderConstants.ORDER_DEFEND_SELF)) {
                    HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                    attack.InitBehaviour(UnitThatAttacked, performingBehaviour);
                    performingBehaviour.BehaviourRunner.SetBehaviour(attack);
                } 
                else if (performingBehaviour.GetOrderVal(OrderConstants.ORDER_FLEE_DANGER))
                {
                    MoveTo_Behaviour move = new MoveTo_Behaviour();
                    Vector3 fleeTo = BehaviourUtilities.GetPositionAwayFromTarget(UnitThatAttacked.transform.position);
                    move.InitBehaviour(performingBehaviour,fleeTo);
                    Debug.Log("Fleeing to " + fleeTo);
                    performingBehaviour.BehaviourRunner.SetBehaviour(move);
                }
            }
        }
        else
        {
            CheckForHostilesNearby(performingBehaviour);
        }
    }

}
