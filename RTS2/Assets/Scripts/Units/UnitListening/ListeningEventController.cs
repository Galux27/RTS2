using UnityEngine;
using System.Collections.Generic;
public class ListeningEventController : MonoBehaviour,Updater
{
    static ListeningEventController instance;
    public static ListeningEventController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ListeningEventController>();
            }
            return instance;
        }
    }

    public List<ListeningEvent> ListeningEvents = new List<ListeningEvent>(), AltListeningEvents = new List<ListeningEvent>();
    private void Awake()
    {
        ManualUpdater.Instance.AddUpdater(this);
    }

    public ListeningEvent GetEventInRange(float maxRange,Vector3 pos)
    {
        float closest = 9999999f;
        float dist2 = 0f;
        ListeningEvent retVal = null;
        for(int x=0;x< ListeningEvents.Count; x++)
        {
            if (ListeningEvents[x].IsValid())
            {
                dist2 = Vector3.Distance(pos, ListeningEvents[x].Position);
                if (dist2 <= maxRange&&dist2<=closest)
                {
                    closest = dist2;
                    retVal = ListeningEvents[x];
                }
            }
        }
        return retVal;
    }

    void CleanupListeningEvents()
    {
        AltListeningEvents.Clear();
        for(int x=0;x<ListeningEvents.Count;x++)
        {
            if (ListeningEvents[x].IsValid())
            {
                AltListeningEvents.Add(ListeningEvents[x]);
            }
        }
        ListeningEvents = AltListeningEvents;
        AltListeningEvents.Clear();
    }


    public void AddListeningEvent(ListeningEvent listeningEvent)
    {
        ListeningEvents.Add(listeningEvent);
    }

    UpdaterType Updater.GetUpdaterType()
    {
        return UpdaterType.Other;
    }

    void Updater.Init()
    {
    }

    void Updater.OnEveryFrame()
    {
        
    }

    void Updater.LimitedUpdate()
    {
        CleanupListeningEvents();
    }

 

}

public class ListeningEvent
{
    public Vector3 Position;
    public float VolumeMultiplier,TimeOfEvent,EventDuration;

    public ListeningEvent(Vector3 p,float volume,float duration)
    {
        Position = p;
        VolumeMultiplier = volume;
        EventDuration = duration;
        TimeOfEvent = GameTime.Instance.InGameTime;
    }

    public bool IsValid()
    {
        return TimeOfEvent + EventDuration < GameTime.Instance.InGameTime;
    }
}
