using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool DebugMode = false;
    public static bool isPlaying = false;
    public static int GameStatus = Status.None; 

    [SerializeField] string MusicID; //ヒエラルキー上からMusicIDを入力するとデバッグモード
    [SerializeField] bool PlayVideo = true;
    [SerializeField] float PlayTime = 3.0f;
    [SerializeField] int Difficulty = 0; //0Easy 1Normal 2Hard 3VeryHard 4Extra
    public float speed;
    public double GlobalOffset = 0;
    public GameObject NotesPrefab;
    public GameObject SliderNotesPrefab;
    public GameObject StartSliderNotesPrefab;
    public GameObject SliderPrefab;
    public GameObject SpecialNotesPrefab;
    //public GameObject DamageNotesPrefab;
    public GameObject NotesParentObject;

    private int slidernotecount = 0;

    //Audio
    public AudioSource audioSource;

    //Video
    public VideoClip DefaultVideoClip;
    public GameObject rawImage;
    public RenderTexture renderTexture;
    private VideoPlayer videoPlayer;

    //UI
    public Image MusicArtwork;
    public Text MusicTitleText;
    public Text AuthorText;
    public TextMeshProUGUI ScoreText;
    public Text ComboText;
    public Text LevelText;
    public GameObject SceneChange;
    public GameObject SceneChangeEnd;

    //Loading
    public float FadeInTime = 3f;
    public float[] FadeInUIColor = {55, 55, 55};

    // 曲のデータ
    string MusicName;
    public static int MaxNotesAmount;
    int Level;
    bool IsVideo;
    string Composer;
    string Lyrics;
    string Vocal;
    int BPM;
    int Offset;
    MusicDataLoader.NoteInfo[] Notes;

    //スコア
    int Combo = 0;
    int Score = 0;
    public static int MaximumScoreForDisplay = 0;

    // ノーツの初期座標
    public static float[] NoteX = {-2.90f, -1.45f, 0, 1.45f, 2.90f};
    public static float NoteY = 18.5f;
    public static float NoteZ = 0.9f;
    public static float DetectionLineY = -1.56f;
    public static float ArrivalTime;
    public static float NoteSpeed; //他のオブジェクト用変数

    //ノーツの起動時間
    public static double[] NoteStartTime;
    public static double[,] NoteStartTimeSlider;
    private int NextNotesNumber = 0;
    private float MusicStartTimeOffset = 0;

    void Awake()
    {

        Debug.Log("ゲームが開始されました");
        GameStatus = Status.Initializing;

        //ノーツ初期化
        foreach (Transform childTransform in NotesParentObject.transform)
        {
            Destroy(childTransform.gameObject);
        }
        TimeComponent.ResetStartTime();

        //到着時間
        if (MusicID=="")
        {
            speed = DataHolder.NoteSpeed;
        }
        NoteSpeed = speed;
        ArrivalTime = (NoteY - DetectionLineY) / speed;

        //シーンチェンジ画面取得
        SceneChange = GameObject.Find("Scene Change");

        //Video用RawImage初期化
        rawImage.SetActive(true);
        videoPlayer = rawImage.GetComponent<VideoPlayer>();
        Color col = rawImage.GetComponent<RawImage>().color;
        col.a = 1f;
        rawImage.GetComponent<RawImage>().color = col;
        renderTexture.Release();

        //初期映像再生
        videoPlayer.url = DefaultVideoClip.originalPath;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.Play();

        StartCoroutine("StartSettings");
    }

    void Update()
    {

        if (NextNotesNumber <= (MaxNotesAmount - 1)) {
            if (TimeComponent.GetCurrentTimePast() >= NoteStartTime[NextNotesNumber] && TimeComponent.StartTime != 0)
            {
                NotesParentObject.transform.Find(NextNotesNumber.ToString()).gameObject.SetActive(true);

                if (Notes[NextNotesNumber].type == 2)
                {
                    for (int i = 0; i < Notes[NextNotesNumber].notes.Length; i++)
                    {
                        NotesParentObject.transform.Find(NextNotesNumber.ToString() + "-" + (i * 2).ToString()).gameObject.SetActive(true);
                        NotesParentObject.transform.Find(NextNotesNumber.ToString() + "-" + (i * 2 + 1).ToString()).gameObject.SetActive(true);
                    }
                }

                NextNotesNumber++;
                while (checkNextNoteIsSameTime(NextNotesNumber - 1))
                {
                    NotesParentObject.transform.Find(NextNotesNumber.ToString()).gameObject.SetActive(true);
                    NextNotesNumber++;

                    if (Notes[NextNotesNumber].type == 2)
                    {
                        for (int i = 0; i < Notes[NextNotesNumber].notes.Length; i++)
                        {
                            //NotesParentObject.transform.Find(NextNotesNumber.ToString() + "-" + (i*2).ToString()).gameObject.SetActive(true);
                            //NotesParentObject.transform.Find(NextNotesNumber.ToString() + "-" + (i * 2 + 1).ToString()).gameObject.SetActive(true);
                            NotesParentObject.transform.Find(NextNotesNumber.ToString() + "-" + i.ToString()).gameObject.SetActive(true);

                        }
                    }
                }
                
            }
        }

        if(TimeComponent.GetCurrentTimePast() >= 15 && GameStatus==Status.InGame && audioSource.isPlaying == false)
        {
            DebugLog("音楽が終了しました");
            if (videoPlayer.isPlaying)
            {
                RawImage FadeInImage = rawImage.GetComponent<RawImage>();
                DOTween.ToAlpha( () => FadeInImage.color, a => FadeInImage.color = a, 0f, 1f);
            }
            GameStatus = Status.Finished;

            //データホルダーに保存
            DataHolder.Score = ScoreCalculation.Score;
            DataHolder.ObjectiveScore = ScoreCalculation.ObjectiveScore;
            DataHolder.MaximumScore = ScoreCalculation.MaximumScore;
            DataHolder.ScorePercentage = ScoreCalculation.ScorePercentage;
            DataHolder.Combo = ScoreCalculation.MaxCombo;
            DataHolder.MaximumCombo = ScoreCalculation.MaximumCombo;
            DataHolder.JudgementAmount = ScoreCalculation.JudgementCount;
            DataHolder.PlayedTime = DataHolder.PlayedTime + 1;

            //SceneChange
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
                DOVirtual.DelayedCall(6f, () =>
                {
                    DataHolder.TemporaryGameObject = SceneChangeEnd;
                    SceneManager.LoadScene("Result");
                })
            );

            sceneChangeTween.Play();
        }
    }

    private bool checkNextNoteIsSameTime(int num)
    {
        if(num+1 < (MaxNotesAmount - 1))
        {
            if (Notes[num].num == Notes[num+1].num)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    IEnumerator StartSettings()
    {
        
        //非デバッグ時の処理
        if (MusicID == "")
        {
            MusicID = DataHolder.NextMusicID;
            Difficulty = DataHolder.Difficulty;
            PlayVideo = DataHolder.isVideo;
            GlobalOffset += DataHolder.GlobalNoteOffset;
        }
        else
        {
            DebugMode = true;
            Debug.LogWarning("エディタ用デバッグモードで起動中");
        }
        DataHolder.DebugMode = DebugMode;

        //楽曲データ取得
        DebugLog("楽曲データを取得中");
        MusicDataLoader.MusicProperty musicinfo = GetComponent<MusicDataLoader>().getMusicProperty(MusicID);
        MusicName = musicinfo.music;
        Level = musicinfo.level[Difficulty];
        Composer = musicinfo.credits;
        MusicDataLoader.Notes note = GetComponent<MusicDataLoader>().getNotesData(Difficulty, MusicID);
        BPM = note.BPM;
        Offset = note.offset;
        Notes = note.notes;
        MaxNotesAmount = Notes.Length;

        //ノーツ生成時刻計算
        DebugLog("ノーツの移動開始時刻を計算中");
        NoteStartTime = new double[MaxNotesAmount];
        NoteStartTimeSlider = new double[MaxNotesAmount, 64];

        double tSPB = ((double)60 / ((double)BPM * (double)Notes[0].LPB));
        if ((tSPB * (double)Notes[0].num + ((double)Offset / (double)6000 * tSPB) - ArrivalTime + GlobalOffset) <= 0)
        {
            MusicStartTimeOffset = Mathf.Ceil(-(float)(tSPB * (double)Notes[0].num + ((double)Offset / (double)6000 * tSPB) - ArrivalTime + GlobalOffset + MusicStartTimeOffset));
            DebugLog("ノーツ開始時間が負の数であるため、オフセットを設定しています " + MusicStartTimeOffset.ToString());
        }

        for (int n = 0; n < MaxNotesAmount; n++)
        {
            //1ノーツの単位 60/(BPM*LPB)
            double SPB = (60f / ((double)BPM * (double)Notes[n].LPB));

            NoteStartTime[n] = SPB * Notes[n].num + ((double)Offset / (double)6000 * SPB) - ArrivalTime + GlobalOffset + MusicStartTimeOffset;

            if (n + 1 != MaxNotesAmount && Notes[n].num == Notes[n + 1].num && Notes[n].LPB == Notes[n + 1].LPB)// && Notes[n].type == Notes[n + 1].type)
            {
                NoteStartTime[n + 1] = NoteStartTime[n];
                n++;
            }
            if (Notes[n].type == 2)
            {
                for (int k=0; k< Notes[n].notes.Length; k++)
                {
                    double tempSPB = (60f / ((double)BPM * (double)Notes[n].notes[k].LPB));
                    NoteStartTimeSlider[n,k] = tempSPB * Notes[n].notes[k].num + ((double)Offset / (double)6000 * SPB) - ArrivalTime + GlobalOffset + MusicStartTimeOffset;

                }
            }
        }

        DebugLog("ノーツを生成中");
        //ノーツ生成
        NotesPrefab.SetActive(false);
        for(int i = 0; i < MaxNotesAmount; i++)
        {
            SpawnNotes(i, Notes[i].block, (60f / ((double)BPM * (double)Notes[i].LPB)));
        }

        //インターフェース
        DebugLog("UIの設定中");
        MusicTitleText.text = MusicName;
        AuthorText.text = Composer;
        LevelText.text = Level.ToString();
        ScoreText.text = Score.ToString();
        ComboText.text = Combo.ToString();

        //アートワーク設定
        byte[] bytes = File.ReadAllBytes("Music/"+MusicID+ "/image.png");
        Texture2D texture = new Texture2D(55, 55, TextureFormat.ARGB4444, false);
        texture.LoadImage(bytes);
        texture.Compress(false);
        MusicArtwork.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.width), new Vector2(0.5f, 0.5f));

        //ビデオの設定
        DebugLog("ビデオの設定中");
        if (PlayVideo)
        {
            string videoPath = Directory.GetCurrentDirectory() + "/" + "Music/" + MusicID + "/video.mp4";
            if (GetComponent<MusicDataLoader>().checkExist(videoPath))
            {
                videoPlayer.url = "file://" + videoPath;
                videoPlayer.source = VideoSource.Url;
            }
            else
            {
                PlayVideo = false;
            }
        }

        if (!DebugMode)
        {
            Vector3 temp = rawImage.transform.localPosition;
            if (DataHolder.VideoSettingMode == "cut")
            {
                temp.z = 260;
                rawImage.transform.localPosition = temp;
            }
            else if (DataHolder.VideoSettingMode == "fit")
            {
                temp.z = 360;
                rawImage.transform.localPosition = temp;
            }
        }
        else
        {
            string VideoSettingMode = "cut";
            Vector3 temp = rawImage.transform.localPosition;
            if (VideoSettingMode == "cut")
            {
                temp.z = 260;
                rawImage.transform.localPosition = temp;
            }
            else if (VideoSettingMode == "fit")
            {
                temp.z = 360;
                rawImage.transform.localPosition = temp;
            }
        }

        //音楽の設定
        DebugLog("音楽の設定中");
        string path = "Music/" + MusicID + "/music.wav";
        StartCoroutine(GetAudioClip(path));

        //画面の遷移
        if (!DebugMode)
        {
            float timeDelay = 0.5f;
            GameObject[] changer = new GameObject[4];
            RectTransform[] rectTran = new RectTransform[4];

            for (int i = 0; i < 4; i++)
            {
                changer[i] = DataHolder.TemporaryGameObject.transform.Find(Difficulty.ToString()).Find(i.ToString()).gameObject;
                rectTran[i] = changer[i].GetComponent<RectTransform>();
            }

            Sequence sceneChangeTween = DOTween.Sequence();
            for (int i = 0; i < 4; i++)
            {
                sceneChangeTween.Insert(1 + 0.25f * (3 - i),
                    rectTran[i].DOLocalMoveX(-1280, timeDelay).SetEase(Ease.OutQuint)
                );
            }

            sceneChangeTween.Join(
                DOVirtual.DelayedCall(4f, () =>
                {
                    Destroy(SceneChange);
                })
            );

            sceneChangeTween.Play();
        }
        yield return new WaitForSeconds(4);
      

        //音楽の再生
        TimeComponent.SetStartTime();
        DebugLog("音楽を再生しました");
        audioSource.PlayScheduled(AudioSettings.dspTime + MusicStartTimeOffset);
        if(PlayVideo)
            videoPlayer.Play();

        //ステータス切り替え
        isPlaying = true;
        GameStatus = Status.InGame;

    }

    private void SpawnNotes(int num, int place, double SPB)
    {
        if (Notes[num].type == ScoreCalculation.NoteType.Normal)
        { //Normal Notes
            GameObject ClonedNotesObject = Instantiate(NotesPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45, 0, 0), NotesParentObject.transform);
            ClonedNotesObject.SetActive(false);
            ClonedNotesObject.transform.localPosition = new Vector3(NoteX[place], NoteY, NoteZ);
            ClonedNotesObject.name = num.ToString();
            ClonedNotesObject.GetComponent<NoteController>().SetRailNumber(place);
            ClonedNotesObject.GetComponent<NoteController>().SetArrivalTime(NoteStartTime[num] + ArrivalTime);
            ClonedNotesObject.GetComponent<NoteController>().speed = speed;

            MaximumScoreForDisplay += 200;
        }
        else if (Notes[num].type == ScoreCalculation.NoteType.Slider)
        { //Slider Notes
            GameObject ClonedNotesObject = Instantiate(StartSliderNotesPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45, 0, 0), NotesParentObject.transform);
            ClonedNotesObject.SetActive(false);
            ClonedNotesObject.transform.localPosition = new Vector3(NoteX[place], NoteY, NoteZ);
            ClonedNotesObject.name = num.ToString();
            ClonedNotesObject.GetComponent<StartSliderNoteController>().SetRailNumber(place);
            ClonedNotesObject.GetComponent<StartSliderNoteController>().SetArrivalTime(NoteStartTime[num] + ArrivalTime);
            ClonedNotesObject.GetComponent<StartSliderNoteController>().speed = speed;
            MaximumScoreForDisplay += 200;

            int name = 0;
            int cnt = 0;

            GameObject ClonedSlider = Instantiate(SliderPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45, 0, 0), NotesParentObject.transform);
            ClonedSlider.SetActive(false);
            ClonedSlider.transform.localPosition = new Vector3(NoteX[place], NoteY, NoteZ - 0.005f);
            ClonedSlider.name = num.ToString() + "-" + name.ToString();
            ClonedSlider.GetComponent<SliderSquareController>().setParam(speed, NoteStartTime[num] + ArrivalTime, NoteStartTimeSlider[num, cnt] + ArrivalTime, BPM, Notes[num].LPB, place, Notes[num].notes[cnt].block);

            MaximumScoreForDisplay += 200 * (int)((NoteStartTimeSlider[num, cnt] - NoteStartTime[num]) * SPB);

            name++;

            foreach(MusicDataLoader.NoteInfoNext sliderNote in Notes[num].notes)
            {
                GameObject ClonedSliderNotes = Instantiate(SliderNotesPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45, 0, 0), NotesParentObject.transform);
                ClonedSliderNotes.SetActive(false);
                ClonedSliderNotes.transform.localPosition = new Vector3(NoteX[Notes[num].notes[cnt].block], NoteY + speed * (float)(NoteStartTimeSlider[num, cnt] - NoteStartTime[num]), NoteZ);
                ClonedSliderNotes.name = num.ToString() + "-" + name.ToString();
                ClonedSliderNotes.GetComponent<SliderNoteController>().SetRailNumber(Notes[num].notes[cnt].block);
                ClonedSliderNotes.GetComponent<SliderNoteController>().SetArrivalTime(NoteStartTimeSlider[num, cnt] + ArrivalTime);
                ClonedSliderNotes.GetComponent<SliderNoteController>().speed = speed;
                name++;

                MaximumScoreForDisplay += 200;

                if (Notes[num].notes.Length > cnt + 1)
                {
                    ClonedSlider = Instantiate(SliderPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45, 0, 0), NotesParentObject.transform);
                    ClonedSlider.SetActive(false);
                    ClonedSlider.transform.localPosition = new Vector3(NoteX[Notes[num].notes[cnt].block], NoteY + speed * (float)(NoteStartTimeSlider[num, cnt] - NoteStartTime[num]), NoteZ - 0.005f);
                    ClonedSlider.name = num.ToString() + "-" + name.ToString();
                    ClonedSlider.GetComponent<SliderSquareController>().setParam(speed, NoteStartTimeSlider[num, cnt] + ArrivalTime, NoteStartTimeSlider[num, cnt + 1] + ArrivalTime, BPM, Notes[num].LPB, Notes[num].notes[cnt].block, Notes[num].notes[cnt+1].block);
                    name++;

                    MaximumScoreForDisplay += 200 * (int)((NoteStartTimeSlider[num, cnt] - NoteStartTime[num]) / SPB);
                }
                cnt++;
            }
        }
        else if (Notes[num].type == ScoreCalculation.NoteType.Special)
        { //Special Notes
            GameObject ClonedNotesObject = Instantiate(SpecialNotesPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45, 0, 0), NotesParentObject.transform);
            ClonedNotesObject.SetActive(false);
            ClonedNotesObject.transform.localPosition = new Vector3(NoteX[place], NoteY, NoteZ);
            ClonedNotesObject.name = num.ToString();
            ClonedNotesObject.GetComponent<SpecialNoteController>().SetRailNumber(place);
            ClonedNotesObject.GetComponent<SpecialNoteController>().SetArrivalTime(NoteStartTime[num] + (double)ArrivalTime);
            ClonedNotesObject.GetComponent<SpecialNoteController>().speed = speed;

            MaximumScoreForDisplay += 400;
        }
        else if (Notes[num].type == ScoreCalculation.NoteType.Damage)
        { //Damage Notes

        }
    }

    IEnumerator GetAudioClip(string path)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + Directory.GetCurrentDirectory() + "/" + path, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
            audioSource.clip.name = MusicID;
        }
    }

    public static void DebugLog(string msg)
    {
        if (DebugMode)
            Debug.Log(msg);
    }

    public static class Status
    {
        public static int None = -1;
        public static int Initializing = 0;
        public static int InGame = 1;
        public static int Finished = 2;
    }
}
