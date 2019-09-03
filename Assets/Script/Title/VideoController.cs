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
    private int video = 0;
    bool flag = false;
    bool KeyPressedFlag = false;

    void Start()
    {
        while (GetComponent<MusicDataLoader>().checkExist("Music/TitleVideos/"+maxVideoAmount.ToString()+".mp4"))
        {
            maxVideoAmount++;
        }

        if (maxVideoAmount > 0)
        {
            videoPlayer.url = "Music/TitleVideos/" + video.ToString() + ".mp4";
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
            RectTransform rectTran = SceneChange.transform.Find("Panel").GetComponent<RectTransform>();
            seq.Insert(1, 
                rectTran.DOLocalMoveX(0, 1f).SetEase(Ease.OutQuint)
            );
            seq.Join(
                audioSource.DOFade(0, 1)
            );
            seq.Join(
                DOVirtual.DelayedCall(3f, () =>
                {
                    Destroy(SceneChange);
                    SceneManager.LoadScene("Music Select");
                })
            );

            seq.Play();
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

            Debug.Log(video);
            vp.url = "Music/TitleVideos/" + video.ToString() + ".mp4";
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
