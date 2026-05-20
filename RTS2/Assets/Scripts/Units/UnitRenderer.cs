using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRenderer : MonoBehaviour
{
    public SpriteRenderer Head, Torso, Legs,Hair,Face;
    public bool DrawHead=true, DrawTorso=true,DrawLegs=true,DrawHair=true,DrawFace=true;    
    UnitVisualStore UnitImRendering;
    Vector3 Scale = Vector3.one;

    public void AlterScale(Vector3 newScale)
    {
        if (newScale == Scale)
        {
            return;
        }
        float transformationx = newScale.x / Scale.x;
        float transformationy = newScale.y / Scale.y;
        float transformationz = newScale.z / Scale.z;

      

        Head.transform.localPosition = new Vector3(Head.transform.localPosition.x * transformationx, Head.transform.localPosition.y * transformationy, Head.transform.localPosition.z * transformationz);
        Torso.transform.localPosition = new Vector3(Torso.transform.localPosition.x * transformationx, Torso.transform.localPosition.y * transformationy, Torso.transform.localPosition.z * transformationz);
        Legs.transform.localPosition = new Vector3(Legs.transform.localPosition.x * transformationx, Legs.transform.localPosition.y * transformationy, Legs.transform.localPosition.z * transformationz);
        Hair.transform.localPosition = new Vector3(Hair.transform.localPosition.x * transformationx, Hair.transform.localPosition.y * transformationy, Hair.transform.localPosition.z * transformationz);
        Face.transform.localPosition = new Vector3(Face.transform.localPosition.x * transformationx, Face.transform.localPosition.y * transformationy, Face.transform.localPosition.z * transformationz);
        Scale = newScale;
    }



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
        DrawFace = visual.DrawFace;
        DrawHead = visual.DrawHead;
        DrawTorso = visual.DrawTorso;
        DrawLegs = visual.DrawLegs;
        DrawHair = visual.DrawHair;
        AlterScale(visual.Scale);
        Head.sprite = visual.Head.GetDirectionalSprite(visual.Direction);
        Face.sprite = visual.Face.GetDirectionalSprite(visual.Direction);
        Hair.sprite = visual.Hair.GetDirectionalSprite(visual.Direction);
        Torso.sprite = visual.Torso.GetDirectionalSprite(visual.Direction);
        Legs.sprite = visual.Legs.GetDirectionalSprite(visual.Direction);
        if (visual.Direction == UnitVisualDirection.Right)
        {
            SetRenderersFlipped(true);
        }
        else
        {
            SetRenderersFlipped(false);
        }
        UpdatePalletes(visual);
    }

    void SetRenderersFlipped(bool val)
    {
        Head.flipX = val;
        Face.flipX = val;
        Hair.flipX = val;
        Torso.flipX = val;
        Legs.flipX = val;
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
        this.gameObject.SetActive(true);
        Head.gameObject.SetActive(DrawHead);
        Torso.gameObject.SetActive(DrawTorso);
        Legs.gameObject.SetActive(DrawLegs);
        Hair.gameObject.SetActive(DrawHair);
        Face.gameObject.SetActive(DrawFace);
    }

    public void HideUnit()
    {
        this.gameObject.SetActive(false);

        Head.gameObject.SetActive(false);
        Torso.gameObject.SetActive(false);
        Legs.gameObject.SetActive(false);
        Hair.gameObject.SetActive(false);
        Face.gameObject.SetActive(false);
    }
}
