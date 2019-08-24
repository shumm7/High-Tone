using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScoreCalculation : MonoBehaviour
{
    //UI
    [SerializeField] TextMeshProUGUI ScoreUI;
    [SerializeField] Text ComboUI;

    public static int Combo = 0;
    public static int MaxCombo = 0;
    public static int Score = 0;
    private static int MaxNotesAmount;

   void Start()
    {
        MaxNotesAmount = GameManager.MaxNotesAmount;
    }

    void FixedUpdate()
    {
        ComboUI.text = Combo.ToString();
        ScoreUI.text = Score.ToString();
    }

    public static void SetNoteJudgement(int judge) //0Perfect 1Great 2Good 3Bad 4Miss
    {
        switch (judge)
        {
            case 0: //Perfect
                Score += 200 * (1 + (Combo / 100));
                Combo++;
                break;
            case 1: //Great
                Score += 100 * (1 + (Combo / 100));
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
    }
}
