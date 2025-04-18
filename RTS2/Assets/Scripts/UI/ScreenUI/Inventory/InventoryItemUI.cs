using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI script for an item in an inventory
/// </summary>
public class InventoryItemUI : MonoBehaviour
{
    public TextMeshProUGUI InfoDisplay;
    public Button transferOne,transferAll,reclaimOne,reclaimAll;
    public InventoryObject MyObject;
    bool invertFunctions;
    public int InventoryID;



    public void InitItem(InventoryObject toDisplay,int invID,bool displayOnly,bool flipFunctions)
    {
        InventoryID = invID;
        invertFunctions = flipFunctions;
        SetButtonState(!displayOnly);
        MyObject = toDisplay;
        InfoDisplay.text = toDisplay.Name() + "(" + toDisplay.Quantity() + ")";
        InitButtonActions();
    }
    public void SetButtonState(bool val)
    {
        transferOne.gameObject.SetActive(val);
        transferAll.gameObject.SetActive(val); 
        reclaimOne.gameObject.SetActive(val); 
        reclaimAll.gameObject.SetActive(val);
    }

    void InitButtonActions()
    {
        if (invertFunctions)
        {
            reclaimOne.onClick.AddListener(() => InventoryParentUI.Instance.TransferOneIntoOtherInventory(this));
            reclaimAll.onClick.AddListener(() => InventoryParentUI.Instance.TransferAllIntoOtherInventory(this));
        }
        else
        {
            transferOne.onClick.AddListener(()=>InventoryParentUI.Instance.TransferOneIntoOtherInventory(this));
            transferAll.onClick.AddListener(() => InventoryParentUI.Instance.TransferAllIntoOtherInventory(this));

        }
    }
    
}
