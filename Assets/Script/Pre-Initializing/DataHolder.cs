using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    //ユーザー情報
    public static string UserID = "Guest";
    public static float NoteSpeed = 10;
    public static float MusicVolume = -20;
    public static float SEVolume = -20;

    //ゲームプレイ
    public static string NextMusicID;
    public static bool isVideo;
    public static double GlobalNoteOffset;
    public static int Difficulty;
    public static bool DebugMode;

    //スコア
    public static int Score = 0;
    public static int ObjectiveScore;
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
