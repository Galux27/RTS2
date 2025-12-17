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
   public const string BaseZombie = "Zombie";
    const string FilePath = "UnitData";
    public Dictionary<string, UnitTypeSO> Units;
    public List<string> UnitKeys;

    public void Init()
    {
        Units = new Dictionary<string, UnitTypeSO>();

        UnitKeys = new List<string>();
        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            UnitTypeSO i = (UnitTypeSO)items[x];
            if (Units.ContainsKey(i.UnitType) == false)
            {
                Units.Add(i.UnitType, i);
                UnitKeys.Add(i.UnitType);
            }
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
