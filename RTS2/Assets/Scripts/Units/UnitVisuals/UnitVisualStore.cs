using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitVisualStore : MonoBehaviour
{
    public string InitHead, InitHair, InitFace, InitTorso, InitLegs;
    public UnitVisual Head, Hair, Face, Torso, Legs;
    public int HeadPallete,HairPallete,TorsoPallete,LegsPallete;
    public bool IsZombie = false,IsDrawn=false;
  

    private void Awake()
    {
        InitVisuals();   
    }

    void InitVisuals()
    {
        Head = UnitVisualManager.Instance.GetUnitVisual(InitHead, VisualType.Head);
        HeadPallete = Random.Range(0, GetSkinTones().Count);
        Hair = UnitVisualManager.Instance.GetUnitVisual(InitHair, VisualType.Hair);
        HairPallete = Random.Range(0, GetHairTones().Count);
        Face = UnitVisualManager.Instance.GetUnitVisual(InitFace, VisualType.Face);       
        Torso = UnitVisualManager.Instance.GetUnitVisual(InitTorso, VisualType.Torso);
        TorsoPallete = Random.Range(0, Torso.ColourPalletes.ColourPalletes.Count);
        Legs = UnitVisualManager.Instance.GetUnitVisual(InitLegs, VisualType.Legs);
        LegsPallete = Random.Range(0, Legs.ColourPalletes.ColourPalletes.Count);
    }

    public void SetVisual(UnitVisual visual,int pallete = -1)
    {
        switch (visual.type)
        {
            case VisualType.None:
                break;
            case VisualType.Head:
                SetHead(visual, pallete);
                break;
            case VisualType.Face:
                SetFace(visual);
                break;
            case VisualType.Hair:
                SetHair(visual, pallete);
                break;
            case VisualType.Torso:
                SetTorso(visual,pallete);
                break;
            case VisualType.Legs:
                SetLegs(visual, pallete);
                break;
            default:
                break;
        }
        OnVisualsChanged();
    }

    void OnVisualsChanged()
    {
        if (IsDrawn)
        {

        }
    }
    public void SetHead(UnitVisual head,int pallete=-1)
    {
        Head = head;
        if (pallete > -1)
        {
            HeadPallete = pallete;
        }
        else
        {
            HeadPallete = Random.Range(0, GetSkinTones().Count);
        }
    }

    public void SetFace(UnitVisual face, int pallete = -1)
    {
        Face = face;      
    }

    public void SetHair(UnitVisual hair, int pallete=-1)
    {
        Hair = hair;
        if (pallete > -1)
        {
            HairPallete = pallete;
        }
        else
        {
            HairPallete = Random.Range(0, GetHairTones().Count);
        }
    }

    public void SetTorso(UnitVisual torso, int pallete = -1)
    {
        Torso = torso;
        if (pallete > -1)
        {
            TorsoPallete = pallete;
        }
        else
        {
            TorsoPallete = Random.Range(0, torso.ColourPalletes.ColourPalletes.Count);
        }
    }
    public void SetLegs(UnitVisual legs, int pallete = -1)
    {
        Legs = legs;
        if (pallete > -1)
        {
            LegsPallete = pallete;
        }
        else
        {
            LegsPallete = Random.Range(0, legs.ColourPalletes.ColourPalletes.Count);
        }
    }
   
    public Color GetLightSkinTone()
    {
        return GetSkinTones()[HeadPallete];
    }

    public Color GetDarkSkinTone()
    {
        return GetSkinTones()[HeadPallete]-UnitVisualManager.Instance.SkinColourData.DarkShadeOffset;
    }
    public Color GetLightHairTone()
    {
        return GetHairTones()[HairPallete];
    }

    public Color GetDarkHairTone()
    {
        return GetHairTones()[HairPallete]-UnitVisualManager.Instance.SkinColourData.DarkShadeOffset;
    }
    List<Color> GetSkinTones()
    {
        if (IsZombie == false)
        {
            return UnitVisualManager.Instance.SkinColourData.HumanSkinTones;
        }
        else
        {
            return UnitVisualManager.Instance.SkinColourData.ZombieSkinTones;

        }
    }

    List<Color> GetHairTones()
    {
       return UnitVisualManager.Instance.SkinColourData.HairColours;  
    }
}
