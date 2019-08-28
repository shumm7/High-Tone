using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMusicSelect : MonoBehaviour
{
    public static string[] SortedMusicList;
    public int SortMode = Sortmode.Category;
    public int Difficulty = 0;

    public static int SelectedFrame = 0;
    private bool isScreenScrolling = false;

    private int MaximumFrameAmount;
    private int SelectedFrameNumber;
    public GameObject FramePrefab;
    public GameObject FrameParentObject;

    private MusicDataLoader.MusicList[] musicList;
    private MusicDataLoader.Category[] categoryList;

    void Start()
    {
        musicList = GetComponent<MusicDataLoader>().getMusicList().music;
        categoryList = GetComponent<MusicDataLoader>().getMusicList().category;

        SortedMusicList = GetSortedMusicID(SortMode, Difficulty);
        GenerateFrame(SortedMusicList);

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Scrolling");

            if (SelectedFrame >= SortedMusicList.Length)
                SelectedFrame = 0;
            if (SelectedFrame < 0)
                SelectedFrame = SortedMusicList.Length - 1;
            FrameScroll(SortedMusicList.Length, SelectedFrame++, 0.2f);
        }
    }

    private void FrameScroll(int MaxFrameAmount, int selection, float time)
    {
            Sequence sequence = DOTween.Sequence();
            isScreenScrolling = true;

            for (int i = 0; i < MaxFrameAmount; i++)
            {
                RectTransform rectTran = FrameParentObject.transform.Find(i.ToString()).GetComponent<RectTransform>();

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
    }

    private void GenerateFrame(string[] musicList)
    {
        for (int i = 0; i < musicList.Length; i++)
        {
            GameObject cloned = Instantiate(FramePrefab, Vector3.zero, Quaternion.identity, FrameParentObject.transform);
            cloned.name = i.ToString();
            cloned.transform.localPosition = new Vector3(440f * (float)i, 30f, 0);
            cloned.GetComponent<MusicFrameComponent>().SetMusicData(Difficulty, musicList[i]);

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
            string[] ret = new string[0];

            for (int i = 0; i < TempMusicList.Length; i++)
                ret[i] = TempMusicList[i].id;

            return ret;
        }

        //辞書順（あいうえお順）
        else if(sortmode == Sortmode.Lexicon)
        {
            string[] ret = new string[0];

            for (int i = 0; i < TempMusicList.Length; i++)
                ret[i] = TempMusicList[i].id;

            return ret.OrderBy(c => c, new NaturalComparer()).ToArray();
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
                temp = temp.OrderBy(c => c, new NaturalComparer()).ToArray();
                ReturnListCat = ReturnListCat.Concat(temp).ToArray();

            }

            return ReturnListCat;
        }
        else
        {
            return null;
        }
    }

    private class Sortmode
    {
        public const int Lexicon = 0;
        public const int Category = 1;
        public const int ListJson = 2;
    }
}
