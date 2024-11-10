using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSenses : MonoBehaviour
{
    public float Sight, Hearing;

    public float GetSightRange()
    {
        return Sight;
    }

    public float GetHearingRange()
    {
        return Hearing;
    }
}
