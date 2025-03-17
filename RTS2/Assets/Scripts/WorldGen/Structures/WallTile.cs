using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "Walls", menuName = "ScriptableObjects/Wall", order = 1)]
public class WallTile :ScriptableObject
{
    //normal tiles
    public Tile NoNeighbours, Left, Right, Above, Below;

    //Corners
    public Tile LeftBelow, LeftAbove;
    public Tile RightBelow, RightAbove;
    public Tile LeftRight, UpDown;

    //T Junction
    public Tile LeftRightBelow, LeftRightAbove;
    public Tile TopBottomLeft, TopBottomRight;

    //cross
    public Tile Cross;


    public string WallName;
    public float Health;
   
}




