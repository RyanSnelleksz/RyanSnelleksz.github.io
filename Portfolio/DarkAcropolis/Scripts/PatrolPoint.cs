using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public GameObject[] connectedPoints;

    [Tooltip("The chance in which he will look around the room on the spot. e.g 0.3 is 30% chance. don't make it too big because it has to fit with leave room chances while leaving room for the change patorl point chance.")]
    public float lookAroundChance = 0.4f;
    [Tooltip("The chance in which he will roar on the spot. e.g 0.3 is 30% chance. don't make it too big because it has to fit with leave room chances while leaving room for the change patorl point chance.")]
    public float roarChance = 0.05f;

    Color[] colours = new Color[7];

    private void Awake()
    {
        colours[0] = Color.grey;
        colours[1] = Color.blue;
        colours[2] = Color.green;
        colours[3] = Color.red;
        colours[4] = Color.cyan;
        colours[5] = Color.magenta;
        colours[6] = Color.yellow;
    }

    private void OnDrawGizmos()
    {
        colours[0] = Color.grey;
        colours[1] = Color.blue;
        colours[2] = Color.green;
        colours[3] = Color.red;
        colours[4] = Color.cyan;
        colours[5] = Color.magenta;
        colours[6] = Color.yellow;

        int cIndex = 0;
        for (int i = 0; i < connectedPoints.Length; i++)
        {
            Gizmos.color = colours[cIndex];
            Gizmos.DrawLine(transform.position, connectedPoints[i].transform.position);
            cIndex++;
            if (cIndex > 6)
            {
                cIndex = 0;
            }
        }
    }
}
