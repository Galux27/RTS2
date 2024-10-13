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
                Debug.Log("Checking unit " + UnitMoniter.Instance.AllUnits[x].MyType + "  " + UnitMoniter.Instance.AllUnits[x].gameObject.name);
                float dist = Vector3.Distance(toCheck.transform.position, UnitMoniter.Instance.AllUnits[x].transform.position);
                if (dist < distToNear && dist<5f)
                {
                    distToNear= dist;
                    UnitNearMe = UnitMoniter.Instance.AllUnits[x];
                }
            }
        }
        Debug.Log("Zombie Decision unit null " + (UnitNearMe == null));
        if (UnitNearMe != null)
        {
            if (currentBehaviour==null || currentBehaviour.GetType() != typeof(ZombieFollowTarget_Behaviour))
            {
                Debug.Log("Zombie Decision current behaviour is null " + (currentBehaviour == null));
                ZombieFollowTarget_Behaviour zombieFollowTarget_Behaviour = new ZombieFollowTarget_Behaviour();
                zombieFollowTarget_Behaviour.InitBehaviour(UnitNearMe.gameObject, toCheck);
                currentBehaviour = zombieFollowTarget_Behaviour;
             Debug.Log("Zombie Decision added follow behaviour" + (currentBehaviour.GetType()==typeof(ZombieFollowTarget_Behaviour)));

            }
        }
        else
        {
            if (currentBehaviour==null || currentBehaviour.GetType() != typeof(ZombieRoam_Behaviour))
            {
                ZombieRoam_Behaviour zombieRoam_Behaviour = new ZombieRoam_Behaviour();
                zombieRoam_Behaviour.InitRoamBehaviour((Zombie)toCheck);
                currentBehaviour = zombieRoam_Behaviour;
                Debug.Log("Zombie Decision added roam behaviour" + (currentBehaviour.GetType()==typeof(ZombieRoam_Behaviour)));

            }
        }

    }
}
