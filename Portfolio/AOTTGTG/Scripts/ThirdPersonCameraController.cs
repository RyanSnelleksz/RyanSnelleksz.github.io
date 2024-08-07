using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public GameObject cameraFocalPoint;

    public float cameraSensitivity = 1.0f;

    public float cameraDistance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraDistance = Vector3.Distance(transform.position, cameraFocalPoint.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMouseMovement = Input.GetAxis("Mouse X");
        float verticalMouseMovement = -Input.GetAxis("Mouse Y");

        Vector3 cameraPos = transform.position; // Get the cameras position

        Vector3 direction = transform.position - cameraFocalPoint.transform.position; // direction pointing to the camera from the player

        direction.Normalize();

        direction = Quaternion.AngleAxis(horizontalMouseMovement, transform.up) * direction;

        direction = Quaternion.AngleAxis(verticalMouseMovement, transform.right) * direction;

        if (direction.y > 0.9f)
        {
            direction.y = 0.9f;
        }
        else if (direction.y < -0.9f)
        {
            direction.y = -0.9f;
        }

        cameraPos = cameraFocalPoint.transform.position; // set cam position to players(origin)

        transform.position = cameraPos + (direction * cameraDistance); // set new position(apply new rotation and move back to right distance)

        transform.LookAt(cameraFocalPoint.transform.position); // Look at focus
    }
}