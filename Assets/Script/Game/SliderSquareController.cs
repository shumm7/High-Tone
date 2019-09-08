using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderSquareController : MonoBehaviour
{
    public float width = 1.05f;
    public float length = 0;
    public int StartRail = 1;
    public int EndRail = 1;
    float[] posX = { -2.9f, -1.45f, 0, 1.45f, 2.9f };

    public float speed;
    public double ArrivalTime;
    public double FinishTime;
    double SPB;
    float scrolledTime;
    int count = 1;
    public float Mask;
    private int BPM;
    private int LPB;
    private float DetectionLine;

    bool flag = false;

    public AudioSource audioSource;
    bool seFlag = false;

    //mesh
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private Mesh mesh;
    private MeshRenderer meshRenderer;


    void FixedUpdate()
    {

        if (this.gameObject.activeSelf)
        {
            this.transform.localPosition -= new Vector3(0, 1f, 0) * speed * Time.fixedDeltaTime;

            float y = transform.localPosition.y;
            bool pressed = isPressing(GetHitting(y));

            if (y <= DetectionLine)
            {
                Mask += speed * Time.fixedDeltaTime;
                if(!flag && pressed){
                    flag = true;
                    seFlag = true;
                    audioSource.Play();
                }
            }
            
            //描画処理
            if (flag)
            {
                setMesh(width, length, Mask - 0.075f);
                CreateMesh();
            }
            if (y + length < -2.5f)
                gameObject.SetActive(false);

            //効果音
            if (seFlag &&( y + length < DetectionLine || !pressed) ) {
                audioSource.Stop();
                seFlag = false;
            }
            if(pressed && !seFlag){
                audioSource.Play();
                seFlag = true;
            }

            //判定処理
            scrolledTime = (float)(TimeComponent.GetCurrentTimePast() - ArrivalTime);
            if (scrolledTime >= SPB * count && scrolledTime + ArrivalTime <= FinishTime)
            {
                count++;

                if (pressed)
                {
                    ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Perfect, 1);
                }
                else
                {
                    if (flag) //一度押している
                    {
                        ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Bad, 1);
                    }
                    else //一度も押していない
                    {
                        ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Miss, 1);
                    }
                }
            }

        }
    }

    void setMesh(float width, float length, float mask)
    {
        int posDiff = EndRail - StartRail;
        
        float wholeWidth = (System.Math.Abs(posDiff) + 1) * width;
        double angle = System.Math.Atan(length / (System.Math.Abs(posDiff) * width));

        if (posDiff >= 0) {
            float maskX = mask / (float)System.Math.Tan(angle);

            vertices = new Vector3[] {
                //上辺
                new Vector3 ((wholeWidth / 2f) - width, length, 0f),
                new Vector3 (wholeWidth / 2f, length, 0f),

                //底辺
                new Vector3 (-wholeWidth / 2f + maskX, 0f + mask, 0f),
                new Vector3 (-(wholeWidth / 2f) + width + maskX, 0f + mask, 0f)
            };
        }
        else
        {
            float maskX = -mask / (float)System.Math.Tan(angle);


            vertices = new Vector3[] {
                //上辺
                new Vector3 (-wholeWidth / 2f, length, 0f),
                new Vector3 (-(wholeWidth / 2f) + width, length, 0f),

                //底辺
                new Vector3 ((wholeWidth / 2f) - width + maskX,  0f + mask, 0f),
                new Vector3 (wholeWidth / 2f + maskX,  0f + mask, 0f)
            };
        }
        uvs = new Vector2[] {
            new Vector2 ((wholeWidth / 2f) - width, length),
            new Vector2 (wholeWidth / 2f, length),
            new Vector2 (-wholeWidth / 2f, 0f),
            new Vector2 (-(wholeWidth / 2f) + width, 0f)
        };
        triangles = new int[] {
            0, 3, 2,
            1, 3, 0
        };


        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        mesh = meshFilter.mesh;
    }

    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    void SetPosition()
    {
        Vector3 localPos = transform.localPosition;
        localPos.x = (posX[StartRail] + posX[EndRail]) / 2f;
        transform.localPosition = localPos;
    }

    int GetHitting(float y)
    {
        if (y > DetectionLine)
        {
            return -1;
        }
        else if(y <= DetectionLine && (y + length) >= DetectionLine)
        {
            float diffLength = DetectionLine - y;
            int railDiff = (System.Math.Abs(EndRail - StartRail) + 1) * 3 - 2;

            float l = length / railDiff;
            int step = (int)(diffLength / l);

            if(StartRail <= EndRail)
            {
                return StartRail * 3 + 1 + step;
            }
            else
            {
                return StartRail * 3 + 1 - step;
            }
        }
        else
        {
            return -1;
        }
    }

    bool isPressing(int button)
    {
        if (button == -1)
            return false;

        for(int i=-2; i<=2; i++)
        {
            if (TimeComponent.isKeyPressing(TimeComponent.GetKeyRail(Range(button + i, 0, 14))))
                return true;
        }
        return false;

     }

    public void setParam(float Speed, double arrivalTime, double finishTime, int bpm, int lpb, int startRail, int endRail)
    {
        speed = Speed;
        ArrivalTime = arrivalTime;
        FinishTime = finishTime;
        BPM = bpm;
        LPB = lpb;

        SPB = ((double)60 / ((double)BPM * (double)LPB));
        length = speed * (float)(FinishTime - ArrivalTime);

        StartRail = Range(startRail, 0, 4);
        EndRail = Range(endRail, 0, 4);

        SetPosition();
        setMesh(width, length, Mask);
        CreateMesh();

        DetectionLine = GameManager.DetectionLineY;
        audioSource = transform.parent.GetComponents<AudioSource>()[1];
    }

    private int Range(int num, int min, int max)
    {
        if (num < min)
            num = min;
        if (num > max)
            num = max;

        return num;
    }
}

