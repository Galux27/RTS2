using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataReaders
{
   public static void ReadData(string data)
    {
        if(data.Length==0||data==string.Empty) return;
        int firstSplit = -1;
        char lookingFor = SerializeDataHelpers.KEY_OBJECT_SPLIT.ToCharArray()[0];
        for (int x = 0; x < data.Length; x++)
        {
            if (data[x] == lookingFor)
            {
                firstSplit = x;
                break;
            }
        }
        string key = data.Substring(0, firstSplit);
        Debug.Log("Loading data "+(firstSplit+"|"+lookingFor+"|") + key);
    }
}
