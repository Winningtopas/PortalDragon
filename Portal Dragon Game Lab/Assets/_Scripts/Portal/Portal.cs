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

        //ParticleSystem.EmissionModule em = particleChild.emission;
        //em.rateOverTime = em.rateOverTime.constant / scaleAmount;
        //Debug.Log(em.rateOverTime);

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
        //Debug.DrawRay(transform.position, new Vector3(-newDirection.x, newDirection.y, newDirection.z) * 10, Color.yellow);
        for (int i = 0; i < portalObjects.Count; ++i)
        {
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

            // If this is true: The player can move through a portal
            // We need to keep track of the objPos as well, because the rayquaza model pivot is offcenter
            //if (dotProduct < -10f && objPos.z < -10f)
            //{
            //    // Teleport him!
            //    Debug.Log("dot: " + dotProduct + " pos.z: " + objPos.z);
            //    portalObjects[i].Warp();
            //}
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
        Debug.Log(destinationRadius + " original: " + originalRadius);
        do
        {
            ps.radius = Mathf.Lerp(originalRadius, destinationRadius, currentTime / time);
            //ps.radius = ps.radius / scaleAmount;


            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);


            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime < time);

        // sets the exact value, because Lerp never get's there
        if(currentTime >= time)
        {
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
}
