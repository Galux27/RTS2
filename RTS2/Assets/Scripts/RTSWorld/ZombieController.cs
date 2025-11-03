using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    static ZombieController instance;
    public static ZombieController Instance 
    { 
        get {  
            
            if(instance == null)
            {
                instance=FindObjectOfType<ZombieController>(true);
            }
            return instance; 
        }
    }

    public float ZombieSpawnRate = 1f;
    public int ZombieSpawnCount = 5;
    float spawnTimer = 0f;
    public GameObject ZombiePrefab;
    public bool Spawn = false;
    public int GetMaxNumberOfZombies()
    {

        return 100;
    }

    List<Unit> Zombies = new List<Unit>();

    public void AddZombieToMoniter(Unit zombie)
    {
        Zombies.Add(zombie);
    }

    public void RemoveZombieToMoniter(Unit zombie)
    {
        Zombies.Remove(zombie);
    }

    bool ShouldSpawnMoreZombies()
    {
        
        return Zombies.Count < GetMaxNumberOfZombies()&&Spawn;
    }

    private void Update()
    {
        if (ShouldSpawnMoreZombies())
        {
            spawnTimer += DeltaTimeWrapper.GameplayDelta;
            if (spawnTimer >= ZombieSpawnRate)
            {
                CreateZombies(ZombieSpawnCount);
                spawnTimer = 0;
            }
        }
    }

    void CreateZombies(int count)
    {
        for(int x = 0; x < count; x++)
        {
            CreateZombie();
        }
    }




    void CreateZombie()
    {
        int xCoord = 0;
        int yCoord = 0;
        if (Random.Range(0, 100) < 50)
        {
            if (Random.Range(0, 100) < 50)
            {
                xCoord = WorldController.Instance.WorldWidth - 1;
                yCoord = Random.Range(0, WorldController.Instance.WorldHeight - 1);
            }
            else
            {
                xCoord = 0;
                yCoord = Random.Range(0, WorldController.Instance.WorldHeight - 1);
            }
        }
        else
        {
            if (Random.Range(0, 100) < 50)
            {
                yCoord = WorldController.Instance.WorldHeight - 1;
                xCoord=Random.Range(0,WorldController.Instance.WorldWidth - 1);
            }
            else
            {
                yCoord = 0;
                xCoord = Random.Range(0, WorldController.Instance.WorldWidth - 1);
            }
        }
        Vector3 posToSpawnAt = new Vector3(xCoord, yCoord, 0);
        PathfindingNode node = Pathfinding.GetNodeFromPosition(posToSpawnAt);
        if (node == null || node.IsPassable == false)
        {
            return;
        }
        GameObject g = Instantiate(ZombiePrefab,node.worldPos , Quaternion.identity);

    }
}
