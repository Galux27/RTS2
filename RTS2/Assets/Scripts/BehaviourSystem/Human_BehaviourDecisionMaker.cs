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

            Debug.Log("Human can retaliate " + canRetaliate+"|"+ performingBehaviour.ItemHolder.IsHoldingWeapon()+"|"+(performingBehaviour.BehaviourRunner.CurrentBehaviour == null));

            if (canRetaliate)
            {
                HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                attack.InitBehaviour(UnitThatAttacked, performingBehaviour);
                performingBehaviour.BehaviourRunner.SetBehaviour(attack);

            }
        }
    }

}
