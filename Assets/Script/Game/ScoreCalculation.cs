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
    [SerializeField] RawImage ScoreGaugeMaskUI;
    public float MaximumGaugeWidth = 400;
    private RectTransform ScoreGaugeRectTran;

    private int MaxNotesAmount;
    private int MaximumScore = 0;
    private int MaximumScoreForDisplay = 0;
    private static int ScoreAddedFlag = -1;

    public static int Combo = 0;
    public static int MaxCombo = 0;
    public static int Score = 0;
    public static int[] JudgementCount;
    public static float ScorePercentage;

   void Start()
    {
        MaxNotesAmount = GameManager.MaxNotesAmount;
        JudgementCount = new int[5];
        MaximumScore = GameManager.MaximumScore;
        MaximumScoreForDisplay = GameManager.MaximumScoreForDisplay;

        ScoreGaugeRectTran = ScoreGaugeMaskUI.gameObject.GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        ComboUI.text = Combo.ToString();
        ScoreUI.text = Score.ToString();

        //パーセンテージ計算
        ScorePercentage = (float)Score / (float)MaximumScore;

        //ゲージ表示
        float PercentageForGauge = (float)Score / (float)MaximumScoreForDisplay;
        if(PercentageForGauge >= 1f)
        {
            PercentageForGauge = 1f;
        }
        Vector2 size = ScoreGaugeRectTran.sizeDelta;
        size.x = MaximumGaugeWidth * PercentageForGauge;
        ScoreGaugeRectTran.sizeDelta = size;
    }

    void Update()
    {
        if (ScoreAddedFlag!=-1)
        {
            GetComponent<ScoreDisplayText>().StartEffect(ScoreAddedFlag);
            ScoreAddedFlag = -1;
        }
    }

    public static void SetNoteJudgement(int judge, int type) //0Perfect 1Great 2Good 3Bad 4Miss
    {
        JudgementCount[judge]++;
        float BonusMultiplier = Mathf.Round(Combo / 20f) * 10;
        BonusMultiplier = (BonusMultiplier / 100f) * 5f + 1;
        float SpecialNoteMultiplier = 1;

        if (type == NoteType.Special)
        {
            BonusMultiplier *= 2;
            SpecialNoteMultiplier = 2;
        }

        switch (judge)
        {
            case 0: //Perfect
                Score += (int)(200f * SpecialNoteMultiplier * BonusMultiplier);
                Combo++;
                break;
            case 1: //Great
                Score += (int)(100f * SpecialNoteMultiplier * BonusMultiplier);
                Combo++;
                break;
            case 2: //Good
                MaxCombo = Combo;
                Combo = 0;
                Score += (int)(50f * SpecialNoteMultiplier);
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

    public static class NoteType
    {
        public const int Normal = 1;
        public const int Slider = 2;
        public const int Special = 3;
        public const int Damage = 4;
    }
}
