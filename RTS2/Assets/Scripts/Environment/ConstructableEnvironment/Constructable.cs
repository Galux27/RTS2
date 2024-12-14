using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Constructable
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
