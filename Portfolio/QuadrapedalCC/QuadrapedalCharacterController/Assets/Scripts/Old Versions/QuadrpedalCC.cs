using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrpedalCC : MonoBehaviour
{
    public GameObject bodyFront;
    public GameObject bodyBack;
    public GameObject backJoint;

    Rigidbody bodyFrontRigidBody;
    Rigidbody bodyBackRigidBody;


    public GameObject leftFrontFoot;
    public GameObject rightFrontFoot;

    public GameObject leftBackFoot;
    public GameObject rightBackFoot;

    GameObject[] feet = new GameObject[4];
    public float distanceToGround;

    public float speed = 10.0f;
    public float slowSpeed = 1.0f;
    public float rotationSpeed = 100.0f;

    Quaternion backStartRotation;
    Vector3 backOffset;
    float setDistance;

    public float jumpSpeed = 10.0f;
    public Vector3 jumpVector = new Vector3(0, 2, 4);

    // Start is called before the first frame update
    void Start()
    {
        feet[0] = leftFrontFoot;
        feet[1] = rightFrontFoot;
        feet[2] = leftBackFoot;
        feet[3] = rightBackFoot;

        backStartRotation = bodyBack.transform.rotation;
        backOffset = backJoint.transform.position - bodyBack.transform.position;
        setDistance = Vector3.Distance(backJoint.transform.position, bodyBack.transform.position);

        distanceToGround = feet[0].GetComponent<SphereCollider>().radius;
        bodyFrontRigidBody = bodyFront.GetComponent<Rigidbody>();
        bodyBackRigidBody = bodyBack.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical movement input.
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        if (IsGrounded()) // Since our body is not touching the gorund we need a way to slow down
        {
            bodyFrontRigidBody.drag = 2;
        }
        else
        {
            bodyFrontRigidBody.drag = 0;
        }
        if (Input.GetAxis("Jump") > 0 && IsGrounded()) // Jump input
        {
            Vector3 direction = Vector3.Normalize(bodyFront.transform.position - backJoint.transform.position);
            bodyFront.GetComponent<Rigidbody>().velocity = bodyFront.transform.TransformDirection(jumpVector * jumpSpeed);  // Using the bodies transform and jump vector to jump forwards
        }

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        if (IsGrounded())
        {
            bodyFront.transform.Translate(0, 0, translation);
            bodyFront.transform.Rotate(0, rotation, 0);
        }

        bodyBack.transform.rotation = new Quaternion(0, bodyBack.transform.rotation.y, 0, bodyBack.transform.rotation.w);
        bodyFront.transform.rotation = new Quaternion(0, bodyFront.transform.rotation.y, 0, bodyFront.transform.rotation.w);

        UpdateBack();
    }

    bool IsGrounded()
    {
        int feetGrounded = 0;

        foreach (GameObject foot in feet)
        {
            if (Physics.Raycast(foot.transform.position, -Vector3.up, 0.3f))
            {
                feetGrounded++;
            }
        }
        if (feetGrounded >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateBack()
    {
        bodyBack.transform.rotation = backStartRotation;

        Vector3 targetDirection = backJoint.transform.position - bodyBack.transform.position;

        Quaternion newRotation = Quaternion.LookRotation(targetDirection);

        bodyBack.transform.rotation = newRotation;

        float currentDistance = Vector3.Distance(backJoint.transform.position, bodyBack.transform.position);

        if (currentDistance != setDistance)
        {
            bodyBack.transform.position += Vector3.Normalize(targetDirection) * (currentDistance - setDistance);
        }
    }

    //public GameObject bodyFront;
    //public GameObject bodyBack;
    //public GameObject backJoint;

    //public float speed = 10.0f;
    //public float rotationSpeed = 100.0f;
    //public float jumpSpeed = 1.0f;

    //float zOffset;

    //Quaternion backStartRotation;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    //backOffset = bodyFront.transform.position - bodyBack.transform.position;
    //    zOffset = Mathf.Abs(bodyFront.transform.position.z - bodyBack.transform.position.z);

    //    backStartRotation = bodyBack.transform.rotation;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // Get the horizontal and vertical axis.
    //    float translation = Input.GetAxis("Vertical") * speed;
    //    float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

    //    if (Input.GetAxis("Jump") > 0) 
    //    { 
    //        bodyFront.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed); 
    //    }

    //    translation *= Time.deltaTime;
    //    rotation *= Time.deltaTime;

    //    bodyFront.transform.Translate(0, 0, translation);

    //    bodyFront.transform.Rotate(0, rotation, 0);

    //    bodyBack.transform.rotation = new Quaternion(0, bodyBack.transform.rotation.y, 0, bodyBack.transform.rotation.w);
    //    bodyFront.transform.rotation = new Quaternion(0, bodyFront.transform.rotation.y, 0, bodyFront.transform.rotation.w);

    //    UpdateBack();
    //}

    //void UpdateBack()
    //{
    //    Vector3 jointOffset = backJoint.transform.position - bodyBack.transform.position;
    //    bodyBack.transform.rotation = backStartRotation;

    //    Vector3 backTranslation;
    //    if (new Vector3(jointOffset.x, jointOffset.y, 0).magnitude < 0.1f)
    //    {
    //        backTranslation = Vector3.zero;
    //    }
    //    else
    //    {
    //        Vector3 direction = Vector3.Normalize(jointOffset);
    //        backTranslation = direction * speed; // + Vector3.Distance(jointOffset, bodyBack.transform.position) / 20);
    //    }

    //    bodyBack.transform.Translate(backTranslation * Time.deltaTime);
    //    bodyBack.transform.position = new Vector3(bodyBack.transform.position.x, bodyBack.transform.position.y, backJoint.transform.position.z);

    //    Vector3 targetDirection = bodyFront.transform.position - bodyBack.transform.position;

    //    Quaternion newRotation = Quaternion.LookRotation(targetDirection);

    //    bodyBack.transform.rotation = newRotation;
    //}
}
