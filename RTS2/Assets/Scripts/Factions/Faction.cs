using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "ScriptableObjects/Faction", order = 1)]
public class Faction : ScriptableObject
{
    public string FactionID;
    public List<string> FactionEnemies;
}
