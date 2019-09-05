using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderNoteController : MonoBehaviour
{

    public float speed;
    [SerializeField] int Rail;
    [SerializeField] double ArrivalTime;
    [SerializeField] GameObject tappedEffectBadPrefab;
    [SerializeField] GameObject tappedEffectGoodPrefab;
    [SerializeField] GameObject tappedEffectGreatPrefab;
    [SerializeField] GameObject tappedEffectPerfectPrefab;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip NotePerfectSE;
    [SerializeField] AudioClip NoteGreatSE;
    [SerializeField] AudioClip NoteGoodSE;
    //[SerializeField] AudioClip NoteBadSE;
    //[SerializeField] AudioClip NoteMissSE;


    public static float[] EffectX = { -2.6f, -1.3f, 0, 1.3f, 2.6f };
    public static float EffectY = -1.4f;
    public static float EffectZ = -5f;
    private double Distance; //初期位置から判定ラインまでの到着時間

    void Start()
    {
        speed = GameManager.NoteSpeed;
        Distance = GameManager.ArrivalTime;
        audioSource = transform.parent.gameObject.GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (this.gameObject.activeSelf)
        {
            this.transform.localPosition -= new Vector3(0, 1f, 0) * speed * Time.fixedDeltaTime;
        }
    }

    void Update()
    {
        if (this.transform.position.y < -2.5f)
        {
            this.gameObject.SetActive(false);
            ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Miss, 1);
        }

        if (this.gameObject.activeSelf)
        {
            double diff = System.Math.Abs(TimeComponent.GetPressedKeyTime(Rail) - ArrivalTime);
            if (diff <= 0.16 && diff > 0.13) //Bad
            {
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Bad, 1);
                AddEffect(ScoreCalculation.Judgement.Bad);

                //GameManager.DebugLog(this.gameObject.name + "番のノーツ: Bad");
            }
            else if (diff <= 0.13 && diff > 0.10) //Good
            {
                audioSource.PlayOneShot(NoteGoodSE);
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Good, 1);
                AddEffect(ScoreCalculation.Judgement.Good);
                //GameManager.DebugLog(this.gameObject.name + "番のノーツ: Good");
            }
            else if (diff <= 0.10 && diff > 0.06) //Great
            {
                audioSource.PlayOneShot(NoteGreatSE);
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Great, 1);
                AddEffect(ScoreCalculation.Judgement.Great);
                //GameManager.DebugLog(this.gameObject.name + "番のノーツ: Great");
            }
            else if (System.Math.Abs(ArrivalTime - TimeComponent.GetCurrentTimePast())<=0.06 && TimeComponent.isKeyPressing(Rail)) //Perfect
            {
                audioSource.PlayOneShot(NotePerfectSE);
                this.gameObject.SetActive(false);
                ScoreCalculation.SetNoteJudgement(ScoreCalculation.Judgement.Perfect, 1);
                AddEffect(ScoreCalculation.Judgement.Perfect);
                //GameManager.DebugLog(this.gameObject.name + "番のノーツ: Perfect");
            }
        }
    }

    public void SetRailNumber(int num)
    {
        Rail = num;
    }

    public void SetArrivalTime(double Time)
    {
        ArrivalTime = Time;
    }

    private void AddEffect(int judgement)
    {
        switch (judgement)
        {
            case ScoreCalculation.Judgement.Perfect:
                Instantiate(tappedEffectGreatPrefab, new Vector3(EffectX[Rail], EffectY, EffectZ), Quaternion.identity);
                Instantiate(tappedEffectPerfectPrefab, new Vector3(EffectX[Rail], EffectY, EffectZ), Quaternion.identity);
                break;
            case ScoreCalculation.Judgement.Great:
                Instantiate(tappedEffectGreatPrefab, new Vector3(EffectX[Rail], EffectY, EffectZ), Quaternion.identity);
                break;
            case ScoreCalculation.Judgement.Good:
                Instantiate(tappedEffectGoodPrefab, new Vector3(EffectX[Rail], EffectY, EffectZ), Quaternion.identity);
                break;
            case ScoreCalculation.Judgement.Bad:
                Instantiate(tappedEffectBadPrefab, new Vector3(EffectX[Rail], EffectY, EffectZ), Quaternion.identity);
                break;
            default:
                break;
        }
    }
}
