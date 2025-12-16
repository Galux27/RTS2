using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ALifeDebugRenderer : MonoBehaviour
{
    public RawImage disp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Texture2D tex = new Texture2D(OverworldGenerator.Instance.OverworldWidth,OverworldGenerator.Instance.OverworldHeight);
        for(int x = 0; x < OverworldGenerator.Instance.OverworldWidth; x++)
        {
            for(int y = 0;y<OverworldGenerator.Instance.OverworldHeight;y++)
            {
                if (OverworldGenerator.Instance.OverworldTiles[x, y].UnitsInTile.ContainsKey(FactionController.ZOMBIE_FACTION))
                {
                    tex.SetPixel(x, y, Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, 50, OverworldGenerator.Instance.OverworldTiles[x, y].UnitsInTile[FactionController.ZOMBIE_FACTION].FactionEntities.Count)));

                }
                else
                {
                    tex.SetPixel(x, y, Color.cyan);
                }

            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        disp.texture = tex;
    }
}
