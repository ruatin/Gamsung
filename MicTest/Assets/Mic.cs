using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mic : MonoBehaviour {
    public string microphone;
    AudioSource audiosource;

   


    void Start () {
        audiosource = GetComponent<AudioSource>();

        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            if(microphone ==null)
            {
                microphone = Microphone.devices[i];
            }
            

        }
        StartMic();
	}
	void StartMic()
    {
        audiosource.Stop();

        audiosource.clip = Microphone.Start(microphone, true, 10, 44100);
        audiosource.loop = true;

        if(Microphone.IsRecording(microphone))
        {
            while(!(Microphone.GetPosition(microphone)>0))
            { }
            Debug.Log("녹음 시작" + microphone);
            audiosource.Play();
        }
        else
        {
            Debug.Log("녹음 실패" + microphone);
        }
    }
	


}
