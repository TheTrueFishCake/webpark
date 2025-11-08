using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

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
    private static bool hasLoadedGameScene = false;

    // ----------------------------------------------------------------------

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.AutomaticallySyncScene = true;
            hasLoadedGameScene = false;
        }
        else
        {
            Debug.Log("🧩 Duplicate NetworkManager destroyed.");
            Destroy(gameObject);
            return;
        }
    }

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

    // ----------------------------------------------------------------------
    // PHOTON CALLBACKS
    // ----------------------------------------------------------------------

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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"❌ Disconnected from Photon: {cause}");
        loadingScreen.SetActive(true);
        mainScreen.SetActive(false);
        hasLoadedGameScene = false;
    }

    // ----------------------------------------------------------------------
    // ROOM CREATION & JOIN
    // ----------------------------------------------------------------------

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
        Debug.Log($"🛠 Creating room '{roomNameMAKE}' (Max players: {maxPlayers})...");
        PhotonNetwork.CreateRoom(roomNameMAKE, roomOptions);
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

    // ----------------------------------------------------------------------
    // ROOM JOIN SUCCESS
    // ----------------------------------------------------------------------

    public override void OnJoinedRoom()
    {
        Debug.Log($"✅ Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Current scene: {SceneManager.GetActiveScene().name}, hasLoadedGameScene: {hasLoadedGameScene}");

        // Only the master client triggers the scene load once
        if (!hasLoadedGameScene && PhotonNetwork.IsMasterClient)
        {
            hasLoadedGameScene = true;
            Debug.Log("🌍 Master client loading 'main_scene' for all players...");
            PhotonNetwork.LoadLevel("main_scene"); // <── Correct main game scene
        }
        else
        {
            Debug.Log("⏳ Client waiting for master to load 'main_scene'...");
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"⭐ New Master Client: {newMasterClient.NickName}");

        // If the new master takes over before scene load, ensure sync continues
        if (PhotonNetwork.IsMasterClient && !hasLoadedGameScene)
        {
            hasLoadedGameScene = true;
            Debug.Log("🌍 New master loading 'main_scene'...");
            PhotonNetwork.LoadLevel("main_scene");
        }
    }
}
