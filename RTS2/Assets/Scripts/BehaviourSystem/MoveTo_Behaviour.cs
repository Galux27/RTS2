using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo_Behaviour : BehaviourBase
{
    public Vector3 TargetPosition;
    List<PathfindingNode> pathfindingNodes;
    public void InitBehaviour(Unit toPerform,Vector3 targetPos)
    {
        base.InitBehaviour(toPerform);
        TargetPosition = targetPos;
        pathfindingNodes = Pathfinding.FindPath(toPerform.transform.position, targetPos);
    }

    public override bool CanPerformBehaviour()
    {
        return Vector3.Distance(unitToMove.transform.position, TargetPosition) >1f && unitToMove!=null;
    }

    public override bool IsBehaviourComplete()
    {
        return Vector3.Distance(unitToMove.transform.position, TargetPosition) < 1f;
    }

    Vector3 DirectionToTarget()
    {
        return (TargetPosition - unitToMove.transform.position).normalized;
    }

    public override void PerformBehaviour()
    {
        if(!IsBehaviourComplete())
        {
            unitToMove.MoveUnit(DirectionToTarget());
        }

        if(pathfindingNodes != null)
        {
            for(int x=0;x<pathfindingNodes.Count-1;x++)
            {
                Debug.DrawLine(pathfindingNodes[x].worldPos, pathfindingNodes[x + 1].worldPos, Color.magenta);

            }
        }
    }
}
