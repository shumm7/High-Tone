using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataController : MonoBehaviour
{
    NFCReader R;
    public bool CardReadingEnabled;

    public float CardReadingTimeOutLength = 1f;
    private float LastCardReadTime;
    public string IDm;

    void Awake()
    {
        R = GetComponent<NFCReader>();
        CardReadingEnabled = false;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if(Time.time - LastCardReadTime >= CardReadingTimeOutLength && CardReadingEnabled)
        {
            LastCardReadTime = Time.time;
            IDm = R.ReadCardData().CardID;
        }
    }
}
