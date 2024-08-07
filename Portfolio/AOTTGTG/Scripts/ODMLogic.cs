using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Wire
{
    public GameObject wireRenderer;
    public SpringJoint wireSpring;
    public Vector3 hookAnchorPoint;
    public bool extending;
}

public class ODMLogic : MonoBehaviour
{
    public LayerMask mask;

    GameObject cameraObject;
    GameObject cameraFocalPoint;

    public GameObject wirePrefab;
    public GameObject sphere;

    Wire[] wires = new Wire[2];

    KeyCode leftHook = KeyCode.LeftArrow;
    KeyCode rightHook = KeyCode.RightArrow;

    public float wirePullStrength = 1.0f;
    public float wireLengthChangeSpeed = 0.1f;

    float minimumWireMax = 0.1f;
    float maximumExtendDifference = 5.0f; // How much the wire max can be from the current max when extending
    float maximumRetractDifference = 5.0f; // How much the wire max can be from the current max when retracting

    Vector3 wireOrigin; // temporary until aninmations is in.
    Vector3 wireOriginOffset; // temporary until aninmations is in.


    // Start is called before the first frame update
    void Start()
    {
        cameraObject = transform.GetChild(0).gameObject;
        cameraFocalPoint = transform.GetChild(1).gameObject;

        wireOriginOffset = new Vector3(0.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        wireOrigin = transform.position + wireOriginOffset;
        // Drawing Wires ~~~~~~~~~~~~~~~~
        if (Input.GetKeyDown(leftHook))
        {
            CreateWire(0);
        }
        else if (Input.GetKey(leftHook))
        {
            if (wires[0].wireSpring != null)
            {
                UpdateWire(0);
                WireScrollInput(0);
            }
        }
        else if (!Input.GetKey(leftHook))
        {
            DestroyWire(0);
        }

        if (Input.GetKeyDown(rightHook))
        {
            CreateWire(1);
        }
        else if (Input.GetKey(rightHook))
        {
            if (wires[1].wireSpring != null)
            {
                UpdateWire(1);
                WireScrollInput(1);
            }
        }
        else if (!Input.GetKey(rightHook))
        {
            DestroyWire(1);
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }


    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Function to create wires
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void CreateWire(int wireIndex)
    {
        // Draw Wire
        //
        Vector3 direction = cameraObject.GetComponent<Camera>().transform.forward;

        direction.Normalize();

        RaycastHit hit;
        Physics.Raycast(cameraObject.transform.position, direction, out hit, 1000.0f, mask);
        if (hit.collider == null )
        {
            return;
        }

        wires[wireIndex].wireRenderer = Instantiate(wirePrefab);

        LineRenderer line = wires[wireIndex].wireRenderer.GetComponent<LineRenderer>();

        line.SetPosition(0, wireOrigin);
        line.SetPosition(1, hit.point);

        GetComponent<Rigidbody>().AddForce(direction * wirePullStrength, ForceMode.Impulse);

        //
        // Draw Wire

        // Create Spring
        //

        // Create a spring
        wires[wireIndex].wireSpring = gameObject.AddComponent<SpringJoint>();

        wires[wireIndex].extending = false;

        wires[wireIndex].wireSpring.autoConfigureConnectedAnchor = false; // give ourself control
        wires[wireIndex].wireSpring.connectedAnchor = hit.point; // set anchor to the springs end
        wires[wireIndex].hookAnchorPoint = hit.point;


        // Spring max and min length
        wires[wireIndex].wireSpring.maxDistance = Vector3.Distance(wireOrigin, hit.point); // * 0.7f; // having the max shorter than when it hits causes a pull but it's too strong far away
        wires[wireIndex].wireSpring.minDistance = 0.0f;

        // Setting sping values
        wires[wireIndex].wireSpring.spring = 6.5f; // how hard spring pulls, works based on distance
        wires[wireIndex].wireSpring.damper = 9.0f; // works against the spring strength, helps to stop oscilating and helps to prevent over shoot
        wires[wireIndex].wireSpring.massScale = 1.5f; // alters mass of swinging object (need to look into this more)

    }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Function to destroy wires
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void DestroyWire(int wireIndex)
    {
        Destroy(wires[wireIndex].wireSpring);
        Destroy(wires[wireIndex].wireRenderer);
    }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Function to update wires
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void UpdateWire(int wireIndex)
    {
        if (wires[wireIndex].wireRenderer != null)
        {
            LineRenderer line = wires[wireIndex].wireRenderer.GetComponent<LineRenderer>();

            line.SetPosition(0, wireOrigin);

            if (Vector3.Distance(wireOrigin, wires[wireIndex].hookAnchorPoint) > wires[wireIndex].wireSpring.maxDistance)
            {
               wires[wireIndex].extending = false;
            }

            if (Vector3.Distance(wireOrigin, wires[wireIndex].hookAnchorPoint) >= wires[wireIndex].wireSpring.maxDistance || wires[wireIndex].extending == true)
            {
                return;
            }
            wires[wireIndex].wireSpring.maxDistance = Vector3.Distance(wireOrigin, wires[wireIndex].hookAnchorPoint);
        }
    }
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    void WireScrollInput(int wireIndex)
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if (wires[wireIndex].wireSpring.maxDistance - Vector3.Distance(wireOrigin, wires[wireIndex].hookAnchorPoint) < maximumExtendDifference) // I dont want the maximum to go to far above from the current distance AT ONCE
            {
                wires[wireIndex].wireSpring.maxDistance += wireLengthChangeSpeed;
                wires[wireIndex].extending = true;
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (wires[wireIndex].wireSpring.maxDistance - Vector3.Distance(wireOrigin, wires[wireIndex].hookAnchorPoint) < maximumRetractDifference) // I dont want the new maximum to go to far above from the current distance AT ONCE
            {

            }
            wires[wireIndex].wireSpring.maxDistance -= wireLengthChangeSpeed;

            if (wires[wireIndex].wireSpring.maxDistance < minimumWireMax)
            {
                wires[wireIndex].wireSpring.maxDistance = minimumWireMax;
            }
        }
    }
}
