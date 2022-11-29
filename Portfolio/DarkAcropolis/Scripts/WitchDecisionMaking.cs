using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WitchDecisionMaking : MonoBehaviour
{
//    GameObject monsterObject; // The object that the components are on
//    Monster monster; // The monster script
//    NavMeshAgent agent; // The monsters navmesh agent

//    public GameObject player;

//    [Tooltip("True if the monster is mid attack animation")]
//    public bool isAttacking = false;

//    float attackTimer = 3.0f;

//    private void Start()
//    {
//        monsterObject = gameObject;
//        monster = gameObject.GetComponent<Monster>();
//        agent = gameObject.GetComponent<NavMeshAgent>();
//    }

//    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//    // Set Decision Tree Variables
//    // Used for setting up the variables the decision tree uses in the monster
//    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//    public void SetDecisionTreeVariables(GameObject mObject, Monster mon, NavMeshAgent monsterAgent)
//    {
//        monsterObject = mObject;
//        monster = mon;
//        agent = monsterAgent;
//    }

//    public void MoveToAttack(GameObject player)
//    {
//        Vector3 destination;
//        Vector3 direction = Vector3.Normalize(player.transform.position - monsterObject.transform.position);

//        destination = player.transform.position + (direction * (monster.monsterAttackRange - 1));

//        agent.SetDestination(destination);
//    }

//    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//    // Root Decision
//    // The decision you must call in the monsters Update() function to get the deicision tree to do anything, if you dont call this, the
//    // monster won't do anything and you only call this function and Set Decision Tree Variables in the Start() function
//    //
//    // It is also the ChasingDecision
//    // Will check if the monster is mid chase and if so what to do when it loses sight and when to end the chase
//    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//    public void RootDecision()
//    {
//        if (!monster.isStunned)
//        {

//            // If I can see
//                // Attack
//            // If not
//                // move
//        }
//        else
//        {
//            agent.enabled = false;
//        }
//    }

//    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//    // Attack Behaviour
//    // How the monster tries to hurt the player
//    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//    void AttackBehaviour()
//    {
//        if (Vector3.Distance(monsterObject.transform.position, monster.myDirector.playerObject.transform.position) <= monster.monsterAttackRange)
//        {
//            agent.SetDestination(monsterObject.transform.position);

//            //Attack

//            Vector3 dir = monster.myDirector.playerObject.transform.position - transform.position;
//            dir.y = 0;
//            Quaternion rot = Quaternion.LookRotation(dir);
//            transform.rotation = Quaternion.Lerp(transform.rotation, rot, agent.angularSpeed * Time.deltaTime);

//            // Temp
//            if (attackTimer > 3.0f)
//            {
//                monster.myDirector.playerObject.GetComponent<PlayerController>().currentHealth -= 1;
//                attackTimer = 0.0f;
//                monster.animator.SetBool("isAttacking", true);
//                isAttacking = true;
//            }
//            else
//            {
//                if (!isAttacking)
//                {
//                    attackTimer += Time.deltaTime;
//                }
//            }
//        }
//        else
//        {
//            //MoveToAttack(monster.myDirector.playerObject);
//            agent.SetDestination(monster.myDirector.playerObject.transform.position);
//        }
//    }
//    public void AttackEnd()
//    {
//        monster.animator.SetBool("isAttacking", false);
//        isAttacking = false;
//        attackTimer = 0.0f;
//    }

//    bool LineOfSight()
//    {
//        Bounds playerBounds = player.GetComponent<CharacterController>().bounds;
//        RaycastHit hit;
//        Vector3 rayDirection = playerBounds.center - visionObject.transform.position;

//        angle = Vector3.Angle(rayDirection, visionObject.transform.forward);
//        hasLineOfSight = false;

//        if (Vector3.Angle(rayDirection, visionObject.transform.forward) > fieldOfView) // Checking if the player is in the monsters FOV
//        {
//            rayDirection = -transform.forward;
//            if (Physics.Raycast(new Vector3(visionObject.transform.position.x, playerBounds.center.y, visionObject.transform.position.z), rayDirection, out hit, behindVisionLength)) // Checking directly behind the monster
//            {
//                if (hit.collider.gameObject.CompareTag("Player"))
//                {
//                    hasLineOfSight = true;
//                }
//            }
//        }
//        else
//        {
//            if (Physics.Raycast(visionObject.transform.position, rayDirection, out hit, Mathf.Infinity)) // Checking if theres anything in the way
//            {
//                if (hit.collider.gameObject.CompareTag("Player"))
//                {
//                    hasLineOfSight = true;
//                }
//            }
//        }
//        return hasLineOfSight;
//    }
}
