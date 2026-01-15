using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRenderer : MonoBehaviour
{
    public SpriteRenderer Head, Torso, Legs,Hair,Face;
    UnitVisualStore UnitImRendering;
   
    public void OnChangeChunkIn(WorldChunk chunkIn)
    {
        if (chunkIn.IsRendered)
        {
            DrawUnit();
        }
        else
        {
            HideUnit();
        }
    }

    public void SetUnitVisuals(UnitVisualStore visual)
    {
        UnitImRendering = visual;
        Head.sprite = visual.Head.GetDirectionalSprite(visual.Direction);
        Face.sprite = visual.Face.GetDirectionalSprite(visual.Direction);
        Hair.sprite = visual.Hair.GetDirectionalSprite(visual.Direction);
        Torso.sprite = visual.Torso.GetDirectionalSprite(visual.Direction);
        Legs.sprite = visual.Legs.GetDirectionalSprite(visual.Direction);
        UpdatePalletes(visual);
    }

    void UpdatePalletes(UnitVisualStore visual)
    {
        Color skinLight = visual.GetLightSkinTone();
        Color skinDark = visual.GetDarkSkinTone();
        Color hairLight = visual.GetLightHairTone();
        SpriteRenderer sr = GetSpriteRendererFromType(VisualType.Head);
        sr.material.SetColor(ColourPallete.GetMaterialKeyword(ColourType.SkinLight), skinLight);
        sr.material.SetColor(ColourPallete.GetMaterialKeyword(ColourType.SkinDark), skinDark);
        sr = GetSpriteRendererFromType(VisualType.Face);
        sr.material.SetColor(ColourPallete.GetMaterialKeyword(ColourType.SkinLight), skinLight);
        sr.material.SetColor(ColourPallete.GetMaterialKeyword(ColourType.SkinDark), skinDark);
        sr = GetSpriteRendererFromType(VisualType.Hair);
        sr.material.SetColor(ColourPallete.GetMaterialKeyword(ColourType.Hair),hairLight);
        SetPallete(visual.Torso, visual.TorsoPallete);
        SetPallete(visual.Legs, visual.LegsPallete);

    }


    void SetPallete(UnitVisual visual,int palleteIndex)
    {
        SpriteRenderer sr = GetSpriteRendererFromType(visual);
        if (sr != null)
        {
            try
            {
                ColourPallete pallete = visual.ColourPalletes.ColourPalletes[palleteIndex];
                for (int x = 0; x < pallete.Elements.Count; x++)
                {
                    sr.material.SetColor(ColourPallete.GetMaterialKeyword(pallete.Elements[x].ColourType), pallete.Elements[x].Colour);
                }
            }
            catch {
                Debug.LogError("Pallete error " + palleteIndex + "/" + visual.ColourPalletes.ColourPalletes.Count+"/"+visual.type);
            }
         
       }
    }
    SpriteRenderer GetSpriteRendererFromType(UnitVisual visual)
    {
        return GetSpriteRendererFromType(visual.type);
    }
    SpriteRenderer GetSpriteRendererFromType(VisualType visual)
    {
        switch (visual)
        {
            case VisualType.None:
                break;
            case VisualType.Head:
                return Head;
                break;
            case VisualType.Face:
                return Face;
                break;
            case VisualType.Hair:
                return Hair;
                break;
            case VisualType.Torso:
                return Torso;
                break;
            case VisualType.Legs:
                return Legs;
                break;
            default:
                break;
        }
        return null;
    }


    public void DrawUnit()
    {
        Head.gameObject.SetActive(true);
        Torso.gameObject.SetActive(true);
        Legs.gameObject.SetActive(true);
        Hair.gameObject.SetActive(true);
        Face.gameObject.SetActive(true);
    }

    public void HideUnit()
    {
        Head.gameObject.SetActive(false);
        Torso.gameObject.SetActive(false);
        Legs.gameObject.SetActive(false);
        Hair.gameObject.SetActive(false);
        Face.gameObject.SetActive(false);
    }
}
