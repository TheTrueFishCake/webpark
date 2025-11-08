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

    float verticalVelocity = 0f;
    float cameraPitch = 0f;

    void Start()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();

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

    bool IsRunning() =>
        Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
}
