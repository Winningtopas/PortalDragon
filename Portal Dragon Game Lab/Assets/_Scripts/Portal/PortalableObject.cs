using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PortalableObject : MonoBehaviour
{
    private GameObject gameMaster;

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

    [SerializeField]
    private GameObject cloneCameraObject;

    [SerializeField]
    private GameObject cameraPosition;
    private GameObject cameraClonePosition;
    private bool hasCamera = false;

    private bool fullPortalMovement = false;

    //needed for slicing

    public Material[] originalMaterials;
    public Material[] cloneMaterials { get; set; }

    private int currentPortalIndex = -1;

    protected virtual void Awake()
    {
        GetComponent<Collider>().enabled = false;

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

    }

    private void Start()
    {
        gameMaster = GameObject.Find("GameMaster");

        cloneObjectContainer = GameObject.Find("Portalable Object Clones");
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
                    hasCamera = true;
                }
                if (allChildren[i].name == "CameraPosition")
                {
                    cameraPosition = allChildren[i];
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
        originalGameObject.GetComponent<PortalableObject>().cloneMaterials = originalMaterials;

        for (int i = 0; i < allChildren.Count; i++)
        {
            Destroy(allChildren[i].GetComponent<PortalableObject>());
            Destroy(allChildren[i].GetComponent<Collider>());
            if (allChildren[i].GetComponent<Camera>() != null)
            {
                hasCamera = true;
                originalGameObject.GetComponent<PortalableObject>().cloneCameraObject = allChildren[i];
            }

            if (allChildren[i].name == "CameraPosition")
            {
                originalGameObject.GetComponent<PortalableObject>().cameraClonePosition = allChildren[i];
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

            var matList = new List<Material>();

            //find the materials so we can slice them later
            if (child.GetComponent<MeshRenderer>() != null)
            {
                MeshRenderer renderer = child.GetComponent<MeshRenderer>();
                foreach (var mat in renderer.materials)
                {
                    matList.Add(mat);
                }
            }

            if (child.GetComponent<SkinnedMeshRenderer>() != null)
            {
                SkinnedMeshRenderer skinnedRenderer = child.GetComponent<SkinnedMeshRenderer>();
                foreach (var mat in skinnedRenderer.materials)
                {
                    matList.Add(mat);
                }
            }

            originalMaterials = matList.ToArray();
        }
    }

    private void LateUpdate()
    {
        if (inPortal == null || outPortal == null)
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

        if (fullPortalMovement && hasCamera)
        {
            cloneCameraObject.SetActive(true);
            ownCameraObject.SetActive(false);
        }
        //Debug.Break();

        ++inPortalCount;
    }

    public void SetSliceOffsetDst(float dst, bool clone)
    {
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (clone)
            {
                cloneMaterials[i].SetFloat("sliceOffsetDst", dst);
            }
            else
            {
                originalMaterials[i].SetFloat("sliceOffsetDst", dst);
            }

        }
    }

    public virtual void Warp()
    {
        if (hasCamera)
        {
            ownCameraObject.SetActive(true);
            cloneCameraObject.SetActive(false);
        }

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
        // remove the parts of the material that shouldnt be visible
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            originalMaterials[i].SetVector("sliceNormal", Vector3.zero);
        }

        if (hasCamera)
        {
            ownCameraObject.SetActive(false);
            cloneCameraObject.SetActive(true);
        }
        Physics.IgnoreCollision(collider, wallCollider, false);
        --inPortalCount;

        if (inPortalCount == 0)
        {
            cloneObject.SetActive(false);
            if (hasCamera)
            {
                cloneCameraObject.SetActive(false);
                ownCameraObject.SetActive(true);
            }
        }
    }

    public void DestroyObject()
    {
        Destroy(cloneGameObject);
        Destroy(this.gameObject);
    }
}
