using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSetUp : MonoBehaviour {

    public List<GameObject> portals = new List<GameObject>();
    public List<GameObject> cameras = new List<GameObject>();
    public List<Material> materials = new List<Material>();
    public List<RenderTexture> renderTextures = new List<RenderTexture>();

    // for portals that do not come in pairs
    public List<GameObject> multiPortals = new List<GameObject>();
    public List<GameObject> multiCameras = new List<GameObject>();
    public List<Material> multiMaterials = new List<Material>();
    public List<RenderTexture> multiRenderTextures = new List<RenderTexture>();

    private GameObject mainCamera;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
    }

    public void MakeNewRenderTexture(GameObject cameraGameObject)
    {
        cameraGameObject.name = "Camera " + cameras.Count;
        cameras.Add(cameraGameObject);

        Camera camera = cameraGameObject.GetComponent<Camera>();

        if (camera.targetTexture != null)
        {
            camera.targetTexture.Release();
        }
        camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        renderTextures.Add(camera.targetTexture);

        //create a new material
        Material material = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        material.name = "NewPortalMat" + materials.Count;

        material.mainTexture = camera.targetTexture;
        materials.Add(material);
    }

    public void AssignMaterialToPortal(GameObject portal, int i)
    {
        //Child(0) is the mesh
        portal.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = materials[portals.Count];
        portal.name = "Portal " + portals.Count;
        portals.Add(portal);
    }

    public void AssignPortalsToCamera()
    {
        for(int i = 0; i < portals.Count; i++)
        {
            if (i % 2 == 0) // if i is even
            {
                portals[i].GetComponent<Portal>().otherPortal = portals[i + 1];
                portals[i].GetComponent<Portal>().portalCamera = cameras[i + 1];
            }
            else
            {
                portals[i].GetComponent<Portal>().otherPortal = portals[i - 1];
                portals[i].GetComponent<Portal>().portalCamera = cameras[i - 1];
            }
        }
        mainCamera.GetComponent<PortalCamera>().UpdateLists(portals, cameras, renderTextures);
    }

    public void MakeNewMultiRenderTexture(GameObject cameraGameObject)
    {
        cameraGameObject.name = "Camera " + multiCameras.Count;
        multiCameras.Add(cameraGameObject);

        Camera camera = cameraGameObject.GetComponent<Camera>();

        if (camera.targetTexture != null)
        {
            camera.targetTexture.Release();
        }
        camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        multiRenderTextures.Add(camera.targetTexture);

        //create a new material
        Material material = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        material.name = "NewPortalMat" + multiMaterials.Count;

        material.mainTexture = camera.targetTexture;
        multiMaterials.Add(material);
    }

    public void AssignMultiMaterialToPortal(GameObject portal, int i)
    {
        //Child(0) is the mesh
        portal.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = multiMaterials[multiPortals.Count];
        portal.name = "Portal " + multiPortals.Count;
        multiPortals.Add(portal);
    }

    public void AssignMultiPortalsToCamera()
    {
        //for (int i = 0; i < multiPortals.Count; i++)
        //{
        //    if (i % 2 == 0) // if i is even
        //    {
        //        multiPortals[i].GetComponent<Portal>().otherPortal = multiPortals[i + 1];
        //        multiPortals[i].GetComponent<Portal>().portalCamera = cameras[i + 1];
        //    }
        //    else
        //    {
        //        multiPortals[i].GetComponent<Portal>().otherPortal = multiPortals[i - 1];
        //        multiPortals[i].GetComponent<Portal>().portalCamera = cameras[i - 1];
        //    }
        //}


        //mainCamera.GetComponent<PortalCamera>().UpdateLists(multiPortals, cameras, multiRenderTextures);
    }
}
