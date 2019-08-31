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

    private Sequence seq;

    float speed = 10f;


    public void Setup()
    {
        rectTran = gameObject.GetComponent<RectTransform>();
        ScrollText = gameObject.GetComponent<Text>();
        TextWidth = ScrollText.preferredWidth;
        flag = false;

        seq.Kill();
        seq = DOTween.Sequence();

        if (TextWidth > 355f)
        {
            isScrolling = true;

            PosBefore = rectTran.transform.localPosition;
            PosBefore.x = (179 + TextWidth / 2f) + 10;
            rectTran.transform.localPosition = PosBefore;
        }
    }

    public void KillSequence()
    {
        seq.Kill();
    }

    void Update()
    {
        if (!flag && isScrolling)
        {
            flag = true;
            PosAfter = rectTran.transform.localPosition;
            PosAfter.x = - PosBefore.x;

            float time = speed;
            seq.Join(rectTran.DOLocalMove(PosAfter, time).SetEase(Ease.Linear));
            seq.Join(
                DOVirtual.DelayedCall(time, () =>
                {
                    if (rectTran != null)
                    {
                        rectTran.transform.localPosition = PosBefore;
                    }
                    flag = false;
                    seq.Kill();
                })
            );

            seq.Play();
        }

    }
}
