using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public GameObject followTool;

    NavMeshAgent agent;
    int targetArea;

    Color c;
    Color c2;

    public Director myDirector;
    public DecisionMaking monsterDecisionMaking;

    public AudioClip[] sounds;
    public float audioRange = 50;

    [Header("Animation")]
    [Tooltip("Put in the monsters animator.")]
    public Animator animator;
    Vector3 monsterLastDirection;
    Vector3 monsterCurrentDirection;

    [Header("Monster Vision")]
    [Tooltip("How far the monster can see")]
    public float visionRange = 1000.0f; // In degrees. Put half your intended amount.
    [Tooltip("How wide the monsters vision is, in degrees. Put in half your intended amount i.e 90 degrees = 180 degrees. For example, putting in 180 means 360 degrees of vision, so the monster can always see all around it.")]
    public float fieldOfView; // In degrees. Put half your intended amount.
    [Tooltip("How wide the monsters vision is, in degrees, while stunned.")]
    public float stunnedFieldOfView = 0.0f; // In degrees. Put half your intended amount.
    [Tooltip("How long the player can be in line of sight before the monster 'can see you'.")]
    public float sightDelay = 1.0f; // In degrees. Put half your intended amount.
    float currentSightTime = 0.0f;
    float patrolingFieldOfView; // Gets set to fieldOfView to record the starting value put into fieldOfView
    float chasingFieldOfView = 181.0f; // The monsters field of view when chasing so that he can see properly when turning corners
    float attackingFieldOfView = 181.0f; // The monsters field of view when attacking
    [Tooltip("An empty gameobject that should get childed to the head transform.")]
    public GameObject visionObject;
    [Tooltip("Is true if the monster can see the player.")]
    public bool hasLineOfSight = false;
    bool hasSquintSight = false;
    [Tooltip("Is true if the player can see the monster")]
    public bool playerHasSight = false;
    [Tooltip("How far behind itself the monster can see")]
    public float behindVisionLength = 8;
    [Tooltip("How long after losing sight of the player the monster will have extended sight for")]
    public float visionShrinkCooldown = 5.0f;
    [Tooltip("Time until vision shrink")]
    public float currentCooldownTime;
    [Tooltip("How close the player can get before the monster can see them from any angle")]
    public float proximityDetectionRadius;

    [Tooltip("FOR DEBUGGING")]
    public float angle;
    float lastAngle;
    [Tooltip("FOR DEBUGGING")]
    public Text text;

    [Header("Monster Movement")]
    [Tooltip("How close the monster has to be to try and attack")]
    public float monsterAttackRange = 1.0f;
    [Tooltip("Is the monster currently stunned?")]
    public bool isStunned = false;
    [Tooltip("How long the monster can be stunned for")]
    public float stunDuration;
    public float stunTimer = 0.0f;
    [Tooltip("How long until the monster can be stunned again")]
    public float immunityDuration;
    float immunityTimer = 0.0f;
    [Tooltip("How fast the monster walks")]
    public float corridorWalkSpeed;
    [Tooltip("How fast the monster walks")]
    public float roomWalkSpeed;
    [Tooltip("How fast the monster runs")]
    public float runSpeed;
    [Tooltip("How fast the monster accelerates and decelerates")]
    public float acceleration = 0.3f;
    float desiredSpeed;
    float currentSpeed = 0.0f;

    [Header("Monster Toggles")]
    [Tooltip("Toggle true to blind the monster")]
    public bool blinded = false;
    [Tooltip("Toggle false to stop the monster from doing anything")]
    public bool thinking = true;

    bool gizmos = false;

    List<NavMeshPath> adjacentPaths;
    List<GameObject> adjacentRooms;

    public float ang;
    public Vector3 erg;

    [Header("Stereo")]
    public AudioSource monsterStereoSource;
    public AudioClip[] monsterAttackAudio;
    public AudioClip[] monsterStunAudio;
    
    [Header("Surround")]
    public AudioSource monsterSurroundSource;
    public AudioClip[] monsterIdleAudio;
    public AudioClip[] monsterWalkingAudio;
    public AudioClip[] monsterRunningAudio;
    public AudioClip monsterRoarAudio;
    
    [Header("Mixer")]
    public AudioMixerGroup monsterAttackMixer;
    public AudioMixerGroup monsterIdleMixer;
    public AudioMixerGroup monsterRoarMixer;
    public AudioMixerGroup monsterStunMixer;
    public AudioMixerGroup monsterWalkMixer;

    int monsterWalkIndex;
    int monsterRunIndex;
    int monsterAttackIndex;
    int monsterIdleIndex;
    int monsterStunIndex;

    float value = 0.0f;
    float t = 0.0f;
    float min = 10.0f;
    float max = 150.0f;

    // Start is called before the first frame update
    void Start()
    {
        patrolingFieldOfView = fieldOfView;

        monsterDecisionMaking = GetComponent<DecisionMaking>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        gizmos = true;

        adjacentPaths = new List<NavMeshPath>();
        adjacentRooms = new List<GameObject>();

        //monsterStereoSource.outputAudioMixerGroup = monsterMixer;
        //monsterSurroundSource.outputAudioMixerGroup = monsterMixer;
    }

    // Update is called once per frame
    void Update()
    {
        erg = transform.forward;

        if (!blinded)
        {
            hasLineOfSight = LineOfSight();
            hasSquintSight = pseudoSight();
            if (currentSightTime > 0)
            { 
                currentSightTime -= Time.deltaTime;
            }

            if (monsterDecisionMaking.isChasing)
            {
                //hasLineOfSight = LineOfSight();
                currentSightTime = sightDelay;
            }
            else if (currentSightTime >= sightDelay)
            {
                //hasLineOfSight = LineOfSight();
                currentSightTime = sightDelay;
            }

            if (hasLineOfSight) // Colour stuff is test code
            {
                c = new Color(1, 0, 0);
            }
            else
            {
                c = new Color(0, 1, 0);
            }
        }
        if (thinking)
        {
            monsterDecisionMaking.RootDecision();
        }
        else
        {
            //agent.SetDestination(followTool.transform.position);
        }

        // Animation Stuff

        monsterLastDirection = monsterCurrentDirection;
        monsterCurrentDirection = transform.forward;

        lastAngle = angle;
        angle = Vector3.Angle(monsterLastDirection, monsterCurrentDirection);
        Vector3 cross = Vector3.Cross(monsterLastDirection, monsterCurrentDirection);

        if (cross.y < 0) // Checking if it's left or right
        {
            angle = -angle;
        }
        if (lastAngle != angle)
        { 
            //Debug.Log(angle); 
        }
        animator.SetFloat("Turn", angle);

        if (isStunned)
        {
            fieldOfView = stunnedFieldOfView; // changing here
            if (LineOfSight())
            {
                agent.SetDestination(myDirector.playerObject.transform.position);
            }
        }
        else if (monsterDecisionMaking.isRoaring || monsterDecisionMaking.isLooking)
        {
            desiredSpeed = 0.0f;
        }
        else if (monsterDecisionMaking.isAttacking)
        {
            fieldOfView = attackingFieldOfView;
            desiredSpeed = 0.0f;
        }
        else if (transform.position == agent.destination)
        {
            //animator.SetFloat("Speed", 0.0f);
            desiredSpeed = 0.0f;
            fieldOfView = patrolingFieldOfView;
        }
        else if (monsterDecisionMaking.isChasing && monsterDecisionMaking.guessedWhichRoom) // Setting speed and field of view for when chasing or not
        {
            //animator.SetFloat("Speed", runSpeed);
            desiredSpeed = runSpeed;
            if (currentCooldownTime <= 0.0f)
            {
                fieldOfView = patrolingFieldOfView;
            }
            else
            {
                fieldOfView = chasingFieldOfView;
                currentCooldownTime -= Time.deltaTime;
            }
        }
        else if (monsterDecisionMaking.isChasing) // Setting speed and field of view for when chasing or not
        {
            //animator.SetFloat("Speed", runSpeed);
            desiredSpeed = runSpeed;
            fieldOfView = chasingFieldOfView;
        }
        else
        {
            //animator.SetFloat("Speed", walkSpeed);
            if (myDirector.monsterCurrentRoom == 0)
            {
                desiredSpeed = corridorWalkSpeed;
            }
            else
            {
                desiredSpeed = roomWalkSpeed;
            }
            fieldOfView = patrolingFieldOfView;
        }

        if (currentSpeed < desiredSpeed) // Accelration/Deceleration
        {
            currentSpeed += acceleration;
        }
        else if (currentSpeed > desiredSpeed)
        {
            currentSpeed -= acceleration;
        }
        if (currentSpeed < 0)
        {
            currentSpeed = 0.0f;
        }
        else if (currentSpeed > desiredSpeed)
        {
            currentSpeed = desiredSpeed;
        }
        //Debug.Log(currentSpeed);
        agent.speed = currentSpeed;
        animator.SetFloat("Speed", agent.speed);

        if (stunTimer < 0.0f) // Stun Timer
        {
            isStunned = false;
            animator.SetBool("isStunned", false);
        }
        else
        {
            stunTimer -= Time.deltaTime;
        }

        //text.text = "Angle:" + angle;


        // DEBUGGING

        //adjacentPaths.Clear(); // 3

        //NavMeshPath tempPath;
        //tempPath = new NavMeshPath();

        //if (agent.isActiveAndEnabled)
        //{
        //    foreach (GameObject room in myDirector.rooms)
        //    {
        //        NavMeshHit myNavHit;
        //        NavMesh.SamplePosition(room.transform.position, out myNavHit, 100, NavMesh.AllAreas);
        //        if (!agent.CalculatePath(myNavHit.position, tempPath))
        //        {
        //             //Keep it null if it doesn't work to keep everything indexed
        //        }
        //        else
        //        {
        //            if (tempPath.corners.Length >= 2)
        //            {
        //                Vector3 cornerDirection = Vector3.Normalize(transform.position - tempPath.corners[1]); // Remove the paths that don't go the right way
        //                ang = Vector3.Angle(cornerDirection, -transform.forward);
        //                if (Vector3.Angle(cornerDirection, -transform.forward) < 90.0f)
        //                {
        //                    adjacentPaths.Add(tempPath);
        //                    adjacentRooms.Add(room);
        //                }
        //            }
        //        }
        //        tempPath = new NavMeshPath();
        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        if (c == null) // 1
        {
            Gizmos.color = Color.white;
        }
        else
        {
            Gizmos.color = c;
        }
        Quaternion visionNormal = Quaternion.AngleAxis(fieldOfView, Vector3.up);

        Vector3 rayDirection = visionNormal * visionObject.transform.forward;
        Gizmos.DrawRay(visionObject.transform.position, rayDirection * 10.0f);

        visionNormal = Quaternion.AngleAxis(-fieldOfView, Vector3.up);

        rayDirection = visionNormal * visionObject.transform.forward;
        Gizmos.DrawRay(visionObject.transform.position, rayDirection * 10.0f);

        Gizmos.DrawWireSphere(transform.position, proximityDetectionRadius);

        if (gizmos == true)
        {
            Bounds playerBounds = myDirector.playerObject.GetComponent<CharacterController>().bounds; // 2
            Gizmos.color = c;

            rayDirection = visionObject.transform.position - playerBounds.center;
            Gizmos.DrawRay(visionObject.transform.position, Vector3.Normalize(-rayDirection) * visionRange);

            //rayDirection = -transform.forward;
            //Gizmos.DrawRay(new Vector3(visionObject.transform.position.x, playerBounds.center.y, transform.position.z), rayDirection * behindVisionLength);

            //Gizmos.color = c2;
            //Gizmos.DrawWireSphere(agent.destination, 10.0f);



            playerBounds = myDirector.playerObject.GetComponent<CharacterController>().bounds; // 1
            rayDirection = -transform.forward;
            Gizmos.DrawRay(new Vector3(visionObject.transform.position.x, playerBounds.center.y, transform.position.z), rayDirection * behindVisionLength);




            //for (int j = 0; j < adjacentPaths.Count; j++) // 3
            //{
            //    Vector3 lastCorner = new Vector3(0, 0, 0);
            //    for (int i = 0; i < adjacentPaths[j].corners.Length; i++)
            //    {
            //        if (i == 0)
            //        {
            //            lastCorner = adjacentPaths[j].corners[i];
            //        }
            //        else
            //        {
            //            if (i == 1)
            //            {
            //                Vector3 cornerDirection = Vector3.Normalize(transform.position - adjacentPaths[j].corners[1]); // Remove the paths that don't go the right way
            //                if (Vector3.Angle(cornerDirection, -transform.forward) < 10.0f)
            //                {
            //                    Gizmos.color = Color.green;
            //                }
            //                else if (Vector3.Angle(cornerDirection, -transform.forward) < 20.0f)
            //                {
            //                    Gizmos.color = Color.blue;
            //                }
            //                else if (Vector3.Angle(cornerDirection, -transform.forward) < 180.0f)
            //                {
            //                    Gizmos.color = Color.red;
            //                }

            //                Gizmos.DrawCube(adjacentPaths[j].corners[i], new Vector3(1.0f, 1.0f, 1.0f));
            //            }
            //            Gizmos.DrawLine(adjacentPaths[j].corners[i], lastCorner);
            //            lastCorner = adjacentPaths[j].corners[i];
            //        }
            //    }
            //}
        }

    }

    bool LineOfSight()
    {
        Bounds playerBounds = myDirector.playerObject.GetComponent<CharacterController>().bounds;
        RaycastHit hit;
        Vector3 rayDirection = playerBounds.center - visionObject.transform.position;

        angle = Vector3.Angle(rayDirection, visionObject.transform.forward);
        bool sight = false;

        if (Vector3.Distance(transform.position, myDirector.playerObject.transform.position) > visionRange)
        {
            return false;
        }
        if (Vector3.Angle(rayDirection, visionObject.transform.forward) > fieldOfView) // Checking if the player is in the monsters FOV
        {
            rayDirection = -transform.forward;
            if (Physics.Raycast(new Vector3(visionObject.transform.position.x, playerBounds.center.y, visionObject.transform.position.z), rayDirection, out hit, behindVisionLength)) // Checking directly behind the monster
            {// ^ Checking directly behind the monster
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    sight = true;
                    currentSightTime += Time.deltaTime * 2.0f;
                }
            }
            if (proximityDetectionRadius != 0.0f)
            {
                // Checking if the player is in proximity of the monster
                if (Vector3.Distance(new Vector3(transform.position.x, playerBounds.center.y, transform.position.z), new Vector3(playerBounds.center.x, playerBounds.center.y, playerBounds.center.z)) <= proximityDetectionRadius)
                {
                    if (Physics.Raycast(visionObject.transform.position, rayDirection, out hit, Mathf.Infinity)) // Checking if theres anything in the way
                    {
                        if (hit.collider.gameObject.CompareTag("Player"))
                        {
                            sight = true;
                            currentSightTime += Time.deltaTime * 2.0f;
                        }
                    }
                }
            }

        }
        else
        {
            if (Physics.Raycast(visionObject.transform.position, rayDirection, out hit, Mathf.Infinity)) // Checking if theres anything in the way
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    sight = true;
                    currentSightTime += Time.deltaTime * 2.0f;
                }
            }
        }
        return sight;
    }

    public void SetTargetArea(int index)
    {
        targetArea = index;
    }

    public int GetTargetArea()
    {
        return targetArea;
    }

    public void GetStunned()
    {
        animator.SetBool("isStunned", true);
        //Monster Staggered sound here
        while (true)
        {
            int i = Random.Range(0, monsterStunAudio.Length);
            if (i != monsterStunIndex)
            {
                monsterStereoSource.outputAudioMixerGroup = monsterStunMixer;
                monsterStereoSource.clip = monsterStunAudio[i];
                monsterStereoSource.Play();
                monsterStunIndex = i;
                break;
            }
        }
        stunTimer = stunDuration;
        isStunned = true;

        monsterDecisionMaking.isChasing = true;
        monsterDecisionMaking.isLooking = false;
        monsterDecisionMaking.guessedWhichRoom = false;
        monsterDecisionMaking.attackCollider.enabled = false;
        //agent.SetDestination(myDirector.playerObject.transform.position);

        monsterDecisionMaking.ResetPatrolNodes();
    }

    public void ReadyMonsterAI() // Has to be called at the end of my directors start
    {
        monsterDecisionMaking.SetDecisionTreeVariables(gameObject, gameObject.GetComponent<Monster>(), agent);
    }

    //public void PlaySound(int index)
    //{
    //    if (index != 2)
    //    {
    //        //Finds the distance from the monster and player
    //        float pathLength = 0.0f;
    //        //Set the distance of the path
    //        pathLength = DistanceToPlayer(pathLength);
    //
    //        //float distance = Vector3.Distance(transform.position, myDirector.playerObject.transform.position);
    //        if (pathLength <= audioRange)
    //        {
    //            GetComponent<AudioSource>().volume = 1.0f - (pathLength / audioRange);
    //            GetComponent<AudioSource>().PlayOneShot(sounds[index]);
    //            //GetComponent<AudioSource>().outputAudioMixerGroup(the mixer group)
    //        }
    //        //Prints the distance
    //        //Debug.Log("Distance " + pathLength);
    //    }
    //    else
    //    {
    //        GetComponent<AudioSource>().PlayOneShot(sounds[index]);
    //    }
    //    //Prints the volume silder
    //    //Debug.Log("Volume " + GetComponent<AudioSource>().volume);
    //
    //}

    public void MonsterMovementSound()
    {
        if(currentSpeed <= corridorWalkSpeed)
        {
            MonsterWalkSound();
        }
        else if (currentSpeed == runSpeed)
        {
            MonsterRunSound();
        }
    }

    public void MonsterWalkSound()
    {
        while(true)
        {
            int i = Random.Range(0, monsterWalkingAudio.Length);
            if(i != monsterWalkIndex)
            {
                monsterSurroundSource.outputAudioMixerGroup = monsterWalkMixer;
                monsterSurroundSource.clip = monsterWalkingAudio[i];
                monsterSurroundSource.Play();
                monsterWalkIndex = i;
                break;
            }
        }
    }

    public void MonsterRunSound()
    {
        while (true)
        {
            int i = Random.Range(0, monsterRunningAudio.Length);
            if (i != monsterRunIndex)
            {
                monsterSurroundSource.outputAudioMixerGroup = monsterWalkMixer;
                monsterSurroundSource.clip = monsterRunningAudio[i];
                monsterSurroundSource.Play();
                monsterRunIndex = i;
                break;
            }
        }
    }


    public void MonsterAttackSound()
    {

        while (true)
        {
            int i = Random.Range(0, monsterAttackAudio.Length);
            if (i != monsterAttackIndex)
            {
                monsterStereoSource.outputAudioMixerGroup = monsterAttackMixer;
                monsterStereoSource.clip = monsterAttackAudio[i];
                monsterStereoSource.Play();
                monsterAttackIndex = i;
                break;
            }
        }
    }

    public void MonsterRoarSound()
    {

        if (monsterDecisionMaking.isChasing == true && monsterDecisionMaking.guessedWhichRoom == false)
        {
            monsterStereoSource.outputAudioMixerGroup = monsterRoarMixer;
            monsterStereoSource.clip = monsterRoarAudio;
            monsterStereoSource.Play();
        }
        else
        {
            monsterSurroundSource.outputAudioMixerGroup = monsterRoarMixer;
            monsterSurroundSource.clip = monsterRoarAudio;
            monsterSurroundSource.Play();
        }
    }
    
    public void MonsterIdleSound()
    {
        while (true)
        {
            int i = Random.Range(0, monsterIdleAudio.Length);
            if (i != monsterIdleIndex)
            {
                monsterSurroundSource.outputAudioMixerGroup = monsterIdleMixer;
                monsterSurroundSource.clip = monsterIdleAudio[i];
                monsterSurroundSource.Play();
                monsterIdleIndex = i;
                break;
            }
        }
    }

    //public float DistanceToPlayer(float pathLength)
    //{
    //    NavMeshPath soundPath = new NavMeshPath();
    //
    //    //Calculates the path length from the monster and player
    //    if (NavMesh.CalculatePath(transform.position, myDirector.playerObject.transform.position, agent.areaMask, soundPath))
    //    {
    //        //Check that the path is not invalid
    //        if (soundPath.status != NavMeshPathStatus.PathInvalid)
    //        {
    //            for (int i = 1; i < soundPath.corners.Length; i++)
    //            {
    //                //Aplly all the distance between points into a float
    //                pathLength += Vector3.Distance(soundPath.corners[i - 1], soundPath.corners[i]);
    //            }
    //        }
    //    }
    //    //Debug.Log(pathLength);
    //    //Return the length of the path
    //    return pathLength;
    //}

    bool pseudoSight()
    {
        Bounds playerBounds = myDirector.playerObject.GetComponent<CharacterController>().bounds;
        RaycastHit hit;
        Vector3 rayDirection = playerBounds.center - visionObject.transform.position;

        angle = Vector3.Angle(rayDirection, visionObject.transform.forward);
        bool sight = false;

        float pseudoFieldOfView = 181.0f;

        if (Vector3.Distance(transform.position, myDirector.playerObject.transform.position) > visionRange)
        {
            return false;
        }
        if (Vector3.Angle(rayDirection, visionObject.transform.forward) > pseudoFieldOfView) // Checking if the player is in the monsters FOV
        {
            rayDirection = -transform.forward;
            if (Physics.Raycast(new Vector3(visionObject.transform.position.x, playerBounds.center.y, visionObject.transform.position.z), rayDirection, out hit, behindVisionLength)) // Checking directly behind the monster
            {// ^ Checking directly behind the monster
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    sight = true;
                    currentSightTime += Time.deltaTime * 2.0f;
                }
            }
            if (proximityDetectionRadius != 0.0f)
            {
                // Checking if the player is in proximity of the monster
                if (Vector3.Distance(new Vector3(transform.position.x, playerBounds.center.y, transform.position.z), new Vector3(playerBounds.center.x, playerBounds.center.y, playerBounds.center.z)) <= proximityDetectionRadius)
                {
                    if (Physics.Raycast(visionObject.transform.position, rayDirection, out hit, Mathf.Infinity)) // Checking if theres anything in the way
                    {
                        if (hit.collider.gameObject.CompareTag("Player"))
                        {
                            sight = true;
                            currentSightTime += Time.deltaTime * 2.0f;
                        }
                    }
                }
            }

        }
        else
        {
            if (Physics.Raycast(visionObject.transform.position, rayDirection, out hit, Mathf.Infinity)) // Checking if theres anything in the way
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    sight = true;
                    currentSightTime += Time.deltaTime * 2.0f;
                }
            }
        }
        return sight;
    }

    public bool Squint()
    {
        return hasSquintSight;
    }
}