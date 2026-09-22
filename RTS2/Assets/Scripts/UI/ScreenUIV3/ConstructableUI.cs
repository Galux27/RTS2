using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConstructableUI : BaseUIElement
{
    const string ConstructableButton = "ConstructableButton";


    public Button Furniture, Walls, Doors;

    public ConstructableCategory CurrentCategory;

    public Transform ButtonParent;
    private void Awake()
    {
        Furniture.onClick.AddListener(()=>SetCategory(ConstructableCategory.Furniture));
        Walls.onClick.AddListener(() => SetCategory(ConstructableCategory.Walls));
        Doors.onClick.AddListener(() => SetCategory(ConstructableCategory.Doors));
        UIEventManager.OnConstructableObjectSelected += OnConstructableObjectSelected;
    }

    private void OnEnable()
    {
        RefreshButtons();
    }


    void SetCategory(ConstructableCategory category)
    {
        CurrentCategory=category;
        RefreshButtons();
    }

    List<GameObject> ActiveButtons = new List<GameObject>();
    void Cleanup()
    {
        for(int x = 0; x < ActiveButtons.Count; x++)
        {
            ActiveButtons[x].transform.parent = null;
            ActiveButtons[x].gameObject.SetActive(false);
            GameObjectPoolManager.Instance.ReturnObjectToPool(ActiveButtons[x], ConstructableButton);
        }
    }

    void RefreshButtons()
    {
        Cleanup();
        switch (CurrentCategory)
        {
            case ConstructableCategory.Furniture:
                DrawFurniture();
                break;
            case ConstructableCategory.Walls:
                break;
            case ConstructableCategory.Doors:
                break;
            default:
                break;
        }
    }

    GameObject GetButton()
    {
        GameObject retVal = GameObjectPoolManager.Instance.GetObjectFromPool(ConstructableButton);
        retVal.gameObject.SetActive(true);
        retVal.transform.parent = ButtonParent;
        ActiveButtons.Add(retVal);
        return retVal;
    }

    void OnConstructableObjectSelected(string key)
    {
        if (ConstructableObjectManager.Instance.AllObjects.ContainsKey(key))
        {
            ResourceCostUI.Instance.UpdateUI(ConstructableObjectManager.Instance.AllObjects[key].RequirementsToBuild);
            ConstructableObjectManager.Instance.SetCursorObject(key);
        }
    }

    void DrawFurniture()
    {
        GameObject currentButton = null;
        foreach (KeyValuePair<string, ConstructableObject> kvp in ConstructableObjectManager.Instance.AllObjects)
        {
            currentButton = GetButton();
            currentButton.GetComponent<ConstructableButton>().SetButton(kvp.Value);
        }
    }
}

public enum ConstructableCategory
{
    Furniture,
    Walls,
    Doors
}
