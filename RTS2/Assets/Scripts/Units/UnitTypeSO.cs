using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitTypeSO", menuName = "ScriptableObjects/UnitType", order = 1)]
public class UnitTypeSO : ScriptableObject
{
    public GameObject Prefab;
    public string UnitType;
    public List<string> ObjectsToTrainFrom = new List<string>();
    public float TrainingTime;
}
