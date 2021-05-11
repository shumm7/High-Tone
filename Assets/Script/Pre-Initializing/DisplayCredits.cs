using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DisplayCredits : MonoBehaviour
{
    void Update()
    {
        this.GetComponent<Text>().text = DataHolder.Credits.ToString() + " CREDIT(S)";
    }
}
