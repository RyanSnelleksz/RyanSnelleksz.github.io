using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class audioAssets 
{
    public string name;

    public AudioClip clip;

   // [Range]
    public float volume;
   // [Range]
    public float pitch;

    public int index;

    [HideInInspector]
    public AudioSource source;
}
