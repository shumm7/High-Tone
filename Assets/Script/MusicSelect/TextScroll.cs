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
    private bool isScrolling = false;
    private bool flag = false;
    private float TextWidth;

    public Sequence seq;

    float speed = 10f;


    public void Setup()
    {
        rectTran = gameObject.GetComponent<RectTransform>();
        ScrollText = gameObject.GetComponent<Text>();
        TextWidth = ScrollText.preferredWidth;
        flag = false;

        isScrolling = false;
        PosBefore = rectTran.transform.localPosition;
        PosBefore.x = 0;
        rectTran.transform.localPosition = PosBefore;

        seq.Kill();
        seq = DOTween.Sequence();
        

        if (TextWidth > 355f)
        {
            isScrolling = true;

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
        rectTran.transform.localPosition = PosBefore;
    }

    public void KillSequence()
    {
        seq.Kill();
    }
}
