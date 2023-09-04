using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnter2DSignal : MonoBehaviour
{
    [SerializeField] private string targetTag;

    public Action<OnTriggerEnter2DSignal> OnSendSignal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            OnSendSignal?.Invoke(this);
        }
    }
}
