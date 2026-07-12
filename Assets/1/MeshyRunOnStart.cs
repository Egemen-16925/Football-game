using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class MeshyRunOnStart : MonoBehaviour
{
    [SerializeField] private string stateName = "Run";

    private void Start()
    {
        var animator = GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        animator.Play(stateName, 0, 0f);
    }
}
