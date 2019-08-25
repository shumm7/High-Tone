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

    public static bool isKeyPressing(int num)
    {
        switch (num)
        {
            case 0:
                return Input.GetKey(KeyCode.Q);
            case 1:
                return Input.GetKey(KeyCode.W);
            case 2:
                return Input.GetKey(KeyCode.E);
            case 3:
                return Input.GetKey(KeyCode.R);
            case 4:
                return Input.GetKey(KeyCode.T);
            default:
                return false;
        }
    }

    public static bool isSomeKeysPressing(int num1, int num2, int mode) //0 - 2
    {
        if (num1 > num2)
        {
            int temp = num2;
            num2 = num1;
            num1 = temp;
        }
        bool res = false;
        if(mode == KeyDetectionMode.and)
        {
            res = true;
        }

        for(int i=num1; i<=num2; i++)
        {
            if (mode == KeyDetectionMode.and)
            {
                if (!isKeyPressing(i))
                    res = false;
            }
            else if(mode == KeyDetectionMode.or)
            {
                if (isKeyPressing(i))
                    res = true;
            }
        }

        return res;
    }

    public static class KeyDetectionMode
    {
        public static int and = 0;
        public static int or = 1;
    }
}
