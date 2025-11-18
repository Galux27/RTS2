using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Order_UIElement : BaseUIElement
{
    public Button SwitchOrder;
    public TextMeshProUGUI nameDisp;
    string key = "";
    bool curVal = false;
    public void SetOrder(string OrderKey,bool baseValue)
    {
        key = OrderKey;
        curVal=baseValue;
        UpdateButton();
        SwitchOrder.onClick.AddListener(SwitchOrders);
        nameDisp.text = key;
    }

    public void Cleanup()
    {
        SwitchOrder.onClick.RemoveListener(SwitchOrders);
    }

    void SwitchOrders()
    {
        curVal = !curVal;
        UpdateButton();
        Unit currentUnit = null;
        for(int x=0;x< SelectableManager.Instance.CurrentlySelected.Count; x++)
        {
            currentUnit = SelectableManager.Instance.CurrentlySelected[x] as Unit;
            if(currentUnit!=null)
            {
                currentUnit.MyOrders.SetOrder(key, curVal);
            }
        }
    }

    void UpdateButton()
    {
        if (curVal == true)
        {
            SwitchOrder.GetComponent<Image>().color = Color.green;
        }
        else
        {
            SwitchOrder.GetComponent<Image>().color = Color.red;


        }
    }
}
