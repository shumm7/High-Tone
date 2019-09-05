using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;



public class initialization : MonoBehaviour
{
    public string nextScene;
    Text Display;

    void Start()
    {
        Display = GetComponent<Text>();
        Option optionData = new Option();

        AddLine("プロセス開始時間: " + Time.time.ToString());
        AddLine("");
        AddLine("システム時刻: " + DateTime.Now.ToString());
        AddLine("Unity Engine バージョン: " + Application.unityVersion);
        AddLine("システムバージョン: " + Application.version);
        AddLine("");
        
        //設定データ
        Display.text += "設定データ読み込み: ";
        try
        {
            string temp = GetComponent<MusicDataLoader>().load("Music/config.json");
            optionData = JsonUtility.FromJson<Option>(temp);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        if (optionData == null){
            Display.text += "- 失敗 ";

            GetComponent<MusicDataLoader>().saveFile("Music/config.json", JsonUtility.ToJson(new Option()));
            Display.text += "- 生成中 ";

            string temp = GetComponent<MusicDataLoader>().load("Music/config.json");
            optionData = JsonUtility.FromJson<Option>(temp);
        }
        Display.text += "- ロード完了";
        DataHolder.DebugMode = optionData.DebugMode;
        DataHolder.MasterVolume = optionData.MasterVolume;
        DataHolder.BGMVolume = optionData.BGMVolume;
        DataHolder.PlayTimePerCredit = optionData.PlayTimePerCredit;
        DataHolder.GlobalNoteOffset = optionData.GlobalNoteOffset;
        Display.text += "- 適用完了\n";

        try
        {
            AddLine("楽曲データ取得中: ");
            MusicDataLoader.List musicList = GetComponent<MusicDataLoader>().getMusicList();
            for(int i=0; i<musicList.music.Length; i++)
            {
                Display.text += "   [" + musicList.music[i].category + "]" + musicList.music[i].id;
            }
            AddLine("");

            AddLine("カテゴリデータ取得中: ");
            MusicDataLoader.Category[] categoryList = GetComponent<MusicDataLoader>().getCategoryList();
            for (int i = 0; i < categoryList.Length; i++)
            {
                Display.text += "   [" + categoryList[i].id + "]" + categoryList[i].name;
            }
            AddLine("");

        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            AddLine("   失敗");
            Quit();
        }



        AddLine("ゲーム開始中");
        LoadScene();
    }

    void AddLine(string text)
    {
        Display.text += text + "\n";
    }

    void Quit()
    {
        #if UNITY_EDITOR
                DOVirtual.DelayedCall(5, () => {
                    UnityEditor.EditorApplication.isPlaying = false;
                });
        #elif UNITY_STANDALONE
                DOVirtual.DelayedCall(5, () => {
                    UnityEngine.Application.Quit();
                });
        #endif
    }

    void LoadScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    public class Option {
        public bool DebugMode = false; //デバッグモード
        public float MasterVolume = 0; //マスター音量[dB]
        public float BGMVolume = 0; //BGM音量[dB]
        public int PlayTimePerCredit = 2; //1クレジットでプレイできる曲数
        public double GlobalNoteOffset = 0.03; //ゲーム全体で適用するノーツ判定時のオフセット [秒]
    }


}
