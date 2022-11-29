using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Tooltip("Every room should be indexed from 1 onwards. No room should have the index 0 or a matching index. This index is not individual to the area, it's all rooms in the map.")]
    public int roomIndex;
    [Tooltip("Put in the room planes for each room connected to this one.")]
    public GameObject[] connectedRooms;
    [Tooltip("Put in the rooms patrol points.")]
    public GameObject[] patrolPoints;
    //[Tooltip("Chance to leave the room while patrolling. Default 0.1 which is 10%")]
    //public float patrolChanceToLeave = 0.05f;
    [Tooltip("Starting chance to leave a room when patrolling")]
    public float StartingPatrolChanceToLeave = 0.0f;
    [Tooltip("Current chance to leave the room while patrolling it. Default 0.1 which is 10%")]
    public float patrolChanceToLeave = 0.0f;
    [Tooltip("How much the patrol chance to leave increases by after every action when searchings")]
    public float patrolChanceToLeaveIncrease = 0.1f;
    [Tooltip("Starting chance to leave a room when stearching")]
    public float StartingSearchChanceToLeave = 0.0f;
    [Tooltip("Current chance to leave the room while seachring it. Default 0.1 which is 10%")]
    public float searchChanceToLeave = 0.0f;
    [Tooltip("How much the search chance to leave increases by after every action when searchings")]
    public float searchChanceToLeaveIncrease = 0.1f;
    [Tooltip("Check as true if you want the monster to use patrol points in that room and tick false if you just want him to go to the center")]
    public bool usePatrolPoints;

    bool hasSeals = false;

    public bool HasSeals()
    {
        return hasSeals;
    }

    public void SetHasSeals(bool state)
    {
        hasSeals = state;
    }
}
