using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dotTest : MonoBehaviour
{
    public float dotProduct;

    private void OnTriggerStay(Collider other)
    {
        Vector3 forward = transform.forward;
        Vector3 toOther = other.transform.position - transform.position;
        dotProduct = Vector3.Dot(forward, toOther);
        // If this is true: The player has moved across the portal
        //if (dotProduct > 0f)
        //{
        //    // Teleport him!
        //    //portalObjects[i].Warp();
        //}
    }
}
