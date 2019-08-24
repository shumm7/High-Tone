using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] int Rail;
    [SerializeField] double ArrivalTime;
    private double Distance; //初期位置から判定ラインまでの到着時間

    private bool flag = false;
    private bool flag2 = false;
    private double time;

    void Start()
    {
        speed = GameManager.NoteSpeed;
        Distance = GameManager.ArrivalTime;
    }

    void FixedUpdate()
    {
        if (this.gameObject.activeSelf)
        {
            this.transform.localPosition -= new Vector3(0, 1f, 0) * speed * Time.fixedDeltaTime;
                if (this.transform.position.y < -2.5f)
            {
                this.gameObject.SetActive(false);
            }

        }
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            double diff = System.Math.Abs(TimeComponent.GetPressedKeyTime(Rail) - ArrivalTime);
            if (diff <= 0.15 && diff > 0.1) //Miss
            {
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(3);
            }
            else if (diff <= 0.1 && diff > 0.06) //Good
            {
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(2);
            }
            else if (diff <= 0.06 && diff > 0.03) //Great
            {
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(1);

            }
            else if (diff <= 0.03 && diff >= 0) //Perfect
            {
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(0);
            }
        }
    }

    public void SetRailNumber(int num)
    {
        Rail = num;
    }

    public void SetArrivalTime(double Time)
    {
        ArrivalTime = Time;
    }
}
