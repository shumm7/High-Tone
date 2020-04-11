using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeComponent : MonoBehaviour
{
    public static double StartTime;
    public static double[] KeyPressedTime;
    public static AudioSource music;
    public static KeyCode[] Key = new KeyCode[] {
            KeyCode.Q, KeyCode.A,KeyCode.Z,
            KeyCode.W, KeyCode.S, KeyCode.X,
            KeyCode.E, KeyCode.D, KeyCode.C,
            KeyCode.R, KeyCode.F, KeyCode.V,
            KeyCode.T, KeyCode.G, KeyCode.B,
        };

void Awake()
    {
        KeyPressedTime = new double[5];
    }

    public static void SetStartTime()
    {
        StartTime = Time.time;
    }

    public static void SetStartTime(float addition)
    {
        StartTime = Time.time + addition;
    }

    public static void ResetStartTime()
    {
        StartTime = 0;
    }

    public static double GetStartTime()
    {
        return StartTime;
    }

    public static double GetCurrentTimePast()
    {
        if (music.isPlaying)
        {
            return music.time;
        }
        else
        {
            return Time.time - StartTime;
        }
    }

    public static void SetPressedKeyTime(int num)
    {
        if(GameManager.isPlaying)
            KeyPressedTime[num] = Time.time - StartTime;
    }

    public static double GetPressedKeyTime(int num)
    {
        return KeyPressedTime[num];
    }

    public static bool isKeyPressing(int num)
    {
        return Input.GetKey(Key[num * 3]) || Input.GetKey(Key[num * 3 + 1]) || Input.GetKey(Key[num * 3 + 2]);
    }

    public static bool isKeyPressingDetailed(int num)
    {
        return Input.GetKey(Key[num]);
    }

    public static int GetKeyRail(int button)
    {
        return button / 3;
    }


    public static bool isKeyPressed(int num)
    {
        return Input.GetKeyDown(Key[num * 3]) || Input.GetKeyDown(Key[num * 3 + 1]) || Input.GetKeyDown(Key[num * 3 + 2]);
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
