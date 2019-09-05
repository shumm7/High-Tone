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
            if (TimeComponent.isKeyPressed(0))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (TimeComponent.isKeyPressed(1))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (TimeComponent.isKeyPressed(2))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (TimeComponent.isKeyPressed(3))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            if (TimeComponent.isKeyPressed(4))
            {
                audioSource.PlayOneShot(audioSource.clip);
            }

            //Effect
            if (TimeComponent.isKeyPressing(0))
            {
                ButtonOverray[0].SetActive(true);
            }
            else
            {
                ButtonOverray[0].SetActive(false);
            }
            if (TimeComponent.isKeyPressing(1))
            {
                ButtonOverray[1].SetActive(true);
            }
            else
            {
                ButtonOverray[1].SetActive(false);
            }
            if (TimeComponent.isKeyPressing(2))
            {
                ButtonOverray[2].SetActive(true);
            }
            else
            {
                ButtonOverray[2].SetActive(false);
            }
            if (TimeComponent.isKeyPressing(3))
            {
                ButtonOverray[3].SetActive(true);
            }
            else
            {
                ButtonOverray[3].SetActive(false);
            }
            if (TimeComponent.isKeyPressing(4))
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
