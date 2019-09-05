using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ApplicationInformation : MonoBehaviour
{
    Text UI;

    void Start()
    {
        UI = GetComponent<Text>();

        UI.text = "High Tone   Version " + Application.version + "\nUnity Runtime " + UnityEngine.Application.unityVersion;
    }
}
