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
        switch (category)
        {
            case ConstructableCategory.Furniture:
                if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Furniture)
                {
                    SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Furniture);
                }
                break;
            case ConstructableCategory.Walls:
            case ConstructableCategory.Doors:
                if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Structures)
                {
                    SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures);
                }
                break;
            default:
                break;
        }
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
                DrawWalls();
                break;
            case ConstructableCategory.Doors:
                DrawDoors();
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
        Debug.Log("Selection Mode: setting object to " + key);
        if (ConstructableObjectManager.Instance.AllObjects.ContainsKey(key))
        {
            ResourceCostUI.Instance.UpdateUI(ConstructableObjectManager.Instance.AllObjects[key].RequirementsToBuild);
            //ConstructableObjectManager.Instance.SetCursorObject(key);
        }
    }

    void DrawFurniture()
    {
        GameObject currentButton = null;
        foreach (KeyValuePair<string, ConstructableObject> kvp in ConstructableObjectManager.Instance.AllObjects)
        {
            currentButton = GetButton();
            currentButton.GetComponent<ConstructableButton>().SetEnvObject(kvp.Value,kvp.Key);
        }
    }

    void DrawWalls()
    {
        GameObject currentButton = null;

        foreach (KeyValuePair<string, WallTile> walls in WallTypeManager.Instance.AllObjects)
        {
            currentButton = GetButton();
            currentButton.GetComponent<ConstructableButton>().SetWallObject(walls.Value);
         
        }
    }

    void DrawDoors()
    {
        GameObject currentButton = currentButton = GetButton();
        currentButton.GetComponent<ConstructableButton>().SetDoorObject(WallTypeManager.Instance.AllObjects["Metal"]);


    }
}

public enum ConstructableCategory
{
    Furniture,
    Walls,
    Doors
}
