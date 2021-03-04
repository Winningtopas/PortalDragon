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
        //GetComponent<Camera>().projectionMatrix = playerCamera.GetComponent<Camera>().projectionMatrix;

        relativePos = otherPortal.InverseTransformPoint(playerCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        transform.position = portal.transform.TransformPoint(relativePos);


        Quaternion relativeRot = Quaternion.Inverse(otherPortal.rotation) * playerCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        transform.rotation = portal.transform.rotation * relativeRot;

        //transform.rotation = startRotation * playerCamera.transform.rotation;
        //Vector3 tempPos = startPosition + (otherPortal.transform.position - playerCamera.transform.position);
        //transform.position = new Vector3(tempPos.x, startPosition.y, tempPos.z);

        //Vector3 playerOffsetFromPortal = playerCamera.transform.position - otherPortal.position;
        //transform.position = portal.position + playerOffsetFromPortal;

        //float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);

        //Quaternion portalRotationalDifference = Quaternion.AngleAxis(-angularDifferenceBetweenPortalRotations, Vector3.up);
        //Vector3 newCameraDirection = portalRotationalDifference * playerCamera.forward;
        //transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
}
