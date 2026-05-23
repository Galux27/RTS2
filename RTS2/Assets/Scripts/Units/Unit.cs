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
    public UnitVisualStore MyVisualStore;
    public PathfindingNode LastNode;
    public TileRaycast myRaycast;
    PathFollower myPathFollower;
    public bool hasLastNode = false;
    

    public TileRaycast GetRaycast(Vector3 startPos,Vector3 endPos)
    {
        if (myRaycast == null)
        {
            myRaycast = new TileRaycast(startPos, endPos);
        }
        else
        {
            if (myRaycast.DoesRaycastNeedReinitializing(startPos, endPos))
            {
                myRaycast.InitRaycast(startPos, endPos);
            }
            }
            return myRaycast;
    }

    public PathFollower GetFollower()
    {
        if(myPathFollower == null)
        {
            myPathFollower = new PathFollower(this);
        }
        return myPathFollower;
    }


    public void SetLastNode(PathfindingNode node)
    {
        if (node.IsPassable == false)
        {
            return;
        }
        LastNode = node;
        hasLastNode = true;
    }

    public void UpdateChunk(WorldChunk newChunk)
    {
        bool isSame = MyCurrentChunk!=null && newChunk.LocalXCoord == MyCurrentChunk.x && newChunk.LocalYCoord == MyCurrentChunk.y;
        if (isSame)
        {
            UpdateUnitRenderer(newChunk.IsRendered);

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
    bool IsDrawn = false;
    public void UpdateUnitRenderer(bool show)
    {
        if (this==null|| this.transform == null)
        {
            return;
        }
        if (show&&IsDrawn==false)
        {
            if (MyRender == null)
            {
                MyRender = GameObjectPoolManager.Instance.GetObjectFromPool("UnitRenderer").GetComponent<UnitRenderer>();
                MyRender.transform.parent = this.transform;
                MyRender.transform.localPosition = new Vector3(0, -0.77f, 0);
                MyRender.transform.SetSiblingIndex(0);
               // MyRender.SetUnitVisuals(UnitVisualManager.Instance.AllVisuals[MyType]);
            }
            MyRender.SetUnitVisuals(MyVisualStore);
            MyRender.DrawUnit();
            MyHealth.OnObjectRender(this.gameObject);
            IsDrawn = true;
        }
        else if(!show&&IsDrawn)
        {
            if (MyRender != null)
            {
                MyRender.HideUnit();
                MyRender.transform.parent = null;
                GameObjectPoolManager.Instance.ReturnObjectToPool(MyRender.gameObject, "UnitRenderer");
                MyRender = null;
                MyHealth.OnObjectHidden(this.gameObject);
            }
            IsDrawn = false;
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
        MyVisualStore = this.GetComponent<UnitVisualStore>();
        offset = new Vector3(0, MyVisualStore.Scale.y / 2f, 0);
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
        holding.sr.sortingOrder = 11;
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

    public void OnUnitStandstill()
    {
        if (MyVisualStore.OnStandStill())
        {
            if (MyRender != null)
            {
                MyRender.SetUnitVisuals(MyVisualStore);
            }
        }
    }

    public void MoveUnit(Vector3 direction)
    {
        Vector3 transformation = (direction * Speed() * DeltaTimeWrapper.GameplayDelta);
        if (MyVisualStore.OnMovement(this.transform.position, this.transform.position + transformation))
        {
            if (MyRender != null)
            {
                MyRender.SetUnitVisuals(MyVisualStore);
            }
        }
        
        this.transform.position = RoundPosition(this.transform.position + transformation);
        HasMovedThisFrame = true;
        WorldChunkManager.Instance.OnUnitMove(this);
        OnUnitMove();
    }

    const float increment = 1f / 64;
    Vector3 RoundPosition(Vector3 pos)
    {
        pos.x = (pos.x * increment) / increment;
        pos.y = (pos.y * increment) / increment;
        pos.z = (pos.z * increment) / increment;

        return pos;
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
        if (HasMovedThisFrame)
        {
            HasMovedThisFrame = false;
        }
        else
        {
            OnUnitStandstill();
        }
#if UNITY_EDITOR
        //SelectionUtilities.DrawBounds(this.transform.position, GetSize(),Color.cyan);
#endif
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
        return this.MyVisualStore.Scale;
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
    Vector3 offset;
    public Vector3 GetCenterOffset()
    {
        return offset ;
    }
}

public enum UnitType {
    None=0,
    Zombie=1,
    Human=2,
    Rifleman=3,
    Civilian=4,
    Engineer=5,
    Gargant=6,
    Siren=7
}

