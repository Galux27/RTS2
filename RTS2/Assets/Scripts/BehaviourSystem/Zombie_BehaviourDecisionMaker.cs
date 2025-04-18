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
    public override void OnUnitAttacked(Unit attackingUnit)
    {
        if(UnitThatAttacked == null)
        {
            UnitThatAttacked = attackingUnit;
        }
    }
    public void OnNewTile(Vector2Int coords)
    {
        if (ObjectAttacking == null)
        {
            PathfindingNode node = Pathfinding.GetNodeFromCoords(coords);
            if (node != null)
            {
                currentBehaviour = null;
            }
        }
    }


    void PerformPassiveZombieBehaviour(Unit toCheck)
    {
        PathfindingNode nodeAtPosition = Pathfinding.GetNodeFromPosition(toCheck.transform.position);
        if(nodeAtPosition != null)
        {
            if (nodeAtPosition.GetPassable(toCheck) == false && toCheck.lastCoords!=Vector2Int.zero)
            {
                MoveTo_Behaviour moveTo = new MoveTo_Behaviour();
                moveTo.InitBehaviour(toCheck, Pathfinding.GetNodeFromCoords(toCheck.lastCoords).worldPos,false);
                currentBehaviour = moveTo;

            }
        }



        Unit UnitNearMe = BehaviourUtilities.GetClosestTargetThatsHostile(toCheck, 5f);
       
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
                    if (currentBehaviour == null || currentBehaviour.GetType() != typeof(ZombieRoam_Behaviour))
                    {
                        ZombieRoam_Behaviour zombieRoam_Behaviour = new ZombieRoam_Behaviour();
                        zombieRoam_Behaviour.InitRoamBehaviour((Zombie)toCheck);
                        currentBehaviour = zombieRoam_Behaviour;
                    }
                }
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
