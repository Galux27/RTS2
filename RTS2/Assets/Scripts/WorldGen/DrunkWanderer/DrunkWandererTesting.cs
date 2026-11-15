using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class DrunkWandererTesting : MonoBehaviour
{
    public RawImage image;
    public int Points = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<Vector2Int> targets = new List<Vector2Int>();
            for (int x = 0; x < Points; x++)
            {
                targets.Add(new Vector2Int(Random.Range(0, DrunkWanderer.Width() - 1), Random.Range(0, DrunkWanderer.Height() - 1)));
            }
            DrunkWandererResult result = new DrunkWandererResult(DrunkWanderer.Width(), DrunkWanderer.Height(), targets);
            image.texture = GenerateTexture(result, targets);
        }
    }

    Texture2D GenerateTexture(DrunkWandererResult toDraw,List<Vector2Int> originalTargets)
    {
        Texture2D texture = new Texture2D(DrunkWanderer.Width(), DrunkWanderer.Height());
        texture.filterMode = FilterMode.Point;
        for(int x = 0; x < toDraw.Data.GetLength(0); x++)
        {
            for (int y = 0; y < toDraw.Data.GetLength(1); y++)
            {
                if (toDraw.Data[x, y])
                {
                    texture.SetPixel(x, y, Color.blue);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }

        for(int x = 0; x < originalTargets.Count; x++)
        {
            texture.SetPixel(originalTargets[x].x, originalTargets[x].y, Color.red);

        }
        texture.Apply();
        return texture;
    }
}
