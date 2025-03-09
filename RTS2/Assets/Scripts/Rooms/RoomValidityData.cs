using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

            if (ValidityObjects[x].ValidityType == ValidityType.NeedsObject)
            {
                if (quantity == 0)
                {
                    Debug.Log("Invalid: room invalid due to not having object " + ValidityObjects[x].ObjectKey+"|"+quantity+"|"+contains+"|"+r.ObjectsInRoom.Count);
                    return false;
                }

            }
            else if (ValidityObjects[x].ValidityType == ValidityType.NeedObjectAndNeedsQuantity && quantity < ValidityObjects[x].Quantity)
            {
                Debug.Log("Invalid: room invalid due to not having enough of object " + ValidityObjects[x].ObjectKey + "|"+quantity+"/"+ ValidityObjects[x].Quantity);

                return false;

            }
            else if (ValidityObjects[x].ValidityType == ValidityType.NeedOneOfMany)
            {
                bool hasOne = false;
                int amount = 0;
                for (int q = 0; q < ValidityObjects[x].OptionalKeys.Count; q++)
                {
                    hasOne = RoomUtils.DoesRoomContainObject(r, ValidityObjects[x].OptionalKeys[q], out quantity);
                    if (hasOne)
                    {
                        break;
                    }
                }
                if (hasOne == false)
                {
                    Debug.Log("Invalid: room invalid due to it not having any of one");

                    return false;
                }
            }
        }

        return true;
    }

    public string GetIssuesWithRoom(Room r)
    {
        List<string> issues = new List<string>();
        int quantity = 0;
        bool contains = false;
        for (int x = 0; x < ValidityObjects.Count; x++)
        {
            contains = RoomUtils.DoesRoomContainObject(r, ValidityObjects[x].ObjectKey, out quantity);
            if (ValidityObjects[x].ValidityType==ValidityType.NeedsObject)
            {
                if (quantity == 0)
                {
                    issues.Add("Need " + (ValidityObjects[x].Quantity) + " " + ValidityObjects[x].ObjectKey);
                }
             
            }else if (ValidityObjects[x].ValidityType == ValidityType.NeedObjectAndNeedsQuantity && quantity < ValidityObjects[x].Quantity)
            {
                issues.Add("Need " + (ValidityObjects[x].Quantity - quantity) + " more " + ValidityObjects[x].ObjectKey);

            }
            else if (ValidityObjects[x].ValidityType == ValidityType.NeedOneOfMany)
            {
                bool hasOne = false;
                int amount = 0;
                for(int q = 0; q < ValidityObjects[x].OptionalKeys.Count; q++)
                {
                    hasOne = RoomUtils.DoesRoomContainObject(r, ValidityObjects[x].OptionalKeys[q], out quantity);
                    if (hasOne)
                    {
                        break;
                    }
                }
                if (hasOne == false)
                {
                    string validItems = "";
                    for (int q = 0; q < ValidityObjects[x].OptionalKeys.Count; q++)
                    {
                        validItems += ValidityObjects[x].OptionalKeys[q] + ", ";
                    }
                        issues.Add("Needs at least one of " + validItems+".");
                }
            }
            else
            {
                if (quantity > 0)
                {
                    issues.Add("Need to remove all"+ ValidityObjects[x].ObjectKey+"("+quantity+")");

                }
            }
        }
        string retVal = "";
        for(int x=0;x< issues.Count; x++)
        {
            retVal += issues[x]+", ";
        }
        return retVal;
    }
}

[System.Serializable]
public class RoomValidityObject
{
    public string ObjectKey;
    public List<string> OptionalKeys;
    public ValidityType ValidityType;
    public int Quantity;
}

public enum ValidityType
{
    None,
    NeedsObject,
    NeedObjectAndNeedsQuantity,
    NeedOneOfMany,
    NeedAllOfMany
}
