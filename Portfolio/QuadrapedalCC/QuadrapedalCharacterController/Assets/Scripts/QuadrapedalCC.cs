using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Walking,
    Jumping
}

public class QuadrapedalCC : MonoBehaviour
{
    GameObject head;
    Transform headTransform;

    State state = State.Walking;
    State lastState = State.Jumping;

    public float maxSpeed;
    public float maxRotationSpeed;

    public float jumpPower;

    public float walkSpeed;
    public float sprintSpeed;
    bool sprinting = false;
    public float walkRotation;
    public float sprintRotation;

    public bool isGrounded = false;
    float distToGround;

    public Vector3 velocity;
    public Vector3 angularVelocity;

    public float animationRotation;

    public float acceleration;
    public float deceleration;

    public float turnForwardAcceleration;
    public float minTurnForwardSpeed;

    public float angularAcceleration;
    public float angularDeceleration;

    float gravity = Physics.gravity.y;
    bool grounded = false;

    Animator animator;
    float animationLerpValue = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;

        animator = GetComponent<Animator>();

        head = GameObject.FindGameObjectWithTag("Head");
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGrounded();
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.1f))
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal));
            }
        }

        if (isGrounded && state != lastState)
        {
            state = State.Walking;
            lastState = State.Jumping;
        }
        else if (!isGrounded && state != lastState)
        {
            state = State.Jumping;
            lastState = State.Walking;
        }

        if (state == State.Walking)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                animator.SetBool("isJumping", true);
            }

            float translation = Input.GetAxisRaw("Vertical");
            float rotation = Input.GetAxisRaw("Horizontal");

            // Sprinting Stuff 
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }

            maxSpeed = sprinting ? sprintSpeed : walkSpeed;
            maxSpeed = translation < 0 ? walkSpeed : maxSpeed;

            maxRotationSpeed = sprinting ? sprintRotation : walkRotation;

            //

            // Directional Translation
            if (translation != 0)
            {
                velocity.z += translation * acceleration;

                if (rotation == 0) // Need to decelerate animation rotation if only moving forward
                {
                    if (animationRotation > -angularDeceleration * 1.5 && animationRotation < angularDeceleration * 1.5)
                    {
                        animationRotation = 0;
                    }
                    else if (animationRotation > 0)
                    {
                        animationRotation -= angularDeceleration;
                    }
                    else if (animationRotation < 0)
                    {
                        animationRotation += angularDeceleration;
                    }
                }
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

            if (velocity.x > -deceleration - 0.1f && velocity.x < deceleration + 0.1f)
            {
                velocity.x = 0.0f;
            }
            else if (velocity.x > 0)
            {
                velocity.x -= deceleration;
            }
            else if (velocity.x < 0)
            {
                velocity.x += deceleration;
            }


            //

            // Rotation
            if (rotation != 0) // Fix the delay between the animation and angular velocity
            {
                // if rotating and very different from animation rotation. Set angular vel to animation rot
                if (angularVelocity.y != animationRotation)
                {
                    angularVelocity.y = animationRotation;
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
            //

            // Animation Rotation
            if (rotation > 0)
            {
                animationRotation += rotation * angularAcceleration;
            }
            else if (rotation < 0)
            {
                animationRotation += rotation * angularAcceleration;
            }
            //

            // Making sure velocity, angular velocity and animation rotation don't exceed limits
            if (velocity.z > maxSpeed)
            {
                velocity.z -= deceleration + acceleration;
                if (rotation != 0)
                {
                    velocity.z -= turnForwardAcceleration;
                }
            }
            else if (velocity.z < -maxSpeed)
            {
                velocity.z += deceleration + acceleration;
                if (rotation != 0)
                {
                    velocity.z += turnForwardAcceleration;
                }
            }

            if (angularVelocity.y > maxRotationSpeed)
            {
                angularVelocity.y -= angularDeceleration + angularAcceleration;
            }
            else if (angularVelocity.y < -maxRotationSpeed)
            {
                angularVelocity.y += angularDeceleration + angularAcceleration;
            }

            if (animationRotation > maxRotationSpeed)
            {
                animationRotation -= angularDeceleration + angularAcceleration;
            }
            else if (animationRotation < -maxRotationSpeed)
            {
                animationRotation += angularDeceleration + angularAcceleration;
            }
            //

            translation = velocity.z;
            rotation = angularVelocity.y;

            // Animation Stuff
            // Walking / Running
            animator.SetFloat("Speed", translation);

            float goal = 1.0f;
            float currentGoal = 1.0f;

            if (Input.GetAxisRaw("Vertical") != 0) // We gotta lerp between turning left and right for when the quadruped walks backwards
            {
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    currentGoal = goal;
                }
                else
                {
                    currentGoal = 0.0f;
                }

                if (animationLerpValue > currentGoal)
                {
                    animationLerpValue -= 1.0f * Time.deltaTime;
                    if (animationLerpValue < 0.0f)
                    {
                        animationLerpValue = 0.0f;
                    }
                }
                else if (animationLerpValue < currentGoal)
                {
                    animationLerpValue += 1.0f * Time.deltaTime;
                    if (animationLerpValue > goal)
                    {
                        animationLerpValue = goal;
                    }
                }
            }

            float animationResult = Mathf.Lerp(-animationRotation, animationRotation, animationLerpValue);
            float rotationResult = Mathf.Lerp(-angularVelocity.y, angularVelocity.y, animationLerpValue);

            animator.SetFloat("Rotation", rotationResult);
            // Animation Rotation
            animator.SetFloat("AnimationRotation", animationResult);

            //Apply

            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") > 0)
            {
                if (animationRotation < angularVelocity.y)
                {
                    angularVelocity.y -= angularAcceleration * 3.0f;
                }
                else if (animationRotation > angularVelocity.y)
                {
                    angularVelocity.y += angularAcceleration * 3.0f;
                }
            }

            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

            if (!isGrounded)
            {
                velocity += Physics.gravity * Time.deltaTime;
            }

            transform.Translate(velocity * Time.deltaTime);
            transform.Rotate(0, rotation, 0);
        }
        else if (state == State.Jumping)
        {
            animator.SetBool("isJumping", false);

            velocity += Physics.gravity * Time.deltaTime;
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    public void ApplyJump()
    {
        velocity.y += jumpPower;
    }

    // Make sure your guy has a rigidbody that is not kinematic and has all constraints on.
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Hit");
        float elasticity = 0.1f;

        foreach (ContactPoint contact in collision.contacts)
        {
            transform.position += -contact.normal * ((contact.separation / collision.contactCount) - 0.01f);

            velocity += (-((1 + elasticity) * Vector3.Dot(velocity, contact.normal) * contact.normal)) / collision.contactCount;
            Debug.Log("H");
        }
    }

    private bool IsGrounded()
    {
        bool grounded = false;

        if (Physics.Raycast(transform.position, -Vector3.up, 0.1f))
        {
            grounded = true;
        }
        return grounded;
    }
}
