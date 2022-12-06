using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeryBasicQCC : MonoBehaviour
{
    public float speed = 1.0f;
    public float rotationSpeed = 100.0f;

    float gravity = Physics.gravity.y;
    bool grounded = false;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical movement input.
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        if (translation != 0)
        {
            animator.SetFloat("Speed", 1.0f);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }

        if (!grounded)
        {
            transform.Translate(0, gravity * Time.deltaTime, 0);
        }

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.isStatic)
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.isStatic)
        {
            grounded = false;
        }
    }
}
