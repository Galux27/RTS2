using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUIElement :BaseUIElement
{
    public GameObject SelectedObjectsParent, SelectedObjectsInfoParent,SelectedObjectButtonPrefab,InfoPrefab;
    bool init = false;
    public override void DrawUI()
    {
        base.DrawUI();
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None);
        RefreshUI();
    }
    private void Start()
    {
        if (!init)
        {
            SelectableManager.OnSelectionChanged += RefreshUI;
        }
    }

    public override void RefreshUI()
    {
        if (this.gameObject.activeInHierarchy == false) { return; }
        SelectedObjectsParent.SetActive(true);
        for(int x=0;x<SelectedObjectsParent.transform.childCount;x++)
        {
            GameObject.Destroy(SelectedObjectsParent.transform.GetChild(x).gameObject);
        }
        for (int x = 0; x < SelectedObjectsInfoParent.transform.childCount; x++)
        {
            GameObject.Destroy(SelectedObjectsInfoParent.transform.GetChild(x).gameObject);
        }
        DrawSelected();
    }

    void DrawSelected()
    {
        switch (SelectableManager.Instance.CurrentSelectedType)
        {
            case SelectableType.None:
                break;
            case SelectableType.Unit:
                DrawSelectedUnits();
                break;
            case SelectableType.Structure:
                break;
            case SelectableType.ConstructableObject:
                break;
            case SelectableType.Item:
                break;
            case SelectableType.UnderConstructionObject:
                break;
            case SelectableType.Resource:
                break;
            default:
                break;
        }
    }

    void DrawSelectedUnits()
    {
        Dictionary<UnitType, List<Unit>> units = SelectableManager.Instance.FilterUnitsByType();
        if (units.Count > 1)
        {

            foreach (var item in units)
            {
                GameObject button = Instantiate(SelectedObjectButtonPrefab, SelectedObjectsParent.transform);
                Action onClick = () => {

                   
                    SelectableManager.Instance.SetOnlyTypeSelected(item.Key);
                    SelectableManager.OnSelectionChanged();
                    SelectionController.Instance.blockInputTimer = .2f;
                    

                };
                button.GetComponent<SelectedObjectUIElement>().SetupButton(item.Value[0] as ObjectInfo,item.Value.Count,onClick);

              
            }
        }
        else if (units.Count == 1)
        {
            foreach (var item in units)
            {
                for (int x = 0; x < item.Value.Count; x++)
                {
                    GameObject button = Instantiate(SelectedObjectButtonPrefab, SelectedObjectsParent.transform);
                    Action onClick = () => {

                        SelectableManager.Instance.ClearSelectables();
                        SelectableManager.Instance.AddSelectable(item.Value[x]);
                        SelectableManager.OnSelectionChanged();
                        SelectionController.Instance.blockInputTimer = .2f;
                        

                    };
                    button.GetComponent<SelectedObjectUIElement>().SetupButton(item.Value[x] as ObjectInfo, 1, onClick);

                }

            }
        }
    }
}
