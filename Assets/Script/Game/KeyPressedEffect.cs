using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressedEffect : MonoBehaviour
{
    [SerializeField] GameObject RailParent;
    [SerializeField] AudioSource audioSource;
    public bool FullEffect = false;
    GameObject[] Effect = new GameObject[15];
    KeyCode[] Key;

    // Start is called before the first frame update
    void Start()
    {
        for(int i=1; i<=15; i++)
        {
            Effect[i - 1] = RailParent.transform.Find("Rail" + (((i - 1) / 3) + 1).ToString()).Find("Effect" + ((i-1) % 3 + 1).ToString()).gameObject;
            Effect[i - 1].SetActive(false);
        }

        Key = new KeyCode[] {
            KeyCode.Q, KeyCode.A,KeyCode.Z,
            KeyCode.W, KeyCode.S, KeyCode.X,
            KeyCode.E, KeyCode.D, KeyCode.C,
            KeyCode.R, KeyCode.F, KeyCode.V,
            KeyCode.T, KeyCode.G, KeyCode.B,
        };
    }

    // Update is called once per frame
    void Update()
    {
        //Sound
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Z))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(0);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(1);
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.C))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(2);
        }
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.V))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(3);
        }
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.B))
        {
            audioSource.PlayOneShot(audioSource.clip);
            TimeComponent.SetPressedKeyTime(4);
        }

        if (!FullEffect)
        {
            for (int i = 0; i < 15; i++)
            {
                if (Input.GetKey(Key[i]))
                {
                    Effect[i].SetActive(true);

                }
                else
                {
                    Effect[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (Input.GetKey(Key[i * 3]) || Input.GetKey(Key[i * 3 + 1]) || Input.GetKey(Key[i * 3 + 2]))
                {
                    Effect[i * 3].SetActive(true);
                    Effect[i * 3 + 1].SetActive(true);
                    Effect[i * 3 + 2].SetActive(true);
                }
                else
                {
                    Effect[i * 3].SetActive(false);
                    Effect[i * 3 + 1].SetActive(false);
                    Effect[i * 3 + 2].SetActive(false);
                }
            }
        }
    }
}
