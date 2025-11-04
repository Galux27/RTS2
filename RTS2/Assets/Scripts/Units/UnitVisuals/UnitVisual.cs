using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Unit Visual", menuName = "UnitVisuals/New Unit Visual", order = 1)]
public class UnitVisual : ScriptableObject
{
    public UnitType TypeFor;
    public Sprite Head,Torso,Legs,LeftHand,RightHand;
}
