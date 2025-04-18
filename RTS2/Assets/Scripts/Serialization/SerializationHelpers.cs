using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SerializationHelpers
{
   
}

public enum DataType
{
    None,
    Unit,
    Chunk,
    Tile,
    EnvironmentObject,
    Behaviour
}

public class DataToSerialize
{
    public Dictionary<string,object> data;
}

public class SerializedData
{
    public Dictionary<string, string> data;
}
