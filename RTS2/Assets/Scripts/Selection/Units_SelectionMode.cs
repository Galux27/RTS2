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

        if (selected.Count == 0 && OnHoverMyUnit!=null)
        {
            selected.Add(OnHoverMyUnit);
        }

        for (int x = 0; x < selected.Count; x++)
        {
            SelectableManager.Instance.AddSelectable(selected[x]);
        }
        SelectableManager.OnSelectionChanged?.Invoke();
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
                        attack.IsUserInstruction = true;
                        br.SetBehaviour(attack);

                    }

                    DoneCommand = true;
                }

                if (OnHoverEnemyUnit != null)
                {
                    for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                    {
                        Unit toPerfrom = ((Unit)SelectableManager.Instance.CurrentlySelected[x]);
                        BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                        HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                        attack.InitBehaviour(OnHoverEnemyUnit, toPerfrom);
                        attack.IsUserInstruction = true;
                        br.SetBehaviour(attack);

                    }

                    DoneCommand = true;
                }


                if (OnHoverBuildable != null)
                {
                    for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                    {
                        Unit toPerfrom = ((Unit)SelectableManager.Instance.CurrentlySelected[x]);
                        if (toPerfrom.MyType == UnitType.Engineer)
                        {
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanBehaviour_ConstructObject construct = new HumanBehaviour_ConstructObject();
                            construct.InitBehaviour( toPerfrom, OnHoverBuildable);
                            construct.IsUserInstruction = true;
                            br.SetBehaviour(construct);
                        }
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
                            Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            MoveTo_Behaviour moveTo_Behaviour = new MoveTo_Behaviour();
                            moveTo_Behaviour.InitBehaviour(toPerfrom, targetPos);
                            moveTo_Behaviour.IsUserInstruction = true;
                            br.SetBehaviour(moveTo_Behaviour);

                        }
                    }
                }
            }
        }
    }


    Unit OnHoverEnemyUnit;
    Unit OnHoverMyUnit;
    ConstructableObjectInstance OnHoverConstructable;
    Constructable OnHoverBuildable;
    public override void OnHover()
    {
        Ray r = CursorSelect.Instance.Camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.UnitLayermask);
        bool SetIcon = false;
        if (hit.collider != null)
        {
            Unit targetUnit = hit.collider.gameObject.GetComponent<Unit>();
            if(targetUnit != null)
            {
                if (targetUnit.MyType == UnitType.Zombie)
                {
                    //attack icon
                    CursorIcon.Instance.SetAttackIcon();
                    return;
                }
            }
        }
        OnHoverMyUnit = SelectionUtilities.GetUserUnitWithinRangeOfPoint(r.origin, 1f);


      

        OnHoverEnemyUnit = SelectionUtilities.GetHostileUnitWithinRangeOfPoint(r.origin, 1f);
        if (OnHoverEnemyUnit != null)
        {
            CursorIcon.Instance.SetAttackIcon();
            return;
        }


        OnHoverBuildable = SelectionUtilities.GetConstructableObjectInstanceWithinRangeOfPoint(r.origin, 1f);
        Debug.Log("On Hover Constructable is null " + (OnHoverConstructable==null));
        if (OnHoverBuildable != null)
        {
            CursorIcon.Instance.SetBuildIcon();
            return;
        }

        CursorIcon.Instance.SetMoveIcon();

    }

}
