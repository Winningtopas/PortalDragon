using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject GameMaster;

    [SerializeField]
    private float speed = 10f;

    public GameObject portalObject;
    public GameObject cameraObject;

    [SerializeField]
    GameObject[] portals = new GameObject[2];
    [SerializeField]
    GameObject[] cameras = new GameObject[2];

    public float lifeTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        GameMaster = GameObject.Find("GameMaster");
    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;



        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

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
        //first make the cameras
        for (int i = 0; i < portals.Length; i++)
        {
            cameras[i] = Instantiate(cameraObject, new Vector3(transform.position.x + 10 * i, transform.position.y, transform.position.z), Quaternion.identity);
            cameras[i].name = "Camera " + i;
            GameMaster.GetComponent<PortalTextureSetup>().MakeNewRenderTexture(cameras[i]);
        }
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i] = Instantiate(portalObject, new Vector3(transform.position.x + 10 * i, transform.position.y, transform.position.z), Quaternion.Euler(0f,180f,0f));
            portals[i].name = "Portal " + i;
            GameMaster.GetComponent<PortalTextureSetup>().AssignMaterialToPortal(portals[i], i);
        }
        // link cameras to the portals
        GameMaster.GetComponent<PortalTextureSetup>().AssignPortalsToCamera();

        Destroy(this.gameObject);
    }
}
