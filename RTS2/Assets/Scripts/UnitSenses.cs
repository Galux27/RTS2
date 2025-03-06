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

    public void CopyToNewSenses(ref UnitSenses newSenses)
    {
        newSenses.Sight = Sight;
        newSenses.Hearing = Hearing;
        newSenses.Intelligence= Intelligence;
        newSenses.Dexterity = Dexterity;
    }
}
