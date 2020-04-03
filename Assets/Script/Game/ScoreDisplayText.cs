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
    public RawImage Fullcombo;
    public RawImage Clear;
    public RawImage Failed;
    public AudioSource audioSource;
    public AudioClip fullcomboAudio;
    public AudioClip clearAudio;
    public AudioClip failedAudio;
    public float ScaleChangeTime = 0.1f;
    public float ScaleChangeTimeFullcombo = 1.2f;
    private GameObject text;

    public void StartEffect(int judge)
    {
        setAllInactive();
        text = getGameObject(judge);
        text.GetComponent<AutoDisable>().resetTime();
        text.SetActive(true);
        text.transform.localScale = Vector3.zero;
        RectTransform rectTran = text.GetComponent<RectTransform>();

        rectTran.DOScale(new Vector3(0.12f, 0.12f, 1f), ScaleChangeTime);
    }

    public void StartEffectResult(string mode)
    {
        GameObject fullcomboText = null;
        if (mode == "fullcombo")
        {
            fullcomboText = Fullcombo.gameObject;
            audioSource.clip = fullcomboAudio;
        }
        else if (mode == "clear")
        {
            fullcomboText = Clear.gameObject;
            audioSource.clip = clearAudio;
        }
        else if (mode == "failed")
        {
            fullcomboText = Failed.gameObject;
            audioSource.clip = failedAudio;
        }

        setAllInactive();
        fullcomboText.SetActive(true);
        RectTransform rectTran = fullcomboText.GetComponent<RectTransform>();
        fullcomboText.transform.localScale = Vector3.zero;

        rectTran.DOScale(new Vector3(0.5f, 0.5f, 1f), ScaleChangeTimeFullcombo).SetEase(Ease.OutBounce);
        audioSource.PlayOneShot(audioSource.clip);

        DOVirtual.DelayedCall(ScaleChangeTimeFullcombo + 4, () =>
        {
            RawImage FadeInImage = fullcomboText.GetComponent<RawImage>();
            DOTween.ToAlpha(() => FadeInImage.color, a => FadeInImage.color = a, 0f, 0.5f);

            DOVirtual.DelayedCall(ScaleChangeTimeFullcombo, () =>
            {
                fullcomboText.SetActive(false);
                Color col = FadeInImage.color;
                col.a = 1;
                FadeInImage.color = col;
            }
            );

        }
        );
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
