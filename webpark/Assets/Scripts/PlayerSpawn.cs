using Photon.Pun;
using UnityEngine;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public override void OnJoinedRoom()
    {
        Debug.Log("🚀 PlayerSpawn: Joined room in main_scene, spawning player...");

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("⚠️ Photon not ready — player not spawned.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("⚠️ Player prefab is missing!");
            return;
        }

        // Pick a random spawn point
        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);

        Debug.Log($"✅ Spawned player: {playerObj.name} at {spawnPos}");
    }
}
