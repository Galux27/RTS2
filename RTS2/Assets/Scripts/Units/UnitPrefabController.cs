using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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


    public GameObject CreateUnitFromSavedData(string data,bool checkForChunkBeingLoaded=false)
    {
        Debug.Log("Unit Data: " + data);
        string[] inventorySplit = data.Split(SerializeDataHelpers.INVENTORY_MARKER);
        Debug.Log("Inventory: inventory split 0" + inventorySplit[0] + " 1 " + inventorySplit[1]);
        string[] behaviourSplit = inventorySplit[0].Split(SerializeDataHelpers.BEHAVIOUR_MARKER);
        string[] splitData = behaviourSplit[0].Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT);
        string[] KeyDataSplit = null;
        Dictionary<string, object> deserialized = new Dictionary<string, object>();
        for(int x = 0; x < splitData.Length; x++)
        {
            Debug.Log("Unit Data: parsing " + splitData[x]);
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


        Vector3 worldPos = (Vector3)deserialized[DataKeys.Pos];

        if (checkForChunkBeingLoaded)
        {
            Vector2Int chunkBatch = WorldChunkManager.Instance.ConvertPositionToChunkBatchCoords(worldPos);
            if(WorldChunkManager.Instance.DoesChunkExist(chunkBatch)==false)
            {
                WorldChunkManager.Instance.AddUnitToLoadWhenChunkLoads(chunkBatch, data);
                return null;
            }
        }


        UnitType type = (UnitType)(int)deserialized[DataKeys.UnitType];
        GameObject retVal = Instantiate(allUnitPrefabs[type].UnitSO.Prefab);

        ObjectHealth health = retVal.GetComponent<ObjectHealth>();
        health.ForceHealthValues((float)deserialized[DataKeys.Health], (float)deserialized[DataKeys.MaxHealth]);
        retVal.GetComponent<UnitFaction>().MyFactionID = (string)deserialized[DataKeys.UnitFaction];
        retVal.transform.position = worldPos;
        retVal.GetComponent<Unit>().SetMyUID((ulong)deserialized[DataKeys.UID]);
        retVal.GetComponent<Unit>().MyHealth.ForceHealthValues((float)deserialized[DataKeys.Health], (float)deserialized[DataKeys.MaxHealth]);
        Debug.Log("Unit Data: invr split 1 " + behaviourSplit[2].ToString());

        KeyDataSplit = behaviourSplit[2].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT);
        // ^INVENTORY_UID; 4214:^INVENTORY;
        KeyDataSplit[0]=KeyDataSplit[0].Replace(SerializeDataHelpers.DATA_OBJECT_SPLIT.ToString(), "");
        KeyDataSplit[1] = KeyDataSplit[1].Split(SerializeDataHelpers.DATA_ELEMENT_SPLIT)[0];
        Debug.Log("unit data: key obj split " + KeyDataSplit[0] + "1 " + KeyDataSplit[1]);

        retVal.GetComponent<UnitOrders>().SetOrdersFromFile((Dictionary<string, bool>)deserialized[DataKeys.Orders]);
        ulong id = (ulong) DataReaders.ParseDataObject(KeyDataSplit[0], KeyDataSplit[1]);

        BehaviourDeserializer.AddBehaviourToDeserialize(behaviourSplit[1],retVal.GetComponent<Unit>());
        if (inventorySplit.Length > 1)
        {
            InventoryDeserializer.AddInventoryToDeserialize(inventorySplit[1],retVal.GetComponent<Inventory>().GetType());


        }
     
        retVal.GetComponent<Inventory>().SetMyUID(id);
        

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