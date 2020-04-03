using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextScroll : MonoBehaviour
{

    private GameObject clone;
    private Text ScrollText;
    private RectTransform rectTran;
    private Vector3 PosBefore;
    private Vector3 PosAfter;
    private float TextWidth;

    public Sequence seq;

    float speed = 10f;


    public void Setup()
    {
        rectTran = gameObject.GetComponent<RectTransform>();
        ScrollText = gameObject.GetComponent<Text>();
        TextWidth = ScrollText.preferredWidth;

        PosBefore = rectTran.transform.localPosition;
        PosBefore.x = 0;
        rectTran.transform.localPosition = PosBefore;

        seq.Kill();
        seq = DOTween.Sequence();
        

        if (TextWidth > 355f)
        {
            PosBefore = rectTran.transform.localPosition;
            PosBefore.x = (179 + TextWidth / 2f) + 10;
            rectTran.transform.localPosition = PosBefore;
            PosAfter.x = -PosBefore.x;

            float time = speed;
            seq.OnComplete(() => SeqOnComplete());
            seq.Join(rectTran.DOLocalMove(PosAfter, time).SetEase(Ease.Linear));

            seq.Play();
        }
    }

    public void SeqOnComplete()
    {
            seq.Restart();
        //seq.Pause();
        if (rectTran != null)
        {
            rectTran.transform.localPosition = PosBefore;
        }
        else
        {
            KillSequence();
        }
    }

    public void KillSequence()
    {
        seq.Kill();
    }
}
