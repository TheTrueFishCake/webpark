using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    [SerializeField] float spinSpeed = 1f;


    void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}
