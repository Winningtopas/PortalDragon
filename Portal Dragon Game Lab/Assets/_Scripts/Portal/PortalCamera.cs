using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour {

    public List<GameObject> portals = new List<GameObject>();
    public List<GameObject> cameras = new List<GameObject>();
    public List<RenderTexture> renderTextures = new List<RenderTexture>();

    private Camera portalCamera;
    private Camera mainCamera;
    private GameObject gameMaster;

    private const int iterations = 7;


    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        gameMaster = GameObject.Find("GameMaster");
    }

    // Update is called once per frame
    void Update () {
        portals = gameMaster.GetComponent<PortalSetUp>().portals;
    }

    private void OnPreRender()
    {
        cameras = gameMaster.GetComponent<PortalSetUp>().cameras;
        renderTextures = gameMaster.GetComponent<PortalSetUp>().renderTextures;

        int cameraAmount = cameras.Count;

        if (cameraAmount > 0)
        {

            for (int i = 0; i < cameraAmount; i++)
            {
                if (portals[i].GetComponent<Portal>().IsRendererVisible())
                {
                    portalCamera = cameras[i].GetComponent<Camera>();
                    portalCamera.targetTexture = renderTextures[i];

                    for (int j = iterations - 1; j >= 0; --j) // render the recursion
                    {
                        if (i % 2 == 0) // if i is even
                            RenderCamera(portals[i].GetComponent<Portal>(), portals[i + 1].GetComponent<Portal>(), j);
                        else
                            RenderCamera(portals[i].GetComponent<Portal>(), portals[i - 1].GetComponent<Portal>(), j);
                    }
                }
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;

            GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, outTransform.position);
        }

        // Set the camera's oblique view frustum.
        //Plane p = new Plane(-outTransform.forward, outTransform.position);
        //Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        //Vector4 clipPlaneCameraSpace =
        //    Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        //var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        //portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        //if(portalCamera != null)
        portalCamera.Render();
    }
}


//public void PrePortalRenderer()
//    {
//        Debug.Log("portal camera ");
//        //GetComponent<Camera>().targetTexture = renderTexture;
//        for (int i = iterations - 1; i >= 0; --i)
//        {
//            RenderCamera(i);
//        }
//    }

//    void RenderCamera(int iterationID)
//    {
//        for (int i = 0; i <= iterationID; ++i)
//        {
//            relativePos = portal.InverseTransformPoint(playerCamera.transform.position);
//            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
//            transform.position = otherPortal.transform.TransformPoint(relativePos);

//            Quaternion relativeRot = Quaternion.Inverse(portal.rotation) * playerCamera.transform.rotation;
//            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
//            transform.rotation = otherPortal.transform.rotation * relativeRot;

//            //adjusting the near clipping plane
//            GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, otherPortal.position);
//        }
//    }

//    public void AssignPortals(GameObject portalGameObject, GameObject otherPortalGameObject, RenderTexture rt)
//    {
//        playerCamera = GameObject.Find("Main Camera");
//        portal = portalGameObject.transform;
//        otherPortal = otherPortalGameObject.transform;

//        //renderTexture = rt;
//    }

//}
