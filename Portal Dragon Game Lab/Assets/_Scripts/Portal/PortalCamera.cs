using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalCamera : MonoBehaviour {

	public GameObject playerCamera;
	public Transform portal;
	public Transform otherPortal;

    [SerializeField]
    private Vector3 relativePos;

    [SerializeField]
    private int iterations = 7;


    // Update is called once per frame
    void Update () {

    }

    //void OnEnable()
    //{
    //    RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    //}
    //void OnDisable()
    //{
    //    RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    //}
    //private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    //{
    //    OnPreRender();
    //}

    private void OnPreRender()
    {
        //for (int i = iterations - 1; i >= 0; --i)
        //{
            RenderCamera(0);
        //}
    }

    void RenderCamera(int iterationID)
    {
        for (int i = 0; i <= 0; ++i)
        {
            relativePos = portal.InverseTransformPoint(playerCamera.transform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            transform.position = otherPortal.transform.TransformPoint(relativePos);

            Quaternion relativeRot = Quaternion.Inverse(portal.rotation) * playerCamera.transform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            transform.rotation = otherPortal.transform.rotation * relativeRot;

            //adjusting the near clipping plane
            GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, otherPortal.position);
        }
    }

    public void AssignPortals(GameObject portalGameObject, GameObject otherPortalGameObject)
    {
        playerCamera = GameObject.Find("Main Camera");
        portal = portalGameObject.transform;
        otherPortal = otherPortalGameObject.transform;
    }

}
