using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Seal : MonoBehaviour
{
    [Header("Seal")]
    [Tooltip("Time to destroy the seal")]
    public float sealDeletion = 5.0f;

    [Header("Heart")]
    [Tooltip("Reference to the heart script")]
    Heart theHeartReference;
    [Tooltip("Seal that surround the heart")]
    [SerializeField] GameObject heartPiece;

    [Header("Flash")]
    [Tooltip("Reference to the flash script")]
    Flash flashReference;

    [Header("Particles")]
    [Tooltip("Explosion after the seal is destroyyed")]
    public GameObject explosionPrefab;
    [Tooltip("Particles that play when destroying the seal")]
    public GameObject buildUpPrefab;

    [Header("Audio")]
    public AudioSource sealStereoSource;
    public AudioClip[] sealBreakAudio;
    public AudioClip sealBuildUpAudio;

    public AudioSource sealSurroundSource;
    public AudioClip sealShimmerAudio;

    int sealBreakIndex;

    [Header("Audio Mixer")]
    public AudioMixerGroup sealMixer;

    public bool audioReset = false;

    [Header("Emmission")]
    [Tooltip("Set it to a value from 0 to 10. What insensity you want the emmission to start at.")]
    public float baseIntensity = 5.0f;
    float intensity = 0.0f;

    [Header("Monster Spawn")]
    [Tooltip("If true then when this seal is destroyed a monster will be spawned")]
    public bool enableMonster = false;
    [Tooltip("Put in the monster")]
    public GameObject monster;

    [Header("Seal volume fade")]
    [Tooltip("Fades out the shimmer when broken")]
    public FadeSealAudio fadeSealAudio;

    Material material;
    Color emissionColour;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.green;
        theHeartReference = FindObjectOfType<Heart>();
        flashReference = FindObjectOfType<Flash>();
        //monster = GameObject.Find("Monster");

        sealStereoSource.clip = sealBuildUpAudio;
        sealStereoSource.outputAudioMixerGroup = sealMixer;

        sealSurroundSource.outputAudioMixerGroup = sealMixer;

        material = GetComponent<Renderer>().material;
        emissionColour = GetComponent<Renderer>().material.color;
    }

    void Awake()
    {
        sealSurroundSource.clip = sealShimmerAudio;
        sealSurroundSource.loop = true;
        sealSurroundSource.Play();
    }

    void Update()
    {
        //Check if seal Deletion equals zero, if so pass gameobject into function
        if (sealDeletion <= 0.0f)
        {
            SealHit(gameObject);
        }

        material.SetFloat("_EmissiveExposureWeight", 1.0f - intensity);
    }

    public void RaiseIntensity(float value)
    {
        intensity += value / 10;
    }

    //Called when the seal is to be destroyed
    public void SealHit(GameObject obj)
    {
        if (enableMonster == true)
        {
            monster.SetActive(true);
        }
        SealFade();
        //Plays seal breaking sound
        while(true)
        {
            int i = Random.Range(0, sealBreakAudio.Length);
            if(i != sealBreakIndex)
            {
                sealStereoSource.clip = sealBreakAudio[i];
                sealStereoSource.Play();
                sealBreakIndex = 1;
                break;
            }
        }
        sealStereoSource.clip = sealBreakAudio[2];
        sealStereoSource.Play();
        //Tell the heart a seal was destroyed
        theHeartReference.ReportSealDestroyed(this);
        //Instantiate explosion particle animation
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        //Gain charge to the flashes
        flashReference.GainCharge();
        //Destroy the heart piece connect to it and itself
        Destroy(heartPiece);
        gameObject.SetActive(false);
    }

    public void SealBuildUpSound()
    {
        sealStereoSource.Play();
    }

    public void SealFade()
    {
        fadeSealAudio.DecreaseVolume(gameObject);
    }
}