using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataTraveller : MonoBehaviour
{
    public int BPM = 100;
    public float arrowSpeed = 5;

    public AudioClip song;
    public string songDuration;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BPM = 100;
        arrowSpeed = 2;
    }
    
    public IEnumerator GetAudioClip(string text)
    {
        string url = string.Format(text);
        Debug.Log(url);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.ConnectionError)
            {
                song = DownloadHandlerAudioClip.GetContent(www);

                var min = Mathf.FloorToInt(DownloadHandlerAudioClip.GetContent(www).length / 60f);
                var sec = Mathf.FloorToInt(DownloadHandlerAudioClip.GetContent(www).length - min * 60);

                string x = string.Format("Duration : {0:0}m {1:00}s", min, sec);
                
                songDuration = x;
                
                Debug.Log(DownloadHandlerAudioClip.GetContent(www).name);
            }
        }
    }

    public void StartSound()
    {
        GetComponent<AudioSource>().clip = song;
        GetComponent<AudioSource>().Play();
    }
}
