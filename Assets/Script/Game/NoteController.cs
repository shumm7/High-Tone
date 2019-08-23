using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] int Rail;
    private double ArrivalTime; //初期位置から判定ラインまでの到着時間

    private bool flag = false;
    private bool flag2 = false;
    private double time;

    void Start()
    {
        speed = GameManager.NoteSpeed;
        ArrivalTime = GameManager.ArrivalTime;
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

    public void SetRailNumber(int num)
    {
        Rail = num;
    }
}
