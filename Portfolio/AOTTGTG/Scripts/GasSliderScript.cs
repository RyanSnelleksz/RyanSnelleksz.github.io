using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GasSliderScript : MonoBehaviour
{
    public GameObject playerObject;
    Slider slider;
    PlayerMovement playerMovementScript;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementScript = playerObject.GetComponent<PlayerMovement>();
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = playerMovementScript.maxGas;
        slider.minValue = playerMovementScript.minGas;

        slider.value = playerMovementScript.currentGas;
    }
}
