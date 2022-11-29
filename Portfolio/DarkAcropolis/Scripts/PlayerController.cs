using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject monster;

    CharacterController cc;
    public Transform cam;
    public Vector3 velocity;
    public Vector2 moveInput = new Vector2();

    Vector3 standCharCenter = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 crouchCharCenter = new Vector3(0.0f, -0.5f, 0.0f);
    float standCharHeight = 2.0f;
    float crouchCharHeight = 1.0f;

    Vector3 cameraCrouch = new Vector3(0.0f, 0.0f, 0.0f);

    [Header("Player Health Values:")]
    [Tooltip("How much damage the player can take")]
    public float maxHealth = 10.0f;
    [Tooltip("How much health the player currently has")]
    public float currentHealth = 10.0f;
    [Tooltip("The most intense the vignette can get")]
    public float maxVignette = 0.3f;
    [Tooltip("Current vignette intensity")]
    float currentVignette = 0.0f;
    [Tooltip("The least intense the vignette can get")]
    public float minVignette = 0.0f;
    public Volume volume;
    Vignette vignette;
    float damageImmunityTimer = 1.0f;
    float currentDamageImmunityTime = 0.0f;

    [Header("MovementSpeed")]
    [Tooltip("Active speed")]
    public float movementSpeed;
    [Tooltip("Base walking speed")]
    public float walkSpeed = 8.0f;
    [Tooltip("Walking backwards speed")]
    public float backwardSpeed;
    [Tooltip("Sprint speed")]
    public float sprintSpeed = 14.0f;
    [Tooltip("When the player uses all their stamina, sprint gets disabled for a-bit")]
    public bool sprintDisabled = false;
    [Tooltip("Crounch speed")]
    public float crouchSpeed = 2.0f;
    [Tooltip("Sprint active")]
    public bool sprinting = false;
    [Tooltip("How many the seconds the player can sprint for")]
    public float stamina = 10.0f;
    [Tooltip("The amount of stamina the player currently have")]
    float currentStamina;
    [Tooltip("Image of the bar that showcase the stamina")]
    public Image sprintBar;
    [Tooltip("How long it takes to regen stamina")]
    public float sprintRegen = 5.0f;
    [Tooltip("Counts in seconds after sprinting before regen")]
    public float sprintTimer = 0.0f;
    [Tooltip("The amount of stamina recovered")]
    public float staminaRegen = 0.0f;
    [Tooltip("Crourch active")]
    public bool crouching = false;
    [Tooltip("Crourch rising")]
    public bool crouchRise = false;
    [Tooltip("Crourch rise speed")]
    public float crouchRiseSpeed = 1.5f;
    public bool isGrounded;
    [Tooltip("If walking")]
    public bool isWalking = false;
    [Tooltip("Out of breath")]
    public bool isOutofBreath = false;

    [Header("Head Bobbing")]
    [Tooltip("Min height of head bob")]
    public float headBobMin;
    [Tooltip("Max height of head bob")]
    public float headBobMax;

    [Tooltip("Min height of walking head bob")]
    public float walkHeadBobMin = 0.4f;
    [Tooltip("Max height of walking head bob")]
    public float walkHeadBobMax = 0.5f;

    [Tooltip("Min height of sprinting head bob")]
    public float sprintHeadBobMin = 0.4f;
    [Tooltip("Max height of sprinting head bob")]
    public float sprintHeadBobMax = 0.6f;

    [Tooltip("Speed of the head bob")]
    public float headBobSpeed = 0.1f;
    [Tooltip("Speed when sprinting for head bob")]
    public float headBobSprintSpeed = 2.0f;
    [Tooltip("Starting head point")]
    public float headCurrentOffSet = 0.3f;
    [Tooltip("Current head bob speed")]
    public float bobspeed;
    float headOffSet = 0.0f;
    bool normalBob = false;
    bool collideBob = true;

    float lastPosition;

    [Header("lose")]
    public MenuManager loseCanvas;

    [Header("Audio")]
    public AudioSource playerBreathWalkSource;
    public AudioClip[] playerBreathWalkAudio;

    public AudioSource playerBreathSprintSource;
    public AudioClip playerBreathSprintAudio;

    public AudioSource playerBreathOutSource;
    public AudioClip[] playerBreathOutAudio;

    public AudioSource playerHurtSource;
    public AudioClip[] playerHurtAudio;

    public AudioSource playerWalkSource;
    public AudioClip[] playerWalkAudio;

    [Header("Mixer")]
    public AudioMixerGroup playerBreathMixer;
    public AudioMixerGroup playerHurtMixer;
    public AudioMixerGroup playerWalkMixer;

    [Header("AudioVolume")]
    float volumeMin = 0.0f;
    float volumeMax = 1.0f;
    float volumeIncrease = 0.5f;
    float volumeDecrease = 0.5f;

    float walkLerp = 0.0f;
    float sprintLerp = 0.0f;
    float outBreathLerp = 0.0f;

    bool changeBreathWalk = false;
    bool changeBreathWalkStop = true;
    bool changeBreathOut = false;


    int playerBreathWalkIndex = 0;
    int playerBreathOutIndex = 0;
    int playerHurtIndex;
    int playerWalkIndex;

    public bool Toggle = false;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cam == null)
        {
            cam = Camera.main.transform;
        }

        currentHealth = maxHealth;

        for (int i = 0; i < volume.sharedProfile.components.Count; i++)
        {
            if (volume.sharedProfile.components[i].name == "Vignette")
            {
                vignette = (Vignette)volume.sharedProfile.components[i];
            }
        }

        loseCanvas = FindObjectOfType<MenuManager>();

        currentStamina = stamina;

        playerBreathWalkSource.outputAudioMixerGroup = playerBreathMixer;
        playerBreathSprintSource.outputAudioMixerGroup = playerBreathMixer;
        playerBreathOutSource.outputAudioMixerGroup = playerBreathMixer;
        playerHurtSource.outputAudioMixerGroup = playerHurtMixer;
        playerWalkSource.outputAudioMixerGroup = playerWalkMixer;

        playerBreathWalkSource.clip = playerBreathWalkAudio[0];
        playerBreathSprintSource.clip = playerBreathSprintAudio;
        playerBreathOutSource.clip = playerBreathOutAudio[0];
        
        playerBreathWalkSource.volume = volumeMin;
        playerBreathSprintSource.volume = volumeMin;
        playerBreathOutSource.volume = volumeMin;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // Health Vignette and Regen
        if (maxVignette - ((currentHealth / maxHealth) * maxVignette) < maxVignette)
        {
            vignette.intensity.value = maxVignette - ((currentHealth / maxHealth) * maxVignette);
        }
        else
        {
            vignette.intensity.value = maxVignette;
        }

        if (currentDamageImmunityTime > 0)
        {
            currentDamageImmunityTime -= Time.deltaTime;
        }

        if (currentHealth <= 0.0f)
        {
            loseCanvas.Lose();
        }

        Vector3 delta;

        Vector3 camForward = cam.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = cam.right;

        Vector3 cameraStandcurrent = new Vector3(0.0f, headCurrentOffSet, 0.0f);

        //Set speed to walking if no other speed options are active
        if (Input.GetKey(KeyCode.S) && !crouching)
        {
            movementSpeed = backwardSpeed;
            sprintDisabled = true;
        }
        else if (!crouching && !sprinting)
        {
            movementSpeed = walkSpeed;
            sprintDisabled = false;
        }

        if(moveInput.y < 0.0f)
        {
            sprintDisabled = true;
        }

        //Check crouch inputs
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.Alpha4))
        {
            cc.center = crouchCharCenter;
            cc.height = crouchCharHeight;
            cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition, cameraCrouch, crouchRiseSpeed * Time.deltaTime);
            crouching = true;
            crouchRise = true;
            movementSpeed = crouchSpeed;
        }
        else
        {
            cc.center = standCharCenter;
            cc.height = standCharHeight;
            cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition, cameraStandcurrent, crouchRiseSpeed * Time.deltaTime);
            if (cam.transform.localPosition == cameraStandcurrent)
            {
                crouching = false;
                crouchRise = false;
            }
        }

        //Check sprint inputs
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Alpha9))
        {
            sprinting = false;
            if (!crouching && currentStamina > 0.0f && sprintDisabled == false && cc.velocity.x != 0.0f && cc.velocity.z != 0.0f)
            {
                sprinting = true;
                movementSpeed = sprintSpeed;
                currentStamina -= Time.deltaTime;
                sprintBar.fillAmount = currentStamina / stamina;
                sprintTimer = 0.0f;

                //Check if sprint sound is playing and will if not
                if (!playerBreathSprintSource.isPlaying)
                {
                    playerBreathSprintSource.Play();
                }
                //Increase sprint breath audio and decrease walk breath audio
                else
                {
                    playerBreathWalkSource.volume = Mathf.Lerp(volumeMin, volumeMax, walkLerp);
                    playerBreathSprintSource.volume = Mathf.Lerp(volumeMin, volumeMax, sprintLerp);
                    walkLerp -= volumeDecrease * Time.deltaTime;
                    sprintLerp += volumeIncrease * Time.deltaTime;
                    walkLerp = Mathf.Clamp(walkLerp, 0.0f, 1.0f);
                    sprintLerp = Mathf.Clamp(sprintLerp, 0.0f, 1.0f);
                    //once walk audio volume equals and changeBreathWalkStop is false make true
                    //ChangeBreathWalkStop will only equal false if audio clip of walk breathing has change
                    //And breath walk volume hasn't hit zero
                    if(playerBreathWalkSource.volume <= 0.0f && !changeBreathWalkStop)
                    {
                        changeBreathWalk = true;
                    }
                    //This decreases out of breath audio when the player is sprinting
                    if (playerBreathOutSource.volume > 0.0)
                    {
                        playerBreathOutSource.volume = Mathf.Lerp(volumeMin, volumeMax, outBreathLerp);
                        outBreathLerp -= volumeDecrease * Time.deltaTime;
                        outBreathLerp = Mathf.Clamp(outBreathLerp, 0.0f, 1.0f);
                        //Change the out of breath audio clip when volume equals volumeMin
                        if (playerBreathOutSource.volume <= volumeMin)
                        {
                            changeBreathOut = true;
                        }
                    }
                }
            }
        }
        //When not sprint and player stamina is less than max stamina
        else if(currentStamina < stamina)
        {
            //Starts timer before refilling stamina
            if(sprintTimer < sprintRegen)
            {
                sprintTimer += sprintRegen / sprintRegen * Time.deltaTime;
            }
            //Once timer equals or ascend the timer berfore stamina refill
            else if (sprintTimer >= sprintRegen)
            {
                //Refills current stamina and increase the stamina bar for the UI
                currentStamina += Time.deltaTime * staminaRegen;
                sprintBar.fillAmount = currentStamina / stamina;
                //Allow the player to sprint
                sprintDisabled = false;
                isOutofBreath = false;
                //If the out of breath is playing start to decrease the audio volume
                if (playerBreathOutSource.isPlaying)
                {
                    playerBreathOutSource.volume = Mathf.Lerp(volumeMin, volumeMax, outBreathLerp);
                    outBreathLerp -= volumeDecrease * Time.deltaTime;
                    outBreathLerp = Mathf.Clamp(outBreathLerp, 0.0f, 1.0f);
                    //once volume hits zero allow the audio clip to change
                    if(playerBreathOutSource.volume <= volumeMin)
                    {
                        changeBreathOut = true;
                    }
                }
            }
            //Player isn't sprinting
            sprinting = false;
        }
        if (currentStamina >= stamina)
        {
            sprintDisabled = false;
        }
        //If player is out of stamina
        else if (currentStamina <= 0.0f)
        {
            //Sprint Oob
            sprintDisabled = true;
            sprinting = false;
            //Checks if out of breath is not player and in that case will play audio
            if (!playerBreathOutSource.isPlaying)
            {
                playerBreathOutSource.Play();
            }
            //increases out of breath audio volume
            else
            {
                playerBreathOutSource.volume = Mathf.Lerp(volumeMin, volumeMax, outBreathLerp);
                outBreathLerp += volumeIncrease * Time.deltaTime;
                outBreathLerp = Mathf.Clamp(outBreathLerp, 0.0f, 1.0f);
                isOutofBreath = true;
            }
        }


        //movement that depend on the direction of the camera
        delta = (moveInput.x * camRight + moveInput.y * camForward) * movementSpeed;

        //if true update velocity speed with delta speed
        if (isGrounded || moveInput.x != 0.0f || moveInput.y != 0.0f)
        {
            velocity.x = delta.x;
            velocity.z = delta.z;
        }

        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = 0.0f;
        } 

        //Check if W,A,S,D is held down 
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            //Not sprinting, crouching or out of breath
            if(!sprinting && !Input.GetKey(KeyCode.LeftControl) && !isOutofBreath)
            {
                //Check if breath walk audio is not playing and will play if not
                if (!playerBreathWalkSource.isPlaying)
                {
                    playerBreathWalkSource.Play();
                }
                //Increases breath walk audio volume and decreases breath sprint audio volume
                else
                {
                    playerBreathWalkSource.volume = Mathf.Lerp(volumeMin, volumeMax, walkLerp);
                    playerBreathSprintSource.volume = Mathf.Lerp(volumeMin, volumeMax, sprintLerp);
                    walkLerp += volumeIncrease * Time.deltaTime;
                    sprintLerp -= volumeDecrease * Time.deltaTime;
                    walkLerp = Mathf.Clamp(walkLerp, 0.0f, 1.0f);
                    sprintLerp = Mathf.Clamp(sprintLerp, 0.0f, 1.0f);
                    //Allow the breath walk to change when audio volume hits zero
                    if(changeBreathWalkStop)
                    {
                        changeBreathWalkStop = false;
                    }
                }
            }
        }

        //If player is moving and not crouching
        if(velocity.x != 0.0f && !crouching || velocity.z != 0.0f && !crouching)
        {
            isWalking = true;
        }
        //If player isn't moving
        else
        {
            isWalking = false;
        }

        //If not walking or the player is out of breath decrease breath walk and sprint audio volume
        if(!isWalking || isOutofBreath)
        {
            playerBreathWalkSource.volume = Mathf.Lerp(volumeMin, volumeMax, walkLerp);
            playerBreathSprintSource.volume = Mathf.Lerp(volumeMin, volumeMax, sprintLerp);
            walkLerp -= volumeDecrease * Time.deltaTime;
            sprintLerp -= volumeDecrease * Time.deltaTime;
            walkLerp = Mathf.Clamp(walkLerp, 0.0f, 1.0f);
            sprintLerp = Mathf.Clamp(sprintLerp, 0.0f, 1.0f);

            //Change the audio clip of breath walk if the statement is met
            if(playerBreathWalkSource.volume <= 0.0f && !changeBreathWalkStop)
            {
                changeBreathWalk = true;
            }
        }

        //apply gravity to the velocity
        velocity += Physics.gravity * Time.fixedDeltaTime;

        float currentSpeed = velocity.magnitude;
        if (currentSpeed > movementSpeed)
        {
            currentSpeed = movementSpeed;
        }

        Vector3 translation = velocity.normalized * currentSpeed * Time.deltaTime;

        //update character movement in game
        cc.Move(translation);
        isGrounded = cc.isGrounded;

        //Head Bobbing
        //if the charcter is moving and not crouching start head bobbing
        //else don't head bob and keep the camera still
        if (!crouching && moveInput.x != 0.0f || !crouching && moveInput.y != 0.0f)
        {
            //Sprinting is true and moving increase head bob speed
            //This stop from holding down sprint and head bobbing fast standing still
            if (sprinting && moveInput.x != 0.0f || sprinting && moveInput.y != 0.0f)
            {
                bobspeed = headBobSprintSpeed;
            }
            else
            {
                bobspeed = headBobSpeed;
            }

            //Gets location of the head bob position
            headCurrentOffSet += bobspeed * Time.deltaTime;

            //apply the head bobbing to the position of the y value in camera
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, headOffSet + headCurrentOffSet, cam.transform.localPosition.z);

            //Changes min and max headbob if walking or sprinting
            if(bobspeed == headBobSpeed)
            {
                headBobMin = walkHeadBobMin;
                headBobMax = walkHeadBobMax;
            }
            else
            {
                headBobMin = sprintHeadBobMin;
                headBobMax = sprintHeadBobMax;
            }
            //increase head bob till hit max value
            if (headCurrentOffSet > headBobMax)
            {
                headCurrentOffSet = headBobMax;
                headBobSpeed = -headBobSpeed;
                headBobSprintSpeed = -headBobSprintSpeed;
            }
            //decrease head bob till hit min value
            else if (headCurrentOffSet < headBobMin)
            {
                headCurrentOffSet = headBobMin;
                headBobSpeed = -headBobSpeed;
                headBobSprintSpeed = -headBobSprintSpeed;
            }

            if (headCurrentOffSet == headBobMin)
            {
                //FindObjectOfType<AudioManager>().PlaySound(0, "Player Walk");
                while (true)
                {
                    int i = Random.Range(0, playerWalkAudio.Length);
                    if (i != playerWalkIndex)
                    {
                        playerWalkSource.clip = playerWalkAudio[i];
                        playerWalkSource.Play();
                        playerWalkIndex = i;
                        break;
                    }
                }
            }
        }

        //If true change the breath walk audio with a new audio clip
        if(changeBreathWalk)
        {
            //Keep playing loop till i doesn't equal playerBreathWalkIndex
            while(true)
            {
                //Grabs a random number between zero and breath walk audio length
                int i = Random.Range(0, playerBreathWalkAudio.Length);
                if (i != playerBreathWalkIndex)
                {
                    //replace audio clip with new audio clip
                    playerBreathWalkSource.clip = playerBreathWalkAudio[i];
                    //Update index for next call
                    playerBreathWalkIndex = i;
                    changeBreathWalk = false;
                    changeBreathWalkStop = true;
                    break;
                }
            }
        }

        //If true change the out of breath audio with a new audio clip
        if (changeBreathOut)
        {
            //Keep playing loop till i doesn't equal playerBreathWalkIndex
            while (true)
            {
                //Grabs a random number between zero and out of breath audio length
                int i = Random.Range(0, playerBreathOutAudio.Length);
                if (i != playerBreathOutIndex)
                {
                    //replace audio clip with new audio clip
                    playerBreathOutSource.clip = playerBreathOutAudio[i];
                    //Update index for next call
                    playerBreathOutIndex = i;
                    changeBreathOut = false;
                    break;
                }
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Stair")
        {
            if (!normalBob && collideBob)
            {
                headBobMin = 0.1f;
                headBobSpeed = 1.0f;
                normalBob = true;
                collideBob = false;
            }
        }
        else
        {
            if (normalBob && !collideBob)
            {
                headBobMin = 0.3f;
                headBobSpeed = 0.1f;
                normalBob = false;
                collideBob = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Bounds playerBounds = GetComponent<CharacterController>().bounds;

        //Vector3 checkPos = playerBounds.center;

        //checkPos.y -= 0.5f;

        //Gizmos.DrawCube(checkPos, new Vector3(1.0f, 1.0f, 1.0f));

        //Vector3 direction = transform.position - monster.transform.position;

        //direction.y = 0.0f;

        //Vector3.Normalize(direction);

        //Gizmos.DrawRay(checkPos, direction * 2.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackHand"))
        {
            if (currentDamageImmunityTime <= 0.0f)
            {
                //Player Damaged
                currentHealth -= 1.0f;
                currentDamageImmunityTime = damageImmunityTimer;
                CameraShake.instance.ShakeCamera(0.5f);

                RaycastHit hit;
                Vector3 direction = transform.position - monster.transform.position;

                Vector3.Normalize(direction);

                direction.y = 0.0f;

                Bounds playerBounds = GetComponent<CharacterController>().bounds;

                Vector3 checkPos = playerBounds.center;

                checkPos.y -= 0.5f;

                if (Physics.Raycast(checkPos, direction, out hit, 2.0f)) // Checking directly behind the player
                {

                }
                else
                {
                    direction.y = 0;
                    direction.Normalize();

                    cc.transform.Translate(direction * 2);
                }

                while (true)
                {
                    int i = Random.Range(0, playerHurtAudio.Length);
                    if (i != playerHurtIndex)
                    {
                        playerHurtSource.clip = playerHurtAudio[i];
                        playerHurtSource.Play();
                        playerHurtIndex = i;
                        break;
                    }
                }
            }
        }
       
    }
}