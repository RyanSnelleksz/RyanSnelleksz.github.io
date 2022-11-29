using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [Tooltip("No area should be given an index of 0. Areas should be indexed in ascending order starting from 1.")]
    public int areaIndex;
    [Tooltip("Put in all the planes that represents rooms in this area.")]
    public GameObject[] rooms;
    [Tooltip("Areas should have a childed empty gameobject that is in the most spacious spot of the centermost room in the area.")]
    public GameObject focalPoint;
}
