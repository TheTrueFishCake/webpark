using Photon.Pun;
using UnityEngine;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private bool hasSpawned = false;

    void Start()
    {
        TrySpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("🚀 PlayerSpawn: OnJoinedRoom triggered, attempting to spawn...");
        TrySpawnPlayer();
    }

    private void TrySpawnPlayer()
    {
        if (hasSpawned)
        {
            Debug.Log("🟡 Player already spawned, skipping...");
            return;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("⚠️ Photon not ready — cannot spawn player yet.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("❌ Player prefab is missing from PlayerSpawn!");
            return;
        }

        // Pick random spawn point
        Transform spawnPoint = null;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);

        hasSpawned = true;
        Debug.Log($"✅ Spawned player: {playerObj.name} at {spawnPos}");
    }
}
