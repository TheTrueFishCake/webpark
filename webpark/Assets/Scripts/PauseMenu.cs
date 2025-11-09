using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public Slider sensitivitySlider;
    public Slider fovSlider;
    public TextMeshProUGUI sensitivityText;
    public TextMeshProUGUI fovText;

    [Header("References")]
    public PlayerController cameraController;
    public Camera playerCamera;

    private bool isPaused = false;

    private const string SensitivityKey = "CameraSensitivity";
    private const string FOVKey = "CameraFOV";

    void Start()
    {
        // Load saved values or set defaults
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);
        float savedFOV = PlayerPrefs.GetFloat(FOVKey, 60f);

        if (cameraController != null)
            cameraController.mouseSensitivity = savedSensitivity;
        if (playerCamera != null)
            playerCamera.fieldOfView = savedFOV;

        // Initialize UI
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = savedSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityText(savedSensitivity);
        }

        if (fovSlider != null)
        {
            fovSlider.value = savedFOV;
            fovSlider.onValueChanged.AddListener(OnFovChanged);
            UpdateFovText(savedFOV);
        }

        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    // 📡 Disconnect from Photon room
    public void DisconnectFromLobby()
    {
        Time.timeScale = 1f;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("main_menu");
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("main_menu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Quit Game");
    }

    void OnSensitivityChanged(float value)
    {
        if (cameraController != null)
            cameraController.mouseSensitivity = value;

        UpdateSensitivityText(value);
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
    }

    void OnFovChanged(float value)
    {
        if (playerCamera != null)
            playerCamera.fieldOfView = value;

        UpdateFovText(value);
        PlayerPrefs.SetFloat(FOVKey, value);
        PlayerPrefs.Save();
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
            sensitivityText.text = $"Sensitivity: {value:F2}";
    }

    void UpdateFovText(float value)
    {
        if (fovText != null)
            fovText.text = $"FOV: {value:F0}";
    }
}
