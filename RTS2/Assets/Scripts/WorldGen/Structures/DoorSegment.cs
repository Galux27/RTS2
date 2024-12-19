using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorSegment : WallSegment
{
    public DoorSegment(int x,int y,Tilemap toPlaceOn) : base(x, y)
    {
        WallType = WallType.Door;

        myAnim = WallHelpers.GetDoorVisual(this, WorldController.Instance.WallManager);
        DoorAnimator=new TilemapAnimator(myAnim,toPlaceOn,new Vector3Int(x,y,0));
    }
    TilemapAnimation myAnim;
    public TilemapAnimator DoorAnimator;

    public void OpenDoor()
    {
        DoorAnimator.Reverse = false;
        DoorAnimator.StartAnimation();
    }

    public void CloseDoor()
    {
        DoorAnimator.Reverse = true;
        DoorAnimator.StartAnimation();
    }

    public void OnAnimDone()
    {
        if (DoorAnimator.Reverse)
        {

        }
        else
        {

        }
    }

    public override void DestroyWall()
    {
        base.DestroyWall();
        Pathfinding.RemovePathModifier(x, y, "Door");
    }
}
