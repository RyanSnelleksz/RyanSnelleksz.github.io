using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    OnFoot,
    Swinging,
    Sliding
}


public class PlayerMovement : MonoBehaviour
{
    public float acceleration = 0.0f;
    public float maxGroundSpeed = 0.0f;
    public float jumpPower = 5.0f;
    public float gasPower = 10.0f; // Refers to strength of the mid-air gas propellant the player can use for air control and speed boost mid-air
    public float maxGas = 10.0f;
    public float minGas = 0.0f;
    public float currentGas;

    public float minFriction = 2.0f;
    public float maxFriction = 0.6f;


    //CharacterController characterController;

    GameObject cameraObject;
    public GameObject meshObject;

    State state;

    bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        //characterController = GetComponent<CharacterController>();

        cameraObject = transform.GetChild(0).gameObject;
        state = State.OnFoot;

        currentGas = maxGas;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        // Checking if touching the ground
        if (Physics.Raycast(transform.position, -Vector3.up, 1.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        // done checking

        float velocityMagnitude = GetComponent<Rigidbody>().velocity.magnitude;

        if (state == State.OnFoot)
        {
            if (GetComponent<CapsuleCollider>().material.dynamicFriction != maxFriction) // If it's not set to zero, set it to zero
            {
                GetComponent<CapsuleCollider>().material.dynamicFriction = maxFriction;
            }

            if (!isGrounded)
            {
                state = State.Swinging;
            }
            else if (velocityMagnitude > maxGroundSpeed * 1.25f)
            {
                state = State.Sliding;
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Insert))
            {
                GetComponent<Rigidbody>().AddForce(transform.up * jumpPower, ForceMode.Impulse); // INPUT DETECTION NEEDS TO BE IN UPDATE
            }

            float verticalMovement = Input.GetAxisRaw("Vertical");
            float horizontalMovement = Input.GetAxisRaw("Horizontal");

            Vector3 cameraForward = cameraObject.transform.forward; // Get cams forward
            Vector3 cameraRight = cameraObject.transform.right; // Get cams right


            Vector3 forward = verticalMovement * cameraForward;
            Vector3 right = horizontalMovement * cameraRight;

            Vector3 vec = new Vector3(forward.x + right.x, 0.0f, forward.z + right.z) * acceleration;

            if (Vector3.Magnitude(GetComponent<Rigidbody>().velocity) < maxGroundSpeed)
            {
                GetComponent<Rigidbody>().AddForce(vec, ForceMode.Acceleration);
            }

            //
            if (currentGas < maxGas)
            {
                currentGas += Time.deltaTime;
                if (currentGas >= maxGas)
                {
                    currentGas = maxGas;
                }
            }
        }


        if (state == State.Swinging)
        {
            if (GetComponent<CapsuleCollider>().material.dynamicFriction != minFriction) // If it's not set to zero, set it to zero
            {
                GetComponent<CapsuleCollider>().material.dynamicFriction = minFriction;
            }

            if (isGrounded && velocityMagnitude > maxGroundSpeed * 1.25f)
            {
                state = State.Sliding;
            }
            else if (isGrounded)
            {
                state = State.OnFoot;
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Insert))
            {
                if (currentGas > minGas)
                {
                    currentGas -= Time.deltaTime;
                    GetComponent<Rigidbody>().AddForce(cameraObject.transform.forward * gasPower, ForceMode.Acceleration);
                    if (currentGas <= minGas)
                    {
                        currentGas = minGas;
                    }
                }
            }
        }

        if (state == State.Sliding)
        {
            if (GetComponent<CapsuleCollider>().material.dynamicFriction != minFriction) // If it's not set to zero, set it to zero
            {
                GetComponent<CapsuleCollider>().material.dynamicFriction = minFriction;
            }


            if (velocityMagnitude <= maxGroundSpeed * 1.25f)
            {
                state = State.OnFoot;
            }
            else if (!isGrounded)
            {
                state = State.Swinging;
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Insert))
            {
                if (currentGas > minGas)
                {
                    currentGas -= Time.deltaTime;
                    GetComponent<Rigidbody>().AddForce(cameraObject.transform.forward * gasPower, ForceMode.Acceleration);
                    if (currentGas <= minGas)
                    {
                        currentGas = minGas;
                    }
                }
            }
        }

        Debug.Log(state);

        Vector3 turnVector = transform.position + cameraObject.transform.forward;
        turnVector.y = transform.position.y;

        meshObject.transform.LookAt(turnVector);
    }
}
