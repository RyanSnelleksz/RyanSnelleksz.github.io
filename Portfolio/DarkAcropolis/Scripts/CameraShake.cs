using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public Transform cameraTransform;
    private Vector3 orignalCameraPos;

    public float shakeDuration;
    public float shakeAmount = 0.1f;
    float shakeTimer;

    public PlayerController player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        cameraTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        orignalCameraPos = cameraTransform.localPosition;

        if (shakeDuration > 0 && player.currentHealth > 0.0f)
        {
            cameraTransform.localPosition = orignalCameraPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0.0f;
            cameraTransform.localPosition = orignalCameraPos;
        }
    }

    public void ShakeCamera(float duration)
    {
        shakeDuration = duration;
    }

    public void PauseCamera()
    {
        shakeTimer = shakeDuration;
        shakeDuration = 0.0f;
    }

    public void UnPauseCamera()
    {
        shakeDuration = shakeTimer;
    }
}
