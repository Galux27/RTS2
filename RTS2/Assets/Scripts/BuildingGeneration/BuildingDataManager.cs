using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BuildingDataManager
{

    static BuildingDataManager instance;
    public static BuildingDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuildingDataManager();
                instance.InitBuildingData();
            }
            return instance;
        }
    }

    public Dictionary<string, BuildingTemplate> BuildingTemplates;
    public Dictionary<string,RoomTemplate> RoomTemplates;
    const string BuildingDataPath = "Buildings/Buildings", RoomDataPath = "Buildings/Rooms";
    public void InitBuildingData()
    {
        BuildingTemplates = new Dictionary<string, BuildingTemplate>();
        RoomTemplates = new Dictionary<string, RoomTemplate>();
        Object[] resources = Resources.LoadAll(BuildingDataPath);
        for (int x = 0; x < resources.Length; x++)
        {
            BuildingTemplate i = (BuildingTemplate)resources[x];
            if (BuildingTemplates.ContainsKey(i.BuildingName) == false)
            {
                BuildingTemplates.Add(i.BuildingName, i);
            }
        }

        resources = Resources.LoadAll(RoomDataPath);
        for (int x = 0; x < resources.Length; x++)
        {
            RoomTemplate i = (RoomTemplate)resources[x];
            if (RoomTemplates.ContainsKey(i.RoomID) == false)
            {
                RoomTemplates.Add(i.RoomID, i);
            }
        }
    }


}
