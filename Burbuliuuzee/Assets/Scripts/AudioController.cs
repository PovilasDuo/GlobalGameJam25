using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private GameObject catMeowContainer;

    [SerializeField] private AudioSource[] audioClips;
    [SerializeField] private int minWaitTime = 2;
    [SerializeField] private int maxWaitTime = 10;

    private float nextMeowTime = 0f;
    private System.Random rand = new System.Random();

    private void Start()
    {
        audioClips = catMeowContainer.GetComponents<AudioSource>();
    }

    void Update()
    {
        if (Time.time > nextMeowTime)
        {
            PlayRandomCatMeow();
            nextMeowTime = Time.time + rand.Next(minWaitTime, maxWaitTime);
        }
    }

    public void PlayRandomCatMeow()
    {
        StopOtherAudioSources();
        audioClips[rand.Next(0, audioClips.Length - 1)].Play();
    }

    private void StopOtherAudioSources()
    {
        foreach (AudioSource source in audioClips) source.Stop();
    }
}
