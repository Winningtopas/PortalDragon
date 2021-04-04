using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private float mouseX, mouseY;

    public AudioSource musicSource;

    [SerializeField]
    private float mouseSens = 100f;

    [SerializeField]
    private float range = 3f;

    public Transform playerBody;
    private float xRotation = 0f;

    public GameObject cloneCamera;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //cloneCamera = rootParent.GetComponent<PortalableObject>().cloneCameraObject;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //if (!PauseMenu.GamePaused)
        //{
        mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (cloneCamera != null)
        {
            cloneCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        playerBody.Rotate(Vector3.up * mouseX);
        //}
    }
}