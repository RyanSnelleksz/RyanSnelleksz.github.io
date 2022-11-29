using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Heart : MonoBehaviour
{
    [Header("Heart")]
    [Tooltip("The Heart")]
    public GameObject heart;
    [Tooltip("Time to destroy the heart")]
    public float heartDeletion = 3.0f;
    [Tooltip("Put in the door to the heart")]
    public GameObject heartDoor;
    [Tooltip("Stop running the update of destroying the seal")]
    bool heartDestroy = true;

    [Header("Seal")]
    [Tooltip("List of seals")]
    public List<Seal> sealList;
    int sealCount;

    [Header("Director")]
    [SerializeField] Director myDirector;

    [Header("Menu")]
    public GameObject endingLetter;

    [Header("Audio")]
    public AudioSource heartDamageSource;
    public AudioClip[] heartDamageAudio;

    public AudioSource heartBeatSource;
    public AudioClip heartBeatAudio;

    [Header("Audio Mixer")]
    public AudioMixerGroup heartMixer;

    int heartDamageIndex;

    // Start is called before the first frame update
    void Start()
    {
        sealCount = FindObjectsOfType<Seal>().Length;
        sealList = new List<Seal>(GameObject.FindObjectsOfType<Seal>());

        myDirector = FindObjectOfType<Director>();

        heartDamageSource.outputAudioMixerGroup = heartMixer;

        heartBeatSource.outputAudioMixerGroup = heartMixer;
    }

    private void Awake()
    {
        heartBeatSource.clip = heartBeatAudio;
        heartBeatSource.loop = true;
        heartBeatSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Insert Heart Idle here that plays based on proximity
        if (heartDeletion <= 0 && heartDestroy)
        {
            while (heartDestroy) // originally said true but it was causing endless loop
            {
                int i = Random.Range(0, heartDamageAudio.Length);
                if (i != heartDamageIndex)
                {
                    heartDamageSource.clip = heartDamageAudio[i];
                    heartDamageSource.Play();
                    heartDamageIndex = i;
                    break;
                }
                HideHeart();
                endingLetter.SetActive(true); // Moved stuff here ~ Ryan
                heartDestroy = false;
            }
            //HideHeart();
            //endingLetter.SetActive(true);
            //heartDestroy = false;
        }

        if (sealCount == 0)
        {
            heartDoor.SetActive(false);
        }
    }

    //Tells the heart a seal was destroyed
    public void ReportSealDestroyed(Seal sealIndex)
    {
        sealList.Remove(sealIndex);
        sealCount--;
        myDirector.IncreaseMenanceDecrease();
    }

    //if all seals are destroyed allows the heart to be destroyable
    public bool HeartLocked()
    {
        if (sealCount < 1)
        {
            return true;
        }
        return false;
    }

    void HideHeart()
    {
        GameObject.Find("heart Variant").SetActive(false);
    }
}