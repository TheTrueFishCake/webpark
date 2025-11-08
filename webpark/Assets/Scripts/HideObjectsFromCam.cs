using UnityEngine;

public class HideObjectsFromCam : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    private void OnPreCull()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }
    private void OnPostRender()
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(true);
        }
    }
}
