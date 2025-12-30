using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionController : MonoBehaviour
{
    static FactionController instance;
    public static FactionController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<FactionController>(true);
            }
            return instance ;
        }
    }

    private void Awake()
    {
        instance = this;
        LoadFactions();
    }

    Dictionary<string, Faction> FactionLookup;
    const string FactionLocation = "Factions";
    public const string USER_FACTION = "User",ZOMBIE_FACTION= "Zombies";
    void LoadFactions()
    {
        FactionLookup = new Dictionary<string, Faction>();
        Object[] factions = Resources.LoadAll(FactionLocation);
        for (int x = 0; x < factions.Length; x++)
        {
            Faction i = (Faction)factions[x];
            if (FactionLookup.ContainsKey(i.FactionID) == false)
            {
                FactionLookup.Add(i.FactionID, i);
            }
        }

    }

    public bool IsHostile(Unit me,Unit target)
    {
        return HostileCheck(me.MyFaction.MyFactionID,target.MyFaction.MyFactionID);
    }

    public bool IsHostile(Unit target,string me)
    {
        return HostileCheck(me, target.MyFaction.MyFactionID);

    }

    public bool IsHostile(string target,string me)
    {
        return HostileCheck(target, me);
    }

    bool HostileCheck(string id1,string id2)
    {
        return FactionLookup[id1].FactionEnemies.Contains(id2);
    }

    public List<string> GetFactionEnemies(string id)
    {
        return FactionLookup[id].FactionEnemies;
    }
}
