using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : MonoBehaviour,Selectable,ObjectInfo
{
    public bool isSelected=false;
    public bool isSelectable = false;
    public UnitType MyType;
    public UnitAttackController MyAttackController;
    public UnitOrders MyOrders;
    public Action<Unit> OnAttacked;
    ObjectHealth MyHealth;
    public Vector2Int MyCurrentChunk;
    public UnitFaction MyFaction;
    public UnitSenses MySenses;

    public void UpdateChunk(Vector2Int newChunk)
    {
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
        MyFaction = this.GetComponent<UnitFaction>();
        MySenses = this.GetComponent<UnitSenses>();
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



    public void OnDeath()
    {
        OnObjectDeselected();
        UnitMoniter.Instance.RemoveUnit(this);
        WorldChunkManager.Instance.OnUnitDeath(this);
        Destroy(this.gameObject);
    }

    public void OnObjectDeselected()
    {

        if (this!=null)
        {
            if (SelectableManager.Instance.CurrentlySelected.Contains(this))
            {
                SelectableManager.Instance.RemoveSelectable(this);
            }
            this.GetComponentInChildren<SelectedOutline>()?.OnDeselect();
        }
    }
        public void OnObjectSelected()
    {
        SelectedOutlineManager.Instance.OnSelectObject(this.gameObject);
    }

    public virtual float Speed()
    {
        return 5f;
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


    public void SetPassable()
    {
        StartCoroutine(MakePassable());
    }

    IEnumerator MakePassable()
    {
        this.gameObject.layer = LayerMask.NameToLayer("PawnsSwap");
        yield return new WaitForSeconds(1f);
        this.gameObject.layer = LayerMask.NameToLayer("Pawns");

    }

    public void MoveUnit(Vector3 direction)
    {
        this.transform.position += (direction * Speed() * DeltaTimeWrapper.GameplayDelta);
        HasMovedThisFrame = true;
        WorldChunkManager.Instance.OnUnitMove(this);
        OnUnitMove();
    }

    Vector2Int lastCoords = new Vector2Int();
 
    void OnUnitMove()
    {
        Vector2Int coordsCurrent=Pathfinding.GetCoordsFromPosition(this.transform.position);

        if (coordsCurrent != lastCoords)
        {
            WorldController.Instance.OnTileExit(lastCoords, this);

            lastCoords = coordsCurrent;
            WorldController.Instance.OnTileEnter(coordsCurrent, this);

        }
        HasBeenSwapped = false;
    }
    public bool HasBeenSwapped = false, HasMovedThisFrame = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Unit unitHit = collision.collider.GetComponent<Unit>();
        if (unitHit!=null && HasMovedThisFrame)
        {
            UnitHelpers.OnUnitCollision(this, unitHit);
        }
    }

    private void Update()
    {
        HasMovedThisFrame = false;
    }

    public virtual void AttackUnit(float damage,Unit isAttackingMe=null)
    {
        MyHealth.DecreaseHealth(damage);
        OnAttacked?.Invoke(isAttackingMe);
    }
 

    private void OnDestroy()
    {
        UnitMoniter.Instance?.RemoveUnit(this);
    }

    public bool IsSelectable()
    {
        return isSelectable;
    }

    public Vector3 GetSize()
    {
        return Vector3.one;
    }

   public bool IsPointInBounds(Vector3 point)
    {
        return SelectionUtilities.IsInBounds(GetSize(), this.transform.position, point);
    }

    public string Name()
    {
        return MyType.ToString();
    }

    public string Description()
    {
        return MyType.ToString();
    }

    public int Quantitiy()
    {
        return 1;
    }

    public float Health()
    {
        return MyHealth.CurrentHealth;
    }

    public float MaxHealth()
    {
        return MyHealth.MaxHealth;
    }

    public Vector3 Position()
    {
        return this.transform.position;
    }
}

public enum UnitType {
    None,
    Zombie,
    Human,
    Rifleman,
    Civilian,
    Engineer
}

