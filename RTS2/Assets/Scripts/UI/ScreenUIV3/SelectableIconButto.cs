using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SelectableIconButto : IconButton
{

    public TextMeshProUGUI QuantityText;
    public ButtonBar HealthDisplay;
    public IconButton Goto;

    private void Awake()
    {
        this.MyButton.onClick.AddListener(OnClick);
    }


    public void SetSelectable(List<Selectable> toSet)
    {
        QuantityText.gameObject.SetActive(true);
        QuantityText.text = toSet.Count.ToString();
        Goto.gameObject.SetActive(false);
    }

    public void SetSelectable(Selectable toSet)
    {
        CurrentSingleSelectable = toSet;
        QuantityText.gameObject.SetActive(false);
        Goto.gameObject.SetActive(true);
        Goto.OnClick = GoTo;
        HealthDisplay.gameObject.SetActive(true);
    }

    Selectable CurrentSingleSelectable;

    void GoTo()
    {
        
    }

    void OnClick()
    {

    }
}
