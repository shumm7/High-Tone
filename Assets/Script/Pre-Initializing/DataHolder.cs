﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    //ユーザー情報
    public static string UserID = "Guest"; //ユーザーID
    public static string UserLoginID = ""; //ユーザーのログインID
    public static string UserPassword = ""; //ログインパスワード
    public static float NoteSpeed = 10; //ノーツの降下スピード（5 - 25）
    public static float MusicVolume = -20; //楽曲音量[dB]
    public static float SEVolume = -20; //効果音音量[dB]
    public static int Credits; //現在のクレジット数

    //ゲームプレイ
    public static string NextMusicID; //次に再生する楽曲のID
    public static bool isVideo = true; //映像を再生するか
    public static int Difficulty = 0; //曲の難易度
    public static string VideoSettingMode = "cut"; //"cut":映像の画面上下を切り取る "fit":動画を画面ピッタリに表示する

    //ゲーム設定
    public static bool DebugMode = false; //デバッグモード
    public static float MasterVolume = 0; //マスター音量[dB]
    public static float BGMVolume = 0; //BGM音量[dB]
    public static int PlayedTime = 0; //プレイした回数
    public static int PlayTimePerCredit = 2; //1クレジットでプレイできる曲数
    public static bool FreePlay = true; //クレジット不要でプレイができるかどうか
    public static double GlobalNoteOffset = 0.03; //ゲーム全体で適用するノーツ判定時のオフセット [秒]

    //スコア
    public static int Score = 0; //スコア
    public static int MaximumScore; //その楽曲での最高スコア
    public static float ScorePercentage; //最高スコアに対する獲得スコアの割合
    public static int Combo = 0; //1ゲーム内での最高コンボ
    public static int MaximumCombo; //その楽曲で獲得できる最高のコンボ
    public static int[] JudgementAmount; //各判定の数

    //デバイス
    public static bool CardReader = false;
    public static string CardReaderName = "";


    //一時保存用
    public static string TemporaryString;
    public static int TemporaryIntNumber;
    public static double TemporaryNumber;
    public static GameObject TemporaryGameObject;

    void Awake()
    {
        DontDestroyOnLoad(this);
        JudgementAmount = new int[5];
    }

    public static void Login(string UserID)
    {

    }

    public static void Logout()
    {
        UserID = "Guest";
        UserLoginID = "";
        UserPassword = "";
        NoteSpeed = 10;
        MusicVolume = -20;
        SEVolume = -20;
        Difficulty = 0;
        isVideo = true;
    }
}
