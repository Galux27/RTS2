using System;
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
    public UnitOrders MyOrders;
    public Action<Unit> OnAttacked;
    ObjectHealth MyHealth;
    public Vector2Int MyCurrentChunk;


    public void UpdateChunk(Vector2Int newChunk)
    {
        Debug.Log("Chunk: updating chunk from " + MyCurrentChunk + " to " + newChunk);
        MyCurrentChunk = newChunk;
    }


    BehaviourRunner behaviourRunner;

    public BehaviourRunner BehaviourRunner {
        get
        {
            return behaviourRunner;
        }
    }


    ItemHolder itemHolder;
    public ItemHolder ItemHolder
    {
        get
        {
            return itemHolder;
        }
    }

    protected void Awake()
    {
        if (this.GetComponent<ItemHolder>() && this.GetComponent<BodyController>())
        {
            this.GetComponent<ItemHolder>().OnSetHolding += OnHoldItem;
        }
        MyHealth=this.GetComponentInChildren<ObjectHealth>();
        MyAttackController = this.GetComponent<UnitAttackController>();
        MyHealth.OnDeath += OnDeath;
        behaviourRunner= this.GetComponent<BehaviourRunner>();
        itemHolder=this.GetComponent<ItemHolder>(); 
        MyOrders= this.GetComponent<UnitOrders>();
    }

    public bool GetOrderVal(string key)
    {
        if (MyOrders == null || MyOrders.GetOrder(key) == null)
        {
            return false;
        }
        return MyOrders.GetOrder(key).Value;
    }



    void Start()
    {
        UnitMoniter.Instance.AddUnit(this);
        WorldChunkManager.Instance.OnUnitCreated(this);
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
        WorldChunkManager.Instance.OnUnitDeath(this);
        Destroy(this.gameObject);
    }

    public void OnObjectDeselected()
    {
        this.GetComponentInChildren<SelectedOutline>()?.OnDeselect();
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
        WorldChunkManager.Instance.OnUnitMove(this);
    }

    public virtual void AttackUnit(float damage,Unit isAttackingMe=null)
    {
        MyHealth.DecreaseHealth(damage);
        OnAttacked?.Invoke(isAttackingMe);
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

