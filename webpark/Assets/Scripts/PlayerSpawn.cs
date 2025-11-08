using Photon.Pun;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-3f, 3f), 2f, Random.Range(-3f, 3f));
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Photon not ready — player not spawned.");
        }
    }
}
