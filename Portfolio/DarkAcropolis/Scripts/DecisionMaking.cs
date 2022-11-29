using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class DecisionMaking : MonoBehaviour
{
    GameObject monsterObject; // The object that the components are on
    Monster monster; // The monster script
    NavMeshAgent agent; // The monsters navmesh agent

    [Tooltip("True if the monster is chasing the player")]
    public bool isChasing = false; // Are we currently in a chase?
    [Tooltip("True if the monster has recieved a clue it hasn't acted on yet")]
    public bool clueActive = false; // Is there a clue to move to?
    [Tooltip("True if the monster is mid attack animation")]
    public bool isAttacking = false;
    [Tooltip("True if the monster is looking back")]
    public bool isLooking = false;
    [Tooltip("True if the monster is roaring")]
    public bool isRoaring = false;
    [Tooltip("How likely the monster is to roar when beginning a chase")]
    public float chanceToRoar = 1.0f;

    bool justLooked = false;

    float eligiblePathAngle;
    float eligiblePathStartAngle = 20.0f;
    float eligiblePathAngleIncrease = 20.0f;

    [Tooltip("What room the monster thinks you're in right after a chase")]
    public bool guessedWhichRoom = false; // What room have we predicted the player to be in?

    [Tooltip("How long the monster will wait between attacks")]
    public float attackDelay = 1.0f;
    float attackTimer = 0.0f;

    // back track takes priority
    [Tooltip("A weight of one means he has an equal chance to pick this option when changing rooms compared to other rooms.")]
    public float backTrackWeight = 1.0f;
    [Tooltip("Modifies how likely they minster is to pick a room a seal is in. A modifier of 1.0f makes this room twice as likely to be picked when compared to other normal rooms.")]
    public float sealRoomModifier = 1.0f;

    public BoxCollider attackCollider;

    NavMeshPath[] paths;
    bool giz = false;

    GameObject lastPatrolNode;
    GameObject currentPatrolNode;
    GameObject lastSearchNode;
    GameObject currentSearchNode;
    int searchRoomIndex;
    GameObject[] rooms;

    [Header("audio")]
    public AudioSource monsterStereoSource;
    public AudioClip monsterSlightaudio;
    
    [Header("Mixer")]
    public AudioMixerGroup monsterSlightMixer;


    private void Start()
    {
        monsterObject = gameObject;
        monster = gameObject.GetComponent<Monster>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        attackCollider.enabled = false;

        monsterStereoSource.outputAudioMixerGroup = monsterSlightMixer;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Set Decision Tree Variables
    // Used for setting up the variables the decision tree uses in the monster
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void SetDecisionTreeVariables(GameObject mObject, Monster mon, NavMeshAgent monsterAgent)
    {
        monsterObject = mObject;
        monster = mon;
        agent = monsterAgent;

        GetRooms();
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Get Rooms
    // Sets up a reference to the rooms in the decision making script
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void GetRooms()
    {
        rooms = new GameObject[monster.myDirector.rooms.Length];

        int i = 0;
        foreach (GameObject room in monster.myDirector.rooms)
        {
            rooms[i] = room;
            i++;
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Move To Attack
    // Gives the agent the position to move to so that it will be in attack range
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void MoveToAttack(GameObject player)
    {
        Vector3 destination;
        Vector3 direction = Vector3.Normalize(player.transform.position - monsterObject.transform.position);

        destination = player.transform.position + (direction * (monster.monsterAttackRange - 1));

        agent.SetDestination(destination);
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Root Decision
    // The decision you must call in the monsters Update() function to get the deicision tree to do anything, if you dont call this, the
    // monster won't do anything and you only call this function and Set Decision Tree Variables in the Start() function
    //
    // It is also the ChasingDecision
    // Will check if the monster is mid chase and if so what to do when it loses sight and when to end the chase
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void RootDecision()
    {
        if (rooms == null)
        {
            GetRooms();
        }
        if (!monster.isStunned)
        {
            agent.isStopped = false;
            if (isChasing && monster.hasLineOfSight) // Keep chasing if we can see the player
            {
                AttackBehaviour();
            }
            else if (isChasing && AtDestination()) // Stop chasing if we can't see the player and went to their last location
            {
                RoomToSearchDecision();
            }
            else
            {
                SightDecision();
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Attack Behaviour
    // How the monster tries to hurt the player
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void AttackBehaviour()
    {
        if (Vector3.Distance(monsterObject.transform.position, monster.myDirector.playerObject.transform.position) <= monster.monsterAttackRange)
        {
            agent.SetDestination(monsterObject.transform.position);

            //Attack

            Vector3 dir = monster.myDirector.playerObject.transform.position - transform.position;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, agent.angularSpeed * Time.deltaTime);

            if (attackTimer >= attackDelay && !isAttacking)
            {
                attackTimer = 0.0f;
                monster.animator.SetBool("isAttacking", true);
                isAttacking = true;
                attackCollider.enabled = true;
                Roar();
            }
            else
            {
                if (!isAttacking)
                {
                    attackTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            agent.SetDestination(monster.myDirector.playerObject.transform.position);
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Attack End
    // Reset attack variables
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void AttackEnd()
    {
        monster.animator.SetBool("isAttacking", false);
        attackCollider.enabled = false;
        isAttacking = false;
        attackTimer = 0.0f;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sight Decision
    // If the monster can see the player, start chasing, if not, keep patroling
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void SightDecision()
    {
        if (monster.hasLineOfSight) // Initiate chase if we can see the player
        {
            float i = Random.Range(0.0f, 1.0f);

            isChasing = true;
            guessedWhichRoom = false;

            if (i <= chanceToRoar)
            {
                Roar();
            }

            isLooking = false;
            monster.animator.SetBool("isLooking", false);

            //Insert Monster Roar hear
            //GetComponent<AudioSource>().volume = 1.0f;
            //FindObjectOfType<AudioManager>().PlaySound(0, "Monster Found");
            monsterStereoSource.clip = monsterSlightaudio;
            monsterStereoSource.Play();

            ResetPatrolNodes(); // Need to reset these so that the monster can restart wandering later
        }
        else // Start patroling if not
        {
            // Patrol Decisions
            PatrolAreaDecision();
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Patrol Area Decision
    // Decides if the monster should move area or keep patroling the current area
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void PatrolAreaDecision()
    {
        if (monster.GetTargetArea() != 0)
        {
            monster.SetTargetArea(0);
            agent.SetDestination(monster.myDirector.areaData[monster.myDirector.playerCurrentArea - 1].focalPoint.transform.position);
        }
        else if (AtDestination())
        {
            PatrolRoomsDecision();
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Patrol Rooms Decision
    // Decides what room the monster should check in his current area
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void PatrolRoomsDecision()
    {
        if (monster.myDirector.monsterCurrentArea != 0)
        {
            if (currentPatrolNode == null) // If null then we are not currently patroling a room
            {
                Room currentRoom = rooms[monster.myDirector.FindCurrentRoom(monsterObject, monster.myDirector.monsterCurrentArea) - 1].GetComponent<Room>();
                currentRoom.patrolChanceToLeave = currentRoom.StartingPatrolChanceToLeave;

                if (!isLooking)
                {
                    float distance = float.MaxValue;
                    monster.myDirector.monsterCurrentRoom = monster.myDirector.FindCurrentRoom(monsterObject, monster.myDirector.monsterCurrentArea);
                    if (monster.myDirector.FindCurrentRoom(monsterObject, monster.myDirector.monsterCurrentArea) != 0)
                    {
                        if (monster.myDirector.rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolPoints.Length != 0) // If we have patrol points
                        {
                            foreach (GameObject patrolPoint in rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolPoints)
                            {
                                if (Vector3.Distance(monsterObject.transform.position, patrolPoint.transform.position) < distance)
                                {
                                    distance = Vector3.Distance(monsterObject.transform.position, patrolPoint.transform.position);
                                    currentPatrolNode = patrolPoint;
                                }
                            }
                            agent.SetDestination(currentPatrolNode.transform.position);
                        }
                        else // If the room doesn't have 
                        {
                            float leaveChance = 1.0f - currentPatrolNode.GetComponent<PatrolPoint>().lookAroundChance - currentPatrolNode.GetComponent<PatrolPoint>().roarChance;
                            float chanceResult = Random.Range(1, 100);
                            chanceResult = chanceResult / 100.0f;

                            if (chanceResult <= leaveChance) // Leave
                            {
                                LeaveRoom();
                            }
                            else if (chanceResult <= leaveChance + currentPatrolNode.GetComponent<PatrolPoint>().lookAroundChance)// Look around on spot
                            {
                                // Look around stuff
                                if (!justLooked)
                                {
                                    LookBack();
                                }
                            }
                            else
                            {
                                // Roar
                                Roar();
                            }
                        }
                    }
                }
            }
            else // else we a patroling a room with patrol points
            {
                if (!isLooking)
                {
                    if (rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolChanceToLeaveIncrease + rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolChanceToLeave + currentPatrolNode.GetComponent<PatrolPoint>().roarChance + currentPatrolNode.GetComponent<PatrolPoint>().lookAroundChance <= 1.0f)
                    {
                        rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolChanceToLeave += rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolChanceToLeaveIncrease;

                    }
                    float changePatrolPointChance = 1.0f - rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolChanceToLeave - currentPatrolNode.GetComponent<PatrolPoint>().lookAroundChance - currentPatrolNode.GetComponent<PatrolPoint>().roarChance;
                    float chanceResult = Random.Range(1, 100);
                    chanceResult = chanceResult / 100.0f;

                    if (chanceResult <= changePatrolPointChance) // Move between patrol points
                    {
                        justLooked = false;
                        int i = Random.Range(0, currentPatrolNode.GetComponent<PatrolPoint>().connectedPoints.Length);
                        if (currentPatrolNode.GetComponent<PatrolPoint>().connectedPoints[i] == lastPatrolNode || currentPatrolNode.GetComponent<PatrolPoint>().connectedPoints[i] == currentPatrolNode)
                        {

                        }
                        else
                        {
                            agent.SetDestination(currentPatrolNode.GetComponent<PatrolPoint>().connectedPoints[i].transform.position);

                            lastPatrolNode = currentPatrolNode;
                            currentPatrolNode = currentPatrolNode.GetComponent<PatrolPoint>().connectedPoints[i];
                        }
                    }
                    else if (chanceResult <= changePatrolPointChance + currentPatrolNode.GetComponent<PatrolPoint>().lookAroundChance) // Look around on spot
                    {
                        // Look around stuff
                        if (!justLooked)
                        {
                            LookBack();
                        }
                    }
                    else if (chanceResult <= changePatrolPointChance + currentPatrolNode.GetComponent<PatrolPoint>().lookAroundChance + currentPatrolNode.GetComponent<PatrolPoint>().roarChance) // Look around on spot
                    {
                        //Roar 
                        Roar();
                    }
                    else // Leave the room
                    {
                        LeaveRoom();
                    }
                }
            }

        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Room To Search Decision
    // Decides which room the monster should search when he loses you after a chase.
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void RoomToSearchDecision()
    {
        // Am I in a room?
        if (monster.myDirector.monsterCurrentRoom != 0)
        {
            // Yes
            // Was the player last seen in the same room as me?
            if (monster.myDirector.monsterCurrentRoom == monster.myDirector.playerLastSeenRoom)
            {
                //Yes
                // Search the current room
                SearchRoom(monster.myDirector.monsterCurrentRoom);
                //isChasing = false; // temp
                return;
            }
        }
        // No
        PickPath();
    }


    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Pick Path
    // Selects a room the monster can go to without crossing over other rooms and sends the monster over
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void PickPath()
    {
        if (guessedWhichRoom == false)
        {
            // Get a path to each room

            NavMeshPath tempPath;
            tempPath = new NavMeshPath();

            List<NavMeshPath> checkedPaths = new List<NavMeshPath>();
            List<NavMeshPath> adjacentPaths = new List<NavMeshPath>();
            List<Vector3> adjacentRoomPositions = new List<Vector3>();
            List<int> adjacentRoomIndexes = new List<int>();

            eligiblePathAngle = eligiblePathStartAngle;
            bool noOptions = true;
            while (noOptions)
            {
                foreach (GameObject room in monster.myDirector.rooms)
                {
                    if (room.GetComponent<Room>().roomIndex == monster.myDirector.monsterCurrentRoom)
                    {
                        continue;
                    }
                    NavMeshHit myNavHit;
                    NavMesh.SamplePosition(room.transform.position, out myNavHit, 100, NavMesh.AllAreas);
                    if (!agent.CalculatePath(myNavHit.position, tempPath))
                    {
                        // Keep it null if it doesn't work to keep everything indexed
                    }
                    else
                    {
                        if (tempPath != null && tempPath.corners.Length >= 2)
                        {
                            Vector3 cornerDirection = Vector3.Normalize(transform.position - tempPath.corners[1]); // Remove the paths that don't go the right way
                            if (Vector3.Angle(cornerDirection, -transform.forward) < eligiblePathAngle)
                            {
                                adjacentPaths.Add(tempPath);
                                adjacentRoomPositions.Add(myNavHit.position);
                                adjacentRoomIndexes.Add(room.GetComponent<Room>().roomIndex);
                            }
                        }
                    }
                    tempPath = new NavMeshPath();
                }
                if (adjacentPaths.Count != 0) // Make sure we found paths
                {
                    noOptions = false;
                }
                else if (adjacentPaths.Count == 0) // If we didn't then 
                {
                    eligiblePathAngle += eligiblePathAngleIncrease;
                }
            }

            // Choose a path
            if (adjacentPaths.Count > 0)
            {
                int i = Random.Range(0, adjacentRoomPositions.Count);
                agent.SetDestination(adjacentRoomPositions[i]);
                searchRoomIndex = adjacentRoomIndexes[i];
                //SearchRoom(adjacentRoomIndexes[i]);
                guessedWhichRoom = true;
            }
        }
        else
        {
            if (monster.myDirector.monsterCurrentRoom != 0)
            {
                SearchRoom(searchRoomIndex);
            }
            //else
            //{
            //    PickPath();
            //}
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    //  Search Room
    //  Same as patrol room but with different variables
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void SearchRoom(int roomIndex)
    {
        searchRoomIndex = roomIndex;
        if (monster.myDirector.monsterCurrentArea != 0)
        {
            if (currentSearchNode == null) // If null then we are not currently searching a room
            {
                Room currentRoom = rooms[monster.myDirector.FindCurrentRoom(monsterObject, monster.myDirector.monsterCurrentArea) - 1].GetComponent<Room>();
                currentRoom.searchChanceToLeave = currentRoom.StartingSearchChanceToLeave;

                if (monster.myDirector.monsterCurrentRoom == roomIndex)
                {
                    if (!isLooking)
                    {
                        float distance = float.MaxValue;
                        monster.myDirector.monsterCurrentRoom = monster.myDirector.FindCurrentRoom(monsterObject, monster.myDirector.monsterCurrentArea);
                        if (monster.myDirector.FindCurrentRoom(monsterObject, monster.myDirector.monsterCurrentArea) != 0)
                        {
                            if (monster.myDirector.rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolPoints.Length != 0) // If we have patrol points
                            {
                                foreach (GameObject patrolPoint in rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().patrolPoints)
                                {
                                    if (Vector3.Distance(monsterObject.transform.position, patrolPoint.transform.position) < distance)
                                    {
                                        distance = Vector3.Distance(monsterObject.transform.position, patrolPoint.transform.position);
                                        currentSearchNode = patrolPoint;
                                    }
                                }
                                agent.SetDestination(currentSearchNode.transform.position);
                            }
                            else // If the room doesn't have 
                            {
                                float leaveChance = 1.0f - currentSearchNode.GetComponent<PatrolPoint>().lookAroundChance - currentSearchNode.GetComponent<PatrolPoint>().roarChance;
                                float chanceResult = Random.Range(1, 100);
                                chanceResult = chanceResult / 100.0f;

                                if (chanceResult <= leaveChance) // Leave
                                {
                                    LeaveRoom();
                                }
                                else if (chanceResult <= leaveChance + currentSearchNode.GetComponent<PatrolPoint>().lookAroundChance) // Look around on spot
                                {
                                    // Look around stuff
                                    if (!justLooked)
                                    {
                                        LookBack();
                                    }
                                }
                                else
                                {
                                    //Roar
                                    Roar();
                                }
                            }
                        }
                    }
                }
                else
                {
                    agent.SetDestination(rooms[roomIndex - 1].transform.position);
                }
            }
            else // else we a searching a room with patrol points
            {
                if (!isLooking)
                {
                    if (rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().searchChanceToLeaveIncrease + rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().searchChanceToLeave + currentSearchNode.GetComponent<PatrolPoint>().lookAroundChance + currentSearchNode.GetComponent<PatrolPoint>().roarChance <= 1.0f)
                    {
                        rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().searchChanceToLeave += rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().searchChanceToLeaveIncrease;

                    }
                    float changePatrolPointChance = 1.0f - rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().searchChanceToLeave - currentSearchNode.GetComponent<PatrolPoint>().lookAroundChance - currentSearchNode.GetComponent<PatrolPoint>().roarChance;
                    float chanceResult = Random.Range(1, 100);
                    chanceResult = chanceResult / 100.0f;

                    if (chanceResult <= changePatrolPointChance) // Move between patrol points
                    {
                        justLooked = false;
                        int i = Random.Range(0, currentSearchNode.GetComponent<PatrolPoint>().connectedPoints.Length);
                        if (currentSearchNode.GetComponent<PatrolPoint>().connectedPoints[i] == lastSearchNode || currentSearchNode.GetComponent<PatrolPoint>().connectedPoints[i] == currentSearchNode)
                        {

                        }
                        else
                        {
                            agent.SetDestination(currentSearchNode.GetComponent<PatrolPoint>().connectedPoints[i].transform.position);

                            lastSearchNode = currentSearchNode;
                            currentSearchNode = currentSearchNode.GetComponent<PatrolPoint>().connectedPoints[i];
                        }
                    }
                    else if (chanceResult <= changePatrolPointChance + currentSearchNode.GetComponent<PatrolPoint>().lookAroundChance) // Look around on spot
                    {
                        // Look around stuff
                        if (!justLooked)
                        {
                            LookBack();
                        }
                    }
                    else if (chanceResult <= changePatrolPointChance + currentSearchNode.GetComponent<PatrolPoint>().lookAroundChance + currentSearchNode.GetComponent<PatrolPoint>().roarChance) // Look around on spot
                    {
                        // Roar
                        Roar();
                    }
                    else // Leave the room
                    {
                        LeaveRoom();
                    }
                }
            }

        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Check if Path Crosses Room
    // Returns true if the given path at any point cuts through the given room.
    // Returns false if otherwise.
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    bool CheckIfPathCrossesRoom(NavMeshPath path, GameObject room)
    {
        // Get the path segments

        int i = 0;
        PathSegment[] pathSegments = new PathSegment[path.corners.Length - 1];
        Vector3 lastCorner = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (Vector3 corner in path.corners)
        {
            if (lastCorner == new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
            {
                lastCorner = corner;
            }
            else
            {
                pathSegments[i] = new PathSegment(lastCorner, corner);
                lastCorner = corner;
                i++;
            }
        }

        // Get the rooms sides

        PathSegment[] rectSegments = new PathSegment[4];

        Vector3 extends = room.GetComponent<MeshFilter>().mesh.bounds.extents;
        Vector3 scale = room.GetComponent<MeshFilter>().transform.localScale;

        extends = new Vector3(extends.x * scale.x, extends.y * scale.y, extends.z * scale.z);

        Vector3 bottomLeft = room.transform.position + new Vector3(-extends.x, 0, -extends.z);
        Vector3 bottomRight = room.transform.position + new Vector3(extends.x, 0, -extends.z);
        Vector3 topLeft = room.transform.position + new Vector3(-extends.x, 0, extends.z);
        Vector3 topRight = room.transform.position + new Vector3(extends.x, 0, extends.z);

        rectSegments[0] = new PathSegment(topLeft, topRight);
        rectSegments[1] = new PathSegment(topRight, bottomRight);
        rectSegments[2] = new PathSegment(bottomRight, bottomLeft);
        rectSegments[3] = new PathSegment(bottomLeft, topLeft);

        // Check if any of the path segments intersect with the room sides

        foreach (PathSegment rectSegment in rectSegments)
        {
            foreach (PathSegment pathSegment in pathSegments)
            {
                if (pathSegment.LineToLineCollision(pathSegment, rectSegment))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Check Path Distance
    // Checks if the length of the path from the monsters current position to the desired room
    // Returns that length(distance)
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    float CheckPathDistance(GameObject room)
    {
        // Get a path

        NavMeshPath path;
        path = new NavMeshPath();

        NavMesh.CalculatePath(monster.transform.position, room.transform.position, 1, path);

        // Get the path segments

        int i = 0;
        float distance = 0.0f;
        Vector3 lastCorner = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (Vector3 corner in path.corners)
        {
            if (lastCorner == new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
            {
                lastCorner = corner;
            }
            else
            {
                distance += Vector3.Distance(lastCorner, corner);
                lastCorner = corner;
                i++;
            }
        }

        return distance;   
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Reset Patrol Nodes
    // Resets Patrol and Search Nodes
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void ResetPatrolNodes()
    {
        currentPatrolNode = null;
        lastPatrolNode = null;
        currentSearchNode = null;
        lastSearchNode = null;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Return Closest Patrol Point By Path
    // Takes a room which is the destination as a parameter
    // Returns the patrol point that is closest to the monster.(Closest by path not as the crow flies.)
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    Vector3 ReturnClosestPatrolPointByPath(Room room)
    {
        NavMeshPath tempPath;
        tempPath = new NavMeshPath();

        NavMeshHit myNavHit;
        NavMesh.SamplePosition(room.transform.position, out myNavHit, 100, NavMesh.AllAreas);

        agent.CalculatePath(myNavHit.position, tempPath);

        Vector3 closestCorner = tempPath.corners[0];
        foreach (Vector3 corner in tempPath.corners) // Get the closest corner to the room that is not in the room.
        {
            if (room.gameObject.GetComponent<MeshCollider>().bounds.Contains(new Vector3(corner.x, room.transform.position.y, corner.z)))
            {
                break;
            }
            else
            {
                closestCorner = corner;
            }
        }

        float distance = float.MaxValue;
        GameObject closestPatrolPoint = room.patrolPoints[0];
        foreach (GameObject patrolPoint in room.patrolPoints) // Get the closest patrol point to the closest corner.
        {
            if (Vector3.Distance(patrolPoint.transform.position, closestCorner) < distance)
            {
                closestPatrolPoint = patrolPoint;
                distance = Vector3.Distance(patrolPoint.transform.position, closestCorner);
            }
        }

        return closestPatrolPoint.transform.position;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Look Back
    // Sets values for the look action
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void LookBack()
    {
        monster.animator.SetBool("isLooking", true);
        isLooking = true;
        justLooked = true;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Finish Looking Back
    // Resets the values for the look action
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void FinishLookingBack()
    {
        monster.animator.SetBool("isLooking", false);
        isLooking = false;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Roar
    // Sets the values for the roar action
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void Roar()
    {
        monster.animator.SetBool("isRoaring", true);
        isRoaring = true;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Roar End
    // Resets the values for the roar action
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void RoarEnd()
    {
        monster.animator.SetBool("isRoaring", false);
        isRoaring = false;
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Leave Room
    // Gets the rooms connected to the monsters current room and weights them. A room will be weighted 1.0f, however if it was the last
    // room the monster was in before it's current room, then it will be weighted equal to the backtrack weight(generally a smaller value
    // than 1.0f) and if the room has a seal in it, it's weight is the standard + the sealRoomModifier making it more likely to be picked.
    // If a room has a seal and is the last room, it's weighted the backtrack weight.
    //
    // A option will then be chosen randomly with higher weighted rooms being more likely to be chosen.
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void LeaveRoom()
    {
        justLooked = false;
        isChasing = false;
        guessedWhichRoom = false;
        ResetPatrolNodes();

        int[] roomIndexes = new int[rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().connectedRooms.Length];
        int[] roomWeights = new int[rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().connectedRooms.Length];

        int weightsTotal = 0;

        // Mark rooms for who has seals
        foreach (GameObject room in rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().connectedRooms)
        {
            bool hasSeals = false;

            foreach (Seal seal in monster.myDirector.theHeart.sealList)
            {
                if (seal != null)
                {
                    if (room.GetComponent<MeshCollider>().bounds.Contains(new Vector3(seal.gameObject.transform.position.x, room.transform.position.y, seal.gameObject.transform.position.z)))
                    {
                        hasSeals = true;
                    }
                }
            }
            room.GetComponent<Room>().SetHasSeals(hasSeals);
        }

        // Weight Rooms - When weighting rooms, if a seal room is the last room a monster is in, it still takes the last room weight
        int i = 0;
        foreach (GameObject room in rooms[monster.myDirector.monsterCurrentRoom - 1].GetComponent<Room>().connectedRooms)
        {
            roomIndexes[i] = room.GetComponent<Room>().roomIndex;
            if (room.GetComponent<Room>().roomIndex == monster.myDirector.monsterLastRoom)
            {
                roomWeights[i] = (int)(backTrackWeight * 100.0f);
            }
            else if (room.GetComponent<Room>().HasSeals())
            {
                roomWeights[i] = (int)((1.0f + sealRoomModifier) * 100);
            }
            else
            {
                roomWeights[i] = 100; // Regular room is weighted 1.0f and needs to be multiplied by 100 like the others
            }
            i++;
        }

        // Get weight total
        foreach (int weight in roomWeights)
        {
            weightsTotal += weight;
        }

        // Get the room
        int randomNum = Random.Range(0, weightsTotal);

        i = 0;
        int range = roomWeights[i];
        int selectedRoom = 0;
        foreach (int room in roomIndexes)
        {
            if (randomNum <= range)
            {
                selectedRoom = roomIndexes[i];
                break;
            }
            i++;
            range += roomWeights[i];
        }

        // Send the monster
        if (rooms[selectedRoom - 1].GetComponent<Room>().usePatrolPoints)
        {
            agent.SetDestination(ReturnClosestPatrolPointByPath(rooms[selectedRoom - 1].GetComponent<Room>()));
        }
        else
        {
            agent.SetDestination(rooms[selectedRoom - 1].transform.position);
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // AtDestination
    // Returns true if the monster is at their destination.
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    bool AtDestination()
    {
        Vector3 pos = monsterObject.transform.position;
        Vector3 destination = agent.destination;

        pos.y = 0;
        destination.y = 0;

        if (pos == destination)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
