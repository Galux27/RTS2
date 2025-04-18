using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemFilter 
{
    public bool CheckForForbidden, CheckForAllowed;
    public List<string> ForbiddenItems=new List<string>(), AllowedItems=new List<string>();

    public bool ItemCanPass(string item)
    {
        if(CheckForForbidden)
        {
            if(ForbiddenItems.Contains(item))
            {
                return false;
            }
        }
        if(CheckForAllowed)
        {
            if (AllowedItems.Contains(item)==false)
            {
                return false;
            }
        }

        return true;
    }
}
