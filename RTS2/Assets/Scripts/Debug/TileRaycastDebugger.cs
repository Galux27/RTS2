using UnityEngine;

public class TileRaycastDebugger : MonoBehaviour
{
    TileRaycast ray;
    public GameObject StartMarker, EndMarker;   
    public bool Refresh = false;
    void Update()
    {
        if (Refresh)
        {
            ray = new TileRaycast(StartMarker.transform.position, EndMarker.transform.position);
            ray.PerformRaycast();
            Refresh = false;
        }
        if (ray != null && ray.TilesHit != null)
        {
            for(int x=0;x<ray.TilesHit.Count-1;x++)
            {
                Debug.DrawLine(ray.TilesHit[x].WorldPos(), ray.TilesHit[x+1].WorldPos(),Color.magenta);
            }
        }
    }
}
