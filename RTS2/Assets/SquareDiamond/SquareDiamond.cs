using System.Drawing;
using UnityEngine;

public static class SquareDiamond
{
    static float[,] Data;
    static int Size;
   public static float[,] GenerateHeightmap(int size,float scale,float roughness,int seed,int Increments=-1)
    {
        Random.InitState(seed);
        Size = size;
        Data = new float[Size, Size];
        Data[0, 0] = Random.Range(-scale, scale);
        Data[0, size - 1] = Random.Range(-scale, scale);
        Data[size - 1, 0] = Random.Range(-scale, scale);
        Data[size - 1, size - 1] = Random.Range(-scale, scale);

        int step = size - 1;
        while (step > 1)
        {
            int halfStep = step / 2;
            for (int x = halfStep; x < size; x += step)
            {
                for (int y = halfStep; y < size; y += step)
                {
                   DiamondStep(x, y, step, scale);
                }
            }
            for (int x = 0; x < size; x += halfStep)
            {
                for (int y = (x + halfStep) % step; y < size; y += step)
                {
                    SquareStep(x, y, step, scale);
                }
            }
            step /= 2;
            scale *= Mathf.Pow(2, -roughness);
        }

        if (Increments > -1)
        {
            float max = -999999f, min = 9999999f;
            for (int x = 0; x < Data.GetLength(0); x++)
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

            for (int x = 0; x < Data.GetLength(0); x++)
            {
                for (int y = 0; y < Data.GetLength(1); y++)
                {
                    
                        float lerp = Mathf.InverseLerp(min, max, Data[x, y]);
                        int thresh = Mathf.RoundToInt(Mathf.Lerp(0, Increments, lerp));

                        Data[x,y]= Mathf.InverseLerp(0, Increments, thresh);

                   

                }
            }
        }
        return Data;
    }

    static void SquareStep(int x, int y, int step, float scale)
    {
        int halfStep = step / 2;
        float average = 0;
        int count = 0;
        if (Valid(x - halfStep, y))
        {
            average += Data[x - halfStep, y];
            count++;
        }
        if (Valid(x + halfStep, y))
        {
            average += Data[x + halfStep, y];
            count++;
        }
        if (Valid(x, y - halfStep))
        {
            average += Data[x, y - halfStep];
            count++;
        }
        if (Valid(x, y + halfStep))
        {
            average += Data[x, y + halfStep];
            count++;
        }
        average /= count;
        Data[x, y] = average + Random.Range(-scale, scale);
    }

    static void DiamondStep(int x, int y, int step, float scale)
    {
        int halfStep = step / 2;
        float average = 0;
        int count = 0;
        if (Valid(x - halfStep, y - halfStep))
        {
            average += Data[x - halfStep, y - halfStep];
            count++;
        }
        if (Valid(x - halfStep, y + halfStep))
        {
            average += Data[x - halfStep, y + halfStep];
            count++;
        }
        if (Valid(x + halfStep, y - halfStep))
        {
            average += Data[x + halfStep, y - halfStep];
            count++;
        }
        if (Valid(x + halfStep, y + halfStep))
        {
            average += Data[x + halfStep, y + halfStep];
            count++;
        }
        average /= count;

        Data[x, y] = average + Random.Range(-scale, scale);
    }

    static bool Valid(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }
}
