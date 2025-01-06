using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sciptable Object for a unit "faction" which will be used in behaviour decisions, 
/// examples are Zombies, Players, Animals
/// </summary>
[CreateAssetMenu(fileName = "Faction", menuName = "ScriptableObjects/Faction", order = 1)]
public class Faction : ScriptableObject
{
    public string FactionID;
    public List<string> FactionEnemies;
}
