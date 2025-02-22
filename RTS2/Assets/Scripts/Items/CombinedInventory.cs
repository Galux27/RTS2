using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedInventory
{
    static CombinedInventory instance;
    public static CombinedInventory Instance
    {
        get
        {
            if(instance== null)
            {
                instance = new CombinedInventory();
            }
            return instance;
        }
    }

    public CombinedInventory()
    {
        SelectableManager.OnSelectionChanged += RefreshInventoriesSelected;
        SelectableManager.OnSelectionChanged += () => InventoryParentUI.Instance.PopulateSelectedInventoryUI();


    }

    List<Inventory> inventorysDisplaying=new List<Inventory>();

    public void RefreshInventoriesSelected()
    {
        CleanupCombinedInventories();
        Unit selectedUnit = null;
        ConstructableObjectInstance selectedObject = null;
        for (int x=0;x< SelectableManager.Instance.CurrentlySelected.Count; x++)
        {
            selectedUnit = SelectableManager.Instance.CurrentlySelected[x] as Unit;
            if (selectedUnit != null)
            {
               AddCombinedInventory( selectedUnit.GetComponent<Inventory>());
            }
            else
            {
                selectedObject = SelectableManager.Instance.CurrentlySelected[x] as ConstructableObjectInstance;
                if (selectedObject != null)
                {
                    AddCombinedInventory(selectedObject.inventoryObject.GetComponent<Inventory>());
                  
                }
            }
        }
    }
    
    public List<InventoryObject> GetAllObjects(Action<InventoryObject> onObject,ref float weight)
    {
        List<InventoryObject> retVal = new List<InventoryObject>();
        for(int x = 0; x < inventorysDisplaying.Count; x++)
        {
            for(int q = 0; q < inventorysDisplaying[x].ObjectsInInventory.Count; q++)
            {
                onObject?.Invoke(inventorysDisplaying[x].ObjectsInInventory[q]);
                weight += inventorysDisplaying[x].ObjectsInInventory[q].Weight();
            }
        }
        return retVal;
    }

    public void AddCombinedInventory(Inventory i)
    {
        i.OnInventoryChange += OnSelectedInventoryChange;
        inventorysDisplaying.Add(i);
    }

    public void RemoveCombinedInventory(Inventory i)
    {
        i.OnInventoryChange-= OnSelectedInventoryChange;
        inventorysDisplaying.Remove(i);
    }

    void CleanupCombinedInventories()
    {
        for(int x=0; x<inventorysDisplaying.Count; x++)
        {
            RemoveCombinedInventory(inventorysDisplaying[x]);
        }
        inventorysDisplaying.Clear();
    }

    void OnSelectedInventoryChange()
    {
        Debug.Log("On selected inventory change");
        InventoryParentUI.Instance.PopulateSelectedInventoryUI();
    }
}
