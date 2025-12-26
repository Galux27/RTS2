using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Unit : MonoBehaviour,Selectable,ObjectInfo,ISerialize
{
    public bool isSelected=false;
    public bool isSelectable = false;
    public UnitType MyType;
    public UnitAttackController MyAttackController;
    public UnitOrders MyOrders;
    public Action<Unit> OnAttacked;
    public EntityHealth MyHealth;
    public Vector2Int MyCurrentChunk,MyCurrentBatch;
    bool SetChunk = false;
    public UnitFaction MyFaction;
    public UnitSenses MySenses;
    public UnitRenderer MyRender;

    public void UpdateChunk(WorldChunk newChunk)
    {
        bool isSame = MyCurrentChunk!=null && newChunk.LocalXCoord == MyCurrentChunk.x && newChunk.LocalYCoord == MyCurrentChunk.y;
        if (isSame)
        {
            return;
        }
        RemoveUnitFromChunkItsIn();
      
        if (!WorldChunkManager.Instance.ChunkBatches[newChunk.BatchCoords].IsActive)
        {
            GameLifeManager.Instance.ConvertUnitToALifeEntity(this, WorldChunkManager.Instance.ChunkBatches[newChunk.BatchCoords]);
        }
        else
        {
            MyCurrentChunk = new Vector2Int(newChunk.LocalXCoord, newChunk.LocalYCoord);
            MyCurrentBatch = newChunk.BatchCoords;
            newChunk.AddUnitToChunk(this);
            UpdateUnitRenderer(newChunk.IsRendered);
            SetChunk = true;

        }
    }

 
    public void RemoveUnitFromChunkItsIn()
    {
        if (SetChunk)
        {
            WorldChunkManager.Instance.ChunkBatches[MyCurrentBatch].Chunks[MyCurrentChunk.x, MyCurrentChunk.y].RemoveUnitFromChunk(this);
        }
    }

    public void DestroyUnit()
    {
        UpdateUnitRenderer(false);
        MyHealth.OnObjectHidden(this.gameObject);
      //  RemoveUnitFromChunkItsIn();
        GameObject.Destroy(this.gameObject);
    }

    public void UpdateUnitRenderer(bool show)
    {
        if (this==null|| this.transform == null)
        {
            return;
        }
        if (show)
        {
            if (MyRender == null)
            {
                MyRender = GameObjectPoolManager.Instance.GetObjectFromPool("UnitRenderer").GetComponent<UnitRenderer>();
                MyRender.transform.parent = this.transform;
                MyRender.transform.localPosition = Vector3.zero;
                MyRender.SetUnitVisuals(UnitVisualManager.Instance.AllVisuals[MyType]);
            }
            MyRender.DrawUnit();
            MyHealth.OnObjectRender(this.gameObject);
        }
        else
        {
            if (MyRender != null)
            {
                MyRender.HideUnit();
                MyRender.transform.parent = null;
                GameObjectPoolManager.Instance.ReturnObjectToPool(MyRender.gameObject, "UnitRenderer");
                MyRender = null;
                MyHealth.OnObjectHidden(this.gameObject);
            }
            }
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
        MyHealth = new EntityHealth();
       
        MyHealth.CurrentHealth=this.GetComponentInChildren<ObjectHealth>().CurrentHealth;
        MyHealth.MaxHealth=this.GetComponentInChildren<ObjectHealth>().MaxHealth;
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
        holding.sr.sortingOrder = 3;
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
        RemoveUnitFromChunkItsIn();
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

    public Vector2Int lastCoords = new Vector2Int();
    bool GotLastCoords = false;
    public Action<Vector2Int> OnEnterNewTile;
    void OnUnitMove()
    {
        Vector2Int coordsCurrent=Pathfinding.GetCoordsFromPosition(this.transform.position);
        if (!GotLastCoords)
        {
            lastCoords = coordsCurrent;
            GotLastCoords = true;
        }
        if (coordsCurrent != lastCoords)
        {
            WorldController.Instance.OnTileExit(lastCoords, this);

            lastCoords = coordsCurrent;
            OnEnterNewTile?.Invoke(coordsCurrent);
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
        SelectionUtilities.DrawBounds(this.transform.position, GetSize(),Color.cyan);
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

    void Health.AdjustHealth(float value)
    {
        if (value > 0)
        {
            MyHealth.IncreaseHealth(value);
        }
        else
        {
            MyHealth.DecreaseHealth(value);
        }
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize data=new DataToSerialize();
        data.AddDataToSerialize(DataKeys.Pos, this.transform.position);
        data.AddDataToSerialize(DataKeys.UID, GetMyUID().Value.ToString());
        data.AddDataToSerialize(DataKeys.UnitType, (int)MyType);
        data.AddDataToSerialize(DataKeys.UnitFaction, MyFaction.MyFactionID);
        data.AddDataToSerialize(DataKeys.Health, MyHealth.CurrentHealth);
        data.AddDataToSerialize(DataKeys.MaxHealth,MyHealth.MaxHealth);
        data.AddDataToSerialize(DataKeys.Orders, MyOrders.SerializeOrders());

        //behaviour is second to last
        data.AddDataToSerialize(DataKeys.Behaviour, behaviourRunner.CurrentBehaviour);


        //inventory is always last
        data.AddDataToSerialize(DataKeys.InventoryUID, GetComponent<Inventory>().GetMyUID().Value);
        data.AddDataToSerialize(DataKeys.Inventory, GetComponent<Inventory>().Serialize().Data);
        //Inventory


        return data;
    }

    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new NotImplementedException();
    }

    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value==0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);

        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
        myUid = new UID(uid);
        Debug.Log("Inventory: setting UID for " + this.GetType().ToString() + " uid " + uid);
        IDManager.OnUIDCreated(this, myUid);
    }

    public UID MyUID()
    {
        return GetMyUID();
    }
}

public enum UnitType {
    None=0,
    Zombie=1,
    Human=2,
    Rifleman=3,
    Civilian=4,
    Engineer=5
}

