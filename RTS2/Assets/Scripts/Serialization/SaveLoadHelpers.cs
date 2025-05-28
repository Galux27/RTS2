using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public static class SaveLoadHelpers
{
   public static List<FoundSave> GetAllSaves()
    {
        List<FoundSave> retVal = new List<FoundSave>();

        string path = SerializationHelpers.GetSaveDir();
        Debug.Log("Saves path was" + path);
        string[] directoriesInFolder = Directory.GetDirectories(path);
        for(int x = 0; x < directoriesInFolder.Length; x++)
        {
            retVal.Add(new FoundSave(directoriesInFolder[x]));
        }
        return retVal;
   
    
    }
    public static void DeleteSave(string name)
    {
        string path = SerializationHelpers.GetSaveDir();
        Directory.Delete(Path.Combine(path, name),true);
    }


  public  static bool FileWithNameAlreadyExists(string name)
    {
        string path = SerializationHelpers.GetSaveDir();
        return Directory.Exists(Path.Combine(path, name));
    }

    public static bool IsSaveNameValid(string name)
    {
        
        if (name == string.Empty)
        {
            return false;
        }

        if (name.Contains(SerializeDataHelpers.KEY_OBJECT_SPLIT))
        {
            return false;
        }
        else if (name.Contains("."))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.DATA_SPLIT))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.DATA_ELEMENT_SPLIT))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.LIST_ELEMENT_SPLIT))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.DATA_OBJECT_SPLIT))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.BEHAVIOUR_MARKER))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.INVENTORY_ELEMENT_SPLIT))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.INVENTORY_MARKER))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.INVENTORY_MARKER_TWO))
        {
            return false;
        }
        else if (name.Contains(SerializeDataHelpers.INVENTORY_SPLIT_TWO))
        {
            return false;
        }
        return true;
    }

    public static List<string> ReasonsForInvalid(string name)
    {
        List<string> result = new List<string>();
        List<string> invalidChars = new List<string>();
        if (name.Contains(SerializeDataHelpers.KEY_OBJECT_SPLIT))
        {
            invalidChars.Add(SerializeDataHelpers.KEY_OBJECT_SPLIT.ToString());
        }
        if (name.Contains(SerializeDataHelpers.DATA_SPLIT))
        {
            invalidChars.Add(SerializeDataHelpers.DATA_SPLIT.ToString());

        }
        if (name.Contains("."))
        {
            invalidChars.Add(".");
        }
        if (name.Contains(SerializeDataHelpers.DATA_ELEMENT_SPLIT))
        {
            invalidChars.Add(SerializeDataHelpers.DATA_ELEMENT_SPLIT.ToString());

        }
        if (name.Contains(SerializeDataHelpers.LIST_ELEMENT_SPLIT))
        {
            invalidChars.Add(SerializeDataHelpers.LIST_ELEMENT_SPLIT.ToString());

        }
        if (name.Contains(SerializeDataHelpers.DATA_OBJECT_SPLIT))
        {
            invalidChars.Add(SerializeDataHelpers.DATA_OBJECT_SPLIT.ToString());

        }
        if (name.Contains(SerializeDataHelpers.BEHAVIOUR_MARKER))
        {
            invalidChars.Add(SerializeDataHelpers.BEHAVIOUR_MARKER.ToString());

        }
        if (name.Contains(SerializeDataHelpers.INVENTORY_ELEMENT_SPLIT))
        {
            invalidChars.Add(SerializeDataHelpers.INVENTORY_ELEMENT_SPLIT.ToString());

        }
        if (name.Contains(SerializeDataHelpers.INVENTORY_MARKER))
        {
            invalidChars.Add(SerializeDataHelpers.INVENTORY_MARKER.ToString());


        }
        if (name.Contains(SerializeDataHelpers.INVENTORY_MARKER_TWO))
        {
            invalidChars.Add(SerializeDataHelpers.INVENTORY_MARKER_TWO.ToString());


        }
        if (name.Contains(SerializeDataHelpers.INVENTORY_SPLIT_TWO))
        {
            invalidChars.Add(SerializeDataHelpers.INVENTORY_SPLIT_TWO.ToString());

        }
        if (FileWithNameAlreadyExists(name))
        {
            result.Add("Save with name exists");
        }
        if (invalidChars.Count > 0)
        {
            string combined = "Can't contain ";
            for(int x=0;x<invalidChars.Count;x++)
            {
                combined += invalidChars[x].ToString()+" ,";
            }
            result.Add(combined);
        }

        if (name == string.Empty)
        {
            result.Add("Name can't be blank");
        }
        return result;
    }

    public static bool DoWeLoadWorld = false;
    public static string SaveToLoad="";
    public static void LoadGame(string name)
    {
        SaveToLoad = name;
        DoWeLoadWorld = true;
        SceneManager.LoadScene("RTSWorld");
    }

}

public class FoundSave
{
    public string path;
    public DateTime CreatedAt;

    public FoundSave(string path)
    {
        this.path = path;
        CreatedAt=Directory.GetCreationTime(path);
    }
}
