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
        Debug.Log("Set Selectable " + toSet.GetSelectableType());
        QuantityText.gameObject.SetActive(false);
        Goto.gameObject.SetActive(true);
        Goto.OnClick = GoTo;
        HealthDisplay.gameObject.SetActive(true);
        currentlySelected = toSet;
        selectedInfo = (ObjectInfo)currentlySelected;
        GetIconForSelectable(toSet);
    }

    void GetIconForSelectable(Selectable toSet)
    {
        if (toSet.GetSelectableType() == SelectableType.Unit)
        {

        }else if (toSet.GetSelectableType() == SelectableType.ConstructableObject)
        {
            IconImage.sprite = ConstructableObjectManager.Instance.AllObjects[toSet.GetObjectType()].ForwardsSprite;
        }
        else if (toSet.GetSelectableType() == SelectableType.Structure)
        {
            IconImage.sprite = WallTypeManager.Instance.GetWallTile(toSet.GetObjectType()).LeftRight.sprite;
        }
        else if (toSet.GetSelectableType() == SelectableType.UnderConstructionObject)
        {

        }
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
