using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CameraContorller : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] Vector2 camRotation;
    [SerializeField] float mouseSensitivity = 100;
    float xRotation = 0f;

    [Header("UI/Menu")]
    PauseManager pause;
    public GameObject destroyText;
    public GameObject letter;
    public GameObject pickUpText;

    GameObject currentSealEfect = null;
    int layermask = 1 << 12;

    bool particleBuildUpEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        pause = FindObjectOfType<PauseManager>();

        layermask = ~layermask;
    }

    // Update is called once per frame
    void Update()
    {
        //If application is not pause
        //Initialize the mouse controls and hide the mouse cursor
        //Destroy heart and seals by raycast from player
        if (!pause.pausePressed)
        {
            //Allows the camera to roatate on the x axis forever
            //But clamp the y axis to stop rotating look stright up and down
            camRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
            camRotation.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
            xRotation -= camRotation.y;
            xRotation = Mathf.Clamp(xRotation, -89f, 89f);
            transform.localRotation = Quaternion.Euler(xRotation, camRotation.x, 0);
            Cursor.lockState = CursorLockMode.Locked;

            bool displayText = false;
            bool startLetter = false;
            //change order of if statements. raycast first to allow text to appear before destroy the seal.
            //Raycasy from the camera positon forward and check if it collided with the seal or heart
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
            if (Physics.Raycast(ray, out hit, 1.5f, layermask))
            {
                //If seal or heart has collided get componet of the object else set null
                Seal sealCollide = hit.collider.gameObject.GetComponent<Seal>();
                Heart heartCollide = hit.collider.gameObject.GetComponent<Heart>();

                //If seal is collided
                //While facing the seal and holding E key start decreasing seal timer before it's destroyed
                //while destroying the seal play particle animation
                if (sealCollide != null)
                {
                    displayText = true;
                    //If holding the E key facing the seal
                    if (Input.GetKey(KeyCode.E))
                    {
                        if(!sealCollide.sealStereoSource.isPlaying)
                        {
                            sealCollide.SealBuildUpSound();
                        }
                        //Once value hits zero seal is destroyed
                        sealCollide.sealDeletion -= Time.deltaTime;
                        sealCollide.RaiseIntensity(Time.deltaTime);
                        //seal buildup
                        if (currentSealEfect == null)
                        {
                            currentSealEfect = Instantiate(sealCollide.buildUpPrefab, sealCollide.transform.position, sealCollide.transform.rotation);
                            particleBuildUpEnd = true;
                        }
                    }
                    else
                    {
                        sealCollide.sealStereoSource.Pause();
                        sealCollide.audioReset = false;
                    }
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.blue);
                }
                else if (currentSealEfect != null && particleBuildUpEnd)
                {
                    Destroy(currentSealEfect);
                    particleBuildUpEnd = false;
                }
                //If heart id collided
                //While colliding with the heart and hold the E key start decreasing heart value before it's destroyed
                else if (heartCollide != null)
                {
                    displayText = true;
                    //destroyText.SetActive(true);
                    //If hold E and all seals in the scene are destroyed
                    if(Input.GetKey(KeyCode.E) && heartCollide.HeartLocked())
                    {
                        //Once value hits zero heart is destroyed
                        heartCollide.heartDeletion -= Time.deltaTime;
                    }
                }
                else if (hit.collider.gameObject.name == "Starting Letter")
                {
                    startLetter = true;
                    if (Input.GetKey(KeyCode.E))
                    {
                        letter.SetActive(true);
                        pause.Paused();
                    }
                }
            }
            else if (currentSealEfect != null && particleBuildUpEnd)
            {
                Destroy(currentSealEfect);
                particleBuildUpEnd = false;
            }
            destroyText.SetActive(displayText);
            pickUpText.SetActive(startLetter);
        }
        //When pause unlock the mouse cursor
        else if (pause.pausePressed)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}