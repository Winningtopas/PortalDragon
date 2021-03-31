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

        portals = gameMaster.GetComponent<PortalSetUp>().portals;
        cameras = gameMaster.GetComponent<PortalSetUp>().cameras;
        renderTextures = gameMaster.GetComponent<PortalSetUp>().renderTextures;
    }

    public void UpdateLists(List<GameObject> port, List<GameObject> cams, List<RenderTexture> rt)
    {
        portals = port;
        cameras = cams;
        renderTextures = rt;
    }

    private void OnPreRender()
    {
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
                            RenderCamera(portals[i].GetComponent<Portal>(), portals[i + 1].GetComponent<Portal>(), j, i);
                        else
                            RenderCamera(portals[i].GetComponent<Portal>(), portals[i - 1].GetComponent<Portal>(), j, i);
                    }
                }
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID, int currentCamera)
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

            //adjusting the near clipping plane
            cameras[currentCamera].GetComponent<Camera>().nearClipPlane = Vector3.Distance(cameras[currentCamera].transform.position, outTransform.position);
        }
        portalCamera.Render();
    }
}