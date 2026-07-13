using UnityEngine;

public class HeadRotator : MonoBehaviour
{
    public Transform headBone;
    public float yRotation = 55f;
    public Vector3 headScale = new Vector3(1.6f, 1.6f, 1.6f);

    void Start()
    {
        FindHeadBone();
    }

    void LateUpdate()
    {
        if (headBone == null)
        {
            FindHeadBone();
        }

        if (headBone != null)
        {
            // Lock local Y rotation to 55 degrees
            headBone.localRotation = Quaternion.Euler(0f, yRotation, 0f);
            
            // Lock local scale
            headBone.localScale = headScale;
        }
    }

    private void FindHeadBone()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "Head")
            {
                headBone = child;
                break;
            }
        }
    }
}
