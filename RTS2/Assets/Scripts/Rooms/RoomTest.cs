using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    List<Vector2Int> coords = new List<Vector2Int>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector2Int v = Pathfinding.GetCoordsFromPosition(this.transform.position);
            RoomUtils.PerformFloodCheck(v);
            coords = RoomUtils.RoomFound();
        }

        for(int x=0; x<coords.Count; x++)
        {
            Vector3 v = new Vector3(coords[x].x, coords[x].y, 0);
            Debug.DrawLine(v,v+Vector3.up, Color.yellow);
        }
    }
}
