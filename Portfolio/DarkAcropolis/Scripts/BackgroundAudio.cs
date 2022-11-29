using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    public AudioClip[] sounds;

    private void Start()
    {
        PlaySound(0);
    }

    void PlaySound(int index)
    {
        GetComponent<AudioSource>().PlayOneShot(sounds[index]);
    }
}
