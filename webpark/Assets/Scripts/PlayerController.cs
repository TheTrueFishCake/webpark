using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera PlayerCamera;
    [SerializeField] float MouseSensitivity = 100f;
    [SerializeField] float MoveSpeed = 5f;
    float xRotation = 0f;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
