
using System;
using UnityEngine;

public class OnTriggerUnlockAllDoors2D : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private DoorsManager doorsManager;
    [SerializeField] private bool disableOnTriggerEnter = true;

    public Action<Vector3> WhenCollected;

    private void Awake()
    {
        if(doorsManager == null) 
            doorsManager = FindObjectOfType<DoorsManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            doorsManager.UnlockAllDoors();

            if (disableOnTriggerEnter)
                PoolManager.ReleaseObject(gameObject);

            WhenCollected?.Invoke(transform.position);
        }
    }
}
