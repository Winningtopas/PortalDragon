using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PortalableObject : MonoBehaviour
{
    private GameObject cloneObject;

    private int inPortalCount = 0;
    
    private Portal inPortal;
    private Portal outPortal;

    private new Rigidbody rigidbody;
    protected new Collider collider;

    private List<GameObject> allChildren = new List<GameObject>();
    public int childCount = 0;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    private GameObject cloneObjectContainer;

    public GameObject cloneGameObject;
    private GameObject ownCameraObject;
    private GameObject cloneCameraObject;

    private bool fullPortalMovement = false;

    protected virtual void Awake()
    {
        //MakeCloneChildren();
        //SetChildren(cloneObject);
        //cloneObject.SetActive(false);

        //var meshFilter = cloneObject.AddComponent<MeshFilter>();
        //var meshRenderer = cloneObject.AddComponent<MeshRenderer>();

        //meshFilter.mesh = GetComponent<MeshFilter>().mesh;
        //meshRenderer.materials = GetComponent<MeshRenderer>().materials;
        //cloneObject.transform.localScale = transform.localScale;

        GetComponent<Collider>().enabled = false;


        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

    }

    private void Start()
    {
        cloneObjectContainer = GameObject.Find("Portable Object Clones");
        FindChildren(this.gameObject);

        if (gameObject.name.Contains(" clone"))
        {
            AssignCloneGameObject();
        }
        else
        {
            GetComponent<Collider>().enabled = true;
            cloneObject = Instantiate(gameObject);
            cloneObject.name = gameObject.name + " clone";

            for (int i = 0; i < allChildren.Count; i++)
            {
                Destroy(allChildren[i].GetComponent<PortalableObject>());
                Destroy(allChildren[i].GetComponent<Collider>());
                Destroy(allChildren[i].GetComponent<AudioListener>());
                if (allChildren[i].GetComponent<Camera>() != null)
                {
                    ownCameraObject = allChildren[i];
                    //Destroy(allChildren[i].gameObject);
                }
            }
        }
    }

    void AssignCloneGameObject()
    {
        transform.parent = cloneObjectContainer.transform;

        string originalName = gameObject.name.Replace(" clone", "");
        GameObject originalGameObject = GameObject.Find(originalName);
        originalGameObject.GetComponent<PortalableObject>().cloneGameObject = gameObject;

        for (int i = 0; i < allChildren.Count; i++)
        {
            Destroy(allChildren[i].GetComponent<PortalableObject>());
            Destroy(allChildren[i].GetComponent<Collider>());
            if (allChildren[i].GetComponent<Camera>() != null)
            {
                originalGameObject.GetComponent<PortalableObject>().cloneCameraObject = allChildren[i];
            }
        }

        Destroy(GetComponent<PortalableObject>());
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<AudioListener>());
        gameObject.SetActive(false);
    }

    void FindChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            childCount++;
            allChildren.Add(child);
            FindChildren(child);
        }
    }

    void MakeCloneChildren()
    {
        for (int i = 0; i < allChildren.Count; i++)
        {
            GameObject childClone = new GameObject();
            childClone.name = allChildren[i].name + " clone";
            childClone.SetActive(false);

            if (allChildren[i].GetComponent<MeshFilter>())
            {
                var meshFilter = childClone.AddComponent<MeshFilter>();
                meshFilter.mesh = allChildren[i].GetComponent<MeshFilter>().mesh;
            }

            if (allChildren[i].GetComponent<MeshRenderer>())
            {
                var meshRenderer = childClone.AddComponent<MeshRenderer>();
                meshRenderer.materials = allChildren[i].GetComponent<MeshRenderer>().materials;
            }

            if (allChildren[i].GetComponent<SkinnedMeshRenderer>())
            {
                var skinnedRenderer = childClone.AddComponent<SkinnedMeshRenderer>();
                skinnedRenderer.materials = allChildren[i].GetComponent<SkinnedMeshRenderer>().materials;
            }

            childClone.transform.localScale = allChildren[i].transform.localScale;

            childClone.transform.parent = cloneObject.transform;
        }
    }

    void SetChildren(GameObject parent)
    {
        for(int i = 0; i < allChildren.Count; i++)
        {
            allChildren[i].transform.parent = parent.transform;
        }
    }

    private void LateUpdate()
    {
        if(inPortal == null || outPortal == null)
        {
            return;
        }

        if(cloneObject.activeSelf)
        {
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            // Update rotation of clone.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = halfTurn * relativeRot;
            cloneObject.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            cloneObject.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
        }
    }

    public void SetIsInPortal(Portal inPortal, Portal outPortal, Collider wallCollider)
    {
        fullPortalMovement = !fullPortalMovement; //otherwise the camera stutters when transitioning

        this.inPortal = inPortal;
        this.outPortal = outPortal;

        Physics.IgnoreCollision(collider, wallCollider);

        cloneObject.SetActive(true);

        if (fullPortalMovement)
        {
            cloneCameraObject.SetActive(true);
            ownCameraObject.SetActive(false);
        }

        ++inPortalCount;
    }

    public virtual void Warp()
    {
        ownCameraObject.SetActive(true);
        cloneCameraObject.SetActive(false);


        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        // Update position of object.
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;
        transform.position = outTransform.TransformPoint(relativePos);

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outTransform.rotation * relativeRot;

        // Update velocity of rigidbody.
        Vector3 relativeVel = inTransform.InverseTransformDirection(rigidbody.velocity);
        relativeVel = halfTurn * relativeVel;
        rigidbody.velocity = outTransform.TransformDirection(relativeVel);

        // Swap portal references.
        var tmp = inPortal;
        inPortal = outPortal;
        outPortal = tmp;
    }

    public void ExitPortal(Collider wallCollider)
    {
        ownCameraObject.SetActive(false);
        cloneCameraObject.SetActive(true);
        Physics.IgnoreCollision(collider, wallCollider, false);
        --inPortalCount;

        if (inPortalCount == 0)
        {
            cloneObject.SetActive(false);
            cloneCameraObject.SetActive(false);
            ownCameraObject.SetActive(true);
        }
    }
}
