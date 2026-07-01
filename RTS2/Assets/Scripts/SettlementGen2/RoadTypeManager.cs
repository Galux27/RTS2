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

    public Dictionary<string, RoadDetails> AllRoadDetails;

    private void Awake()
    {
        LoadRoadTypes();
    }
    const string RoadLocation = "RoadData";

    void LoadRoadTypes()
    {
        AllRoadDetails = new Dictionary<string, RoadDetails>();
        Object[] RoadDetailss = Resources.LoadAll(RoadLocation);
        for (int x = 0; x < RoadDetailss.Length; x++)
        {
            RoadDetails i = (RoadDetails)RoadDetailss[x];
            if (AllRoadDetails.ContainsKey(i.RoadType.ToString()) == false)
            {
                AllRoadDetails.Add(i.RoadType.ToString(), i);
            }
        }
        Debug.Log("RoadDetailss: loaded " + AllRoadDetails.Count);
    }

}
