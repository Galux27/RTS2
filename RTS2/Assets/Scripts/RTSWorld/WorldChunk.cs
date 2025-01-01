using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Class to store data of objects within a given area (units, props, items etc...)
/// </summary>
public class WorldChunk
{
    public List<Unit> UnitsInChunk=new List<Unit>();
    public List<EnvironmentObjectInstance> EnvironmentObjectsInChunk = new List<EnvironmentObjectInstance>();
    public List<Constructable> ToBuild=new List<Constructable>();
    public Color DebugColor;

    public WorldChunk()
    {
        DebugColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f,1f),1f);
    }

    public void AddUnitToChunk(Unit unit)
    {
        UnitsInChunk.Add(unit);
    }

    public void RemoveUnitFromChunk(Unit unit)
    {
        UnitsInChunk.Remove(unit);
    }

    public void AddEnvironmentObject(EnvironmentObjectInstance environmentObject)
    {
        EnvironmentObjectsInChunk.Add(environmentObject);
        if (ShouldDrawEnvironmentObjects() && environmentObject.Drawn == false)
        {
            environmentObject.RenderInstance();
        }
    }

    List<EnvironmentObjectInstance> GetAllObjectsAtCoords(Vector2Int coords)
    {
        List<EnvironmentObjectInstance> retVal = new List<EnvironmentObjectInstance>();

        for (int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            if (EnvironmentObjectsInChunk[x].PosX == coords.x && EnvironmentObjectsInChunk[x].PosY == coords.y)
            {
                retVal.Add( EnvironmentObjectsInChunk[x]);
            }
        }
        return retVal;
    }


    EnvironmentObjectInstance GetObjectAtCoords(Vector2Int coords)
    {
        EnvironmentObjectInstance retVal = null;
        int count = 0;
        for(int x=0;x<EnvironmentObjectsInChunk.Count;x++)
        {
            if (EnvironmentObjectsInChunk[x].PosX==coords.x && EnvironmentObjectsInChunk[x].PosY== coords.y)
            {
                count++;
               retVal= EnvironmentObjectsInChunk[x];
            }
        }
        Debug.Log("Room: found " + count + " objects at " + coords);
        return retVal;
    }

    public bool DoesObjectExistAtCoords(Vector2Int coords,string toCheckFor, out EnvironmentObjectInstance objFound)
    {
        Debug.Log("Room: check at " + coords+" for "+  toCheckFor);
        List<EnvironmentObjectInstance> objects = GetAllObjectsAtCoords(coords);
        if (objects.Count == 0)
        {
            objFound = null;
            return false;
        }
        //for(int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        //{
        //    Debug.Log("Room: Obj in chunk " + EnvironmentObjectsInChunk[x].ObjectKey + " x " + EnvironmentObjectsInChunk[x].PosX + " y " + EnvironmentObjectsInChunk[x].PosY);
        //}


        for(int x = 0; x < objects.Count; x++)
        {
            if (objects[x] != null)
            {
                Debug.Log("Room: object found " + objects[x].ObjectKey + " at " + coords.ToString());
            }

            if (objects[x] != null && objects[x].ObjectKey == toCheckFor)
            {
                objFound = objects[x];
                return true;
            }
        }

     
        objFound = null;
        return false;
    }

    public Constructable GetConstructableAtPosition(int x,int y,ConstructableType type)
    {
        Constructable retVal = null;
        Vector3 pos = new Vector3(x+.5f, y+.5f);
        Bounds b = new Bounds();
        for(int x1 = 0; x1 < ToBuild.Count; x1++)
        {
            if (ToBuild[x1].GetType() != type)
            {
                continue;
            }
            b = new Bounds(ToBuild[x1].GetPosition(), ToBuild[x1].Size());
            if (b.Contains(pos))
            {
                return ToBuild[x1];
            }
        }

        return retVal;
    }


    public void AddConstructable(Constructable toBuild)
    {
        ToBuild.Add(toBuild);
        if (ShouldDrawEnvironmentObjects() && !toBuild.IsDrawn())
        {
            toBuild.Render();
        }
    }

    public void RemoveConstructable(Constructable toRemove)
    {
        if (toRemove == null)
        {
            return;
        }
        if(ToBuild.Contains(toRemove))
        {
            toRemove.Cleanup();
            ToBuild.Remove(toRemove);
        }
    }

    public bool ShouldDrawEnvironmentObjects()
    {
        return EnvironmentObjectsInChunk.Count > 0;
    }

    public bool DrawnEnvironmentObjects()
    {
        return  EnvironmentObjectsInChunk[0].Drawn;
    }

    public void RenderEnvironmentObjects()
    {
        for(int x=0;x<EnvironmentObjectsInChunk.Count;x++)
        {
            EnvironmentObjectsInChunk[x].RenderInstance();
        }

        for (int x = 0; x < ToBuild.Count; x++)
        {
            ToBuild[x].Render();
        }
    }

    public void CleanupEnvironmentObjects()
    {
        for (int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            EnvironmentObjectsInChunk[x].CleanupInstance();
        }

        for (int x = 0; x < ToBuild.Count; x++)
        {
            ToBuild[x].Cleanup();
        }
    }
}
