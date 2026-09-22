using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    static IconManager instance;
    public static IconManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<IconManager>();
            }
            return instance;
        }
    }
    public IconData Data;
    Dictionary<string, Icon> allIcons = new Dictionary<string, Icon>();
    private void Awake()
    {
        for(int x = 0; x < Data.Icons.Count; x++)
        {
            allIcons.Add(Data.Icons[x].Key, Data.Icons[x]);
        }
    }

    public Sprite GetIcon(string key)
    {
        if (allIcons.ContainsKey(key))
        {
            return allIcons[key].IconImage;
        }
        return null;
    }
}
[System.Serializable]
public class Icon
{
    public string Key;
    public Sprite IconImage;
}
