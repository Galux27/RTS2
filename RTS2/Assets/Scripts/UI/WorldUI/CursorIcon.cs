using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorIcon : MonoBehaviour
{
    static CursorIcon instance;
    public static CursorIcon Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<CursorIcon>();
            }
            return instance;
        }
    }

    public Vector2Int CurrentChunkBatch, CurrentChunk, mouseCoords;
    private void Update()
    {
      
        PathfindingNode node = Pathfinding.GetNodeFromPosition(this.transform.position,null,true);
        if (node != null)
        {
            Debug.DrawLine(this.transform.position, node.worldPos, Color.blue);
            for(int x=0;x<node.neighbours.Count;x++)
            {
                Debug.DrawLine(node.worldPos, node.neighbours[x].worldPos, Color.magenta);
            }
        }
    }

    public SpriteRenderer Icon;

    public Sprite Move, Attack, Build,WallPlace,Deconstruct,Harvest,Collect,Multiple,Enter,Select;

   
    public void SetSelectIcon()
    {
        Icon.sprite = Select;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetMoveIcon()
    {
        Icon.sprite = Move;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetAttackIcon()
    {
        Icon.sprite= Attack;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetBuildIcon()
    {
        Icon.sprite= Build;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetDeconstructIcon()
    {
        Icon.sprite = Deconstruct;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetMultipleActionIcon()
    {
        Icon.sprite = Multiple;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetCollectIcon()
    {
        Icon.sprite = Collect;
        Icon.transform.localPosition = Vector3.zero;
    }

    public void SetHarvestIcon()
    {
        Icon.sprite = Harvest;
        Icon.transform.localPosition = Vector3.zero;
    }
    public void SetWallPlaceIcon()
    {
        Icon.sprite= WallPlace;
        Icon.transform.localPosition = Vector3.zero;

    }

    public void SetCustomIcon(Sprite icon,Vector3 offset)
    {
        Icon.sprite= icon;
        Icon.transform.localPosition = offset;
    }


    public void SetPosition(Vector3 pos)
    {
        this.transform.position= pos;
    }

    public void SetVisible(bool visible)
    {
        Icon.enabled = visible;
    }

    public void SetColor(Color c)
    {
        Icon.color = c;
    }
}
