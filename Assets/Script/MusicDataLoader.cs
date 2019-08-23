using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MusicDataLoader : MonoBehaviour
{
    public MusicList getMusicList()
    {
        string json = load(@"Music/list.json");
        MusicList temp = new MusicList();
        try
        {
            temp = JsonUtility.FromJson<MusicList>(json);
        }
        catch(Exception e)
        {
            Debug.LogWarning(e.Message);
            return null;
        }
        return temp;
    }

    public MusicProperty getMusicProperty(string id)
    {
        string dir = "Music/" + id + "/data.json";
        string json = load(@dir);
        MusicProperty temp = new MusicProperty();
        try
        {
            temp = JsonUtility.FromJson<MusicProperty>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return null;
        }
        return temp;
    }

    public Notes getNotesData(string id)
    {
        string json = load(@"Music/Notes/" + id + ".json");
        Notes temp = new Notes();
        try
        {
            temp = JsonUtility.FromJson<Notes>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return null;
        }
        return temp;
    }


    public class MusicList
    {
        public string[] music;
    }

    public class MusicProperty
    {
        public string music;
        public int notes;
        public int[] level;
        public bool video;
        public MusicPropertyCredits credits;
    }

    [Serializable]
    public class MusicPropertyCredits
    {
        public string composer;
        public string lyrics;
        public string vocal;
    }

    public class Notes
    {
        public string name;
        public int maxBlock;
        public int BPM;
        public int offset;
        public NoteInfo[] notes;
    }

    [Serializable]
    public class NoteInfo
    {
        public int LPB;
        public int num;
        public int block;
        public int type;
        [System.NonSerialized] public NoteInfo[] notes;
    }

    public bool saveFile(string Filename, string text)
    {
        if (checkExist(Filename))
        {
            StreamWriter sw = null;
            bool res;

            try
            {
                sw = new StreamWriter(@Filename, false);
                sw.WriteLine(text);
                res = true;
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Cannot open {0}", @Filename);
                Debug.LogWarning(e.Message);
                res = false;
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
            return res;
        }
        else
        {
            return false;
        }
    }

    public string load(string Filename)
    {
        StreamReader sr = null;
        string text;

        try
        {
            sr = new StreamReader(@Filename, false);
            text = sr.ReadToEnd();
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("Cannot open {0}", @Filename);
            Debug.LogWarning(e.Message);
            text = null;
        }
        finally
        {
            if (sr != null)
                sr.Close();
        }
        return text;
    }

    public bool checkExist(string Filename)
    {
        return File.Exists(@Filename);
    }

}
