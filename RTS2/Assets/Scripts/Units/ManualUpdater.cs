using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualUpdater : MonoBehaviour
{
    static ManualUpdater instance;
    public static ManualUpdater Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<ManualUpdater>();
            }
            return instance;
        }
    }

    public Dictionary<UpdaterType,List<Updater>> updating=new Dictionary<UpdaterType, List<Updater>>();
    const int MaxUserUpdatePerFrame = 200, MaxAIUpdatePerFrame = 10,MaxOtherUpdatesPerFrame=10;
    public int UserIndex = 0, AIIndex = 0,OtherIndex=0;
    public void AddUpdater(Updater toAdd)
    {
        if (!updating.ContainsKey(toAdd.GetUpdaterType()))
        {
            updating.Add(toAdd.GetUpdaterType(), new List<Updater>());
        }
        updating[toAdd.GetUpdaterType()].Add(toAdd);
    }

    public void RemoveUpdater(Updater toRemove)
    {
        updating[toRemove.GetUpdaterType()].Remove(toRemove);
    }

    private void Update()
    {
        PerformEveryFrameUpdate();
        PerformLimitedUpdate();
    }

    void PerformLimitedUpdate()
    {
        if (updating.ContainsKey(UpdaterType.User))
        {
            LimitedUpdateForType(UpdaterType.User, MaxUserUpdatePerFrame, ref UserIndex);
        }

        if (updating.ContainsKey(UpdaterType.AI))
        {
            UpdateForTypeNearCamera(UpdaterType.AI);
            LimitedUpdateForType(UpdaterType.AI, MaxAIUpdatePerFrame, ref AIIndex);
        }

        if (updating.ContainsKey(UpdaterType.Other))
        {
            LimitedUpdateForType(UpdaterType.Other, MaxOtherUpdatesPerFrame, ref OtherIndex);
        }
    }
    List<Updater> ToUpdateNearCamera = new List<Updater>(); 
    void UpdateForTypeNearCamera(UpdaterType type)
    {
        ToUpdateNearCamera.Clear();
        Vector3 cameraPos = CameraController.Instance.transform.position;
       // int updatesPerformed = 0;
      //  int updateLimit = Mathf.Min(updating[type].Count, max);
        //int startingIndex = 0;// index;

       for(int x = 0; x < updating[type].Count; x++)
        {
            if (Vector3.Distance(updating[type][x].GetPosition(), cameraPos) < 20f)
            {
                ToUpdateNearCamera.Add(updating[type][x]);
            }
        }

        for(int x = 0; x < ToUpdateNearCamera.Count; x++)
        {
            ToUpdateNearCamera[x].LimitedUpdate();
        }
        
    }



    void LimitedUpdateForType(UpdaterType type,int max,ref int index)
    {
        int updatesPerformed = 0;
        int updateLimit = Mathf.Min(updating[type].Count, max);
        int startingIndex = index;

        bool isDone = false;

        while (!isDone)
        {
           

            if (index >= updating[type].Count)
            {
                index = 0;
            }
            isDone = updatesPerformed >= updateLimit || updatesPerformed > 1 && index == startingIndex;
            if (!isDone)
            {
                try
                {
                    if (ToUpdateNearCamera.Contains(updating[type][index]) == false)
                    {
                        updating[type][index].LimitedUpdate();
                    }
                }
                catch(System.Exception e)
                {
                    Debug.LogError("Error updating " + type.ToString() + " error " + e.ToString());
                }
                    index++;

                updatesPerformed++;
            }
            }
        }

    void PerformEveryFrameUpdate()
    {
        foreach(KeyValuePair<UpdaterType,List<Updater>> item in updating)
        {
            for(int x=0;x<item.Value.Count;x++)
            {
                try
                {
                    item.Value[x].OnEveryFrame();
                }catch(System.Exception e)
                {
                   // Debug.LogError("Error updating every frame " + item.Key.ToString() + " error " + e.ToString());

                }
            }
        }
    }
}
