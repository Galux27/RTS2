using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRoam_Behaviour :BehaviourBase
{
    float MinDistFrom = 1f;

    Vector3 direction = Vector3.zero;
    float directionChangeTimer = 0f;
    const float directionChangeTimerLength = 5f;
    public void InitRoamBehaviour( Zombie me)
    {
        InitBehaviour(me);
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }



    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return false;
    }


    Vector3 DirectionToTarget()
    {

        return direction;
    }

    public override void PerformBehaviour()
    {
        if (!IsBehaviourComplete())
        {
            unitToMove.MoveUnit(DirectionToTarget());
            directionChangeTimer += Mathf.Max(DeltaTimeWrapper.GameplayDelta,0.01f);
            if (directionChangeTimer > directionChangeTimerLength)
            {
                direction=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f), Random.Range(-1f, 1f));
                directionChangeTimer = 0f;
            }
        }


    }
}
