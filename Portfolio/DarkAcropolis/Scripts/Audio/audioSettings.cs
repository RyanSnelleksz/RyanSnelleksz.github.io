using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class audioSettings : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] AudioMixer mixer;

    [Header("Music")]
    [Tooltip("Music volume slider")]
    [SerializeField] Slider musicSlider;
    [Tooltip("Music audio mixer group")]
    [SerializeField] string musicMixer = "MusicVolume";

    [Header("Player")]
    [Tooltip("Player volume slider")]
    [SerializeField] Slider playerSlider;
    [Tooltip("Player audio mixer group")]
    [SerializeField] string playerMixer = "PlayerVolume";

    [Header("Monster")]
    [Tooltip("Monster volume slider")]
    [SerializeField] Slider monsterSlider;
    [Tooltip("Monster audio mixer group")]
    [SerializeField] string monsterMixer = "MonsterVolume";

    [Header("Ambience")]
    [Tooltip("Ambience volume slider")]
    [SerializeField] Slider ambienceSlider;
    [Tooltip("Ambience audio mixer group")]
    [SerializeField] string ambienceMixer = "AmbienceVolume";

    [Header("variables")]
    [Tooltip("slider multipler")]
    [SerializeField] float multipler = 30.0f;

    //On awake that changes the audio mixer when the slider value is change
    void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        playerSlider.onValueChanged.AddListener(SetPlayerVolume);
        monsterSlider.onValueChanged.AddListener(SetMonsterVolume);
        ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);
    }

    //Set the slider value to save data if there is any. Else set to 1.0f
    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(musicMixer, 1.0f);
        playerSlider.value = PlayerPrefs.GetFloat(playerMixer, 1.0f);
        monsterSlider.value = PlayerPrefs.GetFloat(monsterMixer, 1.0f);
        ambienceSlider.value = PlayerPrefs.GetFloat(ambienceMixer, 1.0f);
    }

    //Save the data of the mixer value when sliders are disabled
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(musicMixer, musicSlider.value);
        PlayerPrefs.SetFloat(playerMixer, playerSlider.value);
        PlayerPrefs.SetFloat(monsterMixer, monsterSlider.value);
        PlayerPrefs.SetFloat(ambienceMixer, ambienceSlider.value);
    }

    //Set the new music mixer value
    void SetMusicVolume(float value)
    {
        mixer.SetFloat(musicMixer, Mathf.Log10(value) * multipler);
    }

    //Set the new player mixer value
    public void SetPlayerVolume(float value)
    {
        mixer.SetFloat(playerMixer, Mathf.Log10(value) * multipler);
    }
    
    //Set the new monster mixer value
    public void SetMonsterVolume(float value)
    {
        mixer.SetFloat(monsterMixer, Mathf.Log10(value) * multipler);
    }
    
    //Set the new ambience mixer value
    public void SetAmbienceVolume(float value)
    {
        mixer.SetFloat(ambienceMixer, Mathf.Log10(value) * multipler);
    }
}