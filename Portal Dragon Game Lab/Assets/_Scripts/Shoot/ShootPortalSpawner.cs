using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPortalSpawner : MonoBehaviour
{
    public GameObject portalSpawner;
    private GameObject currentPortalSpawner;
    private GameObject trueParent;

    [SerializeField]
    private bool shoot = false;

    private void Start()
    {
        FindParent(gameObject);
        if (gameObject.transform.root.gameObject.name.Contains(" Clones")) // if the gameobject is a clone for the purpose of portal traveling
        {
            shoot = false;
            Destroy(GetComponent<ShootPortalSpawner>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            shoot = true;
        }

        if (shoot)
        {
            Shoot();
            shoot = false;
        }
    }

    void Shoot()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        var x = UnityEditor.TransformUtils.GetInspectorRotation(trueParent.transform).x;
        var y = UnityEditor.TransformUtils.GetInspectorRotation(trueParent.transform).y;
        var z = UnityEditor.TransformUtils.GetInspectorRotation(trueParent.transform).z;

        Quaternion rotation = Quaternion.Euler(x, y, z);
        currentPortalSpawner = Instantiate(portalSpawner, position, rotation);
        currentPortalSpawner.name = "PortalSpawner";
    }

    void FindParent(GameObject currentObject)
    {
        GameObject parentObject = currentObject.transform.parent.gameObject;

        //find the root parent so we can later use their rotation for the shoot function. 
        //NOTE: we can't use the root for this, because the root gameobject may not be the actual shooting object

        if (parentObject.GetComponent<MainObject>() == null)
        {
            FindParent(parentObject);
        }
        else
        {
            trueParent = parentObject;
        }
    }
}
