using System.Collections;
using System.Collections.Generic;
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
        DebugColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f),Random.Range(0f,1f),1f);
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

    public Constructable GetConstructableAtPosition(int x,int y)
    {
        Constructable retVal = null;
        Vector3 pos = new Vector3(x+.5f, y+.5f);
        Bounds b = new Bounds();
        for(int x1 = 0; x1 < ToBuild.Count; x1++)
        {
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
