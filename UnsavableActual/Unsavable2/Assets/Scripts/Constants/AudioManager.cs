using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public AudioInfo[] audioInfoList;

    public static AudioManager instance;

    // Use this for initialization
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (AudioInfo audioInfo in audioInfoList)
        {
            audioInfo.source = gameObject.AddComponent<AudioSource>();
            audioInfo.source.clip = audioInfo.clip;
            audioInfo.source.volume = audioInfo.volume;
            audioInfo.source.loop = audioInfo.loop;
        }
    }

    public void Play(string name)
    {
        AudioInfo audioInfos = Array.Find(audioInfoList, audioInfo => audioInfo.name == name);

        if (audioInfos == null)
        {
            return;
        }

        audioInfos.source.Play();
    }

    public void Stop(string name)
    {
        AudioInfo audioInfos = Array.Find(audioInfoList, audioInfo => audioInfo.name == name);

        if (audioInfos == null)
        {
            return;
        }

        audioInfos.source.Stop();
    }

    public IEnumerator Fade(string name)
    {
        AudioInfo audioInfos = Array.Find(audioInfoList, audioInfo => audioInfo.name == name);

        float volumeChange = (0 - audioInfos.volume) / 60;

        for (int i = 0; i < 60; i++)
        {
            audioInfos.source.volume += volumeChange;
            yield return null;
        }

    }

}
