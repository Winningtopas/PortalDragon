using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour {

	public GameObject playerCamera;
	public Transform portal;
	public Transform otherPortal;

    [SerializeField]
    private Vector3 relativePos;

    // Update is called once per frame
    void Update () {

        relativePos = portal.InverseTransformPoint(playerCamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        transform.position = otherPortal.transform.TransformPoint(relativePos);

        Quaternion relativeRot = Quaternion.Inverse(portal.rotation) * playerCamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        transform.rotation = otherPortal.transform.rotation * relativeRot;

        //adjusting the near clipping plane
        GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, otherPortal.position);
    }

    public void AssignPortals(GameObject portalGameObject, GameObject otherPortalGameObject)
    {
        playerCamera = GameObject.Find("Main Camera");
        portal = portalGameObject.transform;
        otherPortal = otherPortalGameObject.transform;
    }

}
