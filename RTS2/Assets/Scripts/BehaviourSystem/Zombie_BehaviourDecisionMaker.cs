using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_BehaviourDecisionMaker : BehaviourDecisionMaker
{

    Unit UnitThatAttacked;
    const float DistToLoseAttacker = 150f;
    public override void OnUnitAttacked(Unit attackingUnit)
    {
        if(UnitThatAttacked == null)
        {
            UnitThatAttacked = attackingUnit;
        }
    }


    void PerformPassiveZombieBehaviour(Unit toCheck)
    {
        Unit UnitNearMe = BehaviourUtilities.GetClosestTargetThatsNotType(toCheck, 5f, UnitType.Zombie);
       
        if (UnitNearMe != null)
        {
            if (currentBehaviour == null || currentBehaviour.GetType() != typeof(ZombieAttackTarget_Behaviour))
            {
                ZombieAttackTarget_Behaviour zombieFollowTarget_Behaviour = new ZombieAttackTarget_Behaviour();
                zombieFollowTarget_Behaviour.InitBehaviour(UnitNearMe, toCheck);
                currentBehaviour = zombieFollowTarget_Behaviour;

            }
        }
        else
        {
            if (currentBehaviour == null || currentBehaviour.GetType() != typeof(ZombieRoam_Behaviour))
            {
                ZombieRoam_Behaviour zombieRoam_Behaviour = new ZombieRoam_Behaviour();
                zombieRoam_Behaviour.InitRoamBehaviour((Zombie)toCheck);
                currentBehaviour = zombieRoam_Behaviour;
            }
        }
    }


    void PerformZombieRevengeBehaviour(Unit toCheck)
    {
        float dist = Vector3.Distance(toCheck.transform.position, UnitThatAttacked.transform.position);
        if (dist > DistToLoseAttacker)
        {
            currentBehaviour = null;
            UnitThatAttacked = null;
        }
        else
        {
            if (currentBehaviour == null || currentBehaviour.GetType() != typeof(ZombieAttackTarget_Behaviour))
            {
                ZombieAttackTarget_Behaviour zombieFollowTarget_Behaviour = new ZombieAttackTarget_Behaviour();
                zombieFollowTarget_Behaviour.InitBehaviour(UnitThatAttacked, toCheck);
                currentBehaviour = zombieFollowTarget_Behaviour;

            }
        }

    }


    public override void PerformBehaivourUpdate(Unit toCheck)
    {
        if (UnitThatAttacked == null)
        {
            PerformPassiveZombieBehaviour(toCheck);
        }
        else
        {
            PerformZombieRevengeBehaviour(toCheck);
        }

    }
}
