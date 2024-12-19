using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSenses : MonoBehaviour
{
    [Range(0f,100f)]
    public float Sight, Hearing,Intelligence,Dexterity;

    public float GetSightRange()
    {
        return Sight;
    }

    public float GetHearingRange()
    {
        return Hearing;
    }
}
