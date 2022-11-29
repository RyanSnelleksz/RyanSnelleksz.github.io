using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Director : MonoBehaviour
{
    [Header("Menace Values:")]
    [Tooltip("For Debug")]
    public Slider menaceSlider;
    [Tooltip("Menace Max")]
    public float menaceMaxThreshold;
    [Tooltip("Menace current and starting value")]
    public float menaceGauge;
    [Tooltip("Minimum menace. Recommended Value: 0")]
    public float menaceMinThreshold;
    [Tooltip("How low menace has to be for clues to be sent")]
    public float pressureThreshold;
    [Tooltip("How fast menace goes up when the player sees the monster.")]
    public float menaceIncrease;
    [Tooltip("How fast menace goes down when the player can't see the monster.")]
    public float menaceDecrease = 0.0f;
    [Tooltip("How close the monster needs to be to start affecting the menace.")]
    public float menaceRange;

    [Header("Heat and seal variables:")]
    [Tooltip("Add in the heart")]
    public Heart theHeart;
    public float menaceSealincrease;

    [Header("Clue Values:")]
    [Tooltip("The minimum time the director will wait between giving clues")]
    public float clueCooldown;
    bool clueSent = false;
    [Tooltip("Current time remaining until another clue can be sent")]
    public float clueTime;

    [Header("Area Values")]
    [Tooltip("Put in the area planes")]
    public GameObject[] areas;
    [Tooltip("You don't need to do anything with this")]
    public Area[] areaData;
    public GameObject[] rooms;

    [Header("Entity Values:")]
    [Tooltip("Put in the player gameobject")]
    public GameObject playerObject;
    [Tooltip("Put in the camera")]
    public Camera playerCam;
    [Tooltip("Is true if the player can see the monster")]
    public bool canSeeMonster;
    [Tooltip("Put in the monster gameobject")]
    public GameObject monsterObject;
    Monster monster;
    SkinnedMeshRenderer monsterMeshRenderer;
    NavMeshAgent agent;

    float distance;

    [Header("You can't change these.")]
    [Header("Player/Monster area and room data:")]
    [Tooltip("The area the player is currently in")]
    public int playerCurrentArea; // Both the player and the monster will always be in a area
    [Tooltip("The area the monster is currently in")]
    public int monsterCurrentArea;
    [Tooltip("The room the player is currently in/closest to")]
    public int playerNearestRoom; // But not always in a room e.g corridors
    [Tooltip("The room the monster is currently in/closest to")]
    public int monsterNearestRoom;

    [Tooltip("The last room the monster was in")]
    public int monsterLastRoom;
    [Tooltip("The room the monster is currently in")]
    public int monsterCurrentRoom;
    [Tooltip("The last room the monster saw the player in")]
    public int playerLastSeenRoom;

    [Tooltip("Indicates the level between the top and middle floor")]
    public GameObject highFloorIndicator;
    [Tooltip("Indicates the level between the bottom and middle floor")]
    public GameObject lowFloorIndicator;

    [Header("Other")]
    public float stingCooldown = 20.0f;
    float stingTimer = 0.0f;
    [Tooltip("Range for the Sting SFX")]
    public float stingRange = 1000.0f;
    bool stingReady = true; // Both the player and the monster will always be in a area

    // Start is called before the first frame update
    void Start()
    {
        areaData = new Area[areas.Length]; // Getting area data
        int i = 0;
        foreach (GameObject area in areas)
        {
            areaData[i] = areas[i].GetComponent<Area>();
            i++;
        }

        int roomCount = 0;
        foreach (Area area in areaData) // Counting the rooms
        {
            foreach (GameObject room in area.rooms)
            {
                roomCount++;
            }
        }
        rooms = new GameObject[roomCount];
        i = 0;
        foreach (Area area in areaData) // Making an array with all the rooms
        {
            foreach (GameObject room in area.rooms)
            {
                rooms[i] = room;
                //room.GetComponent<Room>().patrolChanceToLeave = 0.8f; // debug
                //room.GetComponent<Room>().searchChanceToLeave = 0.7f; // debug
                i++;
            }
        }

        monster = monsterObject.GetComponent<Monster>();
        agent = monsterObject.GetComponent<NavMeshAgent>();
        monsterMeshRenderer = monsterObject.GetComponentInChildren<SkinnedMeshRenderer>();

        //monster.ReadyMonsterAI();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(monster.transform.position, menaceRange);

        //Vector3 rayDirection = monster.transform.position - playerObject.transform.position;
        //Gizmos.DrawRay(playerObject.transform.position, rayDirection);
    }

    // Update is called once per frame
    void Update()
    {
        // Menace Gauge //

        canSeeMonster = CanPlayerSeeMonster();

        if (stingReady && canSeeMonster) 
        {
            
            // play Sting
            //FindObjectOfType<AudioManager>().PlaySound(0, "Monster Found");
            stingReady = false;
            stingTimer = stingCooldown;
        }
        if (!stingReady)
        {
            if (stingTimer < 0.0f)
            {
                monster.myDirector.ReadySting();
            }
            else
            {
                stingTimer -= Time.deltaTime;
            }
        }

        distance = Vector3.Distance(playerObject.transform.position, monsterObject.transform.position);

        if (monster.monsterDecisionMaking.isChasing)
        {
            menaceGauge += menaceIncrease;
        }
        else if (canSeeMonster && menaceGauge < menaceMaxThreshold)
        {
            menaceGauge += menaceIncrease;
        }
        else if (distance > menaceRange && menaceGauge > menaceMinThreshold)
        {
            menaceGauge -= Time.deltaTime + menaceDecrease;
        }

        menaceSlider.value = menaceGauge;

        // Tracking Rooms and Areas //

        int i = 0;
        foreach (Area area in areaData) // Finding what area the player/monster is in
        {
            if (WithinArea(areas[i], playerObject))
            {
                playerCurrentArea = area.areaIndex;
            }

            if (WithinArea(areas[i], monsterObject))
            {
                monsterCurrentArea = area.areaIndex;
            }
            i++;
        }
        i = 0;

        // Finding what rooms they are near
        monsterNearestRoom = FindNearestRoomInArea(monsterObject, monsterCurrentArea).roomIndex;
        playerNearestRoom = FindNearestRoomInArea(playerObject, playerCurrentArea).roomIndex;

        // Finding exactly what room the monster is in/ was last in
        if (FindCurrentRoom(monsterObject, monsterCurrentArea) != monsterCurrentRoom && monsterCurrentRoom != 0)
        {
            monsterLastRoom = monsterCurrentRoom;
        }
        monsterCurrentRoom = FindCurrentRoom(monsterObject, monsterCurrentArea);

        // Tracking where the monster last saw the player
        if (monster.Squint())
        {
            playerLastSeenRoom = FindCurrentRoom(playerObject, playerCurrentArea);
        }

        // Debug Slider //

        Vector3 green = new Vector3(0, 1, 0); // Green is relaxed
        Vector3 red = new Vector3(1, 0, 0); // Red is stressed
        Vector3 c = Vector3.Lerp(green, red, menaceSlider.value / menaceSlider.maxValue);
        menaceSlider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color(c.x, c.y, c.z);

        // Sending Area and clues //

        if (clueSent == false)
        {
            if (menaceGauge <= pressureThreshold)
            {
                if (monsterCurrentArea != playerCurrentArea)
                {
                    monster.SetTargetArea(playerCurrentArea);  //agent.SetDestination(areaData[playerCurrentArea - 1].focalPoint.transform.position);
                    clueSent = true;
                    clueTime = clueCooldown;
                }
            }
        }

        clueTime -= Time.deltaTime;
        if (clueTime <= 0)
        {
            clueSent = false;
        }
    }

    bool WithinArea(GameObject area, GameObject entity)
    {
        if (SameFloor(entity, area))
        {
            if (area.GetComponent<MeshCollider>().bounds.Contains(new Vector3(entity.transform.position.x, area.transform.position.y, entity.transform.position.z)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    Room FindNearestRoomInArea(GameObject entity, int areaIndex)
    {
        float distance = float.MaxValue;
        Room closestRoom = null;
        foreach (GameObject room in areaData[areaIndex - 1].rooms)
        {
            if (SameFloor(entity, room))
            {
                if (room.GetComponent<MeshCollider>().bounds.Contains(new Vector3(entity.transform.position.x, room.transform.position.y, entity.transform.position.z)))
                {
                    return room.GetComponent<Room>();
                }
                if (Vector3.Distance(room.transform.position, entity.transform.position) < distance)
                {
                    distance = Vector3.Distance(room.transform.position, entity.transform.position);
                    closestRoom = room.GetComponent<Room>();
                }
            }
        }
        return closestRoom;
    }

    public int FindCurrentRoom(GameObject entity, int areaIndex)
    {
        Room currentRoom = null;
        foreach (GameObject room in areaData[areaIndex - 1].rooms)
        {
            if (SameFloor(entity, room))
            {
                if (room.GetComponent<MeshCollider>().bounds.Contains(new Vector3(entity.transform.position.x, room.transform.position.y, entity.transform.position.z)))
                {
                    return room.GetComponent<Room>().roomIndex;
                }
            }
        }
        return 0;
    }

    bool SameFloor(GameObject entity, GameObject subject)
    {
        if (entity.transform.position.y > highFloorIndicator.transform.position.y && subject.transform.position.y > highFloorIndicator.transform.position.y) // If both on top floor
        {
            return true;
        }
        else if (entity.transform.position.y < highFloorIndicator.transform.position.y && subject.transform.position.y < highFloorIndicator.transform.position.y && entity.transform.position.y > lowFloorIndicator.transform.position.y && subject.transform.position.y > lowFloorIndicator.transform.position.y) // if both on middle floor
        {
            return true;
        }
        else if (entity.transform.position.y < lowFloorIndicator.transform.position.y && subject.transform.position.y < lowFloorIndicator.transform.position.y) // if both on bottom floor
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CanPlayerSeeMonster()
    {
        RaycastHit hit;
        Vector3 rayDirection = monsterObject.transform.position - playerCam.transform.position;

        rayDirection.y = 0;

        float fieldOfView = 40.0f;
        float angle = Vector3.Angle(rayDirection, playerCam.transform.forward);

        bool sight = false;

        if (Vector3.Angle(rayDirection, playerCam.transform.forward) > fieldOfView) // Checking if the player is in the monsters FOV
        {

        }
        else
        {
            if (Physics.Raycast(playerCam.transform.position, rayDirection, out hit, Mathf.Infinity)) // Checking if theres anything in the way
            {
                if (hit.collider.gameObject.CompareTag("Monster"))
                {
                    if (Vector3.Distance(monsterObject.transform.position, playerObject.transform.position) <= stingRange)
                    {
                        sight = true;
                    }
                    //Debug.Log("PlaySting");
                    
                }
            }
        }
        return sight;
    }

    public void IncreaseMenanceDecrease()
    {
        menaceDecrease += menaceSealincrease;
    }

    public void ReadySting()
    {
        stingReady = true;
    }
}
