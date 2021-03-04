using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float walkingSpeed = 15f;
    private float sprintingSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        sprintingSpeed = walkingSpeed * 1.5f;

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        direction.Normalize();

        transform.Translate(direction * Time.deltaTime * currentSpeed);

        //rotation
        float x = 5 * Input.GetAxis("Mouse X");
        float y = 5 * -Input.GetAxis("Mouse Y");
        transform.Rotate(y, x, 0);

        //sprinting
        if (Input.GetButton("Fire1"))
        {
            currentSpeed = sprintingSpeed;
        }
        else
        {
            currentSpeed = walkingSpeed;
        }
    }

}
