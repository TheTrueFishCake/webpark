using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]]
    [SerializeField] Camera PlayerCamera;
    [SerializeField] CharacterController CharacterController;
    [Header]
    [SerializeField] float MouseSensitivity = 100f;
    [SerializeField] float MoveSpeed = 5f;

    float xRotation = 0f;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

}
