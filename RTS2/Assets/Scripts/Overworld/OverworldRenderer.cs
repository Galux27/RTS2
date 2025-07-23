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
                if (OverworldGenerator.Instance.OverworldTiles[x,y].Elevation>OverworldGenerator.Instance.SeaLevel)
                {
                    texture.SetPixel(x, y, Color.Lerp(Color.green,Color.white,Mathf.InverseLerp(OverworldGenerator.Instance.SeaLevel,OverworldGenerator.Instance.MaxElevation, OverworldGenerator.Instance.OverworldTiles[x, y].Elevation)));
                }
                else
                {
                    texture.SetPixel(x, y, Color.blue);
                }
            }
        }
        texture.Apply();
        
        DrawIn.texture = texture;

    }

}
