using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interface for any object that will need constructing by units before being placed in the world
/// </summary>
public interface Constructable: Selectable, ISerialize
{
    public void ConstructObject();
    public void OnObjectConstructed();

    public Vector3 GetPosition();
    public float MaxDistToConstruct();
    public void SetBuilt(bool val);

    public bool IsBuilt();

    public Vector3 Size();

    public void Render();

    public void Cleanup();

    public bool IsDrawn();


    public void OnHover();
    public void OnHoverExit();

    public ConstructableType GetType();
}
public enum ConstructableType
{
    None,
    Wall,
    Furniture,
    Door
}
