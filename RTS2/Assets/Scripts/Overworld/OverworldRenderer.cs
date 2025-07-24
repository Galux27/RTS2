using System.Collections;
using System.Collections.Generic;
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


    public RawImage DrawIn;
    public Button Generate,Render;
    public List<HeightmapColour> Colours;

    private void Awake()
    {
        Render.onClick.AddListener(RenderWorld);
        Generate.onClick.AddListener(OverworldGenerator.Instance.Generate);
    }
    public void RenderWorld()
    {
        Texture2D texture = new Texture2D(OverworldGenerator.Instance.OverworldWidth,OverworldGenerator.Instance.OverworldHeight);

        Color[,] colours = new Color[OverworldGenerator.Instance.OverworldWidth, OverworldGenerator.Instance.OverworldHeight];

        for(int x = 0; x < colours.GetLength(0); x++)
        {
            for (int y = 0; y < colours.GetLength(1); y++)
            {
                texture.SetPixel(x, y, GetColourFromHeight(OverworldGenerator.Instance.OverworldTiles[x, y].Elevation));
            }
        }
        texture.Apply();
        
        DrawIn.texture = texture;

    }

    public Color GetColourFromHeight(float height)
    {
        for(int x = 0; x < Colours.Count-1; x++)
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
                if (height>=Colours[x-1].MaxHeight && height <= Colours[x].MaxHeight)
                {
                    return Color.Lerp(Colours[x-1].Color, Colours[x].Color,Mathf.InverseLerp(Colours[x - 1].MaxHeight, Colours[x].MaxHeight,height));

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
