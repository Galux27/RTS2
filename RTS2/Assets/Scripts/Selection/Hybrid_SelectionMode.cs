using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hybrid_SelectionMode : SelectionMode
{
    List<Selectable> CurrentlySelected;
    SelectableType CurrentSelectionType;
    SelectionMode currentSubSelectionMode;

    public override void OnLeftMouseUp()
    {
        //if (currentSubSelectionMode != null)
        //{
        //    currentSubSelectionMode.OnLeftMouseUp();
        //}
        //else
        {
            CurrentSelectionType = SelectionUtilities.GetSelectablesInRange(out CurrentlySelected);
            Debug.Log("Current selection type "+  CurrentSelectionType.ToString()+"|"+CurrentlySelected.Count);
            if(CurrentSelectionType!=SelectableType.None)
            {
                OnSelectObjects();
            }
            else
            {
                ResetSelected();
            }
        }
    }

    public override void OnRightMouseUp() 
    {
        if (currentSubSelectionMode != null)
        {
            currentSubSelectionMode.OnRightMouseUp();
        }
    }

    public override void OnHover()
    {
        if (currentSubSelectionMode != null)
        {
            currentSubSelectionMode.OnHover();
        }
    }

    public void ResetSelected()
    {
        currentSubSelectionMode = null;
        SelectableManager.Instance.CurrentSelectedType = CurrentSelectionType;

        SelectableManager.Instance.ClearSelectables();
        SelectableManager.OnSelectionChanged?.Invoke();
    }

    void OnSelectObjects()
    {
        SelectedObjectsUI.Instance.CloseUI();
        SelectableManager.Instance.CurrentSelectedType = CurrentSelectionType;

        switch (CurrentSelectionType)
        {
            case SelectableType.Unit:        
                currentSubSelectionMode = SelectionController.Instance.Units;
                SelectionController.OnSwitchSelectionMode?.Invoke(CurrentSelectionMode.Units);  
                break;
            default:
                break;
        }
        SelectableManager.Instance.ClearSelectables();
        SelectableManager.Instance.AddSelectable(CurrentlySelected);
        SelectableManager.OnSelectionChanged?.Invoke();

        switch (CurrentSelectionType)
        {
            case SelectableType.ConstructableObject:
                SelectedObjectsUI.Instance.OpenUI();
                break;
            case SelectableType.Item:
                SelectedObjectsUI.Instance.OpenUI();
                break;
            case SelectableType.UnderConstructionObject:
                SelectedObjectsUI.Instance.OpenUI();
                break;
            case SelectableType.Resource:
                SelectedObjectsUI.Instance.OpenUI();
                break;
            case SelectableType.Structure:
                SelectedObjectsUI.Instance.OpenUI();

                break;
            default:
                break;
        }
    }

}
