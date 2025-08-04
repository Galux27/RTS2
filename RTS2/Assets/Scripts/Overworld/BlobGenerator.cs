using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlobGenerator
{
    static int width, height;
   public static List<Blob> GenerateBlobs(int numToGenerate,ref OverworldTile[,] world)
    {
        List<Blob> retVal = new List<Blob>();
        int[,] VisitedTiles = new int[world.GetLength(0),world.GetLength(1)];
        for(int x = 0; x < world.GetLength(0); x++)
        {
            for(int y=0; y < world.GetLength(1); y++)
            {
                VisitedTiles[x, y] = -1;
            }
        }
        for(int q = 0; q < numToGenerate; q++)
        {
            retVal.Add(new Blob(q));
        }
        Vector2Int coords = new Vector2Int();
        width = world.GetLength(0);
        height = world.GetLength(1);
        for (int q = 0; q < numToGenerate; q++)
        {
            coords.x = Random.Range(0, width - 1);
            coords.y = Random.Range(0, height - 1);
            retVal[q].ToCheck.Add(coords);
        }
        bool CanExpand = true;
        while(CanExpand)
        {
            List<int> indexes = new List<int>();
            for(int x=0;x<numToGenerate; x++)
            {
                indexes.Add(x);
            }
            bool hasAnything = false;

            while (indexes.Count > 0)
            {
                int i = Random.Range(0, indexes.Count - 1);
                int index = indexes[i];
                if (ExpandBlob(retVal[index], ref VisitedTiles))
                {
                    hasAnything = true;
                }
                indexes.RemoveAt(i);
            }
            if (!hasAnything)
            {
                CanExpand = false;
            }
        }
        return retVal;
    }


    public static List<Blob> GenerateBlobsWithFalloff(int numToGenerate, ref OverworldTile[,] world,float targetSize)
    {
        List<Blob> retVal = new List<Blob>();
        int[,] VisitedTiles = new int[world.GetLength(0), world.GetLength(1)];
        for (int x = 0; x < world.GetLength(0); x++)
        {
            for (int y = 0; y < world.GetLength(1); y++)
            {
                VisitedTiles[x, y] = -1;
            }
        }
        for (int q = 0; q < numToGenerate; q++)
        {
            retVal.Add(new Blob(q));
        }
        Vector2Int coords = new Vector2Int();
        width = world.GetLength(0);
        height = world.GetLength(1);
        for (int q = 0; q < numToGenerate; q++)
        {
            coords.x = Random.Range(0, width - 1);
            coords.y = Random.Range(0, height - 1);
            retVal[q].PointsInBlob.Add(coords);
            retVal[q].ToCheck.Add(coords);
        }
        bool CanExpand = true;
        while (CanExpand)
        {
            List<int> indexes = new List<int>();
            for (int x = 0; x < numToGenerate; x++)
            {
                indexes.Add(x);
            }
            bool hasAnything = false;

            while (indexes.Count > 0)
            {
                int i = Random.Range(0, indexes.Count - 1);
                int index = indexes[i];
                if (ExpandWithFalloffBlob(retVal[index], ref VisitedTiles,targetSize))
                {
                    hasAnything = true;
                }
                indexes.RemoveAt(i);
            }
            if (!hasAnything)
            {
                CanExpand = false;
            }
        }
        return retVal;
    }

    public static List<Blob> GenerateBlobs(int numToGenerate, ref OverworldTile[,] world,int sizeLimit)
    {
        List<Blob> retVal = new List<Blob>();
        int[,] VisitedTiles = new int[world.GetLength(0), world.GetLength(1)];
        for (int x = 0; x < world.GetLength(0); x++)
        {
            for (int y = 0; y < world.GetLength(1); y++)
            {
                VisitedTiles[x, y] = -1;
            }
        }
        for (int q = 0; q < numToGenerate; q++)
        {
            retVal.Add(new Blob(q));
        }
        Vector2Int coords = new Vector2Int();
        width = world.GetLength(0);
        height = world.GetLength(1);
        for (int q = 0; q < numToGenerate; q++)
        {
            coords.x = Random.Range(0, width - 1);
            coords.y = Random.Range(0, height - 1);
            retVal[q].ToCheck.Add(coords);
        }
        bool CanExpand = true;
        while (CanExpand)
        {
            List<int> indexes = new List<int>();
            for (int x = 0; x < numToGenerate; x++)
            {
                indexes.Add(x);
            }
            bool hasAnything = false;

            while (indexes.Count > 0)
            {
                int i = Random.Range(0, indexes.Count - 1);
                int index = indexes[i];
                if (ExpandBlob(retVal[index], ref VisitedTiles,sizeLimit))
                {
                    hasAnything = true;
                }
                indexes.RemoveAt(i);
            }
            if (!hasAnything)
            {
                CanExpand = false;
            }
        }
        return retVal;
    }

    static bool ExpandWithFalloffBlob(Blob blob, ref int[,] visited,float targetSize)
    {

        Vector2Int coords = new Vector2Int();
        float chance = 100f;
        float dist = 0f;
        while (blob.ToCheck.Count > 0)
        {
            int index = Random.Range(0, blob.ToCheck.Count - 1);
            coords = blob.ToCheck[index];

            if (visited[coords.x, coords.y] == -1)
            {
                dist = Vector2Int.Distance(coords, blob.GetCenter());
                chance = Random.Range(0, targetSize)+(targetSize*.05f);

                if (chance > dist&&dist<targetSize*1.5f)
                {
                    blob.AddPointToBlob(coords);
                    blob.ToCheckOnNextPass.AddRange(GetNeighbours(coords));
                    visited[coords.x, coords.y] = blob.id;
                }
            }
            else if (visited[coords.x, coords.y] != blob.id)
            {
                blob.Border.Add(coords);
            }
            blob.ToCheck.RemoveAt(index);
        }

        blob.ToCheck = blob.ToCheckOnNextPass;
        blob.ToCheckOnNextPass = new List<Vector2Int>();
        return blob.HasPointsToCheck();

    }

    static bool ExpandBlob(Blob blob,ref int[,] visited)
    {

        Vector2Int coords = new Vector2Int();

        while (blob.ToCheck.Count > 0)
        {
            int index = Random.Range(0, blob.ToCheck.Count - 1);
            coords = blob.ToCheck[index];

            if (visited[coords.x, coords.y] ==-1)
            {
                blob.AddPointToBlob(coords);
                blob.ToCheckOnNextPass.AddRange(GetNeighbours(coords));
                visited[coords.x, coords.y] = blob.id;
            }
            else if(visited[coords.x, coords.y]!=blob.id)
            {
                blob.Border.Add(coords);
            }
            blob.ToCheck.RemoveAt(index);
        }

        blob.ToCheck = blob.ToCheckOnNextPass;
        blob.ToCheckOnNextPass = new List<Vector2Int>();
        return blob.HasPointsToCheck();
      
    }


    static bool ExpandBlob(Blob blob, ref int[,] visited,int sizeLimit)
    {

        Vector2Int coords = new Vector2Int();

        while (blob.ToCheck.Count > 0)
        {
            int index = Random.Range(0, blob.ToCheck.Count - 1);
            coords = blob.ToCheck[index];

            if (visited[coords.x, coords.y] == -1)
            {
                blob.AddPointToBlob(coords);
                blob.ToCheckOnNextPass.AddRange(GetNeighbours(coords));
                visited[coords.x, coords.y] = blob.id;
            }
            else if (visited[coords.x, coords.y] != blob.id)
            {
                blob.Border.Add(coords);
            }
            blob.ToCheck.RemoveAt(index);
        }

        blob.ToCheck = blob.ToCheckOnNextPass;
        blob.ToCheckOnNextPass = new List<Vector2Int>();
        return blob.HasPointsToCheck()&&blob.PointsInBlob.Count<sizeLimit;

    }
    static List<Vector2Int> GetNeighbours(Vector2Int coords)
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        if (coords.x > 0)
        {
            retVal.Add(coords + Vector2Int.left);
        }
        if (coords.x < width - 1)
        {
            retVal.Add(coords + Vector2Int.right);
        }

        if (coords.y > 0)
        {
            retVal.Add(coords + Vector2Int.down);
        }
        if (coords.y < height - 1)
        {
            retVal.Add(coords + Vector2Int.up);
        }
        return retVal;
    }
}

public class Blob
{
    public List<Vector2Int> PointsInBlob =new List<Vector2Int>(),ToCheckOnNextPass=new List<Vector2Int>(),ToCheck=new List<Vector2Int>(),Border=new List<Vector2Int>();
    public int id = -1;
    public Vector2Int origin;
    bool setOrigin = false;
    public Blob(int id)
    {
        this.id = id;
    }
    
    public void AddPointToBlob(Vector2Int point)
    {
        if (!setOrigin)
        {
            origin = point;
        }
        PointsInBlob.Add(point);
    }

    public void AddPointsToBlob(List< Vector2Int> points)
    {
        PointsInBlob.AddRange(points);
    }

    public bool HasPointsToCheck()
    {
        return ToCheck.Count > 0;
    }
    bool gotCenter = false;
    Vector2Int center;
    public Vector2Int GetCenter()
    {

        if (!gotCenter)
        {
            for(int x=0;x<PointsInBlob.Count;x++)
            {
                center += PointsInBlob[x];
            }
            center/=PointsInBlob.Count;
            gotCenter = true;
        }
        return center;
        
    }
}
