using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPortalSpawner : MonoBehaviour
{
    public GameObject portalSpawner;
    private GameObject currentPortalSpawner;

    [SerializeField]
    private bool shoot = false;

    private void Start()
    {
        if (gameObject.transform.root.gameObject.name.Contains(" Clones")) // if the gameobject is a clone for the purpose of portal traveling
        {
            shoot = false;
            Destroy(GetComponent<ShootPortalSpawner>());
        }
    }

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
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        var x = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).x;
        var y = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).y;
        var z = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z;

        Quaternion rotation = Quaternion.Euler(x, y, z);
        currentPortalSpawner = Instantiate(portalSpawner, position, rotation);
        currentPortalSpawner.name = "PortalSpawner";
    }
}
