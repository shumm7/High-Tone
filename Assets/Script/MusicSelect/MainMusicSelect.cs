using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMusicSelect : MonoBehaviour
{
    public GameObject[] Frame = new GameObject[5];
    private bool ScrollingFlag = false;
    private float moveTime = 0.5f;


    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !ScrollingFlag)
        {
            ScrollLeft();
            ScrollingFlag = true;
        }

        if (ScrollingFlag)
        {
            DOVirtual.DelayedCall(moveTime, () =>
            {
                ScrollingFlag = false;
            });
        }
    }

    private void SetFrameData(int difficulty, string id, int frameNumber)
    {
        Frame[frameNumber].GetComponent<MusicFrameComponent>().SetMusicData(difficulty, id);
    }

    public void ScrollLeft()
    {
        /* シーケンスを使わないと Max Tweens reached で止まる
       
        Vector3[] localPos = new Vector3[5];
        Vector3[] localScale = new Vector3[5];
        RectTransform[] RectTran = new RectTransform[5];

        for (int i = 0; i < 5; i++)
        {
            localPos[i] = Frame[i].transform.localPosition;
            RectTran[i] = Frame[i].GetComponent<RectTransform>();
            localScale[i] = Frame[i].transform.localScale;
        }

        DG.Tweening.Ease Easing = Ease.InOutExpo;
        for (int i = 1; i < 5; i++)
        {
            RectTran[i].DOLocalMove(localPos[i - 1], moveTime).SetEase(Easing);
            RectTran[i].DOScale(localScale[i - 1], moveTime).SetEase(Easing);
        }

        DOVirtual.DelayedCall(moveTime, () =>
        {
            for (int i = 0; i < 4; i++)
            {
                Frame[i].transform.localPosition = localPos[i+1];
                Frame[i].transform.localScale = localScale[i+1];
                RectTran[i].DOKill();
            }
        });

    */
    }
}
