using System;
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
        GameActionController.Instance.OnActionPerformed();

        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {
            if (Input.GetMouseButtonUp(1))
            {

                bool DoneCommand = false;

               
                Ray r = CursorSelect.Instance.Camera.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.UnitLayermask);
                if (hit.collider != null)
                {

                    System.Action attack = () =>
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
                    };

                    GameAction ga = new GameAction("Attack", attack);
                    GameActionController.Instance.AddAction(ga);
                    //DoneCommand = true;
                }
                else if (OnHoverEnemyUnit != null)
                {
                    System.Action attack = () =>
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
                    };
                    GameAction ga = new GameAction("Attack", attack);
                    GameActionController.Instance.AddAction(ga);
                  //  DoneCommand = true;
                }


                if (OnHoverBuildable != null)
                {
                    Action Build = () =>
                    {

                        for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)SelectableManager.Instance.CurrentlySelected[x]);
                            if (toPerfrom.MyType == UnitType.Engineer)
                            {
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                HumanBehaviour_ConstructObject construct = new HumanBehaviour_ConstructObject();
                                construct.InitBehaviour(toPerfrom, OnHoverBuildable);
                                construct.IsUserInstruction = true;
                                br.SetBehaviour(construct);
                            }
                        }
                    };
                    GameAction ga = new GameAction("Build", Build);
                    GameActionController.Instance.AddAction(ga);
                    DoneCommand = true;
                }


                if (OnHoverHarvestable != null)
                {
                    EnvironmentObjectInstance toHarvest = OnHoverHarvestable;
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;

                    Action Harvest = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            GatherResources_Behaviour gather = new GatherResources_Behaviour();
                            gather.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(gather);

                        }
                    };
                    GameAction ga = new GameAction("Harvest Resource", Harvest);
                    GameActionController.Instance.AddAction(ga);
                }

                if (OnHoverResource != null)
                {
                    Action Gather = () =>
                    {

                        for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            collect.InitBehaviour(toPerfrom, OnHoverResource);
                            br.SetBehaviour(collect);

                        }
                    };
                    GameAction ga = new GameAction("Collect Resource", Gather);
                    GameActionController.Instance.AddAction(ga);
                }


                if (OnHoverInventory != null)
                {
                    Action Gather = () =>
                    {

                        for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();

                            TransferResourcesToContainer_Behaviour storeResoruces = new TransferResourcesToContainer_Behaviour();
                            storeResoruces.InitBehaviour(toPerfrom, OnHoverInventory);
                            br.SetBehaviour(storeResoruces);

                        }
                    };
                    GameAction ga = new GameAction("Store Resources", Gather);
                    GameActionController.Instance.AddAction(ga);


                    Action transfer = () =>
                    {
                        Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[0];
                        Inventory unitInventory = toPerfrom.GetComponent<Inventory>();
                        InventoryParentUI.Instance.PopulateUI(unitInventory, OnHoverInventory);


                    };
                    GameAction tr = new GameAction("Transfer Items", transfer);
                    GameActionController.Instance.AddAction(tr);
                }

                {
                    hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.CursorLayermask);
                    if (hit.collider != null)
                    {
                        Vector3 targetPos = hit.point;
                        targetPos.z = 0;



                        Action move = () =>
                        {

                            for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
                            {
                                Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[x];
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                MoveTo_Behaviour moveTo_Behaviour = new MoveTo_Behaviour();
                                moveTo_Behaviour.InitBehaviour(toPerfrom, targetPos);
                                moveTo_Behaviour.IsUserInstruction = true;
                                br.SetBehaviour(moveTo_Behaviour);

                            }
                        };
                        GameAction ga = new GameAction("Move", move);
                        GameActionController.Instance.AddAction(ga);
                    }
                }
            }
        }
    }


    Unit OnHoverEnemyUnit;
    Unit OnHoverMyUnit;
    ConstructableObjectInstance OnHoverConstructable;
    Constructable OnHoverBuildable;
    EnvironmentObjectInstance OnHoverHarvestable;
    ResourceInstance OnHoverResource;
    Inventory OnHoverInventory;
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

        OnHoverHarvestable = SelectionUtilities.GetHarvestableObjectInstanceWithinRangeOfPoint(r.origin, 1f);

        if (OnHoverHarvestable != null)
        {
            CursorIcon.Instance.SetBuildIcon();
            return;
        }

        OnHoverResource = SelectionUtilities.GetResourceInstanceObjectInstanceWithinRangeOfPoint(r.origin, 1f);
        if (OnHoverResource != null)
        {
            CursorIcon.Instance.SetBuildIcon();
            return;
        }
        OnHoverInventory = SelectionUtilities.GetInventoryObjectWithinRangeOfPoint(r.origin, 1f);
        if (OnHoverInventory != null)
        {
            CursorIcon.Instance.SetMoveIcon();
            return;
        }
        OnHoverBuildable = SelectionUtilities.GetConstructableObjectInstanceWithinRangeOfPoint(r.origin, 1f);
        if (OnHoverBuildable != null)
        {
            CursorIcon.Instance.SetMoveIcon();
            return;
        }

       

      

        CursorIcon.Instance.SetMoveIcon();

    }

}
