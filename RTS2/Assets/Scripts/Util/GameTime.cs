using UnityEngine;

public class GameTime : MonoBehaviour
{
    static GameTime instance;
    public static GameTime Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameTime>();
            }
            return instance;
        }
    }

    public float InGameTime = 0f;

    // Update is called once per frame
    void Update()
    {
        InGameTime += DeltaTimeWrapper.GameplayDelta;
    }
}
