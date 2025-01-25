using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// UI for a single inventory
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI Title, InfoDisp;
    public Transform ItemsUIParent;
    public GameObject ItemUIPrefab;
    public bool FlipButtonFunctions;
    public int InventoryID=-1;
    bool DisplayOnly = false;
    Inventory drawing;

    private void Start()
    {
        InventoryParentUI.Instance.OnInventoryChange += RedrawUI;
    }

    void RedrawUI()
    {
        Title.text = drawing.gameObject.name;
        InfoDisp.text = drawing.GetRemainingCapacity()+"/"+drawing.InventoryCapacity.ToString();
        CleanupUI();
        for (int x = 0; x < drawing.ObjectsInInventory.Count; x++)
        {
            CreateUIForItem(drawing.ObjectsInInventory[x]);
        }
    }



    public void PopulateInventory(Inventory i,int id,bool displayOnly,bool flipFunctions)
    {
        drawing = i;
        DisplayOnly = displayOnly;
        InventoryID = id;
        FlipButtonFunctions = flipFunctions;
        RedrawUI();
    }

    void CreateUIForItem(InventoryObject i)
    {
        GameObject ui = Instantiate(ItemUIPrefab, ItemsUIParent.transform);
        ui.GetComponent<InventoryItemUI>().InitItem(i,InventoryID,DisplayOnly,FlipButtonFunctions);
    }

    void CleanupUI()
    {
        for(int x=0;x<ItemsUIParent.childCount;x++)
        {
            GameObject.Destroy(ItemsUIParent.GetChild(x).gameObject);
        }
    }

    public void DisplayUI(bool val)
    {
        this.gameObject.SetActive(val);
    }
}
