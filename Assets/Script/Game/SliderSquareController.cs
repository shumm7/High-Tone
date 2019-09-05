using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderSquareController : MonoBehaviour
{
    public float width = 1.22f;
    public float length = 3f;
    public float currentPosition = -1.53f;
    public int StartRail = 1;
    public int EndRail = 1;
    float[] posX = { -2.44f, -1.22f, 0, 1.22f, 2.44f };

    public float speed;
    public double ArrivalTime;
    public double FinishTime;
    private int BPM;
    private int LPB;
    private int cnt = 1;

    public AudioSource audioSource;
    public AudioClip SliderSE;

    void Awake()
    {
        audioSource = transform.parent.parent.gameObject.GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (this.gameObject.activeSelf)
        {
            this.transform.localPosition -= new Vector3(0, 1f, 0) * speed * Time.fixedDeltaTime;
        }
    }


    public void SetPosition()
    {
       GetComponent<RectTransform>().sizeDelta = new Vector2(width, length);
        StartRail = Range(StartRail, 1, 5);
        EndRail = Range(EndRail, 1, 5);

        Vector3 localPos = transform.localPosition;
        localPos.x = (posX[StartRail - 1] + posX[EndRail - 1]) / 2f;
        localPos.y = currentPosition - 8.55f +  length / 2f;
        transform.localPosition = localPos;

        int posDiff = Mathf.Abs(StartRail - EndRail);
        if (StartRail > EndRail)
            posDiff = -posDiff;
        GetComponent<SkewedImage>().Skew.x = posDiff * (width / length);
    }

    bool IsPressed(double timePast)
    {
        return TimeComponent.isKeyPressing(StartRail);
    }

    private int Range(int num, int min, int max)
    {
        if (num < min)
            num = min;
        if (num > max)
            num = max;

        return num;
    }
}
