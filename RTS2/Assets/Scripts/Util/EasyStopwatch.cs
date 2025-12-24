using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class EasyStopwatch
{
    public static Stopwatch Stopwatch;
    public static void StartStopwatch()
    {
        if(Stopwatch == null)
        {
            Stopwatch = new Stopwatch();
        }
        Stopwatch.Restart();
        Stopwatch.Start();
    }

    public static void StopStopwatch()
    {
        Stopwatch.Stop();
    }

    public static float GetStopwatchElapsedTime()
    {
        return (float)Stopwatch.Elapsed.TotalSeconds;
    }
}

public static class MultithreadingStopwatch {
    public static Stopwatch Stopwatch;
    public static void StartStopwatch()
    {
        if (Stopwatch == null)
        {
            Stopwatch = new Stopwatch();
        }
        Stopwatch.Restart();
        Stopwatch.Start();
    }

    public static void StopStopwatch()
    {
        Stopwatch.Stop();
    }

    public static float GetStopwatchElapsedTime()
    {
        return (float)Stopwatch.Elapsed.TotalSeconds;
    }

}

