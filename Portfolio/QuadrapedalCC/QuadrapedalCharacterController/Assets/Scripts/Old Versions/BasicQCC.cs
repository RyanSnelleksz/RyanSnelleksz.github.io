using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicQCC : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float maxRotationSpeed = 100.0f;

    public Vector3 velocity;
    public Vector3 angularVelocity;

    public float acceleration;
    public float deceleration;

    public float turnForwardAcceleration;
    public float minTurnForwardSpeed;

    public float angularAcceleration;
    public float angularDeceleration;

    float gravity = Physics.gravity.y;
    bool grounded = false;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxisRaw("Vertical");
        float rotation = Input.GetAxisRaw("Horizontal");

        if (translation > 0)
        {
            velocity.z += translation * acceleration;
        }
        else if (translation < 0)
        {
            velocity.z += translation * acceleration;
        }
        else
        {
            if (rotation == 0)
            {
                if (velocity.z > -deceleration * 1.5 && velocity.z < deceleration * 1.5)
                {
                    velocity.z = 0;
                }
                else if (velocity.z > 0)
                {
                    velocity.z -= deceleration;
                }
                else if (velocity.z < 0)
                {
                    velocity.z += deceleration;
                }
            }
        }

        if (rotation > 0)
        {
            angularVelocity.y += rotation * angularAcceleration;
            velocity.z += rotation * turnForwardAcceleration;
        }
        else if (rotation < 0)
        {
            angularVelocity.y += rotation * angularAcceleration;
            velocity.z -= rotation * turnForwardAcceleration;
        }
        else
        {
            if (angularVelocity.y > -angularDeceleration * 1.5 && angularVelocity.y < angularDeceleration * 1.5)
            {
                angularVelocity.y = 0;
            }
            else if (angularVelocity.y > 0)
            {
                angularVelocity.y -= angularDeceleration;
            }
            else if (angularVelocity.y < 0)
            {
                angularVelocity.y += angularDeceleration;
            }
        }

        if (velocity.z > maxSpeed)
        {
            velocity.z = maxSpeed;
        }
        else if (velocity.z < -maxSpeed)
        {
            velocity.z = -maxSpeed;
        }

        if (angularVelocity.y > maxRotationSpeed)
        {
            angularVelocity.y = maxRotationSpeed;
        }
        else if (angularVelocity.y < -maxRotationSpeed)
        {
            angularVelocity.y = -maxRotationSpeed;
        }

        translation = velocity.z;
        rotation = angularVelocity.y;

        if (translation != 0)
        {
            animator.SetFloat("Speed", translation);
        }
        else
        {
            animator.SetFloat("Speed", translation);
        }

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        if (!grounded)
        {
            transform.Translate(0, gravity * Time.deltaTime, 0);
        }

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.isStatic)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.isStatic)
        {
            grounded = false;
        }
    }
}
