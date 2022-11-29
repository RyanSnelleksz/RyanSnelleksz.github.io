using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltCharMovement : MonoBehaviour
{
    Camera cam;
    float camRotationX = 0.0f;
    float camRotationY = 0.0f;

    [Header("Movement")]
    [Tooltip("How fast you walk")]
    public float walkSpeed = 10.0f;
    [Tooltip("How fast you sprint")]
    public float sprintSpeed = 14.0f;

    [Header("Camera")]
    [Tooltip("How fast the camera moves")]
    public float mouseSensitivity = 10.0f;

    [Header("Head bobbing")]
    [Tooltip("How far up and down the head will bob")]
    public float headBobRange = 0.2f;
    [Tooltip("How fast the head will bob")]
    public float headBobSpeed = 0.1f;
    [Tooltip("How fast the head will bob when sprinting")]
    public float headBobSprintSpeed = 2.0f;
    float headOffset = 0.0f;
    float headCurrentOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        headOffset = cam.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Sprint Check

        bool sprinting = false;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            sprinting = true;
        }

        float speed = sprinting ? sprintSpeed : walkSpeed;
        float bobSpeed = sprinting ? headBobSprintSpeed : headBobSpeed;

        // Translation

        float translationZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(0, 0, translationZ);  // THERE IS NO DIAGONAL SPEED CORRECTION ATM
        transform.Translate(translationX, 0, 0);

        // Rotation

        camRotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        camRotationY += Input.GetAxis("Mouse Y") * mouseSensitivity;

        cam.transform.rotation = Quaternion.Euler(-camRotationY, camRotationX, 0f);

        Quaternion rotation = cam.transform.rotation;
        rotation.x = 0;
        rotation.z = 0;

        transform.rotation = rotation;

        // Head Bobbing

        headCurrentOffset += bobSpeed * Time.deltaTime;

        cam.transform.position = new Vector3(cam.transform.position.x, headOffset + headCurrentOffset, cam.transform.position.z);

        if (headCurrentOffset > headBobRange)
        {
            headCurrentOffset = headBobRange;
            headBobSpeed = -headBobSpeed;
            headBobSprintSpeed = -headBobSprintSpeed;
        }
        else if (headCurrentOffset < -headBobRange)
        {
            headCurrentOffset = -headBobRange;
            headBobSpeed = -headBobSpeed;
            headBobSprintSpeed = -headBobSprintSpeed;
        }
    }
}
