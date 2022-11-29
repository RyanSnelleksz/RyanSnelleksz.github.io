using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Flash : MonoBehaviour
{
    [Header("Attachments")]
    private Transform playerCamera;
    private Director myDirector;
    private Monster monster;
    private GameObject gauntlet;
    private Material[] gauntletMaterials = new Material[4];
    private Gem[] gems = new Gem[3];
    [Tooltip("Put in the gem prefab, it's just an empty object with the gem script")]
    public GameObject gemPrefab;

    private Animator gauntletAnimator;
    bool armRaised = false;
    bool stun = false;
    bool prepareStun = false;
    float stunDelay = 0.8f;
    float currentStunDelay = 0.0f;

    [Header("Flash Values")]
    [Tooltip("Distance of the flash to hit the monster")]
    public float flashDistance = 5;
    [Tooltip("The amount of flashes")]
    public int flashAmount = 3;
    [Tooltip("Max amount of flashes")]
    public int flashMax = 3;
    [Tooltip("Amount of charges restore")]
    public int restoreAmount = 1;
    [Tooltip("Rate of speed the next flash can be used")]
    public float flashRechageRate = 5.0f;
    float flashNext;
    [Tooltip("Flash Gameobject")]
    public GameObject flash;

    [Header("Audio")]
    public AudioSource flashSource;
    public AudioClip[] flashAudio;

    public AudioMixerGroup playerFlashMixer;

    int flashIndex;

    PauseManager pause;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = gameObject.transform;

        myDirector = FindObjectOfType<Director>();
        monster = myDirector.monsterObject.GetComponent<Monster>();

        flashSource.outputAudioMixerGroup = playerFlashMixer;

        pause = FindObjectOfType<PauseManager>();

        gauntlet = GameObject.FindGameObjectWithTag("Gauntlet");

        gauntletMaterials = gauntlet.GetComponent<Renderer>().materials;

        for (int i = 0; i < flashMax; i++)
        {
            gems[i] = Instantiate(gemPrefab).GetComponent<Gem>();
        }

        for (int i = 0; i < flashMax; i++)
        {
            gems[i].SetMaterial(gauntletMaterials[i + 1]);
            //gauntletMaterials[i + 1].SetColor("_EmissiveColor", Color.white * 100.0f);
        }

        for (int i = 0; i < flashAmount; i++)
        {
            gems[i].SwapState();
        }

        gauntletAnimator = gauntlet.GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause.pausePressed)
        {
            if (stun == true)
            {
                EndStun();
            }
            if (prepareStun == true)
            {
                currentStunDelay -= Time.deltaTime;
            }
            if (currentStunDelay <= 0.0f && prepareStun == true)
            {
                flashNext = Time.time + flashRechageRate;
                //Write the creation of the flash here before it can affect the monster
                GameObject flashclone = (GameObject)Instantiate(flash, myDirector.playerObject.transform);
                //Insert flash sound here
                RaycastHit hit;
                Bounds playerBound = myDirector.playerObject.GetComponent<CharacterController>().bounds;
                Vector3 rayDirectionPlayer = playerBound.center - monster.visionObject.transform.position;
                Destroy(flashclone, 0.5f);
                flashSource.clip = flashAudio[Random.Range(0, flashAudio.Length)];
                flashSource.Play();
                while (true)
                {
                    int i = Random.Range(0, flashAudio.Length);
                    if (i != flashIndex)
                    {
                        flashSource.clip = flashAudio[i];
                        flashSource.Play();
                        flashIndex = i;
                        break;
                    }
                }
                //Check if player has the monster in it's sights
                if (Vector3.Angle(rayDirectionPlayer, -playerCamera.transform.forward) < Camera.main.fieldOfView)
                {
                    Vector3 rayDirectionMonster = monster.visionObject.transform.position - playerBound.center;
                    //Checks if the monster has the player in it's sights
                    if (Vector3.Angle(rayDirectionMonster, -monster.visionObject.transform.forward) < monster.fieldOfView)
                    {
                        //Check the distance from the monster to the player
                        if (Physics.Raycast(monster.visionObject.transform.position, -rayDirectionMonster, out hit, flashDistance))
                        {
                            //If the player is in distance of the monster
                            if (hit.collider.gameObject.CompareTag("Player"))
                            {
                                Debug.Log("Flash");
                                //Add the flash affect here
                                monster.GetStunned();
                            }
                        }
                    }
                }
                UseCharge(flashAmount);

                prepareStun = false;
            }
            if (Input.GetMouseButton(0))
            {
                if (flashAmount != 0 && Time.time > flashNext && currentStunDelay <= 0.0f)
                {
                    prepareStun = true;
                    currentStunDelay = stunDelay;

                    stun = true;
                    gauntletAnimator.SetBool("Stun", stun);

                }
            }
            if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.E))
            {
                armRaised = true;
            }
            else
            {
                armRaised = false;
            }
            gauntletAnimator.SetBool("ArmRaised", armRaised);
        }
    }

    void UseCharge(int index)
    {
        //gauntletMaterials[flashAmount].SetColor("_EmissiveColor", Color.black);
        gems[flashAmount - 1].SwapState();

        flashAmount--;
    }

    public void GainCharge()
    {
        for (int i = 0; i < restoreAmount; i++)
        {
            if (flashAmount < flashMax)
            {
                flashAmount++;
                gems[flashAmount - 1].SwapState();
                //gauntletMaterials[flashAmount].SetColor("_EmissiveColor", Color.white * 100.0f);
            }
        } 
    }

    public void EndStun()
    {
        stun = false;
        gauntletAnimator.SetBool("Stun", stun);
    }
}
