using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform player;
    public GameObject otherPortal;
    private Transform teleportLocation;

    public GameObject portalCamera; 

    public float dot;

    private bool playerIsOverlapping = false;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();

    [SerializeField]
    private Collider wallCollider;

    [SerializeField]
    Vector3 testPosition;

    private void Start()
    {
        player = GameObject.Find("Main Camera").transform;
        wallCollider = otherPortal.GetComponent<Collider>();
    }

    // Update is called once per frame

    private void Update()
    {
        for (int i = 0; i < portalObjects.Count; ++i)
        {
            PortalableObject traveller = portalObjects[i];

            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);
            testPosition = objPos;

            //if (objPos.z > 0.0f)
            //{
            Debug.Log("CALCULATE THE DOT PRODUCT HERE");
                portalObjects[i].Warp();
            //}
        }
    }


    //void Update()
    //{
    //    teleportLocation = otherPortal.transform;


    //    //    //testPositonPlayer = portalToPlayer;
    //    //    //float dotProduct = Vector3.Dot(transform.up, portalToPlayer);


    //    //    Vector3 forward = transform.TransformDirection(transform.forward);
    //    //    Vector3 toOther = player.position - transform.position;
    //    //    float dotProduct = Vector3.Dot(forward, toOther);
    //    //    // If this is true: The player has moved across the portal
    //    //    if (dotProduct < 0f)
    //    //    {
    //    //        // Teleport him!
    //    //        Debug.Log("Teleport!");
    //    //        float rotationDiff = -Quaternion.Angle(transform.rotation, teleportLocation.rotation);
    //    //        rotationDiff += 180;
    //    //        player.Rotate(Vector3.up, rotationDiff);

    //    //        Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
    //    //        player.position = teleportLocation.position + positionOffset;

    //    //        playerIsOverlapping = false;
    //    //    }
    //    //}

    //    Vector3 forward = transform.TransformDirection(Vector3.forward);
    //    Vector3 toOther = player.position - transform.position;
    //    dot = Vector3.Dot(forward, toOther);

    //    if (playerIsOverlapping)
    //    {
    //        Vector3 portalToPlayer = transform.position - player.position;
    //        if (dot > 0) // the player is in front of the portal
    //        {
    //            // Teleport him!
    //            Debug.Log("Teleport!");
    //            float rotationDiff = -Quaternion.Angle(transform.rotation, teleportLocation.rotation);
    //            rotationDiff += 0;
    //            //player.Rotate(Vector3.up, rotationDiff);
    //            var x = UnityEditor.TransformUtils.GetInspectorRotation(portalCamera.transform).x;
    //            var y = UnityEditor.TransformUtils.GetInspectorRotation(portalCamera.transform).y;
    //            var z = UnityEditor.TransformUtils.GetInspectorRotation(portalCamera.transform).z;

    //            Quaternion rotation = Quaternion.Euler(x, y, z);
    //            player.transform.rotation = rotation;

    //            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
    //            player.position = teleportLocation.position + positionOffset;

    //            playerIsOverlapping = false;
    //        }
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        playerIsOverlapping = true;
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        playerIsOverlapping = false;
    //    }
    //}


    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if (obj != null)
        {
            portalObjects.Add(obj);
            obj.SetIsInPortal(this, otherPortal.GetComponent<Portal>(), wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if (portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal(wallCollider);
        }
    }

    public void PostPortalRender()
    {
        //Debug.Log("test pre render");

        //foreach (var traveller in portalObjects)
        //{
        //    UpdateSliceParams(traveller);
        //}
        //ProtectScreenFromClipping(playerCam.transform.position);

    }

    void UpdateSliceParams(PortalableObject traveller)
    {
        // Calculate slice normal
        int side = SideOfPortal(traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = otherPortal.transform.forward * side;

        // Calculate slice centre
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = otherPortal.transform.position;

        // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = transform.localScale.z;

        bool playerSameSideAsTraveller = SameSideOfPortal(player.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller)
        {
            sliceOffsetDst = -screenThickness;
        }
        bool playerSameSideAsCloneAppearing = side != otherPortal.GetComponent<Portal>().SideOfPortal(player.transform.position);
        if (!playerSameSideAsCloneAppearing)
        {
            cloneSliceOffsetDst = -screenThickness;
        }

        // Apply parameters
        for (int i = 0; i < traveller.originalMaterials.Length; i++)
        {
            Debug.Log("SLICE!");
            traveller.originalMaterials[i].SetVector("sliceCentre", slicePos);
            traveller.originalMaterials[i].SetVector("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat("sliceOffsetDst", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector("sliceCentre", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat("sliceOffsetDst", cloneSliceOffsetDst);

        }

    }

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision with: " + collision.gameObject.name);
    //    collision.transform.position = otherPortal.transform.position;
    //}
}
