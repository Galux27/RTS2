using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.UI.ModernUIPack;
using System;

public class ConstructionUIElement : BaseUIElement
{
    public ButtonManager Walls, Doors, Furniture, Storage;
    public GameObject SelectObjectParent,SelectButtonPrefab;
    public Transform SelectButtonContainer;
    public ConstructionMenuMode MenuMode;
    private void Awake()
    {
        Walls.gameObject.GetComponent<Button>().onClick.AddListener(() => SetConstructionMenuMode(ConstructionMenuMode.Walls));
        Doors.gameObject.GetComponent<Button>().onClick.AddListener(() => SetConstructionMenuMode(ConstructionMenuMode.Doors));
        Furniture.gameObject.GetComponent<Button>().onClick.AddListener(() => SetConstructionMenuMode(ConstructionMenuMode.Furniture));
        Storage.gameObject.GetComponent<Button>().onClick.AddListener(() => SetConstructionMenuMode(ConstructionMenuMode.Storage));

    }
    void OnSwitchMode()
    {     
        StructureSelectionMode.Mode = StructureSelectionType.None;   
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None);
        CursorIcon.Instance.SetVisible(false);
        CursorIcon.Instance.SetCustomIcon(null,Vector3.zero);

    }

    public override void DrawUI()
    {
        base.DrawUI();
        SelectableManager.Instance.ClearSelectables();

    }
    private void SetConstructionMenuMode(ConstructionMenuMode toBe)
    {
        OnSwitchMode();
        if(toBe==ConstructionMenuMode.None||MenuMode==toBe)
        {
            SelectObjectParent.SetActive(false);
           MenuMode = ConstructionMenuMode.None;
        }
        else
        {
            SelectObjectParent.SetActive(true);
            ClearExistingButtons();
            switch (toBe)
            {
                case ConstructionMenuMode.None:
                    break;
                case ConstructionMenuMode.Walls:
                    DrawWalls();
                    break;
                case ConstructionMenuMode.Doors:
                    DrawDoors();
                    break;
                case ConstructionMenuMode.Furniture:
                   DrawFurniture();
                    break;
                case ConstructionMenuMode.Storage:
                    DrawStorage();
                    break;
                default:
                    break;
            }
            MenuMode=toBe;
        }
    }

    void ClearExistingButtons()
    {
        for(int x=0;x< SelectButtonContainer.transform.childCount; x++)
        {
            GameObject.Destroy(SelectButtonContainer.transform.GetChild(x).gameObject);
        }
    }


    void DrawWalls()
    {

        if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Structures)
        {
            SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures);
        }
        foreach (KeyValuePair<string, WallTile> walls in WallTypeManager.Instance.AllObjects)
        {
            Action OnButtonClick = new Action(() => StructureSelectionMode.Mode = StructureSelectionType.Walls);
            OnButtonClick += () => WallTypeManager.Instance.SelectedWallTile = walls.Value;
            CursorIcon.Instance.SetVisible(true);

            GameObject button = GameObject.Instantiate(SelectButtonPrefab, SelectButtonContainer.transform);
            SelectConstructableUIElement uiButton = button.GetComponent<SelectConstructableUIElement>();
            uiButton.SetupButton(walls.Key, OnButtonClick);

        }
    }
    void DrawDoors()
    {
        if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Structures)
        {
            SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures);
        }
        GameObject button = GameObject.Instantiate(SelectButtonPrefab, SelectButtonContainer.transform);
        SelectConstructableUIElement uiButton = button.GetComponent<SelectConstructableUIElement>();

        Action OnClick = () =>
        {
            WallTypeManager.Instance.SelectedWallTile = WallTypeManager.Instance.AllObjects["Metal"];
            StructureSelectionMode.Mode = StructureSelectionType.Door;
            if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Structures)
            {
                SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Structures);
            }
            CursorIcon.Instance.SetVisible(true);


        };
        uiButton.SetupButton("Door", OnClick);
    }

    void DrawFurniture()
    {

        if (SelectionController.Instance.selectionMode != CurrentSelectionMode.Furniture)
        {
            SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.Furniture);
        }
        foreach (KeyValuePair<string, ConstructableObject> kvp in ConstructableObjectManager.Instance.AllObjects)
        {
            GameObject button = GameObject.Instantiate(SelectButtonPrefab, SelectButtonContainer.transform);
            SelectConstructableUIElement uiButton = button.GetComponent<SelectConstructableUIElement>();

            Action OnClick = () =>
            {


                if (ConstructableObjectManager.Instance.AllObjects.ContainsKey(kvp.Key))
                {
                    ResourceCostUI.Instance.UpdateUI(kvp.Value.RequirementsToBuild);
                    ConstructableObjectManager.Instance.SetCursorObject(kvp.Key);
                }

            };
            uiButton.SetupButton(kvp.Key, OnClick);
        }
    }

    void DrawStorage()
    {

    }

    public override void HideUI()
    {
        OnSwitchMode();
        base.HideUI();
    }
}

public enum ConstructionMenuMode
{
    None,
    Walls,
    Doors,
    Furniture, 
    Storage
}
