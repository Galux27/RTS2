using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Skin Colour Data", menuName = "UnitVisuals/Skin Colour Data", order = 1)]
public class SkinColourData : ScriptableObject
{
    public List<Color> HumanSkinTones, ZombieSkinTones, HumanEyeColours, ZombieEyeColours,HairColours;
}
