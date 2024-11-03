using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store data of objects within a given area (units, props, items etc...)
/// </summary>
public class WorldChunk
{
    public List<Unit> UnitsInChunk=new List<Unit>();

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
}
