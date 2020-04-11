using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using NfcPcSc;

public class initialization : MonoBehaviour
{
    public string nextScene;
    public bool DebugMode = false;
    Text Display;
    public GameObject DebugInfo;
    public GameObject NFCReader;
    

    private void Awake()
    {
        DebugInfo.SetActive(false);

        //DOTween.Init();
        //DOTween.defaultAutoPlay = AutoPlay.None;
    }

    void Start()
    {

        Display = GetComponent<Text>();
        Option optionData = new Option();
        float StartTime = Time.time;

        //設定データ
        try
        {
            string temp = GetComponent<MusicDataLoader>().load("Music/config.json");
            optionData = JsonUtility.FromJson<Option>(temp);

            DebugMode = optionData.isDebugMode;
            debug.SetDebugMode(DebugMode);
            if (DebugMode)
            {
                DebugInfo.SetActive(true);
                DontDestroyOnLoad(DebugInfo);
            }

            AddText("設定データ読み込み完了:");
            AddText("");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        AddLine("プロセス開始時間: " + StartTime.ToString());
        AddLine("");
        AddLine("システム時刻: " + DateTime.Now.ToString());
        AddLine("Unity Engine バージョン: " + Application.unityVersion);
        AddLine("システムバージョン: " + Application.version);
        AddLine("");

        AddLine("設定データ読み込み中: ");
        AddText("   ");
        if (optionData == null){
            AddText("- 失敗 ");

            GetComponent<MusicDataLoader>().saveFile("Music/config.json", JsonUtility.ToJson(new Option()));
            AddText("- 生成中 ");

            string temp = GetComponent<MusicDataLoader>().load("Music/config.json");
            optionData = JsonUtility.FromJson<Option>(temp);
        }
        AddText("- ロード完了");
        DataHolder.DebugMode = optionData.DebugMode;
        DataHolder.MasterVolume = optionData.MasterVolume;
        DataHolder.BGMVolume = optionData.BGMVolume;
        DataHolder.PlayTimePerCredit = optionData.PlayTimePerCredit;
        DataHolder.GlobalNoteOffset = optionData.GlobalNoteOffset;
        DataHolder.FreePlay = optionData.FreePlay;
        AddText("- 適用完了\n");

        try
        {
            AddLine("楽曲データ取得中: ");
            MusicDataLoader.List musicList = GetComponent<MusicDataLoader>().getMusicList();
            for(int i=0; i<musicList.music.Length; i++)
            {
                AddText("   [" + musicList.music[i].category + "]" + musicList.music[i].id);
            }
            AddLine("");

            AddLine("カテゴリデータ取得中: ");
            MusicDataLoader.Category[] categoryList = GetComponent<MusicDataLoader>().getCategoryList();
            for (int i = 0; i < categoryList.Length; i++)
            {
                AddText("   [" + categoryList[i].id + "]" + categoryList[i].name);
            }
            AddLine("");

        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            AddLine("   失敗");
            Quit();
        }

        AddLine("");
        AddLine("FeliCa カードリーダーの接続中: ");
        NfcMain.ErrorCode ErrorCode = NFCReader.GetComponent<NfcMain>().initNfc();
        switch (ErrorCode)
        {
            case NfcMain.ErrorCode.NO_ERROR:
                AddText("   成功");
                AddText("- " + DataHolder.CardReaderName);
                DataHolder.CardReader = true;
                break;
            case NfcMain.ErrorCode.CANNOT_GET_READER_DATA:
                AddText("   失敗");
                AddText("- リーダー情報の取得に失敗");
                DataHolder.CardReader = false;
                break;
            case NfcMain.ErrorCode.NO_READER_AVAILABLE:
                AddText("   失敗");
                AddText("- リーダーが存在しません");
                DataHolder.CardReader = false;
                break;
            case NfcMain.ErrorCode.SMARTCARD_SERVICE_DISABLED:
                AddText("   失敗");
                AddText("- Smart Cardサービスが無効です");
                DataHolder.CardReader = false;
                break;
        }

        AddLine("");
        AddLine("起動準備完了: 経過時間: " + (Time.time - StartTime).ToString());
        AddLine("   Enterキーでゲームを開始します。");

        if (!DebugMode)
        {
            LoadScene();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            LoadScene();
        }
    }

    void AddLine(string text)
    {
        if (DebugMode)
        {
            Display.text += text + "\n";
        }
    }

    void AddText(string text)
    {
        if (DebugMode)
        {
            Display.text += text;
        }
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
        public bool FreePlay = true;
        public bool isDebugMode = false;
    }


}
