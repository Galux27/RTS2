using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnits_UIElement : BaseUIElement
{
    static SelectedUnits_UIElement instance;
    public static SelectedUnits_UIElement Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<SelectedUnits_UIElement>();   
            }
            return instance;
        }
    }


    public GameObject Prefab;
    public Transform UIParent;
    public UnitOrders_UIElement UnitOrders;
    CanvasGroup cg;
    List<SelectedUnit_UIElement> InactiveUIElements=new List<SelectedUnit_UIElement>(), ActiveUIEleemnt=new List<SelectedUnit_UIElement>();
    private void Awake()
    {
        SelectableManager.OnSelectionChanged += OnSelectionChange;
        cg = this.GetComponent<CanvasGroup>();
    }
    private void OnDestroy()
    {
        SelectableManager.OnSelectionChanged -= OnSelectionChange;
    }
    void OnSelectionChange()
    {
        Cleanup();
        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {
            for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
            {
                AddSelectableToDisplay(SelectableManager.Instance.CurrentlySelected[x]);
                //if (SelectableManager.Instance.CurrentlySelected[x].GetSelectableType() == SelectableType.Unit)
                //{
                //    AddExtraUnitToDisplay(SelectableManager.Instance.CurrentlySelected[x] as Unit);
               // }
            }
        }
        if (ActiveUIEleemnt.Count > 0)
        {
            UnitOrders.OnUnitSelectionUpdated();
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        else
        {
            cg.alpha = 0f;
            cg.interactable =false;
            cg.blocksRaycasts = false;
        }

    }

    void AddExtraConstructableToDisplay(ConstructableObjectInstance u)
    {
        if (u == null)
        {
            return;
        }
        SelectedUnit_UIElement ui = GetUIElement();
        ui.SetConstructableObject(u);
        ui.gameObject.SetActive(true);
        ActiveUIEleemnt.Add(ui);
    }

    void AddWallSegmetToDisplay(WallSegment ws)
    {
        if (ws == null)
        {
            return;
        }
        SelectedUnit_UIElement ui = GetUIElement();
        ui.SetWallSegment(ws);
        ui.gameObject.SetActive(true);
        ActiveUIEleemnt.Add(ui);
    }
    void AddExtraUnitToDisplay(Unit u)
    {
        if (u == null)
        {
            return;
        }
        SelectedUnit_UIElement ui = GetUIElement();
        ui.SetUnit(u);
        ui.gameObject.SetActive(true);
        ActiveUIEleemnt.Add(ui);
    }
    public void AddSelectableToDisplay(Selectable s)
    {
        if (s as Unit != null)
        {
            AddExtraUnitToDisplay(s as Unit);
        } else if (s as ConstructableObjectInstance != null)
        {
            AddExtraConstructableToDisplay(s as ConstructableObjectInstance);
        }else if(s as WallSegment != null)
        {
            AddWallSegmetToDisplay(s as WallSegment);
        }
    }

    public void SetUnitsToDisplay(List<Unit> ToDisplay)
    {
        Cleanup();
        SelectedUnit_UIElement ui = null;
        for(int x = 0; x < ToDisplay.Count; x++)
        {
            ui = GetUIElement();
            ui.SetUnit(ToDisplay[x]);
            ui.gameObject.SetActive(true);
            ActiveUIEleemnt.Add(ui);
        }
    }


    public override void HideUI()
    {
        base.HideUI();
        Cleanup();
    }

    public override void RefreshUI()
    {
        base.RefreshUI();
        OnSelectionChange();
    }

    //get icons working for actions if there's move + something else
    void Cleanup()
    {
        for(int x=0;x<ActiveUIEleemnt.Count;x++)
        {
            ActiveUIEleemnt[x].Cleanup ();
            ActiveUIEleemnt[x].gameObject.SetActive(false);
            InactiveUIElements.Add(ActiveUIEleemnt[x]);
        }
        ActiveUIEleemnt.Clear();
    }

    SelectedUnit_UIElement GetUIElement()
    {
        if (InactiveUIElements.Count == 0)
        {
            InactiveUIElements.Add(Instantiate(Prefab,UIParent).GetComponent<SelectedUnit_UIElement>());
            InactiveUIElements[0].gameObject.SetActive(false);
        }
        SelectedUnit_UIElement retVal = InactiveUIElements[0];
        InactiveUIElements.RemoveAt(0);
        return retVal;
    }
}
