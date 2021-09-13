using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Portal))]

public class SwitchingPortal : MonoBehaviour
{
    private GameObject gameMaster;
    public List<GameObject> otherPortals = new List<GameObject>();

    [SerializeField]
    private int currentOtherPortalIndex = 0;
    private Portal portal;

    [SerializeField]
    private GameObject portalModel;

    private GameObject cameraObject;
    
    [SerializeField]
    private GameObject portalCollection;
    [SerializeField]
    private GameObject portalCameras;

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.Find("GameMaster");
        portalCollection = GameObject.Find("Portal Collection");
        portalCameras = GameObject.Find("Portal Cameras");

        portal = GetComponent<Portal>();
    }

    private void UpdatePortalInformation()
    {
        otherPortals = gameMaster.GetComponent<PortalSetUp>().multiPortals;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)){
            if(portal.otherPortal != null)
            {
                currentOtherPortalIndex++;
                if (currentOtherPortalIndex == otherPortals.Count) // if it goes past the last index we start from 0.
                {
                    currentOtherPortalIndex = 0;
                }
            }
            UpdatePortalInformation();
            AssignOtherPortal();
        }
    }

    private void AssignOtherPortal()
    {
        AssignCamera();
        AssignRenderTexture();
    }

    private void AssignRenderTexture()
    {
    //    cameraObject = Instantiate(cameraObject, new Vector3(transform.position.x + 10, transform.position.y, transform.position.z),
    //Quaternion.identity);
    //    gameMaster.GetComponent<PortalSetUp>().MakeNewRenderTexture(cameraObject);



    //    cameraObject.transform.parent = portalCameras.transform;
        transform.parent = portalCollection.transform;
               
        portal.otherPortal = otherPortals[currentOtherPortalIndex];
        portal.portalCamera = gameMaster.GetComponent<PortalSetUp>().cameras[currentOtherPortalIndex];
        portalModel.GetComponent<Renderer>().material = gameMaster.GetComponent<PortalSetUp>().materials[currentOtherPortalIndex];
        portal.wallCollider = otherPortals[currentOtherPortalIndex].GetComponent<Collider>(); ;

        if (portal.otherPortal)
        {
            Debug.Log(portal.otherPortal.transform.GetChild(0).GetComponent<MeshRenderer>().materials);
        }
        else
        {

        }
    }

    private void AssignCamera()
    {

    }
}
