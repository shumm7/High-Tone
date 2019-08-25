using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class ScoreCalculation : MonoBehaviour
{
    //UI
    [SerializeField] TextMeshProUGUI ScoreUI;
    [SerializeField] Text ComboUI;

    //Sound
    public AudioSource SoundEffect;
    public AudioClip AudioClipPerfect;
    public AudioClip AudioClipGreat;
    //public AudioClip AudioClipGood;
    //public AudioClip AudioClipMiss;
    //public AudioClip AudioClipBad;

    public static int Combo = 0;
    public static int MaxCombo = 0;
    public static int Score = 0;
    public static int[] JudgementCount;
    private int MaxNotesAmount;
    private int MaximumScore = 0;
    private static int ScoreAddedFlag = -1;

   void Start()
    {
        MaxNotesAmount = GameManager.MaxNotesAmount;
        JudgementCount = new int[5];

        for (int i = 1; i <= 500; i++)
        {
            float temp = Mathf.Round(i / 20f) * 10;
            temp = (temp / 100f) * 5f + 1;
            MaximumScore += (int)(200f * temp);
        }
    }

    void FixedUpdate()
    {
        ComboUI.text = Combo.ToString();
        ScoreUI.text = Score.ToString();

        //パーセンテージ計算
    }

    void Update()
    {
        if (ScoreAddedFlag!=-1)
        {
            playsound(ScoreAddedFlag);
            GetComponent<ScoreDisplayText>().StartEffect(ScoreAddedFlag);
            ScoreAddedFlag = -1;
        }
    }

    public static void SetNoteJudgement(int judge) //0Perfect 1Great 2Good 3Bad 4Miss
    {
        JudgementCount[judge]++;
        float BonusMultiplier = Mathf.Round(Combo / 20f) * 10;
        BonusMultiplier = (BonusMultiplier / 100f) * 5f + 1;
        switch (judge)
        {
            case 0: //Perfect
                Score += (int)(200f * BonusMultiplier);
                Combo++;
                break;
            case 1: //Great
                Score += (int)(100f * BonusMultiplier);
                Combo++;
                break;
            case 2: //Good
                MaxCombo = Combo;
                Combo = 0;
                Score += 50;
                break;
            case 3:
            case 4:
                MaxCombo = Combo;
                Combo = 0;
                break;
        }
        ScoreAddedFlag = judge;
    }

    public static class Judgement
    {
        public const int Perfect = 0;
        public const int Great = 1;
        public const int Good = 2;
        public const int Bad = 3;
        public const int Miss = 4;
    }

    private void playsound(int judge)
    {
        switch(judge)
        {
            case 0: //Perfect
                SoundEffect.PlayOneShot(AudioClipPerfect);
                break;
            case 1: //Great
                SoundEffect.PlayOneShot(AudioClipGreat);
                break;
            case 2: //Good
            case 3:
            case 4:
            default:
                break;
        }
    }
}
