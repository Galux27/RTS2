using UnityEngine;
using System.Collections.Generic;
public static class RoomUpdater
{
    public static void UpdateRoom(Room room)
    {
        if (room.roomType == RoomUseType.Hospital)
        {
            HospitalUpdate(room);
        }
    }


    const float HealRate = 5f;
    static void HospitalUpdate(Room room)
    {
        List<Unit> unitsInRoom = room.GetAllUnitsInRoom();
        Debug.Log("Hospital Update: units in room " + unitsInRoom.Count);
        for(int x=0;x<unitsInRoom.Count;x++)
        {
            if (unitsInRoom[x].Health() > 50)
            {
                unitsInRoom[x].AdjustHealth(-50);
            }
            unitsInRoom[x].AdjustHealth(HealRate * DeltaTimeWrapper.GameplayDelta);
        }
    }
}
