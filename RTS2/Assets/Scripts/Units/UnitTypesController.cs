using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitTypesController : MonoBehaviour
{
  static UnitTypesController instance;
    public static UnitTypesController Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<UnitTypesController>(true);
                instance.Init();
            }
            return instance;
        }
    }
   public const string BaseZombie = "Zombie",BaseRilfeman= "Rifleman";
    const string FilePath = "UnitData";
    public Dictionary<string, UnitTypeSO> Units;
    public Dictionary<string, CachedUnitData> UnitData;
    public List<string> UnitKeys;

    public void Init()
    {
        Units = new Dictionary<string, UnitTypeSO>();
        UnitData = new Dictionary<string, CachedUnitData>();
        UnitKeys = new List<string>();
        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            UnitTypeSO i = (UnitTypeSO)items[x];
            if (Units.ContainsKey(i.UnitType) == false)
            {
                CacheDataFromUnitType(i.UnitType, i);
                Units.Add(i.UnitType, i);
                UnitKeys.Add(i.UnitType);
            }
        }
    }

    void CacheDataFromUnitType(string key,UnitTypeSO unitData)
    {
        CachedUnitData data = new CachedUnitData(0, 0, 0, 0, 0,0);
        GetDataFromPrefab(unitData.Prefab, ref data);
        UnitData.Add(key, data);
    }

    void GetDataFromPrefab(GameObject prefab,ref CachedUnitData output)
    {
        Unit u = prefab.GetComponent<Unit>();
        output.MoveSpeed = prefab.GetComponent<Unit>().Speed();
        output.Health = prefab.GetComponent<ObjectHealth>().MaxHealth;
        output.MaxHealth = prefab.GetComponent<ObjectHealth>().MaxHealth;
        bool GotMeleeWeapon = false, GotRangedWeapon = false;
        if (prefab.GetComponent<ItemUnitInit>())
        {
            List<string> ItemsInitWith = prefab.GetComponent<ItemUnitInit>().itemsToAdd;
            Item i = null; Weapon w = null;
            for (int x = 0; x < ItemsInitWith.Count; x++)
            {
                i = ItemController.Instance.AllItems[ItemsInitWith[x]];
                if (i as Weapon != null)
                {
                    w = i as Weapon;
                    if (w.IsRanged)
                    {
                        GotRangedWeapon = true;
                        output.RangedDamage = w.RangedDamage;
                        output.RangeMax = w.FireMinRange;
                        output.RangeMin = w.FireMaxRange;
                        output.AttackRate = w.FireRate;
                    }
                    else
                    {
                        GotMeleeWeapon = true;
                        output.MeleeDamage = w.AttackDamage;
                        output.AttackRate = w.AttackRate;
                    }
                }
            }
        }
        if (!GotRangedWeapon)
        {
            output.RangedDamage = 0;
            output.RangeMax = 0;
            output.RangeMin = 0;
        }
        if (!GotMeleeWeapon)
        {
            output.MeleeDamage = 5;
            output.AttackRate = 1f;
        }

    }

    public bool CanConvertUnitsWithObject(ConstructableObjectInstance objectFound,ref string typeCanConvertTo)
    {
        bool isConversionType = false;
        foreach(KeyValuePair<string,UnitTypeSO> kvp in Units)
        {
            if (kvp.Value.ObjectsToTrainFrom.Contains(objectFound.Name()))
            {
                typeCanConvertTo = kvp.Value.UnitType;
                isConversionType = true;
            }
        }

        if (!isConversionType)
        {
            Debug.Log("convert: wrong type");

            return false;
        }

        bool hasCapacity = false;


       int capacity = UnitCapacityManager.GetMaxCapacityForUnitType(typeCanConvertTo);
        hasCapacity = capacity > UnitMoniter.Instance.GetUserUnitCount(typeCanConvertTo);

        if (!hasCapacity)
        {
            Debug.Log("convert: no capacity");
            return false;
        }



        bool isInRoom = false;

        for(int x=0;x< RoomManager.Instance.roomList.Count; x++)
        {
            if (RoomManager.Instance.roomList[x].ObjectsInRoom.Contains(objectFound) && UnitTrainingHelpers.IsRoomRightToTrainUnit(typeCanConvertTo, RoomManager.Instance.roomList[x].roomType))
            {
                isInRoom = true;
            }
        }

        if (!isInRoom)
        {
            Debug.Log("convert: not in room");

            return false;
        }

        return true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            List<Unit> unitsToTurn = new List<Unit>();
            unitsToTurn.AddRange(UnitMoniter.Instance.AllUnits);
            for (int x=0;x< unitsToTurn.Count; x++)
            {
                UnitTrainingHelpers.TurnUnitIntoOtherUnit(unitsToTurn[x], "Engineer");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            List<Unit> unitsToTurn = new List<Unit>();
            unitsToTurn.AddRange( UnitMoniter.Instance.AllUnits);

            for (int x = 0; x < unitsToTurn.Count; x++)
            {
                UnitTrainingHelpers.TurnUnitIntoOtherUnit(unitsToTurn[x], "Rifleman");
            }
        }
    }
}

public class CachedUnitData
{
    public float MoveSpeed, RangedDamage, RangeMin, RangeMax, MeleeDamage,Health,MaxHealth,AttackRate;
    public CachedUnitData(float speed,float rangedDam,float rangeMin,float rangeMax,float meleeDamage,float attackRate)
    {
        MoveSpeed = speed;
        RangedDamage = rangedDam;
        RangeMin = rangeMin;
        RangeMax=rangeMax;
        MeleeDamage = meleeDamage;
        AttackRate = attackRate;
    }
}
