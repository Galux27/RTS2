using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVisualManager : MonoBehaviour
{
    static UnitVisualManager instance;
    public static UnitVisualManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnitVisualManager>(true);
                instance.Init();
            }
            return instance;
        }
    }

    public Dictionary<VisualType, Dictionary<string,UnitVisual>> AllVisuals;
    const string FilePath = "UnitVisuals";
    public SkinColourData SkinColourData;
    void Init()
    {
        AllVisuals = new Dictionary<VisualType, Dictionary<string, UnitVisual>>();

        Object[] items = Resources.LoadAll(FilePath);
        for (int x = 0; x < items.Length; x++)
        {
            UnitVisual i = (UnitVisual)items[x];
            if (!AllVisuals.ContainsKey(i.type))
            {
                AllVisuals.Add(i.type, new Dictionary<string, UnitVisual>());
            }
            AllVisuals[i.type].Add(i.ID, i);
        }
    }
}
[System.Serializable]
public class ColourPallete
{
    public ColourType ColourType;
    public Color Colour;
}
[System.Serializable]
public class ColourPalleteCollection 
{ 
    public List<ColourPallete> ColourPalletes;
}


public enum ColourType
{
    None,
    SkinLight,
    SkinDark,
    Eye,
    ClothesLight,
    ClothesDark,
    Hair
}

public enum VisualType
{
    None,
    Head,
    Face,
    Hair,
    Torso,
    Legs
}
