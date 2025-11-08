using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject mainScreen;

    [Space]
    [SerializeField] Slider playerAmountSlider;
    [SerializeField] TMP_InputField roomNameJOINInputField;
    [SerializeField] TMP_InputField roomNameMAKEInputField;
    [SerializeField] TMP_Text playerAmountText;

    string roomNameJOIN;
    string roomNameMAKE;
    int maxPlayers;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        loadingScreen.SetActive(true);
        mainScreen.SetActive(false);
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
        roomNameMAKE = roomNameMAKEInputField.text;

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
        roomNameJOIN = roomNameJOINInputField.text;

        if (string.IsNullOrEmpty(roomNameJOIN))
        {
            Debug.LogWarning("⚠️ Room name is empty!");
            return;
        }

        PhotonNetwork.JoinRoom(roomNameJOIN);
        Debug.Log($"🔍 Trying to join room '{roomNameJOIN}'...");
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
        PhotonNetwork.LoadLevel("main_scene");
    }

}
