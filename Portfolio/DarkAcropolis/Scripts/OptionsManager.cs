using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class OptionsManager : MonoBehaviour
{
    [Header("Resolution")]
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Toggle toggleOn;
    int currentWidth;
    int currentHeight;
    int currentIndex = 18;

    [Header("Gamma")]
    [SerializeField] Slider gammaSlider;
    [SerializeField] Volume volume;
    ColorAdjustments brightness;

    [Header("Audio")]
    public AudioMixer mixer;


    List<int> widths = new List<int>() {640, 720, 720, 800, 1024, 1152, 1280, 1280, 1280,
                                        1280, 1280, 1360, 1366, 1440, 1600, 1600, 1680, 1920};
    List<int> heights = new List<int>() {480, 480, 576, 600, 768, 864, 720, 768, 800, 960,
                                        1024, 768, 768, 900, 900, 1024, 1050, 1080 };

    // Start is called before the first frame update
    void Start()
    {
        if(Time.timeScale == 0.0f)
        {
            Time.timeScale = 1.0f;
        }
        volume.profile.TryGet<ColorAdjustments>(out brightness);     

        //See if this is the first time running the script
        //If run before do else
        if(PlayerPrefs.GetInt("runTimes", 1) == 1)
        {
            //Set the gamma slider to the middle
            //Sets the resolution to index from the list of resolutions
            //Set run time to 0 so the if statement won't run during this play again
            gammaSlider.value = -1.0f;
            resolutionDropdown.value = currentIndex;
            Debug.Log("First");
            PlayerPrefs.SetInt("runTimes", 0);
        }
        else
        {
            //Sets gamma slider value to the save data from other sences gamma slider
            //Sets the resolution to the save data from other sences resolution
            //Sets the dropdown index to the save data from the other scnecs dropdown index
            gammaSlider.value = PlayerPrefs.GetFloat("save", gammaSlider.value);
            Screen.SetResolution(PlayerPrefs.GetInt("resolutionWidth", currentWidth), 
                                PlayerPrefs.GetInt("resolutionHeight", currentHeight), Screen.fullScreen);
            resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentIndex);

            if(Screen.fullScreen == false)
            {
                toggleOn.isOn = false;
            }
            else
            {
                toggleOn.isOn = true;
            }

            Debug.Log("Next Time");

            mixer.ClearFloat("MusicVolume");
            mixer.ClearFloat("PlayerVolume");
            mixer.ClearFloat("MonsterVolume");
            mixer.ClearFloat("AmbienceVolume");
        }
    }

    void Update()
    {
        //Update the brightness to equal the value of the gamma slider value
        brightness.postExposure.value = gammaSlider.value;
    }

    //Changes fullscreen on and off depending on the checkbox
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    //Sets the resolution when change and updates the playerprefs to save the data
    public void SetResolution(int index)
    {
        bool fullScreen = Screen.fullScreen;
        currentWidth = widths[index];
        currentHeight = heights[index];
        currentIndex = index;
        Screen.SetResolution(currentWidth, currentHeight, fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", currentIndex);
        PlayerPrefs.SetInt("resolutionWidth", currentWidth);
        PlayerPrefs.SetInt("resolutionHeight", currentHeight);
    }

    //Updates the playerprefs of the gamma slider when the slider value changes
    public void SaveSliderValue(float value)
    {
        gammaSlider.value = value;
        PlayerPrefs.SetFloat("save", gammaSlider.value);
    }

    //When application close reset runtime to run the first time if statement
    //Set the gamma slider to the middle
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("runTimes", 1);
        PlayerPrefs.SetFloat("save", gammaSlider.maxValue - gammaSlider.minValue );
    }
}