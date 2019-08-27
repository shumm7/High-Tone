using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMusicSelect : MonoBehaviour
{
    public static string[] SortedMusicList;
    MusicDataLoader.MusicList[] musicList;
    MusicDataLoader.Category[] categoryList;

    void Start()
    {
        musicList = GetComponent<MusicDataLoader>().getMusicList().music;
        categoryList = GetComponent<MusicDataLoader>().getMusicList().category;
        
        for(int t=0; t<musicList.Length; t++)
        {
            Debug.Log(GetSortedMusicID(Sortmode.Category, 0)[t]);
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
