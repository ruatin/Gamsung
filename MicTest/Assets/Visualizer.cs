using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    public float RmsValue;
    public float DbValue;
    public float PitchValue;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    public GameObject bar;//막대 원본

    public Transform barsParent;//막대 부모

    private RectTransform[] bars = new RectTransform[64];//막대들

    private AudioSource audioSur;//오디오 소스

    public float[] samples = new float[64]; //샘플//샘플수는 64가 최소 2의 거듭제곱으로 해야함

    private float time;
    
    public float timeSpeed = 40;

    public float defaultValue = 0 ;

    private void Start()
    {
        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;
    }
    private void OnEnable()
    {
        

            bars[0] = (Instantiate(bar) as GameObject).GetComponent<RectTransform>();//막대생성
            bars[0].parent = barsParent;//부모설정
            bars[0].localPosition = new Vector2( 0, 0);//포지션설정

            //색깔 랜덤
            bars[0].GetComponent<Image>().color =
                new Vector4(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);

            bars[0].sizeDelta = new Vector2(20, 5);//사이즈 초기화
        

        audioSur = GetComponent<AudioSource>();//오디오 소스 가지고오기

        audioSur.Play();//플레이

    }

    private void Update()
    {
        time += Time.deltaTime;

        AnalyzeSound();

        //기본 음정키 설정시 스페이스바 누르고 5초
        if(Input.GetKeyDown(KeyCode.Space) )
        {
            StartCoroutine("defaultVoice");
        }

        //음정 영역대
        if (PitchValue > defaultValue+ 1400)
        {
            PitchValue = 120;
        }
        else if (PitchValue > defaultValue+ 1100)
        {
            PitchValue = 100;
        }
        else if (PitchValue > defaultValue+ 800)
        {
            PitchValue = 80;
        }
        else if (PitchValue > defaultValue+ 650)
        {
            PitchValue = 60;
        }
        else if (PitchValue > defaultValue+ 500)
        {
            PitchValue = 40;
        }
        else if (PitchValue > defaultValue+ 350)
        {
            PitchValue = 20;
        }
        else
        {
            PitchValue = 0;
        }

        //실제 ui바의 포지션이 변화하는 곳
        bars[0].localPosition = new Vector2(time * timeSpeed, PitchValue);
        

    }
    //스페이스바가 눌렀을시 사용되는 코루틴문 시간이 초기화 되며
    //5초동안 소리를 내서 기본 음정키를 입력한다.
    IEnumerator defaultVoice()
    {
        time = 0;
        
        while(time <5)
        {
            if (PitchValue > 0)
            {
                defaultValue = PitchValue;
            }
            else
            {
                Debug.Log("다시 해주세요");
            }
            yield return new WaitForSeconds(0.1f);
        }

        
        StopCoroutine("defaultVoice");
    }

    //DB 및 Pitch 디텍트 기능
    void AnalyzeSound()
    {
        GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min


        // get sound spectrum
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        {
            // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency


    }
}
