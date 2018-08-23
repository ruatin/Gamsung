using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : pitchdectecter
{

    private bool micgo;
    public GameObject bar;//막대 원본

    public Transform barsParent;//막대 부모

    private RectTransform[] bars = new RectTransform[64];//막대들

    private AudioSource audioSur;//오디오 소스

    public float[] samples = new float[64]; //샘플//샘플수는 64가 최소 2의 거듭제곱으로 해야함

    private float time;
    
    public float timeSpeed = 40;

    public float defaultValue = 0 ;


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

        getAnalyzeSound();

        //기본 음정키 설정시 스페이스바 누르고 5초
        if(Input.GetKeyDown(KeyCode.Space) )
        {
            StartCoroutine("defaultVoice");
        }
        if(micgo)
        checkpitch();

        //실제 ui바의 포지션이 변화하는 곳
        bars[0].localPosition = new Vector2(time * timeSpeed, PitchValue);
        

    }
    //스페이스바가 눌렀을시 사용되는 코루틴문 시간이 초기화 되며
    //5초동안 소리를 내서 기본 음정키를 입력한다.
    IEnumerator defaultVoice()
    {
        time = 0;
        
        while(time <3)
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
        micgo = true;

        StopCoroutine("defaultVoice");
    }

    //음정 영역대 체크
    void checkpitch()
    {
        
        if (PitchValue > defaultValue + 1400)
        {
            PitchValue = 120;
        }
        else if (PitchValue > defaultValue + 1100)
        {
            PitchValue = 100;
        }
        else if (PitchValue > defaultValue + 800)
        {
            PitchValue = 80;
        }
        else if (PitchValue > defaultValue + 650)
        {
            PitchValue = 60;
        }
        else if (PitchValue > defaultValue + 500)
        {
            PitchValue = 40;
        }
        else if (PitchValue > defaultValue + 200)
        {
            PitchValue = 20;
        }
        else
        {
            PitchValue = 0;
        }
    }

    
   
}
