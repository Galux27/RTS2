using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains data on what needs to be in a room for it to be valid
/// </summary>
[CreateAssetMenu(fileName = "Room Validity Data", menuName = "ScriptableObjects/Room Validity Data", order = 1)]
public class RoomValidityData : ScriptableObject
{
    public RoomUseType TypeToCheckFor;
    public List<RoomValidityObject> ValidityObjects;

    public bool IsValid(Room r)
    {
        int quantity = 0;
        bool contains = false;
        for(int x = 0; x < ValidityObjects.Count; x++)
        {
           contains= RoomUtils.DoesRoomContainObject(r, ValidityObjects[x].ObjectKey, out quantity);
            if (ValidityObjects[x].NeedsObject)
            {
                if (quantity == 0)
                {
                    return false;
                }
                if (ValidityObjects[x].NeedsQuantity && quantity < ValidityObjects[x].Quantity)
                {
                    return false;
                }
            }
            else
            {
                if (quantity > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

[System.Serializable]
public class RoomValidityObject
{
    public string ObjectKey;
    public bool NeedsObject, NeedsQuantity;
    public int Quantity;
}
