using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject endingLetter;
    public GameObject playerUi;

    public bool pausePressed = false;

    public AudioMixer mixer;

    void Start()
    {
        playerUi = GameObject.Find("PlayerUI");
    }

    // Update is called once per frame
    void Update()
    {
        if(winCanvas.activeSelf == false && loseCanvas.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && pausePressed != true)
            {
                OptionPaused();
            }
        }
        if(endingLetter.activeSelf == true)
        {
            Paused();
        }
    }

    public void OptionPaused()
    {
        Debug.Log("Pause");
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        pausePressed = true;
        mixer.SetFloat("MusicVolume", -80f);
        mixer.SetFloat("PlayerVolume", -80f);
        mixer.SetFloat("MonsterVolume", -80f);
        mixer.SetFloat("AmbienceVolume", -80f);
        CameraShake.instance.PauseCamera();
    }

    public void OptionUnPaused()
    {
        Debug.Log("unPause");
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        pausePressed = false;
        mixer.ClearFloat("MusicVolume");
        mixer.ClearFloat("PlayerVolume");
        mixer.ClearFloat("MonsterVolume");
        mixer.ClearFloat("AmbienceVolume");
        CameraShake.instance.UnPauseCamera();
    }

    public void Paused()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        pausePressed = true;
        mixer.SetFloat("MusicVolume", -80f);
        mixer.SetFloat("PlayerVolume", -80f);
        mixer.SetFloat("MonsterVolume", -80f);
        mixer.SetFloat("AmbienceVolume", -80f);
        if(playerUi.activeSelf == true)
        {
            playerUi.SetActive(false);
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        pausePressed = false;
        mixer.ClearFloat("MusicVolume");
        mixer.ClearFloat("PlayerVolume");
        mixer.ClearFloat("MonsterVolume");
        mixer.ClearFloat("AmbienceVolume");
        if(!playerUi.activeSelf)
        {
            playerUi.SetActive(true);
        }
    }

}