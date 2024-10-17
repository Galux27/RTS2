using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_BehaviourDecisionMaker : BehaviourDecisionMaker
{
    public override void PerformBehaivourUpdate(Unit toCheck)
    {
        Unit UnitNearMe=null;
        float distToNear = 9999999f;
        for (int x = 0; x < UnitMoniter.Instance.AllUnits.Count; x++) {
            if (UnitMoniter.Instance.AllUnits[x].MyType!=UnitType.Zombie)
            {
                float dist = Vector3.Distance(toCheck.transform.position, UnitMoniter.Instance.AllUnits[x].transform.position);
                if (dist < distToNear && dist<5f)
                {
                    distToNear= dist;
                    UnitNearMe = UnitMoniter.Instance.AllUnits[x];
                }
            }
        }
        if (UnitNearMe != null)
        {
            if (currentBehaviour==null || currentBehaviour.GetType() != typeof(ZombieAttackTarget_Behaviour))
            {
                ZombieAttackTarget_Behaviour zombieFollowTarget_Behaviour = new ZombieAttackTarget_Behaviour();
                zombieFollowTarget_Behaviour.InitBehaviour(UnitNearMe, toCheck);
                currentBehaviour = zombieFollowTarget_Behaviour;

            }
        }
        else
        {
            if (currentBehaviour==null || currentBehaviour.GetType() != typeof(ZombieRoam_Behaviour))
            {
                ZombieRoam_Behaviour zombieRoam_Behaviour = new ZombieRoam_Behaviour();
                zombieRoam_Behaviour.InitRoamBehaviour((Zombie)toCheck);
                currentBehaviour = zombieRoam_Behaviour;
            }
        }

    }
}
