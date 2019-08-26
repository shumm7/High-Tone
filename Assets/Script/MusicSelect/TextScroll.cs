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

    float speed = 0.5f;


    void Awake()
    {
        rectTran = gameObject.GetComponent<RectTransform>();
        ScrollText = gameObject.GetComponent<Text>();
        TextWidth = ScrollText.preferredWidth;

        if (TextWidth > 355f)
        {
            isScrolling = true;

            PosBefore = rectTran.transform.localPosition;
            PosBefore.x = (179 + TextWidth / 2f) + 10;
            rectTran.transform.localPosition = PosBefore;
        }

    }

    void FixedUpdate()
    {
        if (!flag && isScrolling)
        {
            PosAfter = rectTran.transform.localPosition;
            PosAfter.x = - PosBefore.x;
            flag = true;

            rectTran.DOLocalMove(PosAfter, speed * (float)(ScrollText.text.Length)).SetEase(Ease.Linear);
            DOVirtual.DelayedCall(speed * (float)(ScrollText.text.Length), () => {
                rectTran.transform.localPosition = PosBefore;
                flag = false;
                }
            );
        }

    }
}
