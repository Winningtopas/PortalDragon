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
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);

            if (objPos.z > 0.0f)
            {
                portalObjects[i].Warp();
            }
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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision with: " + collision.gameObject.name);
    //    collision.transform.position = otherPortal.transform.position;
    //}
}
