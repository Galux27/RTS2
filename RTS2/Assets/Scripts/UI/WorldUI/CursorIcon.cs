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
    }

    public void SetMoveIcon()
    {
        Icon.sprite = Move;
    }

    public void SetAttackIcon()
    {
        Icon.sprite= Attack;
    }

    public void SetBuildIcon()
    {
        Icon.sprite= Build;
    }

    public void SetDeconstructIcon()
    {
        Icon.sprite = Deconstruct;
    }

    public void SetMultipleActionIcon()
    {
        Icon.sprite = Multiple;
    }

    public void SetCollectIcon()
    {
        Icon.sprite = Collect;
    }

    public void SetHarvestIcon()
    {
        Icon.sprite = Harvest;
    }
    public void SetWallPlaceIcon()
    {
        Icon.sprite= WallPlace;
    }

    public void SetCustomIcon(Sprite icon)
    {
        Icon.sprite= icon;
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
