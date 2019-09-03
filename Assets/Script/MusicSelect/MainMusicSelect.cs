using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMusicSelect : MonoBehaviour
{
    public static string[] SortedMusicList;
    private int SortMode = 0;
    public int Difficulty = 0;
    public static bool flag = false;

    public static int SelectedFrame = 0;
    private bool isScreenScrolling = false;
    private int tempDifficulty;

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
    public Text CategoryUI;
    public GameObject ButtonParent;
    public GameObject CategoryParant;
    public GameObject CategoryPrefab;
    public GameObject CursorFrame;
    public GameObject CursorCategory;
    public static int DisplayMode = -1; //0曲選択 1ゲーム設定 //-1 カテゴリ選択
    private int AllSettings = 4;
    private int Category = 0;

    //Audio
    public AudioMixer audioMixer;
    public AudioSource MusicPreview;
    private float fadeTime = 1;
    private Sequence seq;
    private Sequence seqCategory;
    private int musicVolumePercentage;
    private int seVolumePercentage;
    public AudioClip Enter;

    //List
    private MusicDataLoader.MusicList[] musicList;
    private MusicDataLoader.Category[] categoryList;

    //Time
    private float MaxTime = 99f;
    private double CurrentTime;
    private double StartTime;
    public Text Countdown;

    //Scene Change
    public GameObject SceneChange;


    void Start()
    {
        flag = true;
        musicList = GetComponent<MusicDataLoader>().getMusicList().music;
        categoryList = GetComponent<MusicDataLoader>().getMusicList().category;

        GenerateCategoryPlate(categoryList);
        CursorFrame.SetActive(false);
        CursorCategory.SetActive(true);

        SortedMusicList = GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id);
        GenerateFrame(SortedMusicList, 0, false);
        FrameFade(SortedMusicList.Length, false, 0, true);

        StartTime = Time.time;

        DOVirtual.DelayedCall(0.5f, () =>
        {
            FrameFade(SortedMusicList.Length, true, 0.25f, true);
        });
        DOVirtual.DelayedCall(0.75f, () =>
        {
            seq = DOTween.Sequence();
            PlayMusicPreview(SelectedFrame);
            setUI(SelectedFrame, DisplayMode);
            flag = false;
        });
    }

    void Update()
    {
        //時間
        CurrentTime = Time.time;
        int counttime = RangeNoOver((int)(MaxTime - CurrentTime + StartTime), 0, (int)MaxTime);
        if (counttime <= 10 && Countdown.text != counttime.ToString() && !flag)
        {
            Countdown.GetComponent<AudioSource>().PlayOneShot(Countdown.GetComponent<AudioSource>().clip);
        }
        Countdown.text = counttime.ToString();

        if (MaxTime - (int)(CurrentTime - StartTime) <= 0 && !flag)
        {
            if (DisplayMode == -1)
            {
                if (SortedMusicList.Length <= 0)
                {
                    DataHolder.NextMusicID = musicList[0].id;
                }
                else
                {
                    DataHolder.NextMusicID = SortedMusicList[0];
                }
            }
            else if(DisplayMode == 0)
            {
                DataHolder.NextMusicID = SortedMusicList[SelectedFrame];
            }
            flag = true;
            GameStart();
        }

        //ボタン
        if (!flag && DisplayMode == -1)
        {
            if (Input.GetKeyDown(KeyCode.Q)) //左
            {
                Category = Range(Category - 1, 0, categoryList.Length - 1);
                ScrollCategoryPlate(categoryList.Length, Category, 0.25f);

                FrameFade(SortedMusicList.Length, false, 0.25f, false);

                seqCategory.Complete();
                seqCategory.Join(
                DOVirtual.DelayedCall(0.26f, () =>
                {
                    KillFrame(SortedMusicList.Length);
                    SortedMusicList = GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id);
                    GenerateFrame(SortedMusicList, 620f, false);
                    if (SortedMusicList.Length != 0)
                    {
                        PlayMusicPreview(0);
                    }
                    else
                    {
                        PlayMusicPreview(-1);
                    }
                })
                );
                seqCategory.Join(
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    FrameFade(SortedMusicList.Length, true, 0.25f, false);
                })
                );
            }
            else if (Input.GetKeyDown(KeyCode.W)) //右
            {
                Category = Range(Category + 1, 0, categoryList.Length - 1);
                ScrollCategoryPlate(categoryList.Length, Category, 0.25f);

                FrameFade(SortedMusicList.Length, false, 0.25f, false);
                seqCategory.Complete();
                seqCategory.Join(
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    KillFrame(SortedMusicList.Length);
                    SortedMusicList = GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id);
                    GenerateFrame(SortedMusicList, 620f, false);
                    if (SortedMusicList.Length != 0)
                    {
                        PlayMusicPreview(0);
                    }
                    else
                    {
                        PlayMusicPreview(-1);
                    }
                })
                );
                seqCategory.Join(
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    FrameFade(SortedMusicList.Length, true, 0.25f, false);
                })
                );
            }
            else if (Input.GetKeyDown(KeyCode.E)) //決定
            {
                if (SortedMusicList.Length != 0) {
                    flag = true;

                    DisplayMode = 0;
                    SelectedFrame = 0;
                    FrameScroll(SortedMusicList.Length, SelectedFrame, 0.25f, true);
                    CursorFrame.SetActive(true);
                    DisplayMode = -1;

                    CategoryParant.GetComponent<RectTransform>().DOLocalMoveY(-200f, 0.5f).SetEase(Ease.InOutQuart);
                    CursorCategory.GetComponent<RectTransform>().DOLocalMoveY(-462f, 0.5f).SetEase(Ease.InOutQuart);
                    PlayMusicPreview(SelectedFrame);

                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        CategoryParant.SetActive(false);
                        CursorCategory.SetActive(false);
                        DisplayMode = 0;
                        flag = false;
                    });
                }
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
                        while (GetSortedMusicID(SortMode, Difficulty, "all").Length == 0)
                        {
                            Difficulty = Range(++Difficulty, 0, 4);
                        }
                        SortedMusicList = GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id);
                        GenerateFrame(SortedMusicList, 620f, false);
                    })
                );
                if (SortedMusicList.Length != 0)
                {
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
                else
                {
                    flag = false;
                }
            }
        }
        else if (!flag && DisplayMode==0)
        {
            if (Input.GetKeyDown(KeyCode.Q)) //左
            {
                SelectedFrame--;
                SelectedFrame = Range(SelectedFrame, 0, SortedMusicList.Length - 1);
                PlayMusicPreview(SelectedFrame);
                FrameScroll(SortedMusicList.Length, SelectedFrame, 0.2f, true);
            }
            else if (Input.GetKeyDown(KeyCode.W)) //右
            {
                SelectedFrame++;
                SelectedFrame = Range(SelectedFrame, 0, SortedMusicList.Length - 1);
                PlayMusicPreview(SelectedFrame);
                FrameScroll(SortedMusicList.Length, SelectedFrame, 0.2f, true);
            }
            else if (Input.GetKeyUp(KeyCode.E)) //決定
            {
                flag = true;
                FrameFade(SortedMusicList.Length, false, 0.5f, true);
                DisplayMode = 1;
                SettingDisplay.SetActive(true);

                //フレーム設定
                GameObject frame = FrameParentSettingDisplayObject.transform.Find("0").gameObject;
                frame.GetComponent<MusicFrameComponent>().SetMusicData(Difficulty, SortedMusicList[SelectedFrame]);
                frame.transform.Find("Title Text Mask").transform.Find("Title").GetComponent<TextScroll>().Setup();
                frame.transform.Find("Credits Text Mask").transform.Find("Credits").GetComponent<TextScroll>().Setup();

                //オプション値設定
                GetSettingFrameText(1).text = DataHolder.NoteSpeed.ToString();
                musicVolumePercentage = (int)(GetGain(DataHolder.MusicVolume) * 100f);
                GetSettingFrameText(2).text = musicVolumePercentage.ToString() + " %";
                seVolumePercentage = (int)(GetGain(DataHolder.SEVolume) * 100f);
                GetSettingFrameText(2).text = seVolumePercentage.ToString() + " %";
                tempDifficulty = Difficulty;

                //画面遷移
                DataHolder.NextMusicID = SortedMusicList[SelectedFrame];
                DataHolder.TemporaryIntNumber = SelectedFrame;
                SelectedFrame = 0;
                FrameScroll(AllSettings, SelectedFrame, 0, true);

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
                        while(GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id).Length == 0)
                        {
                            Difficulty = Range(++Difficulty, 0, 4);
                        }
                        SortedMusicList = GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id);
                        GenerateFrame(SortedMusicList, 620f, true);
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
                flag = true;
                FrameScroll(SortedMusicList.Length, SelectedFrame, 0.25f, false);
                CursorFrame.SetActive(false);
                CursorCategory.SetActive(true);

                CategoryParant.GetComponent<RectTransform>().DOLocalMoveY(0, 0.5f).SetEase(Ease.InOutQuart);
                CursorCategory.GetComponent<RectTransform>().DOLocalMoveY(-262f, 0.5f).SetEase(Ease.InOutQuart);
                CategoryParant.SetActive(true);
                setUI(Category, -1);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    DisplayMode = -1;
                    flag = false;
                });

                /* ソート切り替え
                
                SortMode = Range(++SortMode, 0, 1);
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
                        SortedMusicList = GetSortedMusicID(SortMode, Difficulty, categoryList[Category].id);
                        GenerateFrame(SortedMusicList, 620f, true);
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
                */
            }
        }
        else if(!flag && DisplayMode == 1)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectedFrame--;
                SelectedFrame = Range(SelectedFrame, 0, AllSettings - 1);
                setUI(SelectedFrame, DisplayMode);

                FrameScroll(AllSettings, SelectedFrame, 0.2f, true);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                SelectedFrame++;
                SelectedFrame = Range(SelectedFrame, 0, AllSettings - 1);
                setUI(SelectedFrame, DisplayMode);

                FrameScroll(AllSettings, SelectedFrame, 0.2f, true);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                switch (SelectedFrame)
                {
                    case 0: //決定
                        flag = true;
                        Countdown.GetComponent<AudioSource>().PlayOneShot(Enter);
                        GameStart();
                        break;
                    case 1: //スピードダウン
                        DataHolder.NoteSpeed = RangeNoOver((int)(DataHolder.NoteSpeed - 1), 5, 25);
                        GetSettingFrameText(1).text = DataHolder.NoteSpeed.ToString();
                        break;
                    case 2: //Music ダウン
                        musicVolumePercentage = RangeNoOver(musicVolumePercentage - 10, 0, 100);
                        DataHolder.MusicVolume = GetdB(musicVolumePercentage);
                        GetSettingFrameText(2).text = musicVolumePercentage.ToString() + " %";
                        audioMixer.SetFloat("Music", DataHolder.MusicVolume);
                        break;
                    case 3: //SE ダウン
                        seVolumePercentage = RangeNoOver(seVolumePercentage - 10, 0, 100);
                        DataHolder.SEVolume = GetdB(seVolumePercentage);
                        GetSettingFrameText(3).text = seVolumePercentage.ToString() + " %";
                        audioMixer.SetFloat("SE", DataHolder.SEVolume);
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                switch (SelectedFrame)
                {
                    case 0: //難易度アップ
                        tempDifficulty++;
                        FrameParentSettingDisplayObject.transform.Find("0").GetComponent<MusicFrameComponent>().SetMusicData(tempDifficulty, SortedMusicList[DataHolder.TemporaryIntNumber]);
                        break;
                    case 1: //スピードアップ
                        DataHolder.NoteSpeed = RangeNoOver((int)(DataHolder.NoteSpeed + 1), 5, 25);
                        GetSettingFrameText(1).text = DataHolder.NoteSpeed.ToString();
                        break;
                    case 2: //Music アップ
                        musicVolumePercentage = RangeNoOver(musicVolumePercentage + 10, 0, 100);
                        DataHolder.MusicVolume = GetdB(musicVolumePercentage);
                        GetSettingFrameText(2).text = musicVolumePercentage.ToString() + " %";
                        audioMixer.SetFloat("Music", DataHolder.MusicVolume);
                        break;
                    case 3: //SEアップ
                        seVolumePercentage = RangeNoOver(seVolumePercentage + 10, 0, 100);
                        DataHolder.SEVolume = GetdB(seVolumePercentage);
                        GetSettingFrameText(3).text = seVolumePercentage.ToString() + " %";
                        audioMixer.SetFloat("SE", DataHolder.SEVolume);
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

    private void GameStart()
    {
        float timeDelay = 0.5f;
        GameObject[] changer = new GameObject[4];
        RectTransform[] rectTran = new RectTransform[4];

        for (int i = 0; i < 4; i++) {
            changer[i] = SceneChange.transform.Find(Difficulty.ToString()).Find(i.ToString()).gameObject;
            rectTran[i] = changer[i].GetComponent<RectTransform>();
        }
        changer[0].transform.parent.gameObject.SetActive(true);

        DontDestroyOnLoad(SceneChange);
        changer[3].transform.Find("MusicFrame").GetComponent<MusicFrameComponent>().SetMusicData(Difficulty, DataHolder.NextMusicID);
        changer[3].transform.Find("MusicFrame").Find("Title Text Mask").Find("Title").GetComponent<TextScroll>().Setup();
        changer[3].transform.Find("MusicFrame").Find("Credits Text Mask").Find("Credits").GetComponent<TextScroll>().Setup();

        DataHolder.Difficulty = Difficulty;

        Sequence sceneChangeTween = DOTween.Sequence();
        for (int i = 0; i < 4; i++) {
            changer[i].SetActive(true);
            sceneChangeTween.Insert(1 + 0.25f * i,
                rectTran[i].DOLocalMoveX(0, timeDelay).SetEase(Ease.OutQuint)
            );
        }

        sceneChangeTween.Join(
            DOVirtual.DelayedCall(1.75f, () => {
                ButtonParent.SetActive(false);
            })
        );

        sceneChangeTween.Join(
            DOVirtual.DelayedCall(5f, () => {
                PlayMusicPreview(-1);
            })
        );

        sceneChangeTween.Join(
            DOVirtual.DelayedCall(6f, () => {
                SceneManager.LoadScene("Game");
            })
        );

        sceneChangeTween.Play();

    }

    public void setUI(int selection, int mode)
    {
        
        if (mode == -1)
        {
            GetButtonUI(3).text = "決定";
            GetButtonUI(4).text = "難易度  ▲";
            GetButtonUI(5).text = "";

            CategoryUI.text = categoryList[selection].name;
        }
        else if (mode == 0)
        {
            string sortmodeName = null;

            switch (SortMode)
            {
                case Sortmode.Lexicon:
                    sortmodeName = "曲名順";
                    break;
                case Sortmode.ListJson:
                    sortmodeName = "登録順";
                    break;
            }
            GetButtonUI(3).text = "決定";
            GetButtonUI(4).text = "難易度  ▲";
            GetButtonUI(5).text = "戻る";
            //GetButtonUI(5).text = sortmodeName;

            int num = FindMusicNumber(SortedMusicList[selection]);
            CategoryUI.text = GetCategoryName(musicList[num].category);
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

    private void FrameScroll(int MaxFrameAmount, int selection, float time, bool focusSelectedFrame)
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
                    if (focusSelectedFrame)
                    {
                        sequence.Join(
                            rectTran.DOScale(new Vector3(0.9f, 0.9f, 1f), time).SetEase(Ease.InOutQuart)
                        );
                    }
                    else
                    {
                        sequence.Join(
                            rectTran.DOScale(new Vector3(0.6f, 0.6f, 1f), time).SetEase(Ease.InOutQuart)
                        );
                    }
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
                if (DisplayMode == 0 || DisplayMode == -1)
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
                    pos.y = 650;
                    fade.Join(rectTran.DOLocalMove(pos, time).SetEase(Ease.InExpo));
                }
            }
        }
        else //画面内へ
        {
            if (UIfade)
            {
                if (DisplayMode == 0 || DisplayMode==-1)
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
                    pos.y = 30;
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

    private int RangeNoOver(int val, int min, int max)
    {
        if (val > max)
            val = max;
        if (val < min)
            val = min;

        return val;
    }

    private void GenerateFrame(string[] musicList, float posYoffset, bool focusSelectedFrame)
    {
        for (int i = 0; i < musicList.Length; i++)
        {
            GameObject cloned = Instantiate(FramePrefab, Vector3.zero, Quaternion.identity, FrameParentObject.transform);
            cloned.name = i.ToString();
            cloned.transform.localPosition = new Vector3(440f * (float)i, 30f + posYoffset, 0);
            cloned.GetComponent<MusicFrameComponent>().SetMusicData(Difficulty, musicList[i]);
            cloned.transform.Find("Title Text Mask").transform.Find("Title").GetComponent<TextScroll>().Setup();
            cloned.transform.Find("Credits Text Mask").transform.Find("Credits").GetComponent<TextScroll>().Setup();

            if (focusSelectedFrame)
            {
                if (i == SelectedFrameNumber)
                {
                    cloned.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                }
                else
                {
                    cloned.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                }
            }
            else
            {
                cloned.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            }
        }
    }

    private void GenerateCategoryPlate(MusicDataLoader.Category[] categoryList)
    {
        for (int i = 0; i < categoryList.Length; i++)
        {
            GameObject cloned = Instantiate(CategoryPrefab, Vector3.zero, Quaternion.identity, CategoryParant.transform);
            cloned.name = i.ToString();
            cloned.transform.localPosition = new Vector3(300 * (float)i, -220f, 0);
            cloned.transform.Find("Text").GetComponent<Text>().text = categoryList[i].name;
        }
    }

    private void ScrollCategoryPlate(int MaxCategoryAmount, int selection, float time)
    {
        Sequence sequence = DOTween.Sequence();
        isScreenScrolling = true;

        for (int i = 0; i < MaxCategoryAmount; i++)
        {
            RectTransform rectTran = null;
            if (DisplayMode == -1)
            {
                rectTran = CategoryParant.transform.Find(i.ToString()).GetComponent<RectTransform>();
                sequence.Join(rectTran.DOLocalMove(new Vector3(300f * (float)(i - selection), -220f, 0), time));

                sequence.Join(
                    DOVirtual.DelayedCall(time, () =>
                    {
                        isScreenScrolling = false;
                    })
                );
            }
        }

        setUI(selection, DisplayMode);
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

    private string[] GetSortedMusicID(int sortmode, int difficulty, string category)
    {
        MusicDataLoader.MusicList[] TempMusicList = new MusicDataLoader.MusicList[0];
        string[] ReturnListCat = new string[0];
        int MusicAmount = musicList.Length;

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

        string[] temp = new string[0];
        if (category == "all")
        {
            for (int j = 0; j < TempMusicList.Length; j++)
            {
                temp = temp.Concat(new string[] { TempMusicList[j].id }).ToArray();
            }
        }
        else
        {
            for (int j = 0; j < TempMusicList.Length; j++)
            {
                if (TempMusicList[j].category == category)
                {
                    temp = temp.Concat(new string[] { TempMusicList[j].id }).ToArray();
                }
            }
        }


        //JSONデータに記載順
        if (sortmode == Sortmode.ListJson)
        {
            return temp;
        }

        //辞書順（あいうえお順）
        else if(sortmode == Sortmode.Lexicon)
        {
            string[] ret = new string[temp.Length];

            for (int i = 0; i < temp.Length; i++)
                ret[i] = GetComponent<MusicDataLoader>().getMusicProperty(temp[i]).music + ":MusicID:" + temp[i];
            ret = ret.OrderBy(c => c, new NaturalComparer()).ToArray();

            string[] del = { ":MusicID:" };
            for (int i = 0; i < temp.Length; i++)
                ret[i] = ret[i].Split(del, StringSplitOptions.None)[1];

            return ret;
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
        seq.Kill();
        seq = DOTween.Sequence();

        if (selection != -1)
        {
            string id = SortedMusicList[selection];
            MusicDataLoader.MusicProperty list = GetComponent<MusicDataLoader>().getMusicProperty(id);
            float starttime = list.preview.start;
            float endtime = list.preview.end;
            MusicPreview.volume = 0;

            StartCoroutine(GetAudioClip(id, starttime));

            seq.Join(
                MusicPreview.DOFade(1, fadeTime)
            );
            seq.Insert(endtime - starttime,
                MusicPreview.DOFade(0, fadeTime)
            );
            seq.Insert(endtime - starttime + fadeTime,
                DOVirtual.DelayedCall(0, () =>
                {
                    MusicPreview.Stop();
                    PlayMusicPreview(selection);
                })
            );
        }
        else
        {
            seq.Join(
                MusicPreview.DOFade(0, fadeTime)
            );
            seq.Insert(fadeTime,
                DOVirtual.DelayedCall(0, () =>
                {
                    MusicPreview.Stop();
                })
            );
        }

        seq.Play();

    }

    private Text GetButtonUI(int num)
    {
        return ButtonParent.transform.Find("Button" + num.ToString()).Find("Text").GetComponent<Text>();
    }

    private Text GetSettingFrameText(int num)
    {
        return FrameParentSettingDisplayObject.transform.Find(num.ToString()).Find("Setting").Find("Value").GetComponent<Text>();
    }

    private float GetdB(float Gain)
    {
        float dB = (float)(20f * Math.Log(Gain * 0.01f, 10));
        if (Gain == 0)
            dB = -80;
        return dB;
    }

    private float GetGain(float dB)
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
        public const int ListJson = 1;
    }


}
