using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SelectionUIElement :BaseUIElement
{
    public GameObject SelectedObjectsParent, SelectedObjectsInfoParent,SelectedObjectButtonPrefab,InfoPrefab;
    bool init = false;
    CanvasGroup CG;
    public override void DrawUI()
    {
        base.DrawUI();
        Init();
        SelectionController.Instance.SetCursorSelectionMode(CurrentSelectionMode.None);
        RefreshUI();

    }
  
    void Init()
    {
        if (!init)
        {
            SelectableManager.OnSelectionChanged += RefreshUI;
            CG = this.GetComponentInChildren<CanvasGroup>();
            init = true;
        }
    }
    public override void RefreshUI()
    {

        if (SelectableManager.Instance.CurrentlySelected.Count == 0)
        {
            CG.alpha = 0f;
        }
        else
        {
            CG.alpha = 1f;
        }

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
            case SelectableType.ConstructableObject:
            case SelectableType.Item:
            case SelectableType.UnderConstructionObject:
            case SelectableType.Resource:
                DrawSelectedObjects();
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
                    SelectionController.Instance.blockInputTimer = InputController.BlockInputLength;
                    

                };
                button.GetComponent<SelectedObjectUIElement>().SetupButton(item.Value[0] as ObjectInfo,item.Value.Count,onClick);

              
            }
        }
        else if (units.Count == 1)
        {
            foreach (var item in units)
            {
                if (item.Value.Count > 0)
                {
                    for (int x = 0; x < item.Value.Count; x++)
                    {
                        GameObject button = Instantiate(SelectedObjectButtonPrefab, SelectedObjectsParent.transform);
                        Action onClick = () =>
                        {

                            SelectableManager.Instance.ClearSelectables();
                            SelectableManager.Instance.AddSelectable(item.Value[x]);
                            SelectableManager.OnSelectionChanged();
                            SelectionController.Instance.blockInputTimer = InputController.BlockInputLength;


                        };
                        button.GetComponent<SelectedObjectUIElement>().SetupButton(item.Value[x] as ObjectInfo, 1, onClick);

                    }
                }

                }
            }
    }

    void DrawSelectedObjects()
    {
        Dictionary<string, SelectedObjectCategory> dataFromCurrent = new Dictionary<string, SelectedObjectCategory>();
        dataFromCurrent.Clear();
        ObjectInfo oi = null;
        for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
        {
            oi = (ObjectInfo)SelectableManager.Instance.CurrentlySelected[x];
            if (oi != null)
            {
                if (dataFromCurrent.ContainsKey(oi.Name()) == false)
                {
                    dataFromCurrent.Add(oi.Name(), new SelectedObjectCategory(oi.Name()));
                }
                dataFromCurrent[oi.Name()].Increment(oi.Quantitiy());
            }
        }

        if (dataFromCurrent.Count == 0)
        {
            return;
        }
        if (dataFromCurrent.Count > 1)
        {
            foreach (KeyValuePair<string, SelectedObjectCategory> kvp in dataFromCurrent)
            {
                //GenerateFilterSelectedButton(kvp.Value);
                GameObject button = Instantiate(SelectedObjectButtonPrefab, SelectedObjectsParent.transform);
                Action onClick = () => {

                    SelectableManager.Instance.SetToOnlyNameSelected(kvp.Key);
                       SelectionController.Instance.blockInputTimer = InputController.BlockInputLength;

                };
                button.GetComponent<SelectedObjectUIElement>().SetupButton(kvp.Key, kvp.Value.Quantity, onClick);
            }
        }
        else
        {
            for (int x = 0; x < SelectableManager.Instance.CurrentlySelected.Count; x++)
            {
                GameObject button = Instantiate(SelectedObjectButtonPrefab, SelectedObjectsParent.transform);
                Selectable toSelect = SelectableManager.Instance.CurrentlySelected[x];
                Action onClick = () => {

                    SelectableManager.Instance.SetToOnlySelected(toSelect);
                    SelectionController.Instance.blockInputTimer = InputController.BlockInputLength;

                };
                button.GetComponent<SelectedObjectUIElement>().SetupButton(toSelect as ObjectInfo,
                    1, onClick);
            }
        }

    }

}
