using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : MonoBehaviour,Selectable
{
    public bool isSelected=false;
    public bool GetIsSelected()
    {
        return isSelected;
    }

    public SelectableType GetSelectableType()
    {
        return SelectableType.Unit;
    }

    public void OnObjectDeselected()
    {
        this.GetComponentInChildren<SelectedOutline>().OnDeselect();

        //SelectableManager.Instance.AddSelectable(this);
    }

    public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(this.gameObject);
        Debug.Log("Setting unit selected");
        //SelectableManager.Instance.AddSelectable(this);
    }

    public void SetIsSelected(bool v)
    {
        isSelected = v;
        if (isSelected)
        {
            OnObjectSelected();
        }
        else
        {
            OnObjectDeselected();
        }
    }

    private void Awake()
    {
        UnitMoniter.Instance.AddUnit(this);
    }

    private void OnDestroy()
    {
        UnitMoniter.Instance.RemoveUnit(this);
    }
}
