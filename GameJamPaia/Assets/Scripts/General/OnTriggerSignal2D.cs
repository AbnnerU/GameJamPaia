using System;
using UnityEngine;

public class OnTriggerSignal2D : MonoBehaviour, IHasActiveState
{
    [SerializeField] private Collider2D colliderRef;
    [SerializeField] private string targetTag;

    public Action<bool> OnTargetInArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnTargetInArea?.Invoke(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnTargetInArea?.Invoke(false);
        }
    }


    public void Disable()
    {
        colliderRef.enabled = false;
        OnTargetInArea?.Invoke(false);
    }

    public void Enable()
    {
        colliderRef.enabled = true;
    }
}
