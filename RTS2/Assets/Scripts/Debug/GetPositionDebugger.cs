using System.Collections.Generic;
using UnityEngine;

public class GetPositionDebugger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public bool UpdatePoints = false;
    public int PostitionsToGet = 10;
    public Transform TargetPos;
    List<PathfindingNode> targetPositions=new List<PathfindingNode>();
    PathfindingNode NodeAtTarget;
    // Update is called once per frame
    void Update()
    {
        if (UpdatePoints)
        {
            NodeAtTarget = Pathfinding.GetNodeFromPosition(TargetPos.transform.position);
            targetPositions = UnitHelpers.GetWalkableNodesNearTarget(TargetPos.transform.position, PostitionsToGet);
            UpdatePoints = false;
        }
        Color c = Color.black;

        for (int x=0;x<targetPositions.Count;x++)
        {
            c = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, targetPositions.Count, x));
            Debug.DrawLine(targetPositions[x].worldPos, targetPositions[x ].worldPos + Vector3.up, c);


        }

        Debug.DrawLine(NodeAtTarget.worldPos, NodeAtTarget.worldPos + Vector3.up, Color.red);

    }
}
