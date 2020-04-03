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

    public static int MaxNotesAmount;
    public static int MaximumScore = 0;
    public static int ObjectiveScore = 0;
    public static int MaximumCombo = 0;
    private static int ScoreAddedFlag = -1;

    public static int Combo = 0;
    public static int MaxCombo = 0;
    public static int Score = 0;
    public static int[] JudgementCount;
    public static float ScorePercentage;

    private bool FullcomboFlag = false;

    private void Awake()
    {
        MaximumScore = 0;
        MaximumCombo = 0;
        ScoreAddedFlag = -1;
        Combo = 0;
        MaxCombo = 0;
        Score = 0;
    }

    void Start()
    {
        MaxNotesAmount = GameManager.MaxNotesAmount;
        JudgementCount = new int[5];
        ObjectiveScore = GameManager.MaximumScoreForDisplay;

        ScoreGaugeRectTran = ScoreGaugeMaskUI.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        ComboUI.text = Combo.ToString();
        ScoreUI.text = Score.ToString();

        //パーセンテージ計算
        ScorePercentage = (float)Score / (float)MaximumScore;

        //ゲージ表示
        float PercentageForGauge = (float)Score / (float)ObjectiveScore;
        if (PercentageForGauge >= 1f)
        {
            PercentageForGauge = 1f;
        }
        Vector2 size = ScoreGaugeRectTran.sizeDelta;
        size.x = MaximumGaugeWidth * PercentageForGauge;
        ScoreGaugeRectTran.sizeDelta = size;

        //エフェクト
        if (ScoreAddedFlag!=-1)
        {
            GetComponent<ScoreDisplayText>().StartEffect(ScoreAddedFlag);
            ScoreAddedFlag = -1;
        }

        //フルコンボテキスト
        if(GameManager.GameStatus == GameManager.Status.Finished && !FullcomboFlag)
        {
            if (Combo == MaximumCombo)
            {
            GetComponent<ScoreDisplayText>().StartEffectResult("fullcombo");
            FullcomboFlag = true;
            }
            else if (ObjectiveScore <= Score)
            {
                GetComponent<ScoreDisplayText>().StartEffectResult("clear");
                FullcomboFlag = true;
            }
            else if (ObjectiveScore > Score)
            {
                GetComponent<ScoreDisplayText>().StartEffectResult("failed");
                FullcomboFlag = true;
            }
        }
    }

    public static void SetNoteJudgement(int judge, int type) //0Perfect 1Great 2Good 3Bad 4Miss
    {
        if (judge != -1)
        {
            JudgementCount[judge]++;
        }
        MaximumCombo++;

        float BonusMultiplier = 1;

        if (type == NoteType.Special)
        {
            BonusMultiplier = 2;
        }
        MaximumScore += (int)(200f * BonusMultiplier);
        switch (judge)
        {
            case -1:
                break;
            case 0: //Perfect
                Score += (int)(200f * BonusMultiplier);
                Combo++;
                break;
            case 1: //Great
                Score += (int)(100f * BonusMultiplier);
                Combo++;
                break;
            case 2: //Good
                Combo = 0;
                Score += (int)(50f);
                break;
            case 3:
            case 4:
                Combo = 0;
                break;
        }
        ScoreAddedFlag = judge;

        if(MaxCombo < Combo)
        {
            MaxCombo = Combo;
        }
    }

    public static class Judgement
    {
        public const int None = -1; //空のスコア
        public const int Perfect = 0;
        public const int Great = 1;
        public const int Good = 2;
        public const int Bad = 3;
        public const int Miss = 4;
    }

    public static class NoteType {
        public const int Normal = 1;
        public const int Slider = 2;
        public const int Special = 3;
        public const int Damage = 4;
    }
}
