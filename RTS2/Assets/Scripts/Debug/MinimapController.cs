using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{

    static MinimapController instance;
    public static MinimapController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<MinimapController>(true) ;
            }
            return instance;
        }
    }

    public RawImage MinimapDisplay;
    UnitsMinimapRenderer Units;
    WorldMinimapRenderer World;
    // Start is called before the first frame update
    void Start()
    {
        Units = new UnitsMinimapRenderer();
        World=new WorldMinimapRenderer();
        StartCoroutine(UpdateMinimap());

    }

    // Update is called once per frame
    bool isUpdatingData = false, updatedTexture = false;
    void Update()
    {
        if (Units.IsDataUpdateDone()==false)
        {
            isUpdatingData = true;
            StartCoroutine(RefreshUnitData());
           
        }
        else
        {
            if (!updatedTexture)
            {
                Color[,] baseColours = World.GetCurChunkColours();

                Units.RefreshTexture(baseColours);
                MinimapDisplay.texture = Units.Texture;
                updatedTexture = true;
                StartCoroutine(UpdateMinimap());

            }
        }
       
    }
    const float TimeBetweenMinimapUpdates = 1f;

    IEnumerator UpdateMinimap()
    {
        yield return new WaitForSeconds(TimeBetweenMinimapUpdates);
        Units.StartRefresh();
        World.StartRefresh();
        updatedTexture = false;
    }

    IEnumerator RefreshUnitData()
    {
        yield return new WaitForEndOfFrame();
        Units.RefreshData();
        World.RefreshData();


        isUpdatingData = false;
    }
}
