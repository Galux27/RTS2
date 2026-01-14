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
    private void Awake()
    {
        if(AllVisuals==null)
        {
            Init();
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
            UnitVisual i = items[x] as UnitVisual;
            if (i == null)
            {
                continue;
            }
            if (!AllVisuals.ContainsKey(i.type))
            {
                AllVisuals.Add(i.type, new Dictionary<string, UnitVisual>());
            }
            if (AllVisuals[i.type].ContainsKey(i.name))
            {
                continue;
            }
            AllVisuals[i.type].Add(i.ID, i);
        }
        Debug.Log("Total visuals found "+ AllVisuals.Count);
    }

    public UnitVisual GetUnitVisual( string key, VisualType type) {
        if (AllVisuals[type].ContainsKey(key))
        {
            return AllVisuals[type][key];
        }
        return null;
    }

}
[System.Serializable]
public class ColourPalleteElement
{
    public ColourType ColourType;
    public Color Colour;
}
[System.Serializable]
public class ColourPallete { 
    public List<ColourPalleteElement> Elements;
    public static string GetMaterialKeyword(ColourType type)
    {
        switch (type)
        {
            case ColourType.None:
                break;
            case ColourType.SkinLight:
                return "_LightSkinColour";
                break;
            case ColourType.SkinDark:
                return "_DarkSkinColour";
                break;
            case ColourType.Eye:
                return "_EyeColour";
                break;
            case ColourType.ClothesLight:
                return "_ClothesColourLight";
                break;
            case ColourType.ClothesDark:
                return "_ClothesColourDark";
                break;
            case ColourType.Hair:
                return "_HairColour";
                break;
            default:
                break;
        }
        return string.Empty;
    }
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
    Hair,
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
