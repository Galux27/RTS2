using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


/// <summary>
/// Stores information about an instance of an EnvironmentObject in the world (Does not mean that the object is being drawn)
/// </summary>
public class EnvironmentObjectInstance:ObjectInfo,ISerialize
{
    public string ObjectKey;
    public int PosX
    {
        get
        {
            return coords.x;
        }
    }
        
     public int PosY
    {
        get
        {
            return coords.y;
        }
    }
    public bool Drawn = false;
    public GameObject Object;
    WorldChunk myChunk;
    Vector3 position;
    public Vector2Int coords;
    bool needsUpdate = false;
    public void SetChunk(WorldChunk chunk)
    {
        myChunk= chunk;
    }

    public EnvironmentObjectInstance(int x,int y,string envObj)
    {
        try
        {
            ObjectKey = envObj;
            position = new Vector3(x, y);
            coords = new Vector2Int(x, y);
            EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(envObj);
            MyHealth = new EntityHealth();
            MyHealth.MaxHealth = obj.MaxHealth;
            MyHealth.CurrentHealth = obj.MaxHealth;
            needsUpdate = obj.RequiresUpdate;
        }
        catch
        {
            Debug.LogError("Error creating environment object instance " + envObj);
        }
    }


    public virtual void RenderInstance()
    {
        if (Drawn)
        {
            return;
        }

        EnvironmentObject obj = EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey);
        Object = GameObjectPoolManager.Instance.GetObjectFromPool("EnvironmentObject");
        Object.transform.position = new Vector3(PosX, PosY, 0);
        Object.GetComponent<SpriteRenderer>().sprite = obj.ForwardsSprite;     
        Object.SetActive(true);
        Drawn = true;
        GameController.Instance.OnUpdate += OnUpdate;

    }

    public virtual void CleanupInstance()
    {

        if (!Drawn) { return; }
        GameObjectPoolManager.Instance.ReturnObjectToPool(Object,"EnvironmentObject");
        Drawn = false;
        GameController.Instance.OnUpdate -= OnUpdate;

    }

    public virtual bool CanHarvest()
    {
        return EnvironmentObjectManager.Instance.AllObjects.ContainsKey(ObjectKey) &&
            EnvironmentObjectManager.Instance.AllObjects[ObjectKey].CanHarvest;
    }


    void OnUpdate()
    {
        //DebugDrawing.Instance.DrawEnvironmentObjectInstance(this);
    }


    Timer Timer;
    float harvestTimer = 0;
   public bool isHarvested = false;
    public virtual void Harvest()
    {
        if (CanHarvest() == false|| isHarvested)
        {
            return;
        }
        if (Timer == null)
        {
            Timer = new Timer(EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey).Resources.HarvestLength);
            Timer.CreateProgressBarFromTimer(GetPosition());
            
        }
        Timer.ProgressTime(DeltaTimeWrapper.GameplayDelta);
        
        if (Timer.IsTimerFinished())
        {
            EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey).Resources.GenerateResoruces(new Vector3(PosX, PosY, 0));
            DestroyInstance();
            isHarvested = true;
        }
    }

    public virtual Vector3 GetPosition()
    {
        
        return position;
    }

    public void DestroyInstance()
    {
        if (!EnvironmentObjectHelpers.GetEnvironmentObject(ObjectKey).DestroyOnHarvest)
        {
            return;
        }
        if (healthUI != null)
        {
            healthUI.Cleanup();

            GameObjectPoolManager.Instance.ReturnObjectToPool(healthUI.gameObject, "WorldspaceHealthBar");
        }
        myChunk.RemoveEnvironmentObject(this);
        EnvironmentObjectManager.Instance.OnDestroyEnvironmentObject(this);
    }

    public string Name()
    {
        return ObjectKey;
    }

    public string Description()
    {
        return "";
    }

    public int Quantitiy()
    {
        return 1;
    }
    public EntityHealth MyHealth;
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
        return position;
    }
    public void OverrideHealth(float val,float max)
    {
        MyHealth.MaxHealth = max;
        MyHealth.CurrentHealth = val;
    }


    public void AdjustHealth(float value)
    {
        if (value > 0)
        {
            MyHealth.IncreaseHealth(value);
        }
        else
        {
            MyHealth.DecreaseHealth(value*-1);
        }

        if (Health() > MaxHealth())
        {
            MyHealth.CurrentHealth = MaxHealth();
        }
        else if (Health() <= 0)
        {
            OnDeath();
        }

      
      if (healthUI == null)
      {
           DrawHealthUI();
      }
      UpdateHealthUI();
        
    }
        HealthUI healthUI;
    void DrawHealthUI()
    {
            if (!Drawn)
            {
                return;
            }
        healthUI = GameObjectPoolManager.Instance.GetObjectFromPool("WorldspaceHealthBar").GetComponent<HealthUI>();
        healthUI.gameObject.SetActive(true);
        healthUI.LinkToObjectInfo(this);
    }

    void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHealth();
        }
    }

    public void OnDeath()
    {
        DestroyInstance();
    }

    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize data = new DataToSerialize();
        data.AddDataToSerialize(DataKeys.Coords, coords);
        data.AddDataToSerialize(DataKeys.ObjectKey, ObjectKey);
        data.AddDataToSerialize(DataKeys.UID, GetMyUID().Value);
        data.AddDataToSerialize(DataKeys.Health, Health());
        data.AddDataToSerialize(DataKeys.MaxHealth, MaxHealth());
        DataToSerialize extra = GetExtraDataToSerialize();
        if (extra != null)
        {
            foreach (KeyValuePair<string, object> kvp in extra.data)
            {
                data.AddDataToSerialize(kvp.Key, kvp.Value);
            }
        }
        return data;
    }

    public virtual DataToSerialize GetExtraDataToSerialize()
    {
        return null;
    }


    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new System.NotImplementedException();
    }
    UID myUid;
    public UID GetMyUID()
    {
        if (myUid.Value == 0)
        {
            myUid = IDManager.GetUIDForObject();
            IDManager.OnUIDCreated(this, myUid);
        }
        return myUid;
    }

    public void SetMyUID(ulong uid)
    {
        myUid = new UID(uid);
        IDManager.OnUIDCreated(this, myUid);

    }

    public UID MyUID()
    {
        return GetMyUID();
    }
}
