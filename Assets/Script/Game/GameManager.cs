using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using TMPro;

public class GameManager : MonoBehaviour
{
    private bool DebugMode = false;

    [SerializeField] string MusicID; //ヒエラルキー上からMusicIDを入力するとデバッグモード
    [SerializeField] bool PlayVideo = true;
    [SerializeField] float PlayTime = 3.0f;
    [SerializeField] int Difficulty = 0; //0Easy 1Normal 2Hard 3VeryHard 4Extra
    [SerializeField] float speed = 10f;
    public GameObject NotesPrefab;
    public AudioSource audioSource;
    public GameObject rawImage;
    public RenderTexture renderTexture;
    public GameObject NotesParentObject;
    public Text MusicTitleText;
    public Text AuthorText;
    public TextMeshProUGUI ScoreText;
    public Text ComboText;
    public Text LevelText;

    //Loading
    private AsyncOperation async;
    public GameObject LoadingPanel;
    public float FadeInTime = 1f;
    public float[] FadeInUIColor = {55, 55, 55};

    // 曲のデータ
    string MusicName;
    int MaxNotesAmount;
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

    // ノーツの初期座標
    public static float[] NoteX = {-2.90f, -1.45f, 0, 1.45f, 2.90f};
    public static float NoteY = 18.5f;
    public static float NoteZ = 0.9f;
    public static float DetectionLineY = -1.5f;
    public static float ArrivalTime;
    public static float NoteSpeed; //他のオブジェクト用変数

    void Awake()
    {
        Debug.Log("ゲームが開始されました");
        LoadingPanel.SetActive(true);

        //Video用RawImage初期化
        rawImage.SetActive(false);
        renderTexture.Release();
        LoadingPanel.GetComponent<Image>().color = new Color(FadeInUIColor[0] / 255, FadeInUIColor[1] / 255, FadeInUIColor[2] / 255, 1f);

        //到着時間
        NoteSpeed = speed;
        ArrivalTime = (NoteY - DetectionLineY) / speed;

        StartCoroutine("StartSettings");
    }

    void Update()
    {
        
    }

    IEnumerator StartSettings()
    {
        
        //非デバッグ時の処理
        if (MusicID == "")
        {
            //前のシーンから曲のデータを読み込む
        }
        else
        {
            DebugMode = true;
            Debug.LogWarning("エディタ用デバッグモードで起動中");
        }

        //楽曲データ取得
        MusicDataLoader.MusicProperty musicinfo = GetComponent<MusicDataLoader>().getMusicProperty(MusicID);
        MusicName = musicinfo.music;
        MaxNotesAmount = musicinfo.notes;
        Level = musicinfo.level[Difficulty];
        IsVideo = musicinfo.video;
        Composer = musicinfo.credits.composer;
        Lyrics = musicinfo.credits.lyrics;
        Vocal = musicinfo.credits.vocal;
        MusicDataLoader.Notes note = GetComponent<MusicDataLoader>().getNotesData(MusicID);
        BPM = note.BPM;
        Offset = note.offset;
        Notes = note.notes;

        //ノーツ生成
        NotesPrefab.SetActive(false);
        double BPS = BPM / 60;
        int i = 0;
        foreach (MusicDataLoader.NoteInfo temp in Notes)
        {
            SpawnNotes(i, temp.block);
            i++;
        }

        //インターフェース
        MusicTitleText.text = MusicName;
        AuthorText.text = Composer;
        LevelText.text = Level.ToString();
        ScoreText.text = Score.ToString();
        ComboText.text = Combo.ToString();

        //ビデオの設定
        VideoPlayer videoPlayer = rawImage.GetComponent<VideoPlayer>();
        if (PlayVideo)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = "file://" + Directory.GetCurrentDirectory() + "/" + "Music/" + MusicID + "/video.mp4";
        }

        //音楽の設定
        string path = "Music/" + MusicID + "/music.wav";
        StartCoroutine(GetAudioClip(path));

        StartCoroutine(FadeIn());
        yield return new WaitForSeconds( PlayTime + (FadeInTime * 1.5f) );

        //音楽の再生
        audioSource.Play();
        TimeComponent.SetStartTime();
        rawImage.SetActive(true);
        if(PlayVideo)
            videoPlayer.Play();

    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(FadeInTime / 2);
        for (var i = 1f; i >= 0; i -= 0.01f)
        {
            LoadingPanel.GetComponent<Image>().color = new Color(FadeInUIColor[0] / 255, FadeInUIColor[1] / 255, FadeInUIColor[2] / 255, i);
            yield return new WaitForSeconds(FadeInTime / 100);
        }
        LoadingPanel.SetActive(false);
    }

    private void SpawnNotes(int num, int place)
    {
        GameObject cloneObject = Instantiate(NotesPrefab, new Vector3(0f, 0f, 0f), Quaternion.Euler(45,0,0), NotesParentObject.transform);
        cloneObject.SetActive(false);
        cloneObject.transform.localPosition = new Vector3(NoteX[place], NoteY, NoteZ);
        cloneObject.name = num.ToString();
        cloneObject.GetComponent<NoteController>().SetRailNumber(place);
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
}
