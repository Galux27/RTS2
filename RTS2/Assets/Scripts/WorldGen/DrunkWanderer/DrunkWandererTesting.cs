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
            List<List<Vector2Int>> Targets = new List<List<Vector2Int>>();
            List<Vector2Int> OriginalPositions = new List<Vector2Int>();
            for (int x = 0; x < Points; x++)
            {
                List<Vector2Int> targets = new List<Vector2Int>();
                Vector2Int startPosition = new Vector2Int(Random.Range(0, DrunkWanderer.Width() - 1), Random.Range(0, DrunkWanderer.Height() - 1));
                Vector2Int endPosition = new Vector2Int(Random.Range(0, DrunkWanderer.Width() - 1), Random.Range(0, DrunkWanderer.Height() - 1));
               
                for (int q = 0; q < Points; q++)
                {
                    Vector2Int toAdd = GetLerpVal(startPosition, endPosition, Mathf.InverseLerp(0, Points, q));
                    toAdd+= GetRandomness(10,toAdd);
                    OriginalPositions.Add(toAdd);
                    targets.Add(toAdd);
                }
                Targets.Add(targets);
            }
            DrunkWandererResult result = new DrunkWandererResult(DrunkWanderer.Width(), DrunkWanderer.Height(), Targets);
            image.texture = GenerateTexture(result, OriginalPositions);
        }
    }

    Vector2Int GetRandomness(int range,Vector2Int addingTo)
    {
        Vector2Int retVal = new Vector2Int(Random.Range(-range,range), Random.Range(-range, range));
        if(addingTo.x+retVal.x< 0||addingTo.x+retVal.x > DrunkWanderer.Width() - 1)
        {
            retVal.x = 0;
        }
        if (addingTo.y + retVal.y < 0 || addingTo.y + retVal.y > DrunkWanderer.Height() - 1)
        {
            retVal.y = 0;
        }
        return retVal;
    }

    Vector2Int GetLerpVal(Vector2Int start,Vector2Int end,float progress)
    {
        Vector2 lerp = Vector2.Lerp(start, end, progress);
        return new Vector2Int(Mathf.RoundToInt(lerp.x), Mathf.RoundToInt(lerp.y));
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
