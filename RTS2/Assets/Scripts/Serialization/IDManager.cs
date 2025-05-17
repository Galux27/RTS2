using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDManager
{
    static ulong BaseUID=0;
    public static UID GetUIDForObject()
    {
        return new UID(BaseUID++);
    }

    static Dictionary<System.Type, UIDObjectDictionary> IDDictionaries = new Dictionary<System.Type, UIDObjectDictionary>();


    public static object GetObjectByUID(System.Type type,ulong uid)
    {
        if (IDDictionaries.ContainsKey(type))
        {
           return IDDictionaries[type].GetObjectFromUID(uid);
        }
        return null;
    }

    public static void OnUIDCreated(object obj,UID uID)
    {
        if (BaseUID < uID.Value)
        {
            BaseUID=uID.Value;
        }
        if (!IDDictionaries.ContainsKey(obj.GetType()))
        {
            IDDictionaries.Add(obj.GetType(), new UIDObjectDictionary(obj.GetType()));
        }
        IDDictionaries[obj.GetType()].AddObject(uID, obj);
    }
}

public class UIDObjectDictionary
{
    public UIDObjectDictionary(System.Type type)
    {
        typeIStore = type;
        Objects = new Dictionary<ulong, object>();
    }
    public System.Type typeIStore;

    public void AddObject(UID id,object obj)
    {
        if (!Objects.ContainsKey(id.Value))
        {
            Objects.Add(id.Value, obj);
        }
        else
        {
            Debug.LogError("Error, trying toadd existing ID"+id.Value+" for object type "+typeIStore.ToString());
        }
    }

        public object GetObjectFromUID(ulong uid)
    {
        if (Objects.ContainsKey(uid))
        {
            return Objects[uid];
        }
        return null;
    }

    Dictionary<ulong, object> Objects;
}


public struct UID
{
    public UID(ulong value)
    {
        this.Value = value;
    }
    public ulong Value;
}
