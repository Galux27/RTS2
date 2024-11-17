using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UnitSelectButton : MonoBehaviour
{
    public Button SelectButton;
    public TextMeshProUGUI NameText,QuantityText;
    UnitType type;

    private void Awake()
    {
        SelectButton.onClick.AddListener(OnButtonClick);
    }

    public void SetUnit(UnitType unitType, int quantity)
    {
        QuantityText.text=quantity.ToString();
        NameText.text=unitType.ToString();
        type= unitType;
    }


    void OnButtonClick()
    {
        
    }

}
