using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    bool DebugMode;
    public string MusicID;
    bool flag;
    Sequence sequence;
    Sequence sceneChangeTween;

    public Transform Background;
    public Transform Panel;
    public Text OldHighscore;
    public Text OldMaxcombo;
    public Transform Artwork;
    public Image ArtworkImage;
    public Transform Result;
    public Transform[] Judgement = new Transform[5];
    public Transform Score;
    public Transform NewRecord;
    public Transform Rank;
    public RawImage RankImage;
    public RawImage PressButton;
    public Texture[] RankImageSoruce = new Texture[4];
    GameObject SceneChange;
    public AudioSource audioSource;
    public AudioSource ResultBGM;
    public GameObject SceneChangeEnd;

    string[] DifficultyName = new string[5] {"EASY", "NORMAL", "HARD", "MASTER", "EXTRA"};
    int ScoreValue = 0;
    int ComboValue = 0;
    int MaximumScore = 1;
    int MaximumCombo = 1;
    float ScorePercentage = 0;
    float ComboPercentage = 0;
    int Difficulty = 0;
    string UserID = "Guest";
    int CurrentHighscore = 0;
    int CurrentMaxCombo = 0;

    private void Awake()
    {
        if(MusicID == "")
        {
            DebugMode = false;
        }
        else
        {
            DebugMode = true;
        }

        flag = false;
        sequence = DOTween.Sequence();
        if(!DebugMode)
            SceneChange = GameObject.Find("SceneChangeEnd");

        //初期座標設定
        Background.localPosition = new Vector3(-1100f, Background.localPosition.y, Background.localPosition.z);
        Panel.localPosition = new Vector3(-950f, Panel.localPosition.y, Panel.localPosition.z);
        Artwork.localPosition = new Vector3(Artwork.localPosition.x, 180f, Artwork.localPosition.z);
        ArtworkImage.color = new Color(1f, 1f, 1f, 0f);
        Result.localPosition = new Vector3(-470f, Result.localPosition.y, Result.localPosition.z);
        Result.gameObject.SetActive(false);
        for(int i=0; i<5; i++)
        {
            Judgement[i].gameObject.SetActive(false);
            Judgement[i].localPosition = new Vector3(-300f - i*20f, Judgement[i].localPosition.y, Judgement[i].localPosition.z);
        }
        Score.localPosition = new Vector3(-600f, Score.localPosition.y, Score.localPosition.z);
        Score.gameObject.SetActive(false);
        NewRecord.gameObject.SetActive(false);
        Rank.localScale = new Vector3(0f, 0f, 1f);
        RankImage.color = new Color(1f, 1f, 1f, 0f);
        PressButton.color = new Color(1f, 1f, 1f, 0f);

        //BGM 設定
        string path = "Audio/result.wav";
        StartCoroutine(GetAudioClip(path));
        

        //値設定
        if (!DebugMode)
        {
            UserID = DataHolder.UserID;
            ScoreValue = DataHolder.Score;
            ComboValue = DataHolder.Combo;
            MaximumScore = DataHolder.MaximumScore;
            MaximumCombo = DataHolder.MaximumCombo;
            Difficulty = DataHolder.Difficulty;
            MusicID = DataHolder.NextMusicID;

            ScoreController.Score sc = ScoreController.LoadScore(UserID, Difficulty, MusicID);
            CurrentHighscore = sc.MaxScore;
            CurrentMaxCombo = sc.MaxCombo;

            OldHighscore.text = LintNumber(7, sc.MaxScore);
            OldMaxcombo.text = LintNumber(4, sc.MaxCombo);

            for (int i = 0; i < 5; i++)
            {
                Judgement[i].Find("Text").GetComponent<TextMeshProUGUI>().text = LintNumber(4, DataHolder.JudgementAmount[i]);
            }
        }
        MusicDataLoader.MusicProperty MusicData = GetComponent<MusicDataLoader>().getMusicProperty(MusicID);
        if (!DebugMode)
        {
            Panel.Find("Level").Find("Text").GetComponent<Text>().text = MusicData.level[Difficulty].ToString();
            Panel.Find("Difficulty").GetComponent<Text>().text = DifficultyName[Difficulty];
            ScorePercentage = ((float)ScoreValue / (float)MaximumScore) * 100f;
            ComboPercentage = ((float)ComboValue / (float)MaximumCombo) * 100f;
        }
        Panel.Find("Title").GetComponent<Text>().text = MusicData.music;
        Panel.Find("Author").GetComponent<Text>().text = MusicData.credits;
        
        if(ScorePercentage >= 90)
        {
            RankImage.texture = RankImageSoruce[0];
        }
        else if(ScorePercentage < 90 && ScorePercentage >= 80)
        {
            RankImage.texture = RankImageSoruce[1];
        }
        else if (ScorePercentage < 80 && ScorePercentage >= 70)
        {
            RankImage.texture = RankImageSoruce[2];
        }
        else if (ScorePercentage < 70)
        {
            RankImage.texture = RankImageSoruce[3];
        }

        //Artwork
        byte[] bytes = System.IO.File.ReadAllBytes("Music/" + MusicID + "/image.png");
        Texture2D texture = new Texture2D(55, 55, TextureFormat.ARGB4444, false);
        texture.LoadImage(bytes);
        texture.Compress(false);
        ArtworkImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.width), new Vector2(0.5f, 0.5f));

        //Background
        sequence.Insert(2f,
            Background.DOLocalMoveX(-410f, 0.5f).SetEase(Ease.OutQuint)
        );
        //Panel
        sequence.Insert(2.5f,
            Panel.DOLocalMoveX(-490f, 0.5f).SetEase(Ease.OutQuint)
        );
        //Artwork
        sequence.Insert(2.5f,
            Artwork.DOLocalMoveY(125f, 0.5f).SetEase(Ease.OutQuint)
        );
        sequence.Insert(2.5f,
            DOTween.ToAlpha(() => ArtworkImage.color,
                color => ArtworkImage.color = color, 1f, 0.5f
            ).SetEase(Ease.OutQuint)
        );
        //Result
        sequence.Insert(2.5f,
            DOVirtual.DelayedCall(0f, () =>
                Result.gameObject.SetActive(true)
            )
        );
        sequence.Insert(2.5f,
            Result.DOLocalMoveX(250f, 0.5f).SetEase(Ease.OutQuint)
        );

        //Judgement
        sequence.Insert(2.5f,
           DOVirtual.DelayedCall(0f, () =>
                Judgement[0].gameObject.SetActive(true)
           )
        );
        sequence.Insert(2.5f,
           DOVirtual.DelayedCall(0f, () =>
                Judgement[1].gameObject.SetActive(true)
           )
        );
        sequence.Insert(2.5f,
           DOVirtual.DelayedCall(0f, () =>
                Judgement[2].gameObject.SetActive(true)
           )
        );
        sequence.Insert(2.5f,
           DOVirtual.DelayedCall(0f, () =>
                Judgement[3].gameObject.SetActive(true)
           )
        );
        sequence.Insert(2.5f,
           DOVirtual.DelayedCall(0f, () =>
                Judgement[4].gameObject.SetActive(true)
           )
        );
        for (int i=0; i < 5; i++)
        {
            sequence.Insert(3f + 0.25f * i,
                Judgement[i].DOLocalMoveX(180f - i*20f, 0.5f).SetEase(Ease.OutQuint)
            );
        }

        //Score
        sequence.Insert(2.5f,
            DOVirtual.DelayedCall(0f, () =>
                Score.gameObject.SetActive(true)
            )
        );
        sequence.Insert(4.5f,
            Score.DOLocalMoveX(250f, 1f).SetEase(Ease.OutQuint)
        );

        int ScoreNumber = 0;
        sequence.Insert(5f,
                DOTween.To(() => ScoreNumber, (n) => ScoreNumber = n, ScoreValue, 1.5f)
            .OnUpdate(() => Score.Find("Score").GetComponent<TextMeshProUGUI>().text = LintNumber(7, ScoreNumber))
       );
        sequence.Insert(5f,
            DOVirtual.DelayedCall(0f, () =>
                audioSource.PlayOneShot(audioSource.clip)
            )
        );
        sequence.Insert(5.5f,
            DOVirtual.DelayedCall(0f, () => {
                Score.Find("ScorePercentage").GetComponent<Text>().text = ((int)ScorePercentage).ToString() + "％";
                if (CurrentHighscore < ScoreValue)
                {
                    NewRecord.gameObject.SetActive(true);
                }
            }
            )
        );

        int ComboNumber = 0;
        sequence.Insert(5.25f,
                DOTween.To(() => ComboNumber, (n) =>  ComboNumber = n, ComboValue, 1f)
            .OnUpdate(() => Score.Find("Combo").GetComponent<TextMeshProUGUI>().text = LintNumber(4, ComboNumber))
       );
        sequence.Insert(5.5f,
            DOVirtual.DelayedCall(0f, () => {
                Score.Find("ComboPercentage").GetComponent<Text>().text = ((int)ComboPercentage).ToString() + "％";
                }
            )
        );

        //Rank
        sequence.Insert(5.5f,
            DOTween.ToAlpha(() => RankImage.color,
                color => RankImage.color = color, 1f, 0.5f
            ).SetEase(Ease.OutQuint)
        );
        sequence.Insert(5.5f,
            Rank.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.OutQuint)
        );

        //Finish
        sequence.Insert(7f,
            DOTween.ToAlpha(() => PressButton.color,
                color => PressButton.color = color, 1f, 0.5f
            ).SetEase(Ease.OutQuint)
        );
        sequence.Insert(7f,
           DOVirtual.DelayedCall(0f, () =>
                flag = true
           )
        );

        if (!DebugMode)
        {
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
                    sequence.Play();
                })
            );
            
        }
    }

    void Start()
    {
        sceneChangeTween.Play();
    }

    private void Update()
    {
        if (flag)
        {
            if(Input.anyKey == true)
            {
                ResultBGM.DOFade(0, 1);

                ScoreController.SaveScore(UserID, Difficulty, MusicID, ScoreValue, ComboValue);

                flag = false;
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
                        SceneManager.LoadScene("Music Select");
                    })
                );

                sceneChangeTween.Play();
            }
        }
    }

    private string LintNumber(int Length, int number)
    {
        if (number >= Mathf.Pow(10, Length))
        {
            return ((int)Mathf.Pow(10, Length)-1).ToString();
        }
        else
        {
            int amount = Length - number.ToString().Length;
            string res = "";
            for(int i=0; i<amount; i++)
            {
                res += "0";
            }
            return res + number.ToString();
        }
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

            ResultBGM.clip = DownloadHandlerAudioClip.GetContent(uwr);
            ResultBGM.clip.name = "BGM";
            ResultBGM.Play();
        }
    }
}
