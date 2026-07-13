using UnityEngine;

public class StaticFlag : MonoBehaviour
{
    private Vector3 startWorldPos;

    void Start()
    {
        // Record the initial world position set in the editor
        startWorldPos = transform.position;
    }

    void LateUpdate()
    {
        // Lock the flag to its starting world position and keep it facing the camera
        transform.position = startWorldPos;
        transform.rotation = Quaternion.identity;
    }
}
