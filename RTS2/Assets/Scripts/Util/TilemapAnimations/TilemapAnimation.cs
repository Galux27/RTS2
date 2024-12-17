using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "TilemapAnimation", menuName = "ScriptableObjects/TilemapAnimation", order = 1)]
public class TilemapAnimation : ScriptableObject
{
    public List<Tile> AnimationFrames;
    public float TimePerFrame = .1f;
    public string AnimationID;
}
