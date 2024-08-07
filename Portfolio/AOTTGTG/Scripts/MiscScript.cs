using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject treePrefab;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        for (int i = -1000; i < 1000; i++)
        {
            for (int j = -1000; j < 1000; j++)
            {
                if (i % 100 == 0 && j % 100 == 0)
                {
                    GameObject newTree;
                    newTree = Instantiate(treePrefab);

                    newTree.transform.position = new Vector3((float)i, 100.0f, (float)j);
                }
            }
        }

    }
}
