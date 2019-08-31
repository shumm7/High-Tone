using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;

public class MainMusicSelect : MonoBehaviour
{
    public static string[] SortedMusicList;
    private int SortMode = 0;
    public int Difficulty = 0;
    private bool flag = false;

    public static int SelectedFrame = 0;
    private bool isScreenScrolling = false;

    //Display
    public GameObject MainDisplay;
    public GameObject SettingDisplay;

    //Frame
    private int MaximumFrameAmount;
    private int SelectedFrameNumber;
    public GameObject FramePrefab;
    public GameObject FrameParentObject;
    public GameObject FrameParentSettingDisplayObject;

    //UI
    public Text MaxScoreUI;
    public Text MaxComboUI;
    public Text CategoryUI;
    public GameObject ButtonParent;
    public int DisplayMode = 0; //0曲選択 1ゲーム設定
    private int AllSettings = 4;

    //Audio
    public AudioSource MusicPreview;
    private float fadeTime = 1;
    private Sequence seq;

    //List
    private MusicDataLoader.MusicList[] musicList;
    private MusicDataLoader.Category[] categoryList;

    void Start()
    {
        flag = true;
        musicList = GetComponent<MusicDataLoader>().getMusicList().music;
        categoryList = GetComponent<MusicDataLoader>().getMusicList().category;

        SortedMusicList = GetSortedMusicID(SortMode, Difficulty);
        GenerateFrame(SortedMusicList, 620);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            FrameFade(SortedMusicList.Length, true, 0.25f, false);
        });
        DOVirtual.DelayedCall(0.75f, () =>
        {
            seq = DOTween.Sequence();
            PlayMusicPreview(0);
            setUI(0, 0);
            flag = false;
        });
    }

    void Update()
    {
        if (!flag && DisplayMode==0)
        {
            if (Input.GetKeyDown(KeyCode.Q)) //左
            {
                SelectedFrame--;
                SelectedFrame = Range(SelectedFrame, 0, SortedMusicList.Length - 1);
                PlayMusicPreview(SelectedFrame);
                FrameScroll(SortedMusicList.Length, SelectedFrame, 0.2f);
            }
            else if (Input.GetKeyDown(KeyCode.W)) //右
            {
                SelectedFrame++;
                SelectedFrame = Range(SelectedFrame, 0, SortedMusicList.Length - 1);
                PlayMusicPreview(SelectedFrame);
                FrameScroll(SortedMusicList.Length, SelectedFrame, 0.2f);
            }
            else if (Input.GetKeyUp(KeyCode.E)) //決定
            {
                flag = true;
                FrameFade(SortedMusicList.Length, false, 0.5f, true);
                DisplayMode = 1;

                SettingDisplay.SetActive(true);
                GameObject frame = FrameParentSettingDisplayObject.transform.Find("0").gameObject;
                frame.GetComponent<MusicFrameComponent>().SetMusicData(Difficulty, SortedMusicList[SelectedFrame]);
                frame.transform.Find("Title Text Mask").transform.Find("Title").GetComponent<TextScroll>().Setup();
                frame.transform.Find("Credits Text Mask").transform.Find("Credits").GetComponent<TextScroll>().Setup();

                DataHolder.NextMusicID = SortedMusicList[SelectedFrame];
                DataHolder.TemporaryIntNumber = SelectedFrame;
                SelectedFrame = 0;
                FrameScroll(AllSettings, SelectedFrame, 0);

                DOVirtual.DelayedCall(1f, () =>
                {
                    setUI(SelectedFrame, DisplayMode);
                    FrameFade(SortedMusicList.Length, true, 0.5f, true);
                });
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    flag = false;
                });
            }
            else if (Input.GetKeyUp(KeyCode.R)) //難易度
            {
                Difficulty = Range(++Difficulty, 0, 4);
                flag = true;
                float time = 0.2f;

                FrameFade(SortedMusicList.Length, false, time, false);
                Sequence Fade = DOTween.Sequence();

                Fade.Join(MusicPreview.DOFade(0, time));
                Fade.Insert(time,
                    DOVirtual.DelayedCall(0, () =>
                    {
                        MusicPreview.Stop();
                        KillFrame(SortedMusicList.Length);
                        while(GetSortedMusicID(SortMode, Difficulty).Length == 0)
                        {
                            Difficulty = Range(++Difficulty, 0, 4);
                        }
                        SortedMusicList = GetSortedMusicID(SortMode, Difficulty);
                        GenerateFrame(SortedMusicList, 620f);
                    })
                );
                Fade.Insert(time * 2,
                    DOVirtual.DelayedCall(0, () =>
                    {
                        FrameFade(SortedMusicList.Length, true, time, false);
                        SelectedFrame = 0;
                        PlayMusicPreview(0);
                    })
                );
                Fade.Insert(time * 3,
                    DOVirtual.DelayedCall(0, () =>
                    {
                        flag = false;
                        Fade.Kill();
                    })
                );

                Fade.Play();
            }
            else if (Input.GetKeyUp(KeyCode.T)) //カテゴリ
            {
                SortMode = Range(++SortMode, 0, 2);
                flag = true;
                float time = 0.25f;

                FrameFade(SortedMusicList.Length, false, time, false);
                Sequence Fade = DOTween.Sequence();

                Fade.Join(MusicPreview.DOFade(0, time));
                Fade.Insert(time,
                    DOVirtual.DelayedCall(0, () =>
                    {
                        MusicPreview.Stop();
                        KillFrame(SortedMusicList.Length);
                        SortedMusicList = GetSortedMusicID(SortMode, Difficulty);
                        GenerateFrame(SortedMusicList, 620f);
                    })
                );
                Fade.Insert(time * 2,
                    DOVirtual.DelayedCall(0, () =>
                    {
                        FrameFade(SortedMusicList.Length, true, time, false);
                        SelectedFrame = 0;
                        setUI(SelectedFrame, DisplayMode);
                        PlayMusicPreview(0);
                    })
                );
                Fade.Insert(time * 3,
                    DOVirtual.DelayedCall(0, () =>
                    {
                        flag = false;
                        Fade.Kill();
                    })
                );

                Fade.Play();
            }
        }
        else if(!flag && DisplayMode == 1)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectedFrame--;
                SelectedFrame = Range(SelectedFrame, 0, AllSettings - 1);
                setUI(SelectedFrame, DisplayMode);

                FrameScroll(AllSettings, SelectedFrame, 0.2f);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                SelectedFrame++;
                SelectedFrame = Range(SelectedFrame, 0, AllSettings - 1);
                setUI(SelectedFrame, DisplayMode);

                FrameScroll(AllSettings, SelectedFrame, 0.2f);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                switch (SelectedFrame)
                {
                    case 0: //決定
                        break;
                    case 1: //スピードダウン
                        break;
                    case 2: //Music ダウン
                        break;
                    case 3: //SE ダウン
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                switch (SelectedFrame)
                {
                    case 0: //難易度アップ
                        break;
                    case 1: //スピードアップ
                        break;
                    case 2: //Music アップ
                        break;
                    case 3: //SEアップ
                        break;
                }
            }
            else if(Input.GetKeyDown(KeyCode.T))
            {
                flag = true;
                FrameFade(SortedMusicList.Length, false, 0.5f, true);
                DisplayMode = 0;
                SelectedFrame = DataHolder.TemporaryIntNumber;

                DOVirtual.DelayedCall(1f, () =>
                {
                    setUI(SelectedFrame, DisplayMode);
                    FrameFade(SortedMusicList.Length, true, 0.5f, true);
                });
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    flag = false;
                });
            }
        }
    }

    public void setUI(int selection, int mode)
    {
        if (mode == 0)
        {
            string sortmodeName = null;

            switch (SortMode)
            {
                case Sortmode.Lexicon:
                    sortmodeName = "曲名順";
                    break;
                case Sortmode.Category:
                    sortmodeName = "カテゴリ";
                    break;
                case Sortmode.ListJson:
                    sortmodeName = "登録順";
                    break;
            }
            GetButtonUI(3).text = "決定";
            GetButtonUI(4).text = "難易度  ▲";
            GetButtonUI(5).text = sortmodeName;

            int num = FindMusicNumber(SortedMusicList[selection]);
            CategoryUI.text = GetCategoryName(musicList[num].category);
            ScoreController.Score temp = ScoreController.LoadScore(DataHolder.UserID, musicList[num].id);
            MaxScoreUI.text = temp.MaxScore.ToString();
            MaxComboUI.text = temp.MaxCombo.ToString();
        }
        else if (mode == 1)
        {
            switch (selection)
            {
                case 0:
                    GetButtonUI(3).text = "決定";
                    GetButtonUI(4).text = "難易度  ▲";
                    GetButtonUI(5).text = "戻る";
                    break;
                case 1:
                case 2:
                case 3:
                    GetButtonUI(3).text = "設定  ▼";
                    GetButtonUI(4).text = "設定  ▲";
                    GetButtonUI(5).text = "戻る";
                    break;
            }
        }
    }

    private void FrameScroll(int MaxFrameAmount, int selection, float time)
    {
            Sequence sequence = DOTween.Sequence();
            isScreenScrolling = true;

            for (int i = 0; i < MaxFrameAmount; i++)
            {
                RectTransform rectTran = null;
                if (DisplayMode == 0)
                {
                    rectTran = FrameParentObject.transform.Find(i.ToString()).GetComponent<RectTransform>();
                }
                else if (DisplayMode == 1)
                {
                    rectTran = FrameParentSettingDisplayObject.transform.Find(i.ToString()).GetComponent<RectTransform>();
                }

            sequence.Join(rectTran.DOLocalMove(new Vector3(440f * (float)(i - selection), 30f, 0), time));
                if (i != selection) {
                    sequence.Join(
                        rectTran.DOScale(new Vector3(0.6f, 0.6f, 1f), time).SetEase(Ease.InOutQuart)
                    );
                }
                else
                {
                    sequence.Join(
                        rectTran.DOScale(new Vector3(0.9f, 0.9f, 1f), time).SetEase(Ease.InOutQuart)
                    );
                }

                sequence.Join(
                    DOVirtual.DelayedCall(time, () =>
                    {
                        isScreenScrolling = false;
                    })
                );

            }

        setUI(selection, DisplayMode);
    }

    private void FrameFade(int MaxFrameAmount, bool fadeIn, float time, bool UIfade)
    {
        Sequence fade = DOTween.Sequence();

        if (!fadeIn)
        { //画面外へ
            if (UIfade)
            {
                if (DisplayMode == 0)
                {
                    RectTransform rectTran = MainDisplay.GetComponent<RectTransform>();
                    Vector3 pos = rectTran.localPosition;
                    pos.y += 700;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.InExpo));
                    fade.Join(
                        DOVirtual.DelayedCall(time, () => {
                            MainDisplay.SetActive(false);
                        })
                    );
                }else if (DisplayMode == 1)
                {
                    RectTransform rectTran = SettingDisplay.GetComponent<RectTransform>();
                    Vector3 pos = rectTran.localPosition;
                    pos.y += 700;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.InExpo));
                    fade.Join(
                        DOVirtual.DelayedCall(time, () => {
                            SettingDisplay.SetActive(false);
                        })
                    );
                }
            }
            else
            {
                for (int i = 0; i < MaxFrameAmount; i++)
                {
                    RectTransform rectTran = FrameParentObject.transform.Find(i.ToString()).GetComponent<RectTransform>();
                    Vector3 pos = rectTran.localPosition;
                    pos.y += 620;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.InExpo));
                }
            }
        }
        else //画面内へ
        {
            if (UIfade)
            {
                if (DisplayMode == 0)
                {
                    MainDisplay.SetActive(true);
                    RectTransform rectTran = MainDisplay.GetComponent<RectTransform>();
                    Vector3 pos = rectTran.localPosition;
                    pos.y -= 700;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.OutExpo));
                }
                else if (DisplayMode == 1)
                {
                    SettingDisplay.SetActive(true);
                    RectTransform rectTran = SettingDisplay.GetComponent<RectTransform>();
                    Vector3 pos = rectTran.localPosition;
                    pos.y -= 700;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.OutExpo));
                }
            }
            else
            {
                for (int i = 0; i < MaxFrameAmount; i++)
                {
                    RectTransform rectTran = FrameParentObject.transform.Find(i.ToString()).GetComponent<RectTransform>();
                    Vector3 pos = rectTran.localPosition;
                    pos.y -= 620;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.OutExpo));
                }
            }
        }

        fade.Play();
    }

    private int Range(int val, int min, int max)
    {
        if (val > max)
            val = min;
        if (val < min)
            val = max;

        return val;
    }

    private void GenerateFrame(string[] musicList, float posYoffset)
    {
        for (int i = 0; i < musicList.Length; i++)
        {
            GameObject cloned = Instantiate(FramePrefab, Vector3.zero, Quaternion.identity, FrameParentObject.transform);
            cloned.name = i.ToString();
            cloned.transform.localPosition = new Vector3(440f * (float)i, 30f + posYoffset, 0);
            cloned.GetComponent<MusicFrameComponent>().SetMusicData(Difficulty, musicList[i]);
            cloned.transform.Find("Title Text Mask").transform.Find("Title").GetComponent<TextScroll>().Setup();
            cloned.transform.Find("Credits Text Mask").transform.Find("Credits").GetComponent<TextScroll>().Setup();


            if (i == SelectedFrameNumber)
            {
                cloned.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            }
            else
            {
                cloned.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
        }
    }

    private void KillFrame(int MaxFrameAmount)
    {
        for(int i=0; i<MaxFrameAmount; i++)
        {
            GameObject frame = FrameParentObject.transform.Find(i.ToString()).gameObject;
            frame.transform.Find("Title Text Mask").transform.Find("Title").gameObject.GetComponent<TextScroll>().KillSequence();
            Destroy( frame );
        }
    }

    private string[] GetSortedMusicID(int sortmode, int difficulty)
    {
        int MusicAmount = musicList.Length;

        MusicDataLoader.MusicList[] TempMusicList = new MusicDataLoader.MusicList[0];

        if (difficulty == -1) //難易度関係なくソート
        {
            for (int i = 0; i < MusicAmount; i++)
            {
                TempMusicList = TempMusicList.Concat(new MusicDataLoader.MusicList[] { musicList[i] }).ToArray();
            }
        }
        else if(difficulty >= 0 && difficulty < 5) //指定難易度の譜面がある楽曲のみでソート
        {
            for (int i = 0; i < MusicAmount; i++)
            {
                if (GetComponent<MusicDataLoader>().getMusicProperty(musicList[i].id).level[difficulty] != -1)
                    TempMusicList = TempMusicList.Concat(new MusicDataLoader.MusicList[] { musicList[i] }).ToArray();

            }
        }


        //JSONデータに記載順
        if(sortmode == Sortmode.ListJson)
        {
            string[] ret = new string[TempMusicList.Length];

            for (int i = 0; i < TempMusicList.Length; i++)
                ret[i] = TempMusicList[i].id;

            return ret;
        }

        //辞書順（あいうえお順）
        else if(sortmode == Sortmode.Lexicon)
        {
            string[] ret = new string[TempMusicList.Length];

            for (int i = 0; i < TempMusicList.Length; i++)
                ret[i] = GetComponent<MusicDataLoader>().getMusicProperty(TempMusicList[i].id).music + ":MusicID:" + TempMusicList[i].id;
            ret = ret.OrderBy(c => c, new NaturalComparer()).ToArray();

            string[] del = { ":MusicID:" };
            for (int i = 0; i < TempMusicList.Length; i++)
                ret[i] = ret[i].Split(del, StringSplitOptions.None)[1];

            return ret;
        }

        //カテゴリ順（各カテゴリ内は辞書順）
        else if(sortmode == Sortmode.Category)
        {
            string[] ReturnListCat = new string[0];

            for(int i=0; i<categoryList.Length; i++)
            {
                string[] temp = new string[0];

                for (int j = 0; j < TempMusicList.Length; j++)
                {
                    if(TempMusicList[j].category == categoryList[i].id)
                    {
                        temp = temp.Concat( new string[] { TempMusicList[j].id} ).ToArray();
                    }
                }

                for (int j = 0; j < temp.Length; j++)
                    temp[j] = GetComponent<MusicDataLoader>().getMusicProperty(temp[j]).music + ":MusicID:" + temp[j];

                temp = temp.OrderBy(c => c, new NaturalComparer()).ToArray();

                string[] del = { ":MusicID:" };
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Split(del, StringSplitOptions.None)[1];
                }

                ReturnListCat = ReturnListCat.Concat(temp).ToArray();
            }

            return ReturnListCat;
        }
        else
        {
            return null;
        }
    }

    private int FindMusicNumber(string id)
    {
        for(int i=0; i < musicList.Length; i++)
        {
            if (musicList[i].id == id)
            {
                return i;
            }
        }

        return -1;
    }

    private string GetCategoryName(string category)
    {
        for(int i=0; i< categoryList.Length; i++)
        {
            if (categoryList[i].id == category)
                return categoryList[i].name;
        }

        return null;
    }

    private void PlayMusicPreview(int selection)
    {
        string id = SortedMusicList[selection];
        MusicDataLoader.MusicProperty list = GetComponent<MusicDataLoader>().getMusicProperty(id);
        float starttime = list.preview.start;
        float endtime = list.preview.end;

        MusicPreview.volume = 0;

        StartCoroutine(GetAudioClip(id, starttime));

        seq.Kill();
        seq = DOTween.Sequence();
        seq.Join(
            MusicPreview.DOFade(1, fadeTime)
        );
        seq.Insert(endtime - starttime,
            MusicPreview.DOFade(0, fadeTime)
        );
        seq.Insert(endtime - starttime + fadeTime,
            DOVirtual.DelayedCall(0, () => {
                MusicPreview.Stop();
                PlayMusicPreview(selection);
            })
        );

        seq.Play();
    }

    private Text GetButtonUI(int num)
    {
        return ButtonParent.transform.Find("Button" + num.ToString()).Find("Text").GetComponent<Text>();
    }

    private float GetdB(float Gain)
    {
        float dB = (float)(20f * Math.Log(Gain * 0.01f, 10));
        if (Gain == 0)
            dB = -80;
        return dB;
    }

    private float GetGaindB(float dB)
    {
        return (float)Math.Pow(10, (double)(dB / 20f));
    }

    IEnumerator GetAudioClip(string id, float starttime)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + Directory.GetCurrentDirectory() + "/Music/" + id + "/music.wav", AudioType.WAV))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            MusicPreview.clip = DownloadHandlerAudioClip.GetContent(uwr);
            MusicPreview.clip.name = id;
            MusicPreview.time = starttime;
            MusicPreview.Play();

        }
    }

    private class Sortmode
    {
        public const int Lexicon = 0;
        public const int Category = 1;
        public const int ListJson = 2;
    }

}
