using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeedX = 10f;
    [SerializeField]
    private float rotationSpeedY = 10f;
    [SerializeField]
    private float rotationSpeedZ = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate((rotationSpeedX * Time.deltaTime), (rotationSpeedY * Time.deltaTime), (rotationSpeedZ * Time.deltaTime));
    }
}
