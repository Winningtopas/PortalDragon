using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModelMovement : MonoBehaviour
{
    //[SerializeField]
    //private GameObject footstepObject;
    private bool carpetBool;
    private AudioSource footstepSource;
    
    [SerializeField]
    private AudioClip[] carpetStep;

    [SerializeField]
    private AudioClip[] footstep;

    [SerializeField]
    public float horizontalSens; //horizontal camera

    private int audiopicker;
    private float velocity = 5;
    private float yaw = 0.0f;
    public float runningSpeed;
    public float currentSpeed;
    private Vector3 moveDirection;
    private Vector3 newMoveDirection;
    private Vector3 currentDirection;
    public float startDirection;

    //public bool ableToJump = false;
    private float soundSpeed;

    public float height = 2.0f;
    public float heightPadding = 0.2f;
    public LayerMask ground;
    private float groundAngle;
    public float maxGroundAngle = 120;
    private Vector3 lowerHalf;

    private bool resetScale = false;
    private Vector3 forward;
    private RaycastHit hitInfo;
    private bool grounded;

    private Rigidbody modelBody;

    [SerializeField]
    public float moveSpeed;

    //[SerializeField]
    //public float fallMultiplier;

    [SerializeField]
    public float jumpVel;

    private int currentSceneIndex;

    // Start is called before the first frame update
    private void Start()
    {
        //footstepSource = footstepObject.GetComponent<AudioSource>();
        modelBody = GetComponent<Rigidbody>();
        currentSpeed = moveSpeed;
        soundSpeed = 0.6f;
        //InvokeRepeating("MovementSound", 0.0f, soundSpeed);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        yaw = startDirection;
    }

    // Update is called once per frame
    private void Update()
    {
        MovementInput();
        yaw += horizontalSens * Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector3(0, yaw, 0.0f);

        CalculateForward();
        CalculateGroundAngle();
        CheckGround();
        DrawDebugLines();
        HandleMovement();
        Sprint();
    }

    private void MovementInput()
    {
        moveDirection.x = Input.GetAxis("Horizontal");
        moveDirection.z = Input.GetAxis("Vertical");
    }

    private void MovementSound()
    {
        if (grounded)
        {
            audiopicker = Random.Range(0, 3);
            if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
            {
                if (audiopicker == 0)
                {
                    if (carpetBool != true)
                    {
                        footstepSource.PlayOneShot(footstep[0]);
                    }
                    else if (carpetBool == true)
                    {
                        footstepSource.PlayOneShot(carpetStep[0], 0.7f);
                    }
                }
                else if (audiopicker == 1)
                {
                    if (carpetBool != true)
                    {
                        footstepSource.PlayOneShot(footstep[1]);
                    }
                    else if (carpetBool == true)
                    {
                        footstepSource.PlayOneShot(carpetStep[1], 0.7f);
                    }
                }
                else if (audiopicker == 2)
                {
                    if (carpetBool != true)
                    {
                        footstepSource.PlayOneShot(footstep[2]);
                    }
                    else if (carpetBool == true)
                    {
                        footstepSource.PlayOneShot(carpetStep[2], 0.7f);
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            this.gameObject.transform.parent = other.gameObject.transform;
            resetScale = true;
        }
            
        else
        {
            this.gameObject.transform.parent = null;
            if (resetScale)
            {
                this.transform.localScale = new Vector3(1, 1, 1);
                resetScale = false;
            }
        }
            

        if (other.gameObject.tag == "Carpet")
        {
            carpetBool = true;
        }

    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "MovingPlatform")
            this.gameObject.transform.parent = null;

        if (other.gameObject.tag == "Carpet")
        {
            carpetBool = false;
        }
    }

    private void HandleMovement()
    {
        if (groundAngle >= maxGroundAngle) return;

        newMoveDirection = new Vector3(moveDirection.x, 0.0f, moveDirection.z);
        if (newMoveDirection.magnitude > 1.0f)
            newMoveDirection = newMoveDirection.normalized;

        if (newMoveDirection.normalized.x != 0 || newMoveDirection.normalized.z != 0)
            currentDirection = newMoveDirection.normalized;

        newMoveDirection *= currentSpeed * Time.deltaTime;

        transform.Translate(newMoveDirection);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            modelBody.velocity = Vector3.up * jumpVel;
            //ableToJump = false;
        }

        //if (modelBody.velocity.y < 0)
        //{
        //    modelBody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        //}
    }

    public void Sprint()
    {
        if (Input.GetButton("Fire3"))
        {
            currentSpeed = runningSpeed;
            soundSpeed = 0.3f;
        }
        else
        {
            currentSpeed = moveSpeed;
            soundSpeed = 0.6f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WinPoint")
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        if (other.gameObject.tag == "Death")
        {
            SceneManager.LoadScene(currentSceneIndex, LoadSceneMode.Single);
        }
    }

    //-----------------------------------------------SLOPE-----------------------

    private void CalculateForward()
    {
        if (!grounded)
        {
            forward = transform.forward;
            return;
        }

        forward = Vector3.Cross(hitInfo.normal, -transform.right);
    }

    private void CalculateGroundAngle()
    {
        if (!grounded)
        {
            groundAngle = 90;
            return;
        }

        groundAngle = Vector3.Angle(hitInfo.normal, transform.forward);
    }

    private void CheckGround()
    {
        lowerHalf = transform.position;
        lowerHalf.y -= 1.0f;
        if (Physics.Raycast(lowerHalf, -Vector3.up, out hitInfo, height + heightPadding, ground))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    private void DrawDebugLines()
    {
        Debug.DrawLine(transform.position, transform.position + forward * height * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * height, Color.green);
    }
}