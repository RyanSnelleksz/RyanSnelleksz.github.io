using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinorLose : MonoBehaviour
{
    public PauseManager pause;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    //float newTime = 0.0f;
    //float oldTime = 0.0f;
    //bool timeStop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //oldTime += Time.deltaTime;
        //newTime = oldTime % 60;
        //
        //if(newTime >= 100 && !timeStop)
        //{
        //    Lose();
        //    timeStop = true;
        //}
        //Debug.Log(newTime);
    }

    public void Win()
    {
        WinScreen.SetActive(true);
        pause.Paused();
        //timeStop = true;
    }

    public void Lose()
    {
        LoseScreen.SetActive(true);
        pause.Paused();
    }
}
