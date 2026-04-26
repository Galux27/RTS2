using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;

public class ZombieRoam_Behaviour :BehaviourBase
{
    float MinDistFrom = 1f;

    Vector3 direction = Vector3.zero;
    float directionChangeTimer = 0f;
    int count = 0;
    const float directionChangeTimerLength = 2f;
    public void InitRoamBehaviour( Zombie me)
    {
        InitBehaviour(me);


        GenerateDirectionToRoam();
    }

    public override void InitializeFromData(Unit performing, Dictionary<string, object> data)
    {
        InitRoamBehaviour((Zombie)performing);
        direction = (Vector3)data[DataKeys.Pos];
    }


    public override bool CanPerformBehaviour()
    {
        return true;
    }

    public override bool IsBehaviourComplete()
    {
        return count > 0;
    }


    Vector3 DirectionToTarget()
    {

        return direction;
    }
   

    public override void PerformBehaviour()
    {

        if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position,direction,unitToMove))
            {
                unitToMove.MoveUnit(DirectionToTarget());
            }
            else
            {
                directionChangeTimer = directionChangeTimerLength;
            }
            directionChangeTimer += Mathf.Max(DeltaTimeWrapper.GameplayDelta,0.01f);

        if (directionChangeTimer > directionChangeTimerLength)
        {
            GenerateDirectionToRoam();
            if (BehaviourUtilities.CanIMoveInDirection(unitToMove.transform.position, direction, unitToMove))
            {
                directionChangeTimer = 0f;
                count++;
            }

            }

        }

    public override DataToSerialize GetBehaviourSpecificData()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.Pos, direction);
        return data;
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

    public override bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }
}
