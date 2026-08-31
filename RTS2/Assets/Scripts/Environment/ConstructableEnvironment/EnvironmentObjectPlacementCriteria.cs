using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentObjectPlacementCriteria
{
    public List<AccessibilityData> MyAccessiblityData;
}

[System.Serializable]
public struct AccessibilityData
{
    public List<ObjectAccessibility> ObjectAccessibily;
    public DoorAccessibility DoorAccessibilty;
    public List<WallAccessibiltyData> WallAccessibilities;
    public List<OtherObjectAccessiblityData> OtherObjectAccessiblityData;
}
[System.Serializable]
public struct WallAccessibiltyData
{
    public DirectionFromObject Direction;
    public WallAccessibility WallAccessibility;
}
[System.Serializable]
public struct OtherObjectAccessiblityData
{
    public DirectionFromObject Direction;
    public OtherObjectAccessibility ObjectAccessibility;
}

public enum DoorAccessibility
{
    DontCare,
    DontBlockDoors
}
[System.Serializable]
public struct ObjectAccessibility
{
    public DirectionFromObject Direction;
    public AccessableTilesNeeded Accessiblity;
}

public enum WallAccessibility
{
    DontCare,
    MustBeNextToWall,
    CanBeNextToWall,
    NotAgainstWall
}

public enum OtherObjectAccessibility
{
    DontCare,
    SpaceBetweenObjects

}


public enum DirectionFromObject
{
    Up,
    Down,
    Left,
    Right
}

public enum AccessableTilesNeeded
{
    All,
    AtLeastOne,
    DontCare
}
