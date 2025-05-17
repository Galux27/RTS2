using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class BehaviourDeserializer
{

    public static List<string> BehavioursToApply;
    public static List<Unit> ToApplyTo;

    public static void AddBehaviourToDeserialize(string data,Unit applyingTo)
    {
        if(BehavioursToApply == null)
        {
            BehavioursToApply = new List<string>();
            ToApplyTo=new List<Unit>();
        }
        BehavioursToApply.Add(data);
        ToApplyTo.Add(applyingTo);
    }

    public static void DeserializeBehaviours()
    {
        for(int x=0;x<BehavioursToApply.Count;x++)
        {
            try
            {
                DeserializeBehaviour(BehavioursToApply[x], ToApplyTo[x]);
            }
            catch
            {
                Debug.LogError("Error deserializing behaviour " + BehavioursToApply[x]);
            }
        }
        BehavioursToApply.Clear();
        ToApplyTo.Clear();
    }

   public static void DeserializeBehaviour(string data,Unit applyingTo)
    {
        Debug.Log("Behaviour: Deserializing behaviour from " + data);
        data = data.Replace(SerializeDataHelpers.BEHAVIOUR_MARKER.ToString(), "");
        data=data.Remove(0, DataKeys.Behaviour.Length+ 1);
        string[] allData = data.Split(SerializeDataHelpers.DATA_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, object> sortedData = new Dictionary<string, object>();
        string[] keyValueSplit = null;
        for(int x = 0; x < allData.Length; x++)
        {
            allData[x] = allData[x].Replace(SerializeDataHelpers.DATA_ELEMENT_SPLIT.ToString(), "");
            Debug.Log("Behaviour: parsing from " + allData[x]);
            keyValueSplit = allData[x].Split(SerializeDataHelpers.KEY_OBJECT_SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
            sortedData.Add(keyValueSplit[0], DataReaders.ParseDataObject(keyValueSplit[0],keyValueSplit[1]));
        }

        string typeName = (string)sortedData[DataKeys.BehaviourType];
        if (typeName == null)
        {
            Debug.LogError("Behaviour: Type name was null " + (typeName == null) + "|" + sortedData.Count);
            return;
        }
        Type type = Type.GetType(typeName);
        if (type == null)
        {
            Debug.LogError("Behaviour: Could not get type " + typeName);

        }
        else
        {
            Debug.Log("Behaviour: Found type " + type.ToString() + " poggers");
        }
        BehaviourBase behaviour = (BehaviourBase)Activator.CreateInstance(type, false);
        behaviour.InitializeFromData(applyingTo,sortedData);
        applyingTo.GetComponent<BehaviourRunner>().SetBehaviour(behaviour);
    }
}
