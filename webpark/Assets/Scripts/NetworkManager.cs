using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
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
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        loadingScreen.SetActive(true);
        mainScreen.SetActive(false);
    }

    private void Update()
    {
        playerAmountText.text = "Max Players: " + playerAmountSlider.value.ToString();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby();
        loadingScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        maxPlayers = (int)playerAmountSlider.value;
        roomNameMAKE = roomNameMAKEInputField.text;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(roomNameMAKE, roomOptions);
        Debug.Log("Creating Room: " + roomNameMAKE + " with max players: " + maxPlayers);
        PhotonNetwork.JoinRoom(roomNameMAKE);
    }

    public void JoinRoom()
    {
        roomNameJOIN = roomNameJOINInputField.text;
        PhotonNetwork.JoinRoom(roomNameJOIN);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        SceneManager.LoadScene("main_scene");
    }
}
