using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnits_UIElement : BaseUIElement
{

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
                if (SelectableManager.Instance.CurrentlySelected[x].GetSelectableType() == SelectableType.Unit)
                {
                    AddExtraUnitToDisplay(SelectableManager.Instance.CurrentlySelected[x] as Unit);
                }
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
