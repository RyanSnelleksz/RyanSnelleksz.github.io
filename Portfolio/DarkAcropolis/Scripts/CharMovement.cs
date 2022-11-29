using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour
{
    Camera cam;
    float camRotationX = 0.0f;
    float camRotationY = 0.0f;

    public float walkSpeed = 10.0f;
    public float sprintSpeed = 14.0f;
    public float rotationSpeed = 100.0f;

    public float mouseSensitivity = 10.0f;

    public float headBobRange = 0.2f;
    public float headBobSpeed = 0.1f;
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

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Keypad9))
        {
            sprinting = true;
        }

        float speed = sprinting ? sprintSpeed : walkSpeed;
        float bobSpeed = sprinting ? headBobSprintSpeed : headBobSpeed;

        // Translation

        float translationZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translationZ);
        transform.Rotate(0.0f, rotation, 0.0f);

        // Rotation

        camRotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        camRotationX += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        camRotationY += Input.GetAxis("Mouse Y") * mouseSensitivity;

        cam.transform.rotation = Quaternion.Euler(-camRotationY, camRotationX, 0f);

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
