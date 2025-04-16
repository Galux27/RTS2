using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnitCapacityManager
{
    

   
    public static void RefreshCapacities()
    {
        RefreshTotalCapacity();
        RefreshEngineerCapacity();
        RefreshSoldierCapacity();
        UnitCapacityUIElement.Instance.RefreshUI();
    }

    public static int TotalCapacity = 0, EngineerCapacity = 0, SoldierCapacity = 0;

    public static int GetRemainingCapacityForType(string type)
    {
        int max = GetMaxCapacityForUnitType(type);
        int userHas = UnitMoniter.Instance.GetUserUnitCount(type);
        return (max - userHas);
    }


    public static int GetMaxCapacityForUnitType(UserUnitTypeCount toUpdate)
    {
        Debug.Log("invalid: getting capacity for " + toUpdate.Type.ToString());
        switch (toUpdate.Type)
        {
            case UnitType.Civilian: 
                return TotalCapacity - (EngineerCapacity + SoldierCapacity);
                break;
            case UnitType.Rifleman:
                return SoldierCapacity;
                break;
            case UnitType.Engineer: 
                return EngineerCapacity;
                break;
        
        }
        return 0;
    }
    public static int GetMaxCapacityForUnitType(string toUpdate)
    {
        Debug.Log("invalid: getting capacity for " + toUpdate);

        switch (toUpdate)
        {
            case "Civilian":
                return TotalCapacity - (EngineerCapacity + SoldierCapacity);
                break;
            case "Rifleman":
                return SoldierCapacity;
                break;
            case "Engineer":
                return EngineerCapacity;
                break;

        }
        return 0;
    }
    static Dictionary<string, int> PopulationCapacityObjects = new Dictionary<string, int> {
        { "Bunk Bed", 2}
    };

    static Dictionary<string, int> EngineerCapacityObjects = new Dictionary<string, int> {
        { "Workbench", 2}
    };
    static Dictionary<string, int> SoldierCapacityObjects = new Dictionary<string, int> {
        { "Weapon Locker", 2}
    };
    public static void RefreshTotalCapacity()
    {
        int count = 0;
       
        for(int x=0;x<RoomManager.Instance.roomList.Count;x++)
        {
            if (RoomManager.Instance.roomList[x].roomType == RoomUseType.Dwelling && RoomUtils.IsValid( RoomManager.Instance.roomList[x]))
            {
                for(int y=0;y< RoomManager.Instance.roomList[x].ObjectsInRoom.Count;y++) 
                {
             
                    if (PopulationCapacityObjects.ContainsKey(RoomManager.Instance.roomList[x].ObjectsInRoom[y].Name()))
                    {
                        count += PopulationCapacityObjects[RoomManager.Instance.roomList[x].ObjectsInRoom[y].Name()];
                    }
                } 
            }
        }
        TotalCapacity = count;
    }
    
    public static void RefreshEngineerCapacity()
    {
        int count = 0;

        for (int x = 0; x < RoomManager.Instance.roomList.Count; x++)
        {
            if (RoomManager.Instance.roomList[x].roomType == RoomUseType.Workshop && RoomUtils.IsValid(RoomManager.Instance.roomList[x]))
            {
                for (int y = 0; y < RoomManager.Instance.roomList[x].ObjectsInRoom.Count; y++)
                {

                    if (EngineerCapacityObjects.ContainsKey(RoomManager.Instance.roomList[x].ObjectsInRoom[y].Name()))
                    {
                        count += EngineerCapacityObjects[RoomManager.Instance.roomList[x].ObjectsInRoom[y].Name()];
                    }
                }
            }
        }
        EngineerCapacity = count;
        Debug.Log("Convert: engineer " + EngineerCapacity);
    }
    public static void RefreshSoldierCapacity()
    {
        int count = 0;

        for (int x = 0; x < RoomManager.Instance.roomList.Count; x++)
        {
            if (RoomManager.Instance.roomList[x].roomType == RoomUseType.Barracks && RoomUtils.IsValid(RoomManager.Instance.roomList[x]))
            {
                for (int y = 0; y < RoomManager.Instance.roomList[x].ObjectsInRoom.Count; y++)
                {

                    if (SoldierCapacityObjects.ContainsKey(RoomManager.Instance.roomList[x].ObjectsInRoom[y].Name()))
                    {
                        count += SoldierCapacityObjects[RoomManager.Instance.roomList[x].ObjectsInRoom[y].Name()];
                    }
                }
            }
        }
        SoldierCapacity = count;
        Debug.Log("Convert: soldier " + SoldierCapacity);

    }


}
