using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{

    //ゲームプレイ
    public static string NextMusicID;
    public static bool isVideo;
    public static float NoteSpeed;
    public static double GlobalNoteOffset;
    public static int Difficulty;
    public static bool DebugMode;

    //スコア
    public static int Score;
    public static int ObjectiveScore;
    public static int MaximumScore;
    public static float ScorePercentage;
    public static int Combo;
    public static int MaximumCombo;
    public static int[] JudgementAmount;

    //一時保存用
    public static string TemporaryString;
    public static double TemporaryNumber;

    void Awake()
    {
        DontDestroyOnLoad(this);
        JudgementAmount = new int[5];
    }
}
