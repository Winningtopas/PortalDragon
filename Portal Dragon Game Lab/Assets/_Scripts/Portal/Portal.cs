using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform player;
    public GameObject otherPortal;
    private Transform teleportLocation;

    public GameObject portalCamera;
    private new Renderer renderer;

    public float dot;

    private bool playerIsOverlapping = false;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();

    [SerializeField]
    private Collider wallCollider;

    [SerializeField]
    Vector3 testPosition;

    public float dotProduct;

    // resizing the portals

    private Vector3 destinationScale;
    private float scaleSpeed = 0.25f;
    private float scaleAmount = 10f;

    private ParticleSystem particleChild;
    private float destinationRadius;

    private void Awake()
    {
        renderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        player = GameObject.Find("Main Camera").transform;
        wallCollider = otherPortal.GetComponent<Collider>();

        ScalePortal();
    }

    private void ScalePortal()
    {
        // scale the portal to it's normal size

        FindChildren(gameObject);

        ParticleSystem.ShapeModule ps = particleChild.shape;
        destinationRadius = ps.radius;
        ps.radius = ps.radius / scaleAmount;

        destinationScale = transform.localScale;
        transform.localScale = transform.localScale / scaleAmount;

        StartCoroutine(ScaleOverTime(scaleSpeed));
    }

    // Update is called once per frame

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
        for (int i = 0; i < portalObjects.Count; ++i)
        {
            //if the portalobject got destroyed
            if (portalObjects[i] == null)
            {
                break;
            }

            PortalableObject traveller = portalObjects[i];

            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);
            testPosition = objPos;

            if (objPos.z > 0.0f)
            {
                //Debug.Log("CALCULATE THE DOT PRODUCT HERE");
                portalObjects[i].Warp();
            }

            Vector3 forward = transform.forward;
            Vector3 toOther = portalObjects[i].transform.position - transform.position;
            dotProduct = Vector3.Dot(forward, toOther);
        }
    }

    void FindChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child.GetComponent<ParticleSystem>() != null)
            {
                particleChild = child.GetComponent<ParticleSystem>();
                break;
            }
        }
    }

    IEnumerator ScaleOverTime(float time)
    {
        ParticleSystem.ShapeModule ps = particleChild.shape;
        float originalRadius = ps.radius;

        Vector3 originalScale = transform.localScale;
        float currentTime = 0.0f;

        do
        {
            ps.radius = Mathf.Lerp(originalRadius, destinationRadius, currentTime / time);
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);

            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime < time);

        // sets the exact value, because Lerp never get's there
        if (currentTime >= time)
        {
            ps.radius = destinationRadius;
            transform.localScale = destinationScale;
            yield return null;
        }
    }

    public bool IsRendererVisible()
    {
        return renderer.isVisible;
    }

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
}