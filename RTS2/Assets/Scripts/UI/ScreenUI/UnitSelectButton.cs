using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UnitSelectButton : MonoBehaviour
{
    public Button SelectButton,GoToButton;
    public TextMeshProUGUI NameText,QuantityText;
    UnitType type;
    Unit myUnit;
    bool isSingleUnit = false;

    private void Awake()
    {
        SelectButton.onClick.AddListener(OnButtonClick);
        GoToButton.onClick.AddListener(AutoMoveToUnit);
    }

    public void SetUnitType(UnitType unitType, int quantity)
    {
        QuantityText.text=quantity.ToString();
        NameText.text=unitType.ToString();
        GoToButton.gameObject.SetActive(false);
        type= unitType;
        isSingleUnit = false;
    }

    public void SetUnit(Unit unit)
    {
        NameText.text = unit.MyType.ToString();
        type = unit.MyType;
        QuantityText.text = unit.Health() + "/" + unit.MaxHealth();
        GoToButton.gameObject.SetActive(true);
        myUnit = unit;
        isSingleUnit = true;
    }

    void OnButtonClick()
    {
        if (isSingleUnit)
        {
            SelectableManager.Instance.ClearSelectables();
            SelectableManager.Instance.AddSelectable(myUnit);
        }
        else
        {
            SelectableManager.Instance.SetOnlyTypeSelected(type);
        }
        SelectableManager.OnSelectionChanged();
        SelectionController.Instance.blockInputTimer = .2f;

    }

    void AutoMoveToUnit()
    {
        CameraController.Instance.SetToAutoMove(myUnit.Position());
        SelectionController.Instance.blockInputTimer = .2f;

    }

}
