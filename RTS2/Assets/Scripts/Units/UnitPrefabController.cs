using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPrefabController : MonoBehaviour
{
    public List<UnitPrefab> AllUnitPrefabs;
    Dictionary<UnitType,UnitPrefab> allUnitPrefabs;

    static UnitPrefabController instance;
    public static UnitPrefabController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnitPrefabController>(true);
                instance.Init();
            }
            return instance;
        }
    }


    public GameObject CreateUnitFromSavedData(string data)
    {
        Debug.Log("Unit Data: " + data);
        string[] splitData = data.Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT);
        string[] KeyDataSplit = null;
        Dictionary<string, object> deserialized = new Dictionary<string, object>();
        for(int x = 0; x < splitData.Length; x++)
        {
            KeyDataSplit = splitData[x].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT);
            if (KeyDataSplit.Length == 2)
            {
                KeyDataSplit[0] = KeyDataSplit[0].Replace(SerializeDataHelpers.DATA_OBJECT_SPLIT.ToString(), "");
                object o = DataReaders.ParseDataObject(KeyDataSplit[0], KeyDataSplit[1]);
                deserialized.Add(KeyDataSplit[0], o);
                Debug.Log("Unit data: added key " + KeyDataSplit[0] + "|" + KeyDataSplit[1]);
            }
        }
            Debug.Log("Unit data: type " + deserialized[DataKeys.UnitType].ToString());

        UnitType type = (UnitType)(int)deserialized[DataKeys.UnitType];
        GameObject retVal = Instantiate(allUnitPrefabs[type].UnitSO.Prefab);

        ObjectHealth health = retVal.GetComponent<ObjectHealth>();
        health.ForceHealthValues((float)deserialized[DataKeys.Health], (float)deserialized[DataKeys.MaxHealth]);
        retVal.GetComponent<UnitFaction>().MyFactionID = (string)deserialized[DataKeys.UnitFaction];
        retVal.transform.position = (Vector3)deserialized[DataKeys.Pos];
        retVal.GetComponent<Unit>().SetMyUID((ulong)deserialized[DataKeys.UID]);

        return retVal;
    }

    public void Init()
    {
        allUnitPrefabs=new Dictionary<UnitType, UnitPrefab> ();
        for(int x=0;x<AllUnitPrefabs.Count;x++)
        {
            allUnitPrefabs.Add(AllUnitPrefabs[x].type, AllUnitPrefabs[x]);
        }
    }




}
[System.Serializable]
public class UnitPrefab
{
    public UnitType type;
    public UnitTypeSO UnitSO;
}