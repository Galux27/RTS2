using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Overworld Generator Type", menuName = "Overworld/Base Generator", order = 1)]
public class OverworldFeatureGenerator : ScriptableObject
{
   public virtual void GenerateFeature(OverworldTile[,] world)
    {

    }
}
