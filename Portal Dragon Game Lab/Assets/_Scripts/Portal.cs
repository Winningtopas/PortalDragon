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

    private void Start()
    {
        player = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame

    void Update()
    {
        teleportLocation = otherPortal.transform;


        //    //testPositonPlayer = portalToPlayer;
        //    //float dotProduct = Vector3.Dot(transform.up, portalToPlayer);


        //    Vector3 forward = transform.TransformDirection(transform.forward);
        //    Vector3 toOther = player.position - transform.position;
        //    float dotProduct = Vector3.Dot(forward, toOther);
        //    // If this is true: The player has moved across the portal
        //    if (dotProduct < 0f)
        //    {
        //        // Teleport him!
        //        Debug.Log("Teleport!");
        //        float rotationDiff = -Quaternion.Angle(transform.rotation, teleportLocation.rotation);
        //        rotationDiff += 180;
        //        player.Rotate(Vector3.up, rotationDiff);

        //        Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
        //        player.position = teleportLocation.position + positionOffset;

        //        playerIsOverlapping = false;
        //    }
        //}

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = player.position - transform.position;
        dot = Vector3.Dot(forward, toOther);

        if (playerIsOverlapping)
        {
            Vector3 portalToPlayer = transform.position - player.position;
            if (dot > 0) // the player is in front of the portal
            {
                // Teleport him!
                Debug.Log("Teleport!");
                float rotationDiff = -Quaternion.Angle(transform.rotation, teleportLocation.rotation);
                rotationDiff += 0;
                //player.Rotate(Vector3.up, rotationDiff);
                var x = UnityEditor.TransformUtils.GetInspectorRotation(portalCamera.transform).x;
                var y = UnityEditor.TransformUtils.GetInspectorRotation(portalCamera.transform).y;
                var z = UnityEditor.TransformUtils.GetInspectorRotation(portalCamera.transform).z;

                Quaternion rotation = Quaternion.Euler(x, y, z);
                player.transform.rotation = rotation;

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                player.position = teleportLocation.position + positionOffset;

                playerIsOverlapping = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = false;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision with: " + collision.gameObject.name);
    //    collision.transform.position = otherPortal.transform.position;
    //}
}
