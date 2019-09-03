using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    //ユーザー情報
    public static string UserID = "Guest";
    public static float NoteSpeed = 10; //5 - 25
    public static float MusicVolume = -20;
    public static float SEVolume = -20;

    //ゲームプレイ
    public static string NextMusicID;
    public static bool isVideo = true;
    public static double GlobalNoteOffset = 0.03;
    public static int Difficulty = 0;
    public static bool DebugMode = false;

    //スコア
    public static int Score = 0;
    public static int ObjectiveScore = 0;
    public static int MaximumScore;
    public static float ScorePercentage;
    public static int Combo = 0;
    public static int MaximumCombo;
    public static int[] JudgementAmount;

    //一時保存用
    public static string TemporaryString;
    public static int TemporaryIntNumber;
    public static double TemporaryNumber;

    void Awake()
    {
        DontDestroyOnLoad(this);
        JudgementAmount = new int[5];
    }
}
