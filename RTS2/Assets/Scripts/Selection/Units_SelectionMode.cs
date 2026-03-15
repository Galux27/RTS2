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

        if (OnHoverMyUnit!=null)
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
        CheckForActionsToPerform();
        
        GameActionController.Instance.OnManualInput();

    }

    void CheckForActionsToPerform()
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
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    Unit targetUnit = hit.collider.gameObject.GetComponent<Unit>();
                    System.Action attack = () =>
                    {
                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)currentlySelected[x]);
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                            attack.InitBehaviour(targetUnit, toPerfrom);
                            attack.IsUserInstruction = true;
                            br.SetBehaviour(attack);

                        }
                    };

                    GameAction ga = new GameAction("Attack", attack, InputController.Instance.GetShortcutFromType(typeof(HumanAttackUnit_Behaviour)));
                    GameActionController.Instance.AddAction(ga);
                    //DoneCommand = true;
                }
                else if (OnHoverEnemyUnit != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    Unit enemy = OnHoverEnemyUnit;
                    System.Action attack = () =>
                    {
                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)currentlySelected[x]);
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                            attack.InitBehaviour(enemy, toPerfrom);
                            attack.IsUserInstruction = true;
                            br.SetBehaviour(attack);

                        }
                    };
                    GameAction ga = new GameAction("Attack", attack, InputController.Instance.GetShortcutFromType(typeof(HumanAttackUnit_Behaviour)));
                    GameActionController.Instance.AddAction(ga);
                    //  DoneCommand = true;
                }

                if (OnHoverConstructable != null)
                {
                    string convertToType = "";
                    if (UnitTypesController.Instance.CanConvertUnitsWithObject(OnHoverConstructable, ref convertToType))
                    {
                        List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                        Action Convert = () =>
                        {
                            for (int x = 0; x < currentlySelected.Count; x++)
                            {
                                Unit toPerfrom = (Unit)currentlySelected[x];
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                HumanBehaviour_ChangeUnitType change = new HumanBehaviour_ChangeUnitType();
                                change.InitBehaviour(toPerfrom, OnHoverConstructable, convertToType);
                                //CollectResources_Behaviour collect = new CollectResources_Behaviour();
                                // collect.InitBehaviour(toPerfrom, toHarvest);
                                br.SetBehaviour(change);

                            }
                        };
                        GameAction ga = new GameAction("Convert to " + convertToType, Convert, KeyCode.None);
                        GameActionController.Instance.AddAction(ga);
                    }
                }



                if (OnHoverBuildable != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    Constructable toBuild = OnHoverBuildable;
                    Action Build = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)currentlySelected[x]);
                            if (toPerfrom.MyType == UnitType.Engineer)
                            {
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                HumanBehaviour_ConstructObject construct = new HumanBehaviour_ConstructObject();
                                construct.InitBehaviour(toPerfrom, toBuild);
                                construct.IsUserInstruction = true;
                                br.SetBehaviour(construct);
                            }
                        }
                    };
                    GameAction ga = new GameAction("Build", Build, InputController.Instance.GetShortcutFromType(typeof(HumanBehaviour_ConstructObject)));
                    GameActionController.Instance.AddAction(ga);
                    DoneCommand = true;
                }


                if (OnHoverHarvestable != null)
                {
                    EnvironmentObjectInstance toHarvest = OnHoverHarvestable;
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(currentlySelected,toHarvest.GetPosition());

                    Action Harvest = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            GatherResources_Behaviour gather = new GatherResources_Behaviour();
                            gather.InitBehaviour(toPerfrom, toHarvest, targetPositions[x]);
                            br.SetBehaviour(gather);

                        }
                    };
                    GameAction ga = new GameAction("Harvest Resource", Harvest, InputController.Instance.GetShortcutFromType(typeof(GatherResources_Behaviour)));
                    GameActionController.Instance.AddAction(ga);
                }

                if (OnHoverResource != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    ResourceInstance toHarvest = OnHoverResource;
                    Action Gather = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            collect.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(collect);

                        }
                    };
                    GameAction ga = new GameAction("Collect Resource", Gather, InputController.Instance.GetShortcutFromType(typeof(CollectResources_Behaviour)));
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
                    GameAction ga = new GameAction("Store Resources", Gather, KeyCode.None);
                    GameActionController.Instance.AddAction(ga);


                    Action transfer = () =>
                    {
                        Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[0];
                        Inventory unitInventory = toPerfrom.GetComponent<Inventory>();
                        InventoryParentUI.Instance.PopulateUI(unitInventory, OnHoverInventory);


                    };
                    GameAction tr = new GameAction("Transfer Items", transfer, KeyCode.None);
                    GameActionController.Instance.AddAction(tr);
                }

                if (OnHoverEnvironmentObject != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(currentlySelected,OnHoverEnvironmentObject.GetPosition());

                    Action Convert = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanBehaviour_DeconstructObject deconstruct = new HumanBehaviour_DeconstructObject();
                            deconstruct.InitBehaviour(toPerfrom, OnHoverEnvironmentObject, targetPositions[x]);
                            //CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            // collect.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(deconstruct);

                        }


                    };
                    GameAction ga = new GameAction("Deconstruct: " + OnHoverEnvironmentObject.Name(), Convert, InputController.Instance.GetShortcutFromType(typeof(HumanBehaviour_DeconstructObject)));
                    GameActionController.Instance.AddAction(ga);
                }


                if (OnHoverWallSegment != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(currentlySelected,OnHoverWallSegment.Position());

                    Action Convert = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanBehaviour_DeconstructObject deconstruct = new HumanBehaviour_DeconstructObject();
                            deconstruct.InitBehaviour(toPerfrom, OnHoverWallSegment, targetPositions[x]);
                            //CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            // collect.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(deconstruct);

                        }


                    };
                    GameAction ga = new GameAction("Deconstruct: " + OnHoverWallSegment.Name(), Convert, InputController.Instance.GetShortcutFromType(typeof(HumanBehaviour_DeconstructObject)));
                    GameActionController.Instance.AddAction(ga);
                }

                {
                    hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.CursorLayermask);
                    if (hit.collider != null)
                    {
                        Vector3 targetPos = hit.point;
                        targetPos.z = 0;
                        List<Selectable> selected = SelectableManager.Instance.CurrentlySelected;
                        List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(selected, CursorSelect.Instance.tileMousePos);


                        Action move = () =>
                        {

                            for (int x = 0; x < selected.Count; x++)
                            {
                                Unit toPerfrom = (Unit)selected[x];
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                MoveTo_Behaviour moveTo_Behaviour = new MoveTo_Behaviour();
                                moveTo_Behaviour.InitBehaviour(toPerfrom, targetPositions[x], true);
                                moveTo_Behaviour.IsUserInstruction = true;
                                br.SetBehaviour(moveTo_Behaviour);

                            }
                        };
                        GameAction ga = new GameAction("Move", move, KeyCode.None);
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
    EnvironmentObjectInstance OnHoverEnvironmentObject;
    WallSegment OnHoverWallSegment;
    ResourceInstance OnHoverResource;
    Inventory OnHoverInventory;
    public override void OnHover()
    {
        Ray r = CursorSelect.Instance.Camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.UnitLayermask);

        OnHoverMyUnit = SelectionUtilities.GetUserUnitWithinRangeOfPoint(r.origin, 1f);


      

        OnHoverEnemyUnit = SelectionUtilities.GetHostileUnitWithinRangeOfPoint(r.origin, 1f);
      

        OnHoverHarvestable = SelectionUtilities.GetHarvestableObjectInstanceWithinRangeOfPoint(r.origin, 1f);

       


        OnHoverEnvironmentObject = SelectionUtilities.GetEnvironmentObjectInstanceWithinRangeOfPoint(r.origin, 1f);

        OnHoverWallSegment = SelectionUtilities.GetWallTilesWithinRangeOfPoint(r.origin, 1f);


        OnHoverResource = SelectionUtilities.GetResourceInstanceObjectInstanceWithinRangeOfPoint(r.origin, 1f);
        
       // OnHoverInventory = SelectionUtilities.GetInventoryObjectWithinRangeOfPoint(r.origin, 1f);
        CursorIcon.Instance.SetMoveIcon();

        //if (OnHoverInventory != null)
        //{
        //    CursorIcon.Instance.SetMoveIcon();
        //}
        OnHoverBuildable = SelectionUtilities.GetConstructableObjectInstanceWithinRangeOfPoint(r.origin, 1f);
        if (OnHoverBuildable != null)
        {
            CursorIcon.Instance.SetMoveIcon();
        }
        OnHoverConstructable = SelectionUtilities.GetConstructedObjectInRangeOfPoint(r.origin, 1f);
        if (OnHoverResource != null)
        {
            CursorIcon.Instance.SetCollectIcon();
        }
        if (OnHoverHarvestable != null)
        {
            CursorIcon.Instance.SetHarvestIcon();
        }
        if (OnHoverConstructable != null)
        {
            CursorIcon.Instance.SetBuildIcon();
        }

        if (OnHoverEnvironmentObject != null)
        {
            CursorIcon.Instance.SetDeconstructIcon();
        }
      
        if (OnHoverEnemyUnit != null)
        {
            CursorIcon.Instance.SetAttackIcon();
        }
        if(GameActionController.Instance.currentValidGameActions!=null&& GameActionController.Instance.currentValidGameActions.Count > 2)
        {
            CursorIcon.Instance.SetMultipleActionIcon();
        }


        CheckForActionsToPerformWithoutInput();
    }
    void CheckForActionsToPerformWithoutInput()
    {
        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {
            GameActionController.Instance.currentValidGameActions.Clear();
            {

                bool DoneCommand = false;


                Ray r = CursorSelect.Instance.Camera.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.UnitLayermask);
                if (hit.collider != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    Unit targetUnit = hit.collider.gameObject.GetComponent<Unit>();
                    System.Action attack = () =>
                    {
                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)currentlySelected[x]);
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                            attack.InitBehaviour(targetUnit, toPerfrom);
                            attack.IsUserInstruction = true;
                            br.SetBehaviour(attack);

                        }
                    };

                    GameAction ga = new GameAction("Attack", attack, InputController.Instance.GetShortcutFromType(typeof(HumanAttackUnit_Behaviour)));
                    GameActionController.Instance.AddAction(ga);
                    //DoneCommand = true;
                }
                else if (OnHoverEnemyUnit != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    Unit enemy = OnHoverEnemyUnit;
                    System.Action attack = () =>
                    {
                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)currentlySelected[x]);
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanAttackUnit_Behaviour attack = new HumanAttackUnit_Behaviour();
                            attack.InitBehaviour(enemy, toPerfrom);
                            attack.IsUserInstruction = true;
                            br.SetBehaviour(attack);

                        }
                    };
                    GameAction ga = new GameAction("Attack", attack, InputController.Instance.GetShortcutFromType(typeof(HumanAttackUnit_Behaviour)));
                    GameActionController.Instance.AddAction(ga);
                    //  DoneCommand = true;
                }

                if (OnHoverConstructable != null)
                {
                    string convertToType = "";
                    if (UnitTypesController.Instance.CanConvertUnitsWithObject(OnHoverConstructable, ref convertToType))
                    {
                        List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                        Action Convert = () =>
                        {
                            for (int x = 0; x < currentlySelected.Count; x++)
                            {
                                Unit toPerfrom = (Unit)currentlySelected[x];
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                HumanBehaviour_ChangeUnitType change = new HumanBehaviour_ChangeUnitType();
                                change.InitBehaviour(toPerfrom, OnHoverConstructable, convertToType);
                                //CollectResources_Behaviour collect = new CollectResources_Behaviour();
                                // collect.InitBehaviour(toPerfrom, toHarvest);
                                br.SetBehaviour(change);

                            }
                        };
                        GameAction ga = new GameAction("Convert to " + convertToType, Convert, KeyCode.None);
                        GameActionController.Instance.AddAction(ga);
                    }
                }



                if (OnHoverBuildable != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    Constructable toBuild = OnHoverBuildable;
                    Action Build = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = ((Unit)currentlySelected[x]);
                            if (toPerfrom.MyType == UnitType.Engineer)
                            {
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                HumanBehaviour_ConstructObject construct = new HumanBehaviour_ConstructObject();
                                construct.InitBehaviour(toPerfrom, toBuild);
                                construct.IsUserInstruction = true;
                                br.SetBehaviour(construct);
                            }
                        }
                    };
                    GameAction ga = new GameAction("Build", Build, InputController.Instance.GetShortcutFromType(typeof(HumanBehaviour_ConstructObject)));
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
                    GameAction ga = new GameAction("Harvest Resource", Harvest, InputController.Instance.GetShortcutFromType(typeof(GatherResources_Behaviour)));
                    GameActionController.Instance.AddAction(ga);
                }

                if (OnHoverResource != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    ResourceInstance toHarvest = OnHoverResource;
                    Action Gather = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            collect.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(collect);

                        }
                    };
                    GameAction ga = new GameAction("Collect Resource", Gather, InputController.Instance.GetShortcutFromType(typeof(CollectResources_Behaviour)));
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
                    GameAction ga = new GameAction("Store Resources", Gather, KeyCode.None);
                    GameActionController.Instance.AddAction(ga);


                    Action transfer = () =>
                    {
                        Unit toPerfrom = (Unit)SelectableManager.Instance.CurrentlySelected[0];
                        Inventory unitInventory = toPerfrom.GetComponent<Inventory>();
                        InventoryParentUI.Instance.PopulateUI(unitInventory, OnHoverInventory);


                    };
                    GameAction tr = new GameAction("Transfer Items", transfer, KeyCode.None);
                    GameActionController.Instance.AddAction(tr);
                }

                if (OnHoverEnvironmentObject != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(currentlySelected, OnHoverEnvironmentObject.GetPosition());

                    Action Convert = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanBehaviour_DeconstructObject deconstruct = new HumanBehaviour_DeconstructObject();
                            deconstruct.InitBehaviour(toPerfrom, OnHoverEnvironmentObject, targetPositions[x]);
                            //CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            // collect.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(deconstruct);

                        }


                    };
                    GameAction ga = new GameAction("Deconstruct: " + OnHoverEnvironmentObject.Name(), Convert, InputController.Instance.GetShortcutFromType(typeof(HumanBehaviour_DeconstructObject)));
                    GameActionController.Instance.AddAction(ga);
                }


                if (OnHoverWallSegment != null)
                {
                    List<Selectable> currentlySelected = SelectableManager.Instance.CurrentlySelected;
                    List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(currentlySelected, OnHoverWallSegment.Position());

                    Action Convert = () =>
                    {

                        for (int x = 0; x < currentlySelected.Count; x++)
                        {
                            Unit toPerfrom = (Unit)currentlySelected[x];
                            BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                            HumanBehaviour_DeconstructObject deconstruct = new HumanBehaviour_DeconstructObject();
                            deconstruct.InitBehaviour(toPerfrom, OnHoverWallSegment, targetPositions[x]);
                            //CollectResources_Behaviour collect = new CollectResources_Behaviour();
                            // collect.InitBehaviour(toPerfrom, toHarvest);
                            br.SetBehaviour(deconstruct);

                        }


                    };
                    GameAction ga = new GameAction("Deconstruct: " + OnHoverWallSegment.Name(), Convert, InputController.Instance.GetShortcutFromType(typeof(HumanBehaviour_DeconstructObject)));
                    GameActionController.Instance.AddAction(ga);
                }

                {
                    hit = Physics2D.Raycast(r.origin, r.direction, 999f, CursorSelect.Instance.CursorLayermask);
                    if (hit.collider != null)
                    {
                        Vector3 targetPos = hit.point;
                        targetPos.z = 0;
                        List<Selectable> selected = SelectableManager.Instance.CurrentlySelected;
                        List<PathfindingNode> targetPositions = UnitHelpers.GetWalkableNodesNearTarget(selected, targetPos);


                        Action move = () =>
                        {

                            for (int x = 0; x < selected.Count; x++)
                            {
                                Unit toPerfrom = (Unit)selected[x];
                                BehaviourRunner br = toPerfrom.GetComponent<BehaviourRunner>();
                                MoveTo_Behaviour moveTo_Behaviour = new MoveTo_Behaviour();
                                moveTo_Behaviour.InitBehaviour(toPerfrom, targetPositions[x],true);
                                moveTo_Behaviour.IsUserInstruction = true;
                                br.SetBehaviour(moveTo_Behaviour);

                            }
                        };
                        GameAction ga = new GameAction("Move", move, KeyCode.None);
                        GameActionController.Instance.AddAction(ga);
                    }
                }
            }
        }
    }

}
