using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Camera playerCamera;

    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpHeight = 2f;

    [Header("Look & Physics")]
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float gravity = -9.81f;

    float verticalVelocity = 0f;
    float cameraPitch = 0f;

    void Start()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = Camera.main;

        // Lock cursor for FPS control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();
        HandleMovement();
    }

    // Handles horizontal movement and applies vertical velocity (gravity / jump)
    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        float speed = IsRunning() ? runSpeed : walkSpeed;

        Vector3 velocity = move.normalized * speed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    // Handles jump initiation and gravity integration
    void HandleJump()
    {
        if (characterController.isGrounded)
        {
            // small negative to keep grounded consistently
            if (verticalVelocity < 0f) verticalVelocity = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                // v = sqrt(2 * g * h) ; gravity is negative so multiply by -1
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // apply gravity every frame
        verticalVelocity += gravity * Time.deltaTime;
    }

    // Handles mouse look: yaw rotates the player, pitch rotates the camera
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw
        transform.Rotate(Vector3.up * mouseX);

        // Pitch
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        if (playerCamera != null)
            playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
    }

    // Simple run check (Left Shift)
    bool IsRunning()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}

