using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using UnityEngine.UI;

public class PauseMenuUIElement : BaseUIElement
{
    public ButtonManager Resume, Options, SaveLoad, Quit;

    private void Start()
    {
        Resume.gameObject.GetComponent<Button>().onClick.AddListener(HideUI);
        SaveLoad.gameObject.GetComponent<Button>().onClick.AddListener(SaveTest);
    }
    float SpeedGameAtWhenOpened = 0f;
    public override void HideUI()
    {
        DeltaTimeWrapper.GameplayDeltaMultiplier =SpeedGameAtWhenOpened;
        base.HideUI();
    }
    public override void DrawUI()
    {
        SpeedGameAtWhenOpened=DeltaTimeWrapper.GameplayDeltaMultiplier;
        DeltaTimeWrapper.GameplayDeltaMultiplier = 0f;
        base.DrawUI();
    }

    void SaveTest()
    {
        SerializationHelpers.SaveGame("TestWorld");
        EasyStopwatch.StartStopwatch();
        List<string> dataFromFile = SerializationHelpers.ReadFile(SerializationHelpers.GetWorldFilePath("TestWorld"));
      //  string[] splitObjects = null;
        for (int x = 0; x < dataFromFile.Count; x++)
        {
            Debug.Log("Data From File Line:" + x + " contents||" + dataFromFile[x]);
            WorldChunk wc = DataReaders.ParseWorldChunk(dataFromFile[x]);
            Debug.Log("Parsed chunk at " + wc.WorldCoords);
            //splitObjects = dataFromFile[x].Split(SerializeDataHelpers.DATA_OBJECT_SPLIT);
            //for(int y=0; y < splitObjects.Length; y++)
            //{
            //    DataReaders.ReadData(splitObjects[y]);
            //}
        }
        Debug.Log("reading took " + EasyStopwatch.GetStopwatchElapsedTime() + "s");

    }
}
