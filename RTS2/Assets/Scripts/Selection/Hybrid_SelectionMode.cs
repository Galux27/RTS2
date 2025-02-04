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
        if (currentSubSelectionMode != null)
        {
            currentSubSelectionMode.OnLeftMouseUp();
        }
        else
        {
            CurrentSelectionType = SelectionUtilities.GetSelectablesInRange(out CurrentlySelected);
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

    void ResetSelected()
    {
        currentSubSelectionMode = null;
    }

    void OnSelectObjects()
    {
        switch (CurrentSelectionType)
        {
            case SelectableType.None:
                break;
            case SelectableType.Unit:
                
                currentSubSelectionMode = SelectionController.Instance.Units;
                SelectionController.OnSwitchSelectionMode.Invoke(CurrentSelectionMode.Units);
                SelectableManager.Instance.AddSelectable(CurrentlySelected);
                SelectableManager.OnSelectionChanged?.Invoke();

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

}
