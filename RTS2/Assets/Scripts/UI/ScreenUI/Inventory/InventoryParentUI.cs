using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI script for the inventory UI screen
/// </summary>
public class InventoryParentUI :BaseUI
{
    static InventoryParentUI instance;
    public static InventoryParentUI Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<InventoryParentUI>(true);
            }
            return instance;
        }
    }
    public Transform TransferParent, SelectedParent;
    public InventoryUI InventoryOneUI, InventoryTwoUI,SelectedInventory;
    public Inventory InventoryOne, InventoryTwo;
    public Button closeUI;
    public Action OnInventoryChange;
    private void Awake()
    {
        closeUI.onClick.AddListener(CloseUI);
        CombinedInventory.Instance.RefreshInventoriesSelected();
    }

    public override void RefreshUI()
    {
        if (SelectedParent.gameObject.activeInHierarchy)
        {
            PopulateSelectedInventoryUI();
        }
    }

    public void PopulateUI(Inventory toDisplay)
    {
        
        InventoryOne = toDisplay;
        InventoryOneUI.PopulateInventory(toDisplay, 1, true,false);
        InventoryOneUI.DisplayUI(true);
        InventoryTwoUI.DisplayUI(false);
        TransferParent.gameObject.SetActive(true);
        SelectedParent.gameObject.SetActive(false);

    }

    public void PopulateUI(Inventory toDisplay,Inventory toDisplay2)
    {
        InventoryOne = toDisplay;
        InventoryOneUI.PopulateInventory(toDisplay, 1, false, false);
        InventoryOneUI.DisplayUI(true);

        InventoryTwo = toDisplay2;
        InventoryTwoUI.PopulateInventory(toDisplay2, 2, false,true);
        InventoryTwoUI.DisplayUI(true);
        SelectedInventory.DisplayUI(false);
        TransferParent.gameObject.SetActive(true);
        SelectedParent.gameObject.SetActive(false);

    }

    public void PopulateSelectedInventoryUI()
    {
        if (SelectableManager.Instance.CurrentlySelected.Count > 0)
        {

            SelectedInventory.PopulateWithCombined();
            SelectedInventory.DisplayUI(true);
            InventoryTwoUI.DisplayUI(false);
            InventoryOneUI.DisplayUI(false);

            TransferParent.gameObject.SetActive(false);
            SelectedParent.gameObject.SetActive(true);
        }
        else
        {
            CloseUI();
        }
    }


        public void DisplayUI(bool val)
    {
        this.gameObject.SetActive(val);
    }

    public void CloseUI()
    {
        TransferParent.gameObject.SetActive(false);
        SelectedParent.gameObject.SetActive(false);
    }

    public bool IsVisible()
    {
        return this.gameObject.activeInHierarchy;
    }


    Inventory GetOtherInventory(InventoryItemUI toCheck)
    {
        if (toCheck.InventoryID== 2)
        {
            return InventoryOne;
        }
        return InventoryTwo;
    }

    Inventory GetInventory(InventoryItemUI toCheck)
    {
        if (toCheck.InventoryID == 1)
        {
            return InventoryOne;
        }
        return InventoryTwo;
    }

    public bool CouldTransferIntoInventory(InventoryItemUI toCheck)
    {
        if (InventoryTwo == null)
        {
            return false;
        }
        Inventory i = GetOtherInventory(toCheck);
        return i.CanAddItemToInventory(toCheck.MyObject);
    }


    public bool CouldTransferSomeIntoInventory(InventoryItemUI toCheck)
    {
        if (InventoryTwo == null)
        {
            return false;
        }
        Inventory i = GetOtherInventory(toCheck);
        return i.CanAddSomeOfItemToInventory(toCheck.MyObject);
    }

    public void TransferOneIntoOtherInventory(InventoryItemUI toTransfer)
    {
        Inventory i = GetOtherInventory(toTransfer);
        i.TransferItemBetweenInventory(toTransfer.MyObject, GetInventory(toTransfer),1);
        OnInventoryChange?.Invoke();
    }

    public void TransferAllIntoOtherInventory(InventoryItemUI toTransfer)
    {
        Inventory i = GetOtherInventory(toTransfer);
        i.TransferItemBetweenInventory(toTransfer.MyObject, GetInventory(toTransfer));
        OnInventoryChange?.Invoke();
    }
}
