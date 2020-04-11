using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndscreenController : MonoBehaviour
{
    Sequence sceneChangeTween;
    Sequence StartTween;
    Sequence Countdown;
    [SerializeField] bool DebugMode = false;
    public GameObject Button;
    public Transform TextSaved;
    public Text TextSavedColor;
    public Transform ContinueCheck;
    public Text[] ContinueCheckColor = new Text[2];
    public Text ThankyouMessage;
    bool CountdownFlag = false;
    bool flag = false;
    bool ButtonPressed = false;
    int CountStartNumber = 15;

    public AudioSource audioSource;
    GameObject SceneChange;
    public GameObject SceneChangeEnd;

    void Awake()
    {
        //DOTween.Init();
        //DOTween.defaultAutoPlay = AutoPlay.None;

        //初期位置設定
        Button.transform.localPosition = new Vector3(0, -100f, 0);
        Button.SetActive(false);
        TextSaved.gameObject.SetActive(true);
        TextSaved.localScale = new Vector3(0f, 0f, 1f);
        TextSavedColor.color = new Color(50f / 255f, 50f / 255f, 50f / 255f, 0f);
        ContinueCheck.gameObject.SetActive(true);
        ContinueCheck.localScale = new Vector3(0f, 0f, 1f);
        ContinueCheckColor[0].color = new Color(50f / 255f, 50f / 255f, 50f / 255f, 0f);
        ContinueCheckColor[1].color = new Color(50f / 255f, 50f / 255f, 50f / 255f, 0f);
        ContinueCheckColor[1].text = CountStartNumber.ToString();
        ThankyouMessage.color = new Color(50f / 255f, 50f / 255f, 50f / 255f, 0f);


        //セーブ完了

        StartTween.Insert(3f, DOVirtual.DelayedCall(3f, () => {
            TextSaved.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.OutQuint);
            DOTween.ToAlpha(() => TextSavedColor.color,
                color => TextSavedColor.color = color, 1f, 0.5f
            );
        }));
            
        StartTween.Insert(4f, DOVirtual.DelayedCall(5f, () => {
            TextSaved.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.5f).SetEase(Ease.OutQuint);
                DOTween.ToAlpha(() => TextSavedColor.color,
                    color => TextSavedColor.color = color, 0f, 0.5f
                );
        }));

        //コンティニュー確認
        StartTween.Insert(5f, DOVirtual.DelayedCall(6f, () => {
            DOTween.ToAlpha(() => ContinueCheckColor[0].color,
                color => ContinueCheckColor[0].color = color, 1f, 1f
            );
            DOTween.ToAlpha(() => ContinueCheckColor[1].color,
               color => ContinueCheckColor[1].color = color, 1f, 1f
           );
            ContinueCheck.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.OutQuint);
        }));

        StartTween.Insert(9f, DOVirtual.DelayedCall(6f, () => {
            Button.SetActive(true);
        }));
        StartTween.Insert(9f, DOVirtual.DelayedCall(8f, () => {
            CountdownFlag = true;
        }));

        //ボタン
        StartTween.Insert(9f,
            DOVirtual.DelayedCall(6f, ()=>
            {
                Button.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuint);
            }
           )
        );

        //BGM 設定
        string path = "Audio/endscreen.wav";
        StartCoroutine(GetAudioClip(path));

        if (!DebugMode)
        {
            //SceneChange 
            SceneChange = GameObject.Find("SceneChangeEnd");

            float timeDelay = 0.5f;
            GameObject[] changer = new GameObject[4];
            RectTransform[] rectTran = new RectTransform[4];

            for (int i = 0; i < 4; i++)
            {
                changer[i] = GameObject.Find("SceneChangeEnd").transform.Find(i.ToString()).gameObject;
                rectTran[i] = changer[i].GetComponent<RectTransform>();
            }

            sceneChangeTween = DOTween.Sequence();
            for (int i = 0; i < 4; i++)
            {
                sceneChangeTween.Insert(1 + 0.25f * (3 - i),
                    rectTran[i].DOLocalMoveX(-1280, timeDelay).SetEase(Ease.OutQuint)
                );
            }

            sceneChangeTween.Join(
                DOVirtual.DelayedCall(6f, () =>
                {
                    Destroy(SceneChange);
                })
            );
        }
    }

    void Start()
    {
        if (!DebugMode)
        {
            sceneChangeTween.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CountdownFlag && !flag)
        {
            flag = true;
            Countdown = DOTween.Sequence()
                .Prepend(
                    DOTween.To(() => CountStartNumber, (n) => CountStartNumber = n, 0, 18)
                      .OnUpdate(() => ContinueCheckColor[1].text = CountStartNumber.ToString())
                      .OnComplete(() =>
                      {
                          Continue(false);
                          ButtonPressed = true;
                      })
                      .SetEase(Ease.Linear)
              );
        }

        if (!ButtonPressed && CountdownFlag)
        {
            if (TimeComponent.isKeyPressed(0) || TimeComponent.isKeyPressed(1))
            {
                Continue(true);
                ButtonPressed = true;
            }
            else if (TimeComponent.isKeyPressed(3) || TimeComponent.isKeyPressed(4))
            {
                Continue(false);
                ButtonPressed = true;
            }
        }
    }

    private void Continue(bool select)
    {
        DataHolder.PlayedTime = 0;
        audioSource.DOFade(0, 1);
        Countdown.Kill();

        DOTween.ToAlpha(() => ContinueCheckColor[0].color,
                color => ContinueCheckColor[0].color = color, 0f, 0.5f
            );
        DOTween.ToAlpha(() => ContinueCheckColor[1].color,
           color => ContinueCheckColor[1].color = color, 0f, 0.5f
       );
        Button.transform.DOLocalMoveY(-100f, 0.5f).SetEase(Ease.OutQuint);
        DOVirtual.DelayedCall(0.5f, () => Button.SetActive(false));

        if (select) //コンティニューする
        {
            DOVirtual.DelayedCall(2f, () => SceneChangeOut());
        }
        else //コンティニューしない
        {
            DataHolder.Logout();
            DOVirtual.DelayedCall(1f, () =>
             {
                 DOTween.ToAlpha(() => ThankyouMessage.color,
                     color => ThankyouMessage.color = color, 1f, 1f
                 );
             });
            DOVirtual.DelayedCall(3.5f, () =>
            {
                DOTween.ToAlpha(() => ThankyouMessage.color,
                    color => ThankyouMessage.color = color, 0f, 0.5f
                );
            });
            DOVirtual.DelayedCall(5f, () =>
            {
                SceneManager.LoadScene("Title");
            });
        }
    }

    private void SceneChangeOut()
    {
        float timeDelay = 0.5f;
        GameObject[] changer = new GameObject[4];
        RectTransform[] rectTran = new RectTransform[4];

        for (int i = 0; i < 4; i++)
        {
            changer[i] = SceneChangeEnd.transform.Find(i.ToString()).gameObject;
            rectTran[i] = changer[i].GetComponent<RectTransform>();
        }
        changer[0].transform.parent.gameObject.SetActive(true);

        DontDestroyOnLoad(SceneChangeEnd);

        Sequence sceneChangeTween = DOTween.Sequence();
        for (int i = 0; i < 4; i++)
        {
            changer[i].SetActive(true);
            sceneChangeTween.Insert(1 + 0.25f * i,
                rectTran[i].DOLocalMoveX(0, timeDelay).SetEase(Ease.OutQuint)
            );
        }

        sceneChangeTween.Join(
            DOVirtual.DelayedCall(3f, () =>
            {
                DataHolder.TemporaryGameObject = SceneChangeEnd;
                SceneManager.LoadScene("MusicSelect");
            })
        );

        sceneChangeTween.Play();
    }

    IEnumerator GetAudioClip(string path)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + System.IO.Directory.GetCurrentDirectory() + "/" + path, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
            audioSource.clip.name = "BGM";
            audioSource.Play();
        }
    }
}
