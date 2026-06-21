using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class OverworldRenderer : MonoBehaviour
{
    static OverworldRenderer instance;
    public static OverworldRenderer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OverworldRenderer>();
            }
            return instance;
        }
    }



    public List<HeightmapColour> Colours;
    public Color RiverColour,SettlementColour,MajorRoadColour, MinorRoadColour,BackroadColour,MinorFeatureColour;
   
    public void RenderWorld()
    {
        Texture2D texture = new Texture2D(OverworldGenerator.Instance.OverworldWidth,OverworldGenerator.Instance.OverworldHeight);

        Color[,] colours = new Color[OverworldGenerator.Instance.OverworldWidth, OverworldGenerator.Instance.OverworldHeight];

        for (int x = 0; x < colours.GetLength(0); x++)
        {
            for (int y = 0; y < colours.GetLength(1); y++)
            {
              
                    texture.SetPixel(x, y, GetColourFromHeight(OverworldGenerator.Instance.OverworldTiles[x, y].Elevation));
                
            }
        }


        for (int x = 0; x < colours.GetLength(0); x++)
        {
            for (int y = 0; y < colours.GetLength(1); y++)
            {
                if (HasColourByFeature(x, y))
                {
                    if(HasNoneHeightBasedFeautre(x, y))
                    {
                        texture.SetPixel(x, y, GetColourByFeature(x, y));

                    }
                  
                } 
            }
        }
        Vector2Int pos = Vector2Int.zero;
        if (OverworldGenerator.Instance.Settlements != null)
        {
            for (int q = 0; q < OverworldGenerator.Instance.Settlements.Length; q++)
            {
                for (int r = 0; r < OverworldGenerator.Instance.Settlements[q].pointsInSettlement.Count; r++)
                {
                    pos = OverworldGenerator.Instance.Settlements[q].pointsInSettlement[r];
                    texture.SetPixel(pos.x, pos.y, OverworldGenerator.Instance.Settlements[q].DebugColour);
                }
            }
        }
        texture.Apply();
        MapScreen_UIElement.Instance.SetMapImage(texture);
    }

    bool HasNoneHeightBasedFeautre(int x,int y) {
       
        int count = OverworldGenerator.Instance.OverworldTiles[x, y].Features.Count;
        if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.LargeWaterBody))
        {
            count--;
        }
        if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.Mountain))
        {
            count--;
        }
        return count>0;
    }
    

    bool HasHeightBasedFeature(int x,int y)
    {
        return OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.LargeWaterBody) || OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.Mountain);
    }

    bool HasColourByFeature(int x,int y)
    {
       return OverworldGenerator.Instance.OverworldTiles[x, y].Features.Count > 0;
    }

    public Color GetColourByFeature(int x,int y)
    {
        if(OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.River))
        {
            return RiverColour;
        }else if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.Settlement))
        {
            return SettlementColour;
        }
        else if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.MiscFeature))
        {
            return MinorFeatureColour;
        }
        else if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.MajorRoad))
        {
            return MajorRoadColour;
        }else if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.MinorRoad))
        {
            return MinorRoadColour;
        }else if (OverworldGenerator.Instance.OverworldTiles[x, y].Features.Contains(OverworldFeature.Backroad))
        {
            return BackroadColour;
        }
        return Color.cyan;
    }

    public Color GetColourFromHeight(float height)
    {
        for (int x = 0; x < Colours.Count; x++)
        {
            if (x == 0)
            {
                if (height < Colours[x].MaxHeight)
                {
                    return Colours[x].Color;
                }
            }
            else
            {
                if (height >= Colours[x - 1].MaxHeight && height <= Colours[x].MaxHeight)
                {
                    return Color.Lerp(Colours[x - 1].Color, Colours[x].Color, Mathf.InverseLerp(Colours[x - 1].MaxHeight, Colours[x].MaxHeight, height));
                }
            }
        }
        return Color.black;
    }

}
[System.Serializable]
public struct HeightmapColour
{
    public Color Color;
    public float MaxHeight;
}
