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


        GenerateDirectionToRoam();
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
               
                directionChangeTimer = 0f;
            }
        }
    }

    void GenerateDirectionToRoam()
    {
        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        if(unitToMove.transform.position.x<1f && direction.x < 0f)
        {
            direction.x = 1;
        }else if(unitToMove.transform.position.x > WorldController.Instance.WorldWidth - 2 && direction.x>0f)
        {
            direction.x = -1f;
        }

        if (unitToMove.transform.position.y < 1f && direction.y < 0f)
        {
            direction.y = 1;
        }
        else if (unitToMove.transform.position.y > WorldController.Instance.WorldHeight - 2 && direction.y > 0f)
        {
            direction.y = -1f;
        }
        direction = direction.normalized;
    }
}
