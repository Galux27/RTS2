using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : MonoBehaviour,Selectable
{
    public bool isSelected=false;
    public bool isSelectable = false;
    public UnitType MyType;
    public UnitAttackController MyAttackController;
    ObjectHealth MyHealth;
    protected void Awake()
    {
        UnitMoniter.Instance.AddUnit(this);
        if (this.GetComponent<ItemHolder>() && this.GetComponent<BodyController>())
        {
            this.GetComponent<ItemHolder>().OnSetHolding += OnHoldItem;
        }
        MyHealth=this.GetComponentInChildren<ObjectHealth>();
        MyAttackController = this.GetComponent<UnitAttackController>();
        MyHealth.OnDeath += OnDeath;
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


    void OnDeath()
    {
        OnObjectDeselected();
        UnitMoniter.Instance.RemoveUnit(this);
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

    public virtual void AttackUnit(float damage)
    {
        Debug.Log("UNit " + this.gameObject.name + " attacked for " + damage);
        MyHealth.DecreaseHealth(damage);
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

