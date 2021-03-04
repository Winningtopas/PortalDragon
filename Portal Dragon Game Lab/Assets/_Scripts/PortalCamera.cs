using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {

	public GameObject playerCamera;
	public Transform portal;
	public Transform otherPortal;

    [SerializeField]
    private Quaternion startRotation;
    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private Vector3 relativePos;

    private void Start()
    {
        startRotation = transform.rotation;
        startPosition = portal.transform.position;

    }

    // Update is called once per frame
    void Update () {

        relativePos = otherPortal.InverseTransformPoint(playerCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        transform.position = portal.transform.TransformPoint(relativePos);


        Quaternion relativeRot = Quaternion.Inverse(otherPortal.rotation) * playerCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        transform.rotation = portal.transform.rotation * relativeRot;

        //adjusting the near clipping plane
        GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, portal.position);
    }
}
