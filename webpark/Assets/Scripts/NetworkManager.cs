using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainScreen;

    [Space]
    [SerializeField] private Slider playerAmountSlider;
    [SerializeField] private TMP_InputField roomNameJOINInputField;
    [SerializeField] private TMP_InputField roomNameMAKEInputField;
    [SerializeField] private TMP_Text playerAmountText;

    private string roomNameJOIN;
    private string roomNameMAKE;
    private int maxPlayers;

    void Start()
    {
        loadingScreen.SetActive(true);
        mainScreen.SetActive(false);

        Debug.Log("🔌 Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        playerAmountText.text = $"Max Players: {(int)playerAmountSlider.value}";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Connected to Photon Master Server!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Joined Lobby.");
        loadingScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    public void CreateRoom()
    {
        maxPlayers = (int)playerAmountSlider.value;
        roomNameMAKE = roomNameMAKEInputField.text.Trim();

        if (string.IsNullOrEmpty(roomNameMAKE))
        {
            Debug.LogWarning("⚠️ Room name is empty!");
            return;
        }

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers };
        PhotonNetwork.CreateRoom(roomNameMAKE, roomOptions);
        Debug.Log($"🛠 Creating room '{roomNameMAKE}' (Max players: {maxPlayers})...");
    }

    public void JoinRoom()
    {
        roomNameJOIN = roomNameJOINInputField.text.Trim();

        if (string.IsNullOrEmpty(roomNameJOIN))
        {
            Debug.LogWarning("⚠️ Room name is empty!");
            return;
        }

        Debug.Log($"🔍 Attempting to join room '{roomNameJOIN}'...");
        PhotonNetwork.JoinRoom(roomNameJOIN);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"❌ CreateRoom failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"❌ JoinRoom failed: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"✅ Joined Room: {PhotonNetwork.CurrentRoom.Name}");

        // Only master client loads the scene; others auto-sync.
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("🌍 Loading 'main_scene' for all players...");
            PhotonNetwork.LoadLevel("main_scene");
        }
    }
}
