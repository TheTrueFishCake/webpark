using UnityEngine;

public class playerControl : MonoBehaviour
{
    
    Rigidbody rb;

    SphereCollider floorDetectorCol;

    Transform cam;

    [Range(0f, 10.0f)]
    [SerializeField] float moveSpeed = 7f;
    [Range(0f, 15.0f)]
    [SerializeField] float jumpHeight = 10f;

    [Space]

    [Range(0f,10f)]
    [SerializeField] float cameraSensitivityX = 5f;
    [Range(0f, 10f)]
    [SerializeField] float cameraSensitivityY = 5f; //didn't know how to make these without giving both floats spaces so I made them seperate ;*(

    bool touchingGround, canJump;
    float xCamRot, extraJumpTime;
    Vector3 movementInp;
    Vector3 cameraInp;

    void Start()
    {
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        cam = FindFirstObjectByType<Camera>().GetComponent<Transform>(); //please dont make fun of me for this
        floorDetectorCol = GetComponent<SphereCollider>();
    }

    void Update()
    {

        // MOVEMENT //

        movementInp = new Vector3 (Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        Vector3 MoveVector = transform.TransformDirection(movementInp) * moveSpeed;  // allowing direction to change based on rotation
        rb.linearVelocity = new Vector3(MoveVector.x, rb.linearVelocity.y, MoveVector.z); //movement


        // JUMPING //

        if (extraJumpTime >= 0 && !touchingGround)
            extraJumpTime -= Time.deltaTime;
        else if (extraJumpTime <= 0 && !touchingGround)
            canJump = false;

        if (Input.GetButtonDown("Jump") && canJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z); //adds jump velocity
            touchingGround = false; //disallow jumping
        }

        // CAMERA //

        cameraInp = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        xCamRot -= cameraInp.y * cameraSensitivityY;

        transform.Rotate(0f,cameraInp.x * cameraSensitivityX, 0f);
        cam.localRotation = Quaternion.Euler(xCamRot,0f,0f);

        //camera rotation currently needs to be clamped on Y
    }

    private void OnCollisionEnter(Collision collision) //onCollisionEnter so that it will only detect when falling onto ground and not when in the middle of jumping up
    {
        touchingGround = true; //if sphere collider is touching anything, set touchingGround to true and allow jumping
        canJump = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        extraJumpTime = 0.25f;
        touchingGround = false; //if off ground, disallow jumping after a 0.25s timer
    }
}
