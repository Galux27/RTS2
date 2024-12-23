using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDrawrer : MonoBehaviour
{
    static RoomDrawrer instance;

    public static RoomDrawrer Instance {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RoomDrawrer>();
            }
            return instance;
        }
    }

    public const string BaseTileKey="RoomUI";

    List<Vector2Int> coords;
    public void SetCoords(List<Vector2Int> coords)
    {
        this.coords = coords;
    }

    public void RenderRoom()
    {
        CleanupRoom();
        GameObject cur = null;
        for(int x=0; x<coords.Count; x++)
        {
            cur = GameObjectPoolManager.Instance.GetObjectFromPool(BaseTileKey);
            cur.transform.position = new Vector3(coords[x].x, coords[x].y);
            cur.transform.parent = this.transform;
            cur.SetActive(true);
        }
    }

    public void CleanupRoom()
    {
        GameObject g = null;
        for(int x = 0; x < this.transform.childCount; x++)
        {
            g = this.transform.GetChild(x).gameObject;
            GameObjectPoolManager.Instance.ReturnObjectToPool(g, BaseTileKey);
        }
    }
}
