using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Units_SelectionMode : SelectionMode
{
    public override void OnLeftMouseUp()
    {
        SelectableManager.Instance.ClearSelectables();
        List<Unit> selected = UnitMoniter.Instance.GetUnitsWithinBounds(CursorSelect.Instance.startPoint, CursorSelect.Instance.endPoint);
        Debug.Log("Selected unit count " + selected.Count);
        for (int x = 0; x < selected.Count; x++)
        {
            SelectableManager.Instance.AddSelectable(selected[x]);
        }
    }


    public override void OnRightMouseUp()
    {
        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {
            if (Input.GetMouseButtonUp(1))
            {

                bool DoneCommand = false;

               
                Ray r = CursorSelect.Instance.Camera.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.UnitLayermask);
                if (hit.collider != null)
                {
                    Unit targetUnit = hit.collider.gameObject.GetComponent<Unit>();
                    for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                    {
                        Unit toPerfrom = ((Unit)SelectableManager.Instance.CurrentlySelected[x]);
                        BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                        HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                        attack.InitBehaviour(targetUnit, toPerfrom);
                        br.SetBehaviour(attack);

                    }

                    DoneCommand = true;
                }

                if (!DoneCommand)
                {


                    hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.CursorLayermask);
                    if (hit.collider != null)
                    {
                        Vector3 targetPos = hit.point;
                        targetPos.z = 0;

                        for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)SelectableManager.Instance.CurrentlySelected[x]);
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            MoveTo_Behaviour moveTo_Behaviour = new MoveTo_Behaviour();
                            moveTo_Behaviour.InitBehaviour(toPerfrom, targetPos);
                            br.SetBehaviour(moveTo_Behaviour);
                        }
                    }
                }
            }
        }
    }

   
}
