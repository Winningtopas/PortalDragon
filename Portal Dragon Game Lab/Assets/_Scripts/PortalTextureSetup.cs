using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour {

    public List<GameObject> portals = new List<GameObject>();
    public List<GameObject> cameras = new List<GameObject>();
    public List<Material> materials = new List<Material>();
	
    private int currentPortal = 0;

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

        //create a new material
        Material material = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        material.name = "NewPortalMat" + materials.Count;

        material.mainTexture = camera.targetTexture;
        materials.Add(material);
    }

    public void AssignMaterialToPortal(GameObject portal, int i)
    {
        portal.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = materials[currentPortal];
        portal.name = "Portal " + portals.Count;
        portals.Add(portal);
        currentPortal++;
    }

    public void AssignPortalsToCamera()
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            if (i % 2 == 0) // if i is even
            {
                cameras[i].GetComponent<PortalCamera>().AssignPortals(portals[i], portals[i + 1]);
                portals[i].GetComponent<Portal>().otherPortal = portals[i + 1];
                portals[i].GetComponent<Portal>().portalCamera = cameras[i + 1];
            }
            else
            {
                cameras[i].GetComponent<PortalCamera>().AssignPortals(portals[i], portals[i - 1]);
                portals[i].GetComponent<Portal>().otherPortal = portals[i - 1];
                portals[i].GetComponent<Portal>().portalCamera = cameras[i - 1];
            }
        }
    }
}
