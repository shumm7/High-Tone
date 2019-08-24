using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeComponent : MonoBehaviour
{
    public static double StartTime;
    public static double[] KeyPressedTime;

    void Awake()
    {
        KeyPressedTime = new double[5];
    }

    public static void SetStartTime()
    {
        StartTime = Time.time;
    }

    public static double GetStartTime()
    {
        return StartTime;
    }

    public static double GetCurrentTime()
    {
        return Time.time;
    }

    public static double GetCurrentTimePast()
    {
        return Time.time - StartTime;
    }

    public static void SetPressedKeyTime(int num)
    {
        KeyPressedTime[num] = Time.time - StartTime;
    }

    public static double GetPressedKeyTime(int num)
    {
        return KeyPressedTime[num];
    }
}
