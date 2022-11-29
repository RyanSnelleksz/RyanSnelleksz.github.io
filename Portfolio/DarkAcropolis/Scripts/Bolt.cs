using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    MenuManager winCanvas;

    //When the bolt collides with a object that has a collider
    private void OnCollisionEnter(Collision collision)
    {
        //Create a reference to the heart and monster they collided with the bolt
        Heart heartCollide = collision.gameObject.GetComponent<Heart>();
        Monster monster = collision.gameObject.GetComponent<Monster>();

        winCanvas = FindObjectOfType<MenuManager>();

        //If the heart was collided
        if (heartCollide != null)
        {
            //If all the seal have been destroyed
            if(heartCollide.HeartLocked())
            {
                //FindObjectOfType<AudioManager>().PlaySound(0, "HeartAttack");
                //Destroy the heart and open the win screen
                Destroy(collision.gameObject);
                winCanvas.Win();
            }
        }
        //If the monster was collided
        else if (monster != null)
        {
            //Stun the monster
            monster.GetStunned();
        }

        //Destroy the bolt
        Destroy(gameObject);
    }
}