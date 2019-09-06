using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderSquareController : MonoBehaviour
{
    public float width;
    public float length;
    public int StartRail = 1;
    public int EndRail = 1;
    float[] posX = { -2.9f, -1.45f, 0, 1.45f, 2.9f };

    public float speed;
    public double ArrivalTime;
    public double FinishTime;
    private int BPM;
    private int LPB;
    private float DetectionLine;

    public AudioSource audioSource;
    public AudioClip SliderSE;

    //mesh
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    public Material material;


    void Awake()
    {
        audioSource = transform.parent.gameObject.GetComponent<AudioSource>();
        StartRail = Range(StartRail, 1, 5);
        EndRail = Range(EndRail, 1, 5);

        SetPosition();
        setMesh(width, length, StartRail, EndRail);
        CreateMesh();

        DetectionLine = GameManager.DetectionLineY;
    }



    void FixedUpdate()
    {
        if (this.gameObject.activeSelf)
        {
            this.transform.localPosition -= new Vector3(0, 1f, 0) * speed * Time.fixedDeltaTime;
        }
        Debug.Log(GetHitting(transform.localPosition.y));
        Debug.Log(isPressing(GetHitting(transform.localPosition.y)));
    }

    void setMesh(float width, float length, int startRail, int endRail)
    {
        int posDiff = EndRail - StartRail;
        
        float wholeWidth = (System.Math.Abs(posDiff) + 1) * width;

        if (posDiff >= 0) {
            vertices = new Vector3[] {
                //上辺
                new Vector3 ((wholeWidth / 2f) - width, length, 0f),
                new Vector3 (wholeWidth / 2f, length, 0f),

                //底辺
                new Vector3 (-wholeWidth / 2f, 0f, 0f),
                new Vector3 (-(wholeWidth / 2f) + width, 0f, 0f)
            };
        }
        else
        {
            vertices = new Vector3[] {
                //上辺
                new Vector3 (-wholeWidth / 2f, length, 0f),
                new Vector3 (-(wholeWidth / 2f) + width, length, 0f),

                //底辺
                new Vector3 ((wholeWidth / 2f) - width, 0f, 0f),
                new Vector3 (wholeWidth / 2f, 0f, 0f)
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

    public void SetPosition()
    {
        Vector3 localPos = transform.localPosition;
        localPos.x = (posX[StartRail - 1] + posX[EndRail - 1]) / 2f;
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
                return (StartRail - 1) * 3 + 1 + step;
            }
            else
            {
                return (StartRail - 1) * 3 + 1 - step;
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

        for(int i=-3; i<=3; i++)
        {
            button = Range(button + i, 0, 14);
            if (TimeComponent.isKeyPressingDetailed(button))
                return true;
        }

        return false;
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

