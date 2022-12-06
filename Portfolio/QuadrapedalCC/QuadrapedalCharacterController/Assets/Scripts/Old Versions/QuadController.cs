using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadController : MonoBehaviour
{

    public float zAcceleration = 1.0f;
    public float rotationSpeed = 100.0f;
    public float slowSpeed = 5.0f;

    public float maxSpeed = 10.0f;
    public float maxRotationSpeed = 100.0f;

    float zVelocity; // These are not to be confused with Unitys physics systems velocity and acceleration, they are purely for this character
    float yVelocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical movement input.
        float translation = Input.GetAxis("Vertical") * zAcceleration;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        zVelocity += zAcceleration;

        if (zVelocity > maxSpeed)
        {
            zVelocity = maxSpeed;
        }
        else if (zVelocity < 0)
        {
            zVelocity = 0;
        }
        transform.Translate(0, 0, zVelocity);
        transform.Rotate(0, rotation, 0);
    }
}
