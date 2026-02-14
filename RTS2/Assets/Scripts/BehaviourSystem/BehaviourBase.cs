using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Base class for a behaviour that a unit can perform e.g. move to location, attack other unit...
/// </summary>
public class BehaviourBase:ISerialize
{
    protected Unit unitToMove;

    public Action OnComplete;
    public bool IsUserInstruction = false;

    public virtual void InitBehaviour(Unit toPerform)
    {
        unitToMove= toPerform; 
    }


    public virtual bool CanPerformBehaviour()
    {
        return false;
    }

    public virtual void PerformBehaviour()
    {

    }

    public virtual bool IsBehaviourComplete()
    {
        return false;
    }

    public virtual bool DoWeNullBehaviourOnComplete()
    {
        return true;
    }

    public virtual DataToSerialize GetBehaviourSpecificData()
    {


        return new DataToSerialize();
    }

    public virtual string BehaviourType()
    {
        return this.GetType().ToString();
    }


    public virtual KeyCode GetShortcutForBehaviour()
    {
        return KeyCode.None;
    }


    public DataToSerialize GetDataToSerialize()
    {
        DataToSerialize otherData = GetBehaviourSpecificData();
        DataToSerialize retVal = new DataToSerialize();
        retVal.AddDataToSerialize(DataKeys.UID, unitToMove.GetMyUID().Value);
        retVal.AddDataToSerialize(DataKeys.BehaviourType, BehaviourType());
        foreach(KeyValuePair<string,object> kvp in otherData.data)
        {
            retVal.AddDataToSerialize(kvp.Key,kvp.Value);
        }
       return retVal;
    }

    public SerializedData Serialize()
    {
        return new SerializedData(GetDataToSerialize());
    }

    public void Deserialize(SerializedData data)
    {
        throw new NotImplementedException();
    }

    public virtual void InitializeFromData(Unit performing,Dictionary<string,object> data)
    {

    }
    
    public UID GetMyUID()
    {
        throw new NotImplementedException();
    }

    public void SetMyUID(ulong uid)
    {
        throw new NotImplementedException();
    } 

    public virtual void OnDestroy()
    {

    }
}
