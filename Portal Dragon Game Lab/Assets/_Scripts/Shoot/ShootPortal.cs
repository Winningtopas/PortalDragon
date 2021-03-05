using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPortal : MonoBehaviour
{
    public GameObject portalSpawner;
    private GameObject currentPortalSpawner;

    [SerializeField]
    private bool shoot = false;

    private void Update()
    {
        if (shoot)
        {
            Shoot();
            shoot = false;
        }
    }

    void Shoot()
    {
        Vector3 position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

        var x = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).x;
        var y = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).y;
        var z = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z;

        Quaternion rotation = Quaternion.Euler(x, y, z);
        currentPortalSpawner = Instantiate(portalSpawner, position, rotation);
        currentPortalSpawner.name = "PortalSpawner";
    }
}
