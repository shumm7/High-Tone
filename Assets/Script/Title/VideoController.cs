using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class VideoController : MonoBehaviour
{
    private int maxVideoAmount = 0;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public GameObject SceneChange;
    public string nextScene;
    private int video = 0;
    bool flag = false;
    bool KeyPressedFlag = false;

    void Start()
    {
        while (GetComponent<MusicDataLoader>().checkExist("Audio/TitleVideos/"+maxVideoAmount.ToString()+".mp4"))
        {
            maxVideoAmount++;
        }

        if (maxVideoAmount > 0)
        {
            videoPlayer.url = "Audio/TitleVideos/" + video.ToString() + ".mp4";
            videoPlayer.source = VideoSource.Url;
            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.Play();
        }
    }

    void Update()
    {
        if (Input.anyKeyDown && !KeyPressedFlag)
        {
            KeyPressedFlag = true;
            AudioSource seAudioSource = GetComponent<AudioSource>();
            seAudioSource.PlayOneShot(seAudioSource.clip);

            Sequence seq = DOTween.Sequence();

            SceneChange.SetActive(true);
            DontDestroyOnLoad(SceneChange);

            Sequence sceneChangeTween = DOTween.Sequence();


            float timeDelay = 0.5f;
            GameObject[] changer = new GameObject[4];
            RectTransform[] rectTran = new RectTransform[4];
            for (int i = 0; i < 4; i++)
            {
                changer[i] = SceneChange.transform.Find(i.ToString()).gameObject;
                rectTran[i] = changer[i].GetComponent<RectTransform>();
            }
            for (int i = 0; i < 4; i++)
            {
                changer[i].SetActive(true);
                sceneChangeTween.Insert(1 + 0.25f * i,
                    rectTran[i].DOLocalMoveX(0, timeDelay).SetEase(Ease.OutQuint)
                );
            }
            seq.Join(
                audioSource.DOFade(0, 1)
            );
            seq.Join(
                DOVirtual.DelayedCall(5f, () =>
                {
                    DataHolder.TemporaryGameObject = SceneChange;
                    SceneManager.LoadScene(nextScene);
                })
            );

            seq.Play();

            DataHolder.PlayedTime = 0;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (!flag)
        {
            flag = true;
            vp.Stop();
            vp.isLooping = false;

            video++;
            if (video >= maxVideoAmount)
            {
                video = 0;
            }

            vp.url = "Audio/TitleVideos/" + video.ToString() + ".mp4";
            vp.source = VideoSource.Url;
            vp.Prepare();
            vp.loopPointReached += OnVideoFinished;
            vp.time = 0;

            vp.Play();
            vp.isLooping = true;

            DOVirtual.DelayedCall(0.1f, () =>
            {
                flag = false;
            });
        }
    }
}
