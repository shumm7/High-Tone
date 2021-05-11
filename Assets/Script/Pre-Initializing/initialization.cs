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
    public bool DebugMode = false;
    Text Display;
    public GameObject DebugInfo;
    public GameObject NFCReader;

    bool RunGame;

    private void Awake()
    {
        DebugInfo.SetActive(false);

        //DOTween.Init();
        //DOTween.defaultAutoPlay = AutoPlay.None;
    }

    void Start()
    {
        RunGame = true;
        Display = GetComponent<Text>();
        Option optionData = new Option();
        float StartTime = Time.time;

        //設定データ
        try
        {
            string temp = GetComponent<MusicDataLoader>().load("Music/config.json");
            optionData = JsonUtility.FromJson<Option>(temp);

            DebugMode = optionData.DebugMode;
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
            RunGame = false;
        }

        AddLine("");
        AddLine("FeliCa カードリーダーの接続中: ");
        string NfcReaderName = NFCReader.GetComponent<NFCReader>().GetCardReaderName();
        NFCReader.GetComponent<UserDataController>().CardReadingEnabled = false;
        NFCReader.GetComponent<UserDataController>().CardReadingTimeOutLength = optionData.NFCReaderTimeOut;
        NFCReader.GetComponent<NFCReader>().DetectOnlyFeliCa = optionData.NFCReaderDetectOnlyFeliCa;
        NFCReader.GetComponent<NFCReader>().ThrowExceptionLog = optionData.NFCReaderExceptionLog;
        DataHolder.CardReader = optionData.NFCReader;
        if (DataHolder.CardReader)
        {
            if (NfcReaderName == "")
            {
                AddLine("   失敗");
                DataHolder.CardReader = false;
                DataHolder.CardReaderName = "";
            }
            else
            {
                AddText("   成功");
                AddLine(" - 【接続済】" + NfcReaderName);
                DataHolder.CardReader = true;
                DataHolder.CardReaderName = NfcReaderName;
            }
        }
        else
        {
            AddLine("   失敗 - 設定ファイルで無効化されています。");
            DataHolder.CardReader = false;
            DataHolder.CardReaderName = "";
        }

        if (RunGame)
        {
            AddLine("");
            AddLine("起動準備完了: 経過時間: " + (Time.time - StartTime).ToString());
            AddLine("【注意】ゲームを終了するとクレジットは消滅します。");
            AddLine("");
            AddLine("   Enterキーでゲームを開始します。");
        }
        else
        {
            AddLine("");
            AddLine("起動準備失敗: 経過時間: " + (Time.time - StartTime).ToString());
            AddLine("   Enterキーでゲームを終了します。");
        }

        if (!DebugMode && RunGame)
        {
            LoadScene();
        }
        else if(!DebugMode && !RunGame)
        {
            Quit();
        }
    }

    private void Update()
    {
        if (RunGame)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                LoadScene();
            }
        }
        else
        {
            Quit();
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
        public bool NFCReader = true;
        public float NFCReaderTimeOut = 1f;
        public bool NFCReaderExceptionLog = false;
        public bool NFCReaderDetectOnlyFeliCa = false;
    }


}
