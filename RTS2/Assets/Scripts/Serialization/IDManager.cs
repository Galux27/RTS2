using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDManager
{
    static ulong BaseUID=0;
    public static UID GetUIDForObject()
    {
        return new UID(BaseUID++);
    }
}

public struct UID
{
    public UID(ulong value)
    {
        this.Value = value;
    }
    public ulong Value;
}
