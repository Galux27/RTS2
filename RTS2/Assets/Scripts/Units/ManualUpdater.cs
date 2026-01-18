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
    const int MaxUserUpdatePerFrame = 200, MaxAIUpdatePerFrame = 50;
    public int UserIndex = 0, AIIndex = 0;
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
            LimitedUpdateForType(UpdaterType.AI, MaxAIUpdatePerFrame, ref AIIndex);
        }
    }

    void LimitedUpdateForType(UpdaterType type,int max,ref int index)
    {
        int updatesPerformed = 0;
        int updateLimit = Mathf.Min(updating[type].Count, max);
        while (updatesPerformed < updateLimit)
        {
            if (index >= updating[type].Count)
            {
                index = 0;
            }
            updating[type][index].LimitedUpdate();
            index++;
            
            updatesPerformed++;
        }
    }

    void PerformEveryFrameUpdate()
    {
        foreach(KeyValuePair<UpdaterType,List<Updater>> item in updating)
        {
            for(int x=0;x<item.Value.Count;x++)
            {
                item.Value[x].OnEveryFrame();
            }
        }
    }
}
