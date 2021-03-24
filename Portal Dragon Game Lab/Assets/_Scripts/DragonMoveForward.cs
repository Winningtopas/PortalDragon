using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMoveForward : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, (transform.forward) * 10, Color.blue);
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        rb.velocity = forward * speed * Time.deltaTime;
    }
}
