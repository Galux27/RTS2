using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : MonoBehaviour,Selectable
{
    bool isSelected=false;
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
        SelectableManager.Instance.AddSelectable(this);
    }

    public void OnObjectSelected()
    {
        SelectableManager.Instance.AddSelectable(this);
    }

    public void SetIsSelected(bool v)
    {
        isSelected = v;
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
