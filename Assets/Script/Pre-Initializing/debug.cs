﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class debug : MonoBehaviour
{
    public static bool isDebugMode;
    public GameObject DebugInfo;
    public AudioMixer audioMixer;

    public Text FrameRate;
    public Text StabilizedFrameRate;
    public Text TimeUI;
    public Text MasterVolume;
    public Text AudioVolume;
    public Text MusicID;
    public Text GameSettings;
    public Text GameSettings2;
    public Text KeyInput;
    public Text UserID;
    public Text LoginID;
    public Text CardReader;
    public Text CardReaderName;

    public GameObject DebugLog;

    int DisplayMode = 0;
    int keyCount = 0;
    int frameCount;
    float prevTime;
    float[] audioVolume = new float[4];


    private void Start()
    {
        DisplayMode = 0;
        frameCount = 0;
        prevTime = 0.0f;
    }

    void Update()
    {
        if (isDebugMode)
        {
            //FrameRate
            FrameRate.text = "Frame Rate: " + (1f / Time.deltaTime).ToString() + " fps";

            //Stabilized Frame Rate
            frameCount++;
            float time = Time.realtimeSinceStartup - prevTime;
            if (time >= 0.5f)
            {
                StabilizedFrameRate.text = "Stabilized Frame Rate: " + (frameCount / time).ToString() + " fps";

                frameCount = 0;
                prevTime = Time.realtimeSinceStartup;
            }

            //Time
            System.TimeSpan ts = new System.TimeSpan(0, 0, (int)System.Math.Floor(Time.time));
            TimeUI.text = "Time: " + ts.ToString();

            //Master Volume
            audioMixer.GetFloat("Master", out audioVolume[0]);
            MasterVolume.text = "Master Volume: " + audioVolume[0] + " dB";

            //Audio Volume
            audioMixer.GetFloat("BGM", out audioVolume[1]);
            audioMixer.GetFloat("Music", out audioVolume[2]);
            audioMixer.GetFloat("SE", out audioVolume[3]);
            AudioVolume.text = "BGM: " + (int)audioVolume[1] + "dB / Music: " + (int)audioVolume[2] + "dB / SE: " + (int)audioVolume[3] + "dB";

            //MusicID
            MusicID.text = "Music ID: " + DataHolder.NextMusicID;

            //GameSettings
            GameSettings.text = "Difficulty: " + DataHolder.Difficulty.ToString() + "   Video: " + DataHolder.isVideo.ToString() + "   Speed: " + DataHolder.NoteSpeed.ToString();
            GameSettings2.text = "Played: " + DataHolder.PlayedTime.ToString() + " / " + DataHolder.PlayTimePerCredit.ToString() + "   Video Mode: " + DataHolder.VideoSettingMode;

            //Input
            KeyInput.text = "Input: ";
            keyCount = 0;
            foreach (var keyRecord in TimeComponent.Key)
            {
                if (Input.GetKey(keyRecord))
                {
                    KeyInput.text += "●";
                }
                else
                {
                    KeyInput.text += "○";
                }
                if ((keyCount + 1) % 3 == 0)
                {
                    KeyInput.text += " ";
                }
                keyCount++;
            }

            //UserID
            UserID.text = "User ID: " + DataHolder.UserID;

            //LoginID
            LoginID.text = DataHolder.UserLoginID;

            //CardReader
            CardReader.text = "Card Reader: " + DataHolder.CardReader;

            //CardReaderName
            CardReaderName.text = "Reader Name: " + DataHolder.CardReaderName;

        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (DisplayMode == -1)
            {
                DisplayMode = 0;
                DebugLog.SetActive(true);
                SetDebugMode(true);
            }
            else if (DisplayMode == 0)
            {
                DisplayMode = 1;
                DebugLog.SetActive(false);
                SetDebugMode(true);
            }
            else if (DisplayMode == 1)
            {
                DisplayMode = -1;
                DebugLog.SetActive(false);
                SetDebugMode(false);
            }
        }

        DebugInfo.SetActive(isDebugMode);
    }

    public static void SetDebugMode(bool mode)
    {
        isDebugMode = mode;
    }
}
