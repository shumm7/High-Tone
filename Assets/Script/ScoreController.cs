using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static void SaveScore(string UserID, int difficulty, string MusicID, int Score, int Combo)
    {
        StreamWriter sw = null;
        string Filename = "Music/" + MusicID + "/score" + difficulty.ToString() + ".csv";

        try
        {
            if(LoadScore(UserID, difficulty, MusicID) == null)
            {
                sw = new StreamWriter(@Filename, true);
                sw.WriteLine(UserID + "," + Score.ToString() + "," + Combo.ToString());
                sw.Flush();
            }
            else
            {
                List<string[]> list = ReadCSV(MusicID, difficulty);
                int cnt = 0;
                foreach (string[] index in list)
                {
                    if (index[0] == UserID)
                        break;
                    cnt++;
                }

                string[] lines = File.ReadAllLines(@Filename);
                lines[cnt] = UserID + "," + Score.ToString() + "," + Combo.ToString();

                sw = new StreamWriter(@Filename, false);
                foreach(string line in lines)
                {
                    sw.WriteLine(line);
                }
                sw.Flush();
            }

        }
        catch(Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            if (sw != null)
                sw.Close();
        }
    }

    public static Score LoadScore(string UserID, int difficulty, string MusicID) {
        Score res= new Score();
        string Filename = "Music/" + MusicID + "/score" + difficulty.ToString() +".csv";

        if (checkExist(@Filename))
        {
            List<string[]> list = ReadCSV(MusicID, difficulty);
            foreach(string[] index in list)
            {
                if (index[0] == UserID)
                {
                    res.UserID = UserID;
                    res.MusicID = MusicID;
                    res.MaxScore = int.Parse(index[1]);
                    res.MaxCombo = int.Parse(index[2]);

                    return res;
                }
            }

            return null;
        }
        else
        {
            GenerateScoreData(MusicID, difficulty);
            SaveScore(UserID, difficulty, MusicID, 0, 0);
            return LoadScore(UserID, difficulty, MusicID);
        }
    }

    public static void DeleteScore(string UserID, int difficulty, string MusicID)
    {
        string Filename = "Music/" + MusicID + "/score" + difficulty.ToString() + ".csv";
        StreamWriter sw = null;

        try {
            if(LoadScore(UserID, difficulty, MusicID) != null)
            {
                string[] lines = File.ReadAllLines(@Filename);

                sw = new StreamWriter(@Filename, false);
                foreach (string line in lines)
                {
                    if(line.Split(',')[0] != UserID)
                        sw.WriteLine(line);
                }
                sw.Flush();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            if (sw != null)
                sw.Close();
        }

    }

    public static void GenerateScoreData(string MusicID, int difficulty)
    {
        StreamWriter sw = null;
        string Filename = "Music/" + MusicID + "/score" + difficulty.ToString() + ".csv";

        try
        {
            sw = new StreamWriter(@Filename, false);
            sw.WriteLine("UserID,MaxScore,MaxCombo");
            sw.WriteLine("Guest, 0, 0");
            sw.Flush();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            if (sw != null)
                sw.Close();
        }
    }

    public static bool checkExist(string Filename)
    {
        return File.Exists(@Filename);
    }

    private static List<string[]> ReadCSV(string MusicID, int difficulty)
    {
        string Filename = "Music/" + MusicID + "/score" + difficulty.ToString() + ".csv";

        string csvText = File.ReadAllText(@Filename, Encoding.GetEncoding("utf-8"));
        StringReader sr = new StringReader(csvText);
        List<string[]> list = new List<string[]>();

        while (sr.Peek() > -1)
        {
            string record = sr.ReadLine();
            string[] fields = record.Split(',');
            list.Add(fields);
        }
        sr.Close();
        return list;
    }

    public class Score
    {
        public string UserID;
        public string MusicID;
        public int MaxScore;
        public int MaxCombo;
    }
}
