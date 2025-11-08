using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun
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

    [Header("Leg Motion")]
    [SerializeField] Transform leftLeg;
    [SerializeField] Transform rightLeg;
    [SerializeField] float legAngle = 30f;    // max rotation angle in degrees
    [SerializeField] float legSpeed = 6f;     // speed of back-and-forth motion

    float verticalVelocity = 0f;
    float cameraPitch = 0f;

    Quaternion leftLegInitialRot = Quaternion.identity;
    Quaternion rightLegInitialRot = Quaternion.identity;

    void Start()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();

        // cache initial local rotations for legs so we rotate relative to their starting pose
        if (leftLeg != null) leftLegInitialRot = leftLeg.localRotation;
        if (rightLeg != null) rightLegInitialRot = rightLeg.localRotation;

        if (!photonView.IsMine)
        {
            // Disable camera and script for remote players
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);

            this.enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleJump();
        HandleMovement();
        HandleLegMotion();
    }

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

    void HandleJump()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = -2f;
            if (Input.GetButtonDown("Jump"))
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        if (playerCamera != null)
            playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
    }

    // Rotates the two leg transforms back-and-forth to simulate a simple leg motion.
    // The motion amplitude is scaled by the current movement input magnitude (so legs stop when not moving).
    void HandleLegMotion()
    {
        if (leftLeg == null && rightLeg == null)
            return;

        float inputMagnitude = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
        // compute oscillation; when inputMagnitude == 0 the cycle becomes 0 and legs return to initial rotations
        float cycle = Mathf.Sin(Time.time * legSpeed) * legAngle * inputMagnitude;

        if (leftLeg != null)
        {
            Quaternion offset = Quaternion.Euler(cycle, 0f, 0f);
            leftLeg.localRotation = leftLegInitialRot * offset;
        }

        if (rightLeg != null)
        {
            // opposite phase to left leg for a simple alternating walk
            Quaternion offset = Quaternion.Euler(-cycle, 0f, 0f);
            rightLeg.localRotation = rightLegInitialRot * offset;
        }
    }

    bool IsRunning() =>
        Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
}
