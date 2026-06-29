using System.Collections.Generic;
using UnityEngine;

public class RoadTypeManager : MonoBehaviour
{
    static RoadTypeManager instance;
    public static RoadTypeManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<RoadTypeManager>();
            }
            return instance;
        }
    }

    public Dictionary<string, RoadDetails> AllRoadDetailss;

    private void Awake()
    {
        LoadRoadTypes();
    }
    const string RoadLocation = "RoadData";

    void LoadRoadTypes()
    {
        AllRoadDetailss = new Dictionary<string, RoadDetails>();
        Object[] RoadDetailss = Resources.LoadAll(RoadLocation);
        for (int x = 0; x < RoadDetailss.Length; x++)
        {
            RoadDetails i = (RoadDetails)RoadDetailss[x];
            if (AllRoadDetailss.ContainsKey(i.RoadType.ToString()) == false)
            {
                AllRoadDetailss.Add(i.RoadType.ToString(), i);
            }
        }
        Debug.Log("RoadDetailss: loaded " + AllRoadDetailss.Count);
    }

}
