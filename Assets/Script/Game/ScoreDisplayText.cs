using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ScoreDisplayText : MonoBehaviour
{
    public RawImage Perfect;
    public RawImage Great;
    public RawImage Good;
    public RawImage Bad;
    public RawImage Miss;
    public float ScaleChangeTime = 0.1f;
    private GameObject text;

    public void StartEffect(int judge)
    {
        setAllInactive();
        text = getGameObject(judge);
        text.GetComponent<AutoDisable>().resetTime();
        text.SetActive(true);
        text.transform.localScale = Vector3.zero;
        RectTransform rectTran = text.GetComponent<RectTransform>();

        rectTran.DOScale(new Vector3(0.2f, 0.2f, 0.2f), ScaleChangeTime);
    }

    private GameObject getGameObject(int judge)
    {
        switch (judge)
        {
            case ScoreCalculation.Judgement.Perfect:
                return Perfect.gameObject;
            case ScoreCalculation.Judgement.Great:
                return Great.gameObject;
            case ScoreCalculation.Judgement.Good:
                return Good.gameObject;
            case ScoreCalculation.Judgement.Bad:
                return Bad.gameObject;
            case ScoreCalculation.Judgement.Miss:
                return Miss.gameObject;
            default:
                return null;
        }
    }

    private void setAllInactive()
    {
        Perfect.gameObject.SetActive(false);
        Great.gameObject.SetActive(false);
        Good.gameObject.SetActive(false);
        Bad.gameObject.SetActive(false);
        Miss.gameObject.SetActive(false);
    }
}
