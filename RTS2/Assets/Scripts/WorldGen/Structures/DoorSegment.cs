using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSegment : WallSegment
{
    public DoorSegment(int x,int y) : base(x, y)
    {
        WallType = WallType.Door;
    }

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

}
