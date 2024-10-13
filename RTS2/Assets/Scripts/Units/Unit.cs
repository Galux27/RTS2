using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : MonoBehaviour,Selectable
{
    public bool isSelected=false;
    public bool isSelectable = false;
    public UnitType MyType;
    protected void Awake()
    {
        UnitMoniter.Instance.AddUnit(this);
        if (this.GetComponent<ItemHolder>() && this.GetComponent<BodyController>())
        {
            this.GetComponent<ItemHolder>().OnSetHolding += OnHoldItem;
        }
    }

    void OnHoldItem(ItemInWorld holding)
    {
        holding.sr.sortingOrder = this.GetComponent<BodyController>().Torso.sortingOrder + 1;
    }


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
    }

    public virtual float Speed()
    {
        return 5f;
    }

  

    public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(this.gameObject);
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


    public void MoveUnit(Vector3 direction)
    {
        this.transform.position += (direction * Speed() * Time.deltaTime);
    }

 

    private void OnDestroy()
    {
        UnitMoniter.Instance.RemoveUnit(this);
    }

    public bool IsSelectable()
    {
        return isSelectable;
    }
}

public enum UnitType {
None,
Zombie,
Human

}

