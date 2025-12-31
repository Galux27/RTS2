using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRenderer : MonoBehaviour
{
    public SpriteRenderer Head, Torso, Legs,Hair,Face;//, LeftHand, RightHand;
    public void OnChangeChunkIn(WorldChunk chunkIn)
    {
        if (chunkIn.IsRendered)
        {
            DrawUnit();
        }
        else
        {
            HideUnit();
        }
    }

    public void SetUnitVisuals(UnitVisual visual)
    {
        //Head.sprite = visual.Head;
      //  Torso.sprite = visual.Torso;
       // Legs.sprite = visual.Legs;
       // LeftHand.sprite = visual.LeftHand;
       // RightHand.sprite = visual.RightHand;
    }


    public void DrawUnit()
    {
        Head.gameObject.SetActive(true);
        Torso.gameObject.SetActive(true);
        Legs.gameObject.SetActive(true);
        Hair.gameObject.SetActive(true);
        Face.gameObject.SetActive(true);
     //   LeftHand.gameObject.SetActive(true);
     //   RightHand.gameObject.SetActive(true);

    }

    public void HideUnit()
    {
        Head.gameObject.SetActive(false);
        Torso.gameObject.SetActive(false);
        Legs.gameObject.SetActive(false);
        Hair.gameObject.SetActive(false);
        Face.gameObject.SetActive(false);
    }
}
