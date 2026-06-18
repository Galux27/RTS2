using UnityEngine;
using UnityEngine.UI;
public class SquareDiamondTest : MonoBehaviour
{
    public float[,] Data;
    public int Size,Seed;
    public float Scale;
    public float Roughness;
    public RawImage image;
    public float max = -999999f, min = 9999999f;
    public int Threshold=-1;
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Data=SquareDiamond.GenerateHeightmap(Size,Scale, Roughness,Seed,Threshold);
            for(int x = 0; x < Data.GetLength(0); x++)
            {
                for (int y = 0; y < Data.GetLength(1); y++)
                {
                    if (Data[x, y] > max)
                    {
                        max = Data[x, y];
                    }
                    if (Data[x, y] < min)
                    {
                        min = Data[x, y];
                    }
                }
            }
            Texture2D t = new Texture2D(Size,Size);
            
            for(int x=0;x< Data.GetLength(0); x++)
            {
                for (int y = 0; y < Data.GetLength(1); y++)
                {
                
                        t.SetPixel(x, y, Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(min, max, Data[x, y])));

                    

                }
            }
            t.Apply();
            image.texture = t;
            Debug.Log("Bounds " + min + "," + max);
        }

    }

}
