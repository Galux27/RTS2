using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldspaceUIManager : MonoBehaviour
{
    static WorldspaceUIManager instance;
    public static WorldspaceUIManager Instance
    {
        get { if (instance == null) { instance = FindObjectOfType<WorldspaceUIManager>(true); } return instance; }
    }

    public GameObject WorldspaceHealthBar;
}
