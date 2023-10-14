

using UnityEngine;

public class ColliderHandler2D : MonoBehaviour
{
    [SerializeField] private Collider2D colliderRef;

    public void EnableCollider()
    {
        colliderRef.enabled = true;

        print("Enable Collider");
    }

    public void DisableCollider()
    {
        colliderRef.enabled = false;

        print("Disable Collider");
    }
}
