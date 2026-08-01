using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCheats : MonoBehaviour
{
    static DebugCheats instance;
    public static DebugCheats Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<DebugCheats>(true);
            }
            return instance;
        }
    }

    public bool InstantConstructVal = true;
    public bool InstantConstruct()
    {
#if UNITY_EDITOR
        return InstantConstructVal;
#endif
        return false;
    }

    public bool CareAboutResources = false;
    public bool CareAboutResourcesNeeded()
    {
#if UNITY_EDITOR
        return CareAboutResources;
#endif
        return true;
    }

    public bool DrawDebugPathfinding = false;
    public bool DoWeDrawDebugPathfinidng()
    {
        return DrawDebugPathfinding;
    }

    public bool SpawnEnemies = true;
    public bool DoWeSpawnEnemies()
    {
#if UNITY_EDITOR
        return SpawnEnemies;
#endif
        return false;
    }

    public bool LogBehaviourDetails = true;
    public bool DoWeLogBehaviourDetails()
    {
#if UNITY_EDITOR
        return LogBehaviourDetails;
#endif
        return false;
    }


    public bool DrawSelectableBounds = true;
    public bool DoWeDrawSelectableBounds()
    {
#if UNITY_EDITOR
        return DrawSelectableBounds;
#endif
        return false;
    }
    public SettlementTileArea LastArea;
    public GeneratedSettlement LastSettlement;
    public Settlement_Settings LastSettings;
    public bool DrawSettlementAreas = false;
    public bool DrawSettlementBuildingAreas = false;
    public int sectionCount = 0;
    public Vector2 WorldCenter;
    private void Update()
    {
        if (LastSettlement != null &&DrawSettlementBuildingAreas)
        {
            for(int x = 0; x < LastSettlement.areas.GetLength(0); x++)
            {
                for(int y=0;y<LastSettlement.areas.GetLength(1); y++)
                {
                   for(int i=0;i< LastSettlement.areas[x, y].Buildings.Count; i++)
                    {
                        LastSettlement.areas[x, y].Buildings[i].DebugDrawArea();
                    }
                }
            }
        }
        WorldCenter = OverworldGenerator.Instance.WorldCenter;
        if (LastArea!=null && DrawSettlementAreas)
        {
            sectionCount = LastArea.Sections.Count;
            for(int x = 0; x < LastArea.Sections.Count; x++)
            {
                if (!LastArea.Sections[x].IsValid)
                {
                    continue;
                }
                LastArea.Sections[x].DebugDrawArea();
            }
        }
    }
}

