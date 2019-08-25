using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoDisable : MonoBehaviour
{
    double disableTime = 3;
    private double time;
    private bool flag = false;

    private RawImage fadeImage;
    private Color col;

    void Update()
    {
        autoDisable();
    }

    public void autoDisable()
    {
        if (this.gameObject.activeSelf)
        {
            if (!flag)
            {
                time = Time.time;
                flag = true;
                fadeImage = gameObject.GetComponent<RawImage>();
                col = fadeImage.color;
                col.a = 1.0f;
                fadeImage.color = col;
            }

            if ((Time.time - time) > disableTime)
            {
                gameObject.SetActive(false);
                flag = false;
            }

        }
        else
        {
            flag = false;
        }
    }

    public void resetTime()
    {
        time = Time.time;
    }
}
