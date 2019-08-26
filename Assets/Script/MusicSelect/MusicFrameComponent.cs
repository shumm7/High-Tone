using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MusicFrameComponent : MonoBehaviour
{
    public Text MusicTitleUI;
    public Text CreditsUI;
    public RawImage ArtworkUI;
    public Text BPMUI;
    public Text LevelUI;
    public Text TimeUI;

    public Texture[] FrameTexture = new Texture[5];

    private AudioClip music;

    public void SetMusicData(int difficulty, string id)
    {
        try
        {
            MusicDataLoader.MusicProperty data = GetComponent<MusicDataLoader>().getMusicProperty(id);
            if (data.level[difficulty] == -1)
                return;

            //Frame
            RawImage Frame = GetComponent<RawImage>();
            Frame.texture = FrameTexture[difficulty];

            //Title
            MusicTitleUI.text = data.music;

            //Credits
            string creditsText = "";
            if (data.credits.composer != "")
                creditsText += ("作曲 " + data.credits.composer + "     ");
            if (data.credits.lyrics != "")
                creditsText += ("作曲 " + data.credits.lyrics + "     ");
            if (data.credits.vocal != "")
                creditsText += ("歌 " + data.credits.vocal + "     ");
            CreditsUI.text = creditsText.Remove(creditsText.LastIndexOf("     "), "     ".Length);

            //Artwork
            byte[] bytes = File.ReadAllBytes("Music/" + id + "/image.png");
            Texture2D texture = new Texture2D(500, 500);
            texture.LoadImage(bytes);
            ArtworkUI.texture = texture;

            //BPM
            BPMUI.text = "BPM " + GetComponent<MusicDataLoader>().getNotesData(difficulty, id).BPM.ToString();

            //Level
            LevelUI.text = data.level[difficulty].ToString();

            //Time
            string path = "Music/" + id + "/music.wav";
            StartCoroutine(SetAudioTime(path));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private string GetAboutTime(float time)
    {
        int retTime = Mathf.RoundToInt(time / 15f) * 15;
        int minute, sec;
        minute = Mathf.FloorToInt((float)retTime / 60f);
        sec = (int)(retTime % 60f);

        return minute.ToString() + ":" + sec.ToString();
    }

    IEnumerator SetAudioTime(string path)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + Directory.GetCurrentDirectory() + "/" + path, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            music = DownloadHandlerAudioClip.GetContent(uwr);
            TimeUI.text = GetAboutTime(music.length);

        }
    }
}