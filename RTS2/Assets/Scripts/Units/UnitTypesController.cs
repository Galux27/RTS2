using System.Collections;
using System.Collections.Generic;
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
