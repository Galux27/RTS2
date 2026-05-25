using UnityEngine;

public class TileRaycastDebugger : MonoBehaviour
{
    TileRaycast ray,path;
    public GameObject StartMarker, EndMarker;
    public bool Refresh = false, PathCast = false;
    void Update()
    {
        if (Refresh)
        {
            ray = new TileRaycast(StartMarker.transform.position, EndMarker.transform.position);
            path=new TileRaycast(StartMarker.transform.position, EndMarker.transform.position);
            EasyStopwatch.StartStopwatch();
                path.PerformPathCast();
            EasyStopwatch.StopStopwatch();
            Debug.Log("Path Test: path cast took " + EasyStopwatch.GetStopwatchElapsedTime());
            
            //EasyStopwatch.StartStopwatch();
            //Pathfinding.FindPath(StartMarker.transform.position, EndMarker.transform.position);
            //EasyStopwatch.StopStopwatch();
            //Debug.Log("Path Test: pathfinding took " + EasyStopwatch.GetStopwatchElapsedTime()+" checked "+  Pathfinding.debugcount);

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

        if (path != null && path.TilesHit != null)
        {
            for (int x = 0; x < path.TilesHit.Count - 1; x++)
            {
                Vector3 perp = path.TilesHit[x + 1].WorldPos() - path.TilesHit[x].WorldPos();
                perp = Vector2.Perpendicular(perp);
                Debug.DrawLine(path.TilesHit[x].WorldPos()+(Vector3.up*.2f), path.TilesHit[x + 1].WorldPos() + (Vector3.up * .2f), Color.cyan);
                Debug.DrawLine(path.TilesHit[x].WorldPos() + (Vector3.up * .2f), path.TilesHit[x].WorldPos() + (Vector3.up * .2f) + (perp * 2));
            }
        }
    }
}
