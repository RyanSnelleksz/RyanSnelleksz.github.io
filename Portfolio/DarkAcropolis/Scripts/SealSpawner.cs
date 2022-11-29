using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SealSpawner : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject[] seals;

    public GameObject[] monsterSpawnPoints;
    public GameObject monster;

    List<int> spawnGroups = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject seal in seals)
        {
            bool spawned = false;
            while (!spawned)
            {
                int i = Random.Range(0, spawnPoints.Length - 1);

                if (spawnPoints[i].GetComponent<SpawnPoint>().taken == false && !GroupUsed(spawnPoints[i].GetComponent<SpawnPoint>().groupIndex))
                {
                    seal.transform.position = spawnPoints[i].transform.position;
                    spawnPoints[i].GetComponent<SpawnPoint>().taken = true;
                    spawnGroups.Add(spawnPoints[i].GetComponent<SpawnPoint>().groupIndex);
                    spawned = true;
                }
            }
        }

        int j = Random.Range(0, monsterSpawnPoints.Length);

        monster.GetComponent<NavMeshAgent>().Warp(monsterSpawnPoints[j].transform.position);

    }

    bool GroupUsed(int groupIndex)
    {
        if (spawnGroups != null)
        {
            foreach (int index in spawnGroups)
            {
                if (index == groupIndex)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
