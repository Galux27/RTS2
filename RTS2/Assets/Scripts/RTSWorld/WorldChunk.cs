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
        Debug.Log("On Hover Constructable added instance " + environmentObject.PosX + "," + environmentObject.PosY);
        EnvironmentObjectsInChunk.Add(environmentObject);
        if (ShouldDrawEnvironmentObjects() && environmentObject.Drawn == false)
        {
            environmentObject.RenderInstance();
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
    }

    public void CleanupEnvironmentObjects()
    {
        for (int x = 0; x < EnvironmentObjectsInChunk.Count; x++)
        {
            EnvironmentObjectsInChunk[x].CleanupInstance();
        }
    }
}
