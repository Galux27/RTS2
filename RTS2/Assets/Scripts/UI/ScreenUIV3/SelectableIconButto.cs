using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SelectableIconButto : IconButton
{

    public TextMeshProUGUI QuantityText;
    public ButtonBar HealthDisplay;
    public IconButton Goto;

    Selectable currentlySelected;
    ObjectInfo selectedInfo;

    private void Awake()
    {
        this.MyButton.onClick.AddListener(OnClick);
    }


 

    public void SetSelectable(Selectable toSet)
    {
        QuantityText.gameObject.SetActive(false);
        Goto.gameObject.SetActive(true);
        Goto.OnClick = GoTo;
        HealthDisplay.gameObject.SetActive(true);
        currentlySelected = toSet;
        selectedInfo = (ObjectInfo)currentlySelected;
    }


    void GoTo()
    {
        CameraController.Instance.SetToAutoMove(selectedInfo.Position());
    }

    void OnClick()
    {
        SelectableManager.Instance.SetToOnlySelected(currentlySelected);
    }
}
