using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Ambience : MonoBehaviour
{
    public AudioSource wanderSource;
    public AudioClip wanderAudio;
    public AudioSource chasingSource;
    public AudioClip chasingAudio;
    public AudioSource randomSource;
    public AudioClip randomAudio;

    public AudioMixerGroup ambienceMixer;

    float volumeMin = 0.0f;
    float volumeMax = 1.0f;

    float volumeIncrease = 0.3f;
    float volumeDecrease = 0.5f;

    float chaseLerp = 0.0f;
    float wanderLerp = 0.0f;

    public DecisionMaking decisionMaking;

    public bool chasing = false;

    int randonAmbience = 0;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        wanderSource.clip = wanderAudio;
        chasingSource.clip = chasingAudio;
        randomSource.clip = randomAudio;

        randonAmbience = Random.Range(100, 200);
    }

    // Update is called once per frame
    void Update()
    {
        if(decisionMaking.isChasing)
        {
            chasingSource.volume = Mathf.Lerp(volumeMin, volumeMax, chaseLerp);
            wanderSource.volume = Mathf.Lerp(volumeMin, volumeMax, wanderLerp);
            chaseLerp += volumeIncrease * Time.deltaTime;
            wanderLerp -= volumeDecrease * Time.deltaTime;
            chaseLerp = Mathf.Clamp(chaseLerp, 0.0f, 1.0f);
            wanderLerp = Mathf.Clamp(wanderLerp, 0.0f, 1.0f);
            if(!chasingSource.isPlaying)
            {
                chasingSource.Play();
            }
        }

        if (!decisionMaking.isChasing)
        {
            chasingSource.volume = Mathf.Lerp(volumeMin, volumeMax, chaseLerp);
            wanderSource.volume = Mathf.Lerp(volumeMin, volumeMax, wanderLerp);
            chaseLerp -= volumeDecrease * Time.deltaTime;
            wanderLerp += volumeIncrease * Time.deltaTime;
            chaseLerp = Mathf.Clamp(chaseLerp, 0.0f, 1.0f);
            wanderLerp = Mathf.Clamp(wanderLerp, 0.0f, 1.0f);
            if(!wanderSource.isPlaying)
            {
                wanderSource.Play();
            }
        }

        timer += Time.deltaTime;
        int seconds = (int)timer % 60;
        AmbienceTimer(seconds);
    }

    void AmbienceTimer(int second)
    {
        if(second == randonAmbience)
        {
            randomSource.Play();
            timer = 0.0f;
            randonAmbience = Random.Range(100, 200);
        }
    }
}