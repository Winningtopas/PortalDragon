using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public MeshRenderer screen;
    [SerializeField]
    Camera playerCam;
    [SerializeField]
    Camera portalCam;
    RenderTexture viewTexture;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    private void Start()
    {
        Render();
    }

    void CreateViewTexture()
    {
        if(viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
                viewTexture.Release(); // replace the old rendertexture with a new one
        }
        viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

        portalCam.targetTexture = viewTexture;
        linkedPortal.screen.material.SetTexture("_MainTex", viewTexture);
    }

        //called before the player camera is rendered
    public void Render()
    {
        Debug.Log("RENDER");
        screen.enabled = false;
        CreateViewTexture();

        var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * playerCam.transform.worldToLocalMatrix;
        portalCam.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

        //render to the view texture
        portalCam.Render();

        screen.enabled = true;
    }
}
