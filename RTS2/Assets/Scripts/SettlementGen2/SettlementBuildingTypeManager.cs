using System.Collections.Generic;
using UnityEngine;

public class SettlementBuildingTypeManager : MonoBehaviour
{
    static SettlementBuildingTypeManager instance;
    public static SettlementBuildingTypeManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SettlementBuildingTypeManager>();
                instance.Init();
            }
            return instance;
        }
    }
    const string DataLocation = "SettlementData";
    Dictionary<string, SettlementBuildingData> AllData;
    List<string> DataKeys;
    void Init()
    {
        AllData = new Dictionary<string, SettlementBuildingData>();
        DataKeys = new List<string>();
        Object[] objects = Resources.LoadAll(DataLocation);
        for(int x = 0; x < objects.Length; x++)
        {
            if (objects[x] as SettlementBuildingData != null)
            {
                SettlementBuildingData toAdd = objects[x] as SettlementBuildingData;
                DataKeys.Add(toAdd.SettlementType);
                AllData.Add(toAdd.SettlementType, toAdd);
            }
        }

    }

    public SettlementBuildingData GetData(string key)
    {
        if (AllData.ContainsKey(key))
        {
            return AllData[key];
        }
        return null;
    }

    public SettlementBuildingData GetRandomData()
    {
        return GetData(DataKeys[Random.Range(0, DataKeys.Count)]);
    }
}
