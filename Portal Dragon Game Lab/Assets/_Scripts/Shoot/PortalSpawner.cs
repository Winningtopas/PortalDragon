using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    private GameObject GameMaster;

    public float speed = 10f;

    public GameObject portalObject;
    public GameObject cameraObject;

    [SerializeField]
    private GameObject portalCollection;
    [SerializeField]
    private GameObject portalCameras;

    GameObject[] portals = new GameObject[2];
    GameObject[] cameras = new GameObject[2];

    private float lifeTime = 2f;
    private float rayCastDistance = 10f;

    private int layerMask = 1 << 0;

    // Start is called before the first frame update
    void Start()
    {
        GameMaster = GameObject.Find("GameMaster");
        portalCollection = GameObject.Find("Portal Collection");
        portalCameras = GameObject.Find("Portal Cameras");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayCastDistance, layerMask))
        {
            OnDeath();
        }

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0.0f)
        {
            OnDeath();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        rb.velocity = forward * speed * Time.deltaTime;
    }

    void OnDeath()
    {
        //make the cameras
        for (int i = 0; i < portals.Length; i++)
        {
            cameras[i] = Instantiate(cameraObject, new Vector3(transform.position.x + 10 * i, transform.position.y, transform.position.z),
                Quaternion.identity);
            GameMaster.GetComponent<PortalSetUp>().MakeNewRenderTexture(cameras[i]);
            cameras[i].transform.parent = portalCameras.transform;
        }

        //portal rotation values
        var x = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).x;
        var y = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).y;
        var z = UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z;

        Quaternion rotation = Quaternion.Euler(x, y, z);

        //make the portals
        for (int i = 0; i < portals.Length; i++)
        {
            if(i == 0) //the first portal
                portals[i] = Instantiate(portalObject, transform.position, rotation);
            else // the second portal, rotated 180 degrees on the y and put behind the other portal
                portals[i] = Instantiate(portalObject, transform.position + transform.forward * -75,
                    Quaternion.Euler(-x, y + 180f, -z));

            portals[i].transform.parent = portalCollection.transform;

            GameMaster.GetComponent<PortalSetUp>().AssignMaterialToPortal(portals[i], i);
        }

        // link cameras to the portals
        GameMaster.GetComponent<PortalSetUp>().AssignPortalsToCamera();

        //destroys this object, and it's clone
        GetComponent<PortalableObject>().DestroyObject();
    }
}
