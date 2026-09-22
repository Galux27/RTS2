using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class SelectedUnitUI : BaseUIElement
{
    private void Awake()
    {
        UIEventManager.OnObjectSelected += OnSelected;
        UIEventManager.OnObjectDeselected+= OnDeselected;
        UIEventManager.SetUnitsToFollowOrder += OnOrderChanged;
        HideUI();
    }

    List<Unit> SelectedUnits = new List<Unit>();

    public TextMeshProUGUI InfoDisplay;
    public Transform OrdersParent;
    public UnitOrdersUI OrdersUI;

    void OnOrderChanged(string orderKey, bool value)
    {
        for (int x = 0; x < SelectedUnits.Count; x++)
        {
            SelectedUnits[x].MyOrders.SetOrder(orderKey, value);
        }
        RefreshUnitOrderUI();
    }
    void OnSelected(Selectable selected)
    {
        GameObject gameObject = selected.GetGameObject();
        if (gameObject != null)
        {
            if (gameObject.GetComponent<Unit>())
            {
                if (SelectedUnits.Count == 0)
                {
                    DrawUI();
                }
                SelectedUnits.Add(gameObject.GetComponent<Unit>());
                UpdateDisplay();
            }
        }
    }

    void OnDeselected(Selectable selected)
    {
        GameObject gameObject = selected.GetGameObject();
        if (gameObject != null)
        {
            if (gameObject.GetComponent<Unit>())
            {
                SelectedUnits.Remove(gameObject.GetComponent<Unit>());
                UpdateDisplay();
                if (SelectedUnits.Count == 0)
                {
                    HideUI();
                }
            }
        }
    }

    public override void HideUI()
    {
        OrdersUI.Cleanup();
        base.HideUI();

    }

    void UpdateDisplay()
    {
        if (SelectedUnits.Count == 0)
        {
            InfoDisplay.text = "";
        }else if (SelectedUnits.Count == 1)
        {
            UpdateDisplayForOneUnit();
        }
        else
        {
            UpdateDisplayForManyUnits();
        }
        RefreshUnitOrderUI();
    }

    void UpdateDisplayForOneUnit()
    {
        Unit u = SelectedUnits[0];
        StringBuilder displayText = new StringBuilder();
        displayText.Append("Type: "+u.MyType.ToString());
        displayText.Append(System.Environment.NewLine);
        displayText.Append("Action: " + u.BehaviourRunner.GetBehaviourDisplayText());
        displayText.Append(System.Environment.NewLine);
        displayText.Append("Melee Damage: " + u.MyAttackController.AttackDamage);
        displayText.Append(System.Environment.NewLine);
        displayText.Append("Ranged Damage: " + u.MyAttackController.RangedDamage);
        InfoDisplay.text = displayText.ToString();
    }
    Dictionary<UnitType, int> UnitTypes = new Dictionary<UnitType, int>();
    void UpdateDisplayForManyUnits()
    {
        UnitTypes.Clear();
        for(int x = 0; x < SelectedUnits.Count; x++)
        {
            if (!UnitTypes.ContainsKey(SelectedUnits[x].MyType))
            {
                UnitTypes.Add(SelectedUnits[x].MyType, 0);
            }
            UnitTypes[SelectedUnits[x].MyType]++;
        }
        StringBuilder displayText= new StringBuilder();
        foreach(KeyValuePair<UnitType,int> kvp in UnitTypes)
        {
            displayText.Append(kvp.Key.ToString() + " X" + kvp.Value.ToString());
            displayText.Append(System.Environment.NewLine);

        }
        InfoDisplay.text=displayText.ToString();
    }

    void RefreshUnitOrderUI()
    {
        OrdersUI.UpdateOrderUI(SelectedUnits);
    }
}
