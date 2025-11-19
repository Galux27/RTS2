using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitOrders_UIElement : BaseUIElement
{
    public GameObject Prefab;
    public Transform UIParent;
    CanvasGroup cg;
    List<Order_UIElement> InactiveUIElements = new List<Order_UIElement>(), ActiveUIEleemnt = new List<Order_UIElement>();
    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
    }

    public void OnUnitSelectionUpdated()
    {
        Cleanup();
        List<string> OrdersSet = new List<string>();
        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {
            for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
            {
                if (SelectableManager.Instance.CurrentlySelected[x].GetSelectableType() == SelectableType.Unit)
                {
                    Unit u = SelectableManager.Instance.CurrentlySelected[x] as Unit;
                    foreach(KeyValuePair<string,Order> kvp in u.MyOrders.Orders)
                    {
                        if (!OrdersSet.Contains(kvp.Key))
                        {
                            AddOrderToDisplay(kvp.Key, kvp.Value.Value);
                            OrdersSet.Add(kvp.Key);
                        }
                    }
                   // AddExtraUnitToDisplay(SelectableManager.Instance.CurrentlySelected[x] as Unit);
                }
            }
        }
        if (ActiveUIEleemnt.Count > 0)
        {
            cg.alpha = 1f;
        }
        else
        {
            cg.alpha = 0f;
        }

    }

 

    void AddOrderToDisplay(string order,bool startVal)
    {
       
        Order_UIElement ui = GetUIElement();
        ui.SetOrder(order, startVal);
        ui.gameObject.SetActive(true);
        ActiveUIEleemnt.Add(ui);
    }
    void Cleanup()
    {
        for (int x = 0; x < ActiveUIEleemnt.Count; x++)
        {
            ActiveUIEleemnt[x].Cleanup();
            ActiveUIEleemnt[x].gameObject.SetActive(false);
            InactiveUIElements.Add(ActiveUIEleemnt[x]);
        }
        ActiveUIEleemnt.Clear();
    }

    Order_UIElement GetUIElement()
    {
        if (InactiveUIElements.Count == 0)
        {
            InactiveUIElements.Add(Instantiate(Prefab, UIParent).GetComponent<Order_UIElement>());
            InactiveUIElements[0].gameObject.SetActive(false);
        }
        Order_UIElement retVal = InactiveUIElements[0];
        InactiveUIElements.RemoveAt(0);
        return retVal;
    }
}
