using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressedEffect : MonoBehaviour
{
    [SerializeField] GameObject Effect1;
    [SerializeField] GameObject Effect2;
    [SerializeField] GameObject Effect3;
    [SerializeField] GameObject Effect4;
    [SerializeField] GameObject Effect5;
    [SerializeField] AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Effect1.SetActive(false);
        Effect2.SetActive(false);
        Effect3.SetActive(false);
        Effect4.SetActive(false);
        Effect5.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Sound
        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(0);
            Debug.Log(TimeComponent.GetCurrentTimePast());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(2);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(3);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(4);
        }

        //Effect
        if (Input.GetKey(KeyCode.Q))
        {
            Effect1.SetActive(true);
        }
        else
        {
            Effect1.SetActive(false);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Effect2.SetActive(true);
        }
        else
        {
            Effect2.SetActive(false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            Effect3.SetActive(true);
        }
        else
        {
            Effect3.SetActive(false);
        }
        if (Input.GetKey(KeyCode.R))
        {
            Effect4.SetActive(true);
        }
        else
        {
            Effect4.SetActive(false);
        }
        if (Input.GetKey(KeyCode.T))
        {
            Effect5.SetActive(true);
        }
        else
        {
            Effect5.SetActive(false);
        }
    }
}
