using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainLogin : MonoBehaviour
{
    public bool DebugMode = false;

    void Start()
    {
        if (!DebugMode)
        {
            //画面遷移
            float timeDelay = 0.5f;
            GameObject SceneChange = GameObject.Find("Scene Change");
            GameObject[] changer = new GameObject[4];
            RectTransform[] rectTran = new RectTransform[4];

            for (int i = 0; i < 4; i++)
            {
                changer[i] = SceneChange.transform.Find(i.ToString()).gameObject;
                rectTran[i] = changer[i].GetComponent<RectTransform>();
            }

            Sequence sceneChangeTween = DOTween.Sequence();
            for (int i = 0; i < 4; i++)
            {
                sceneChangeTween.Insert(1 + 0.25f * (3 - i),
                    rectTran[i].DOLocalMoveX(-1280, timeDelay).SetEase(Ease.OutQuint)
                );
            }

            sceneChangeTween.Join(
                DOVirtual.DelayedCall(4f, () =>
                {
                    Destroy(SceneChange);
                })
            );

            sceneChangeTween.Play();
        }
    }

}
