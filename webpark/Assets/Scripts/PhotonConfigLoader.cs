using System.IO;
using UnityEngine;
using Photon.Pun;

public class PhotonConfigLoader : MonoBehaviour
{
    [System.Serializable]
    private class PhotonConfig
    {
        public string AppIdRealtime;
    }

    void Awake()
    {
        // The file is directly under Assets/
        string path = Path.Combine(Application.dataPath, "photonappid.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"Photon App ID file not found at: {path}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            PhotonConfig config = JsonUtility.FromJson<PhotonConfig>(json);

            if (!string.IsNullOrEmpty(config.AppIdRealtime))
            {
                PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = config.AppIdRealtime;
                Debug.Log("✅ Photon App ID loaded successfully from photonappid.json");
            }
            else
            {
                Debug.LogWarning("⚠️ photonappid.json found, but AppIdRealtime was empty!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Failed to read Photon App ID file: " + e.Message);
        }
    }
}
