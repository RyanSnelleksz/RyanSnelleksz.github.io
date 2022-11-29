using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossBow : MonoBehaviour
{
    [Header("Bolt Specs")]
    public GameObject bolt;
    public float boltSpeed = 20.0f;
    public float shootRate = 1.0f;
    public bool sprintReload = false;
    private float shootNext;

    [Header("Reload Bar")]
    public Image reloadBar;
    private bool reload = false;
    private float seconds = 1.0f;
    private float mixAmount = 0.0f;
    private float maxAmount = 1.0f;

    PlayerController playerController;

    public PauseManager pause;

    public AudioSource shootSound;

    void Start()
    {
        pause = FindObjectOfType<PauseManager>();
        playerController = gameObject.GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is not paused
        if (!pause.pausePressed)
        {
            //If left mouse button is pressed and time has passed after the bolt was instantiate
            if (Input.GetMouseButtonDown(0) && Time.time > shootNext)
            {
                //GetComponent<Animator>().SetBool("Fire", true);
                shootNext = Time.time + shootRate;

                //Instantiate the bolt that is fired from the crossbow
                GameObject boltClone = Instantiate(bolt, transform.GetChild(1).position, transform.GetChild(1).rotation);
                Rigidbody rig = boltClone.GetComponent<Rigidbody>();
                rig.AddForce(transform.GetChild(1).forward * boltSpeed, ForceMode.Impulse);

                //FindObjectOfType<AudioManager>().Play("CrossBowShoot");

                //Set the reload bar to zero
                reloadBar.fillAmount = mixAmount;
                reload = true;

                //FindObjectOfType<AudioManager>().Play("CrossBowReload");
            }
            //Debug.DrawRay(transform.GetChild(1).GetChild(1).position, transform.GetChild(1).forward * 5.0f, Color.red);
            //Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue);

            //After the bolt is fired and player is not sprinting
            if (sprintReload == false)
            {
                //Increase the value of the reload bar
                if (reload == true && playerController.sprinting == false)
                {
                    //GetComponent<Animator>().SetBool()
                    reloadBar.fillAmount += seconds / shootRate * Time.deltaTime;

                    //Once bar is full not increasing the bar
                    if (reloadBar.fillAmount == maxAmount)
                    {
                        reload = false;
                    }
                }
            }

            if (sprintReload == true)
            {
                //Increase the value of the reload bar
                if (reload == true)
                {
                    //GetComponent<Animator>().SetBool()
                    reloadBar.fillAmount += seconds / shootRate * Time.deltaTime;

                    //Once bar is full not increasing the bar
                    if (reloadBar.fillAmount == maxAmount)
                    {
                        reload = false;
                    }
                }
            }
        }
    }
}