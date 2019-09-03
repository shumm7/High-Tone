using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEffect : MonoBehaviour
{
    [SerializeField] GameObject[] Button;
    [SerializeField] AudioSource audioSource;

    private GameObject[] ButtonOverray;

    void Awake()
    {
        ButtonOverray = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            ButtonOverray[i] = Button[i].transform.Find("Button Pressed").gameObject;
            ButtonOverray[i].SetActive(false);
        }

        
    }

    void Update()
    {
        if (!MainMusicSelect.flag)
        {
            //Sound
            if (Input.GetKeyDown(KeyCode.Q))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }

            //Effect
            if (Input.GetKey(KeyCode.Q))
            {
                ButtonOverray[0].SetActive(true);
            }
            else
            {
                ButtonOverray[0].SetActive(false);
            }
            if (Input.GetKey(KeyCode.W))
            {
                ButtonOverray[1].SetActive(true);
            }
            else
            {
                ButtonOverray[1].SetActive(false);
            }
            if (Input.GetKey(KeyCode.E))
            {
                ButtonOverray[2].SetActive(true);
            }
            else
            {
                ButtonOverray[2].SetActive(false);
            }
            if (Input.GetKey(KeyCode.R))
            {
                ButtonOverray[3].SetActive(true);
            }
            else
            {
                ButtonOverray[3].SetActive(false);
            }
            if (Input.GetKey(KeyCode.T))
            {
                ButtonOverray[4].SetActive(true);
            }
            else
            {
                ButtonOverray[4].SetActive(false);
            }
        }
    }
}
