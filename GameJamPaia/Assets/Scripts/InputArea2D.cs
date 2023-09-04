using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputArea2D : MonoBehaviour, IHasActiveState
{
    [SerializeField]private InputManager inputManager;
    [SerializeField] private Collider2D colliderRef;
    [SerializeField] private string targetTag;
    private bool targetInArea;
    private bool interacting;

    public Action<bool> OnInputPerformed;

    private void Awake()
    {
        if (inputManager == null)
            inputManager = FindObjectOfType<InputManager>();

        if(colliderRef == null)
            colliderRef = GetComponent<Collider2D>();

        inputManager.OnInteract += Input_OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            targetInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            targetInArea = false;
            interacting = false;
            OnInputPerformed?.Invoke(false);
        }
    }

    private void Input_OnInteract(bool value)
    {
        if (!targetInArea) return;

        interacting = value;

        if (interacting)
        {
            OnInputPerformed?.Invoke(true);
        }
        else
        {
            OnInputPerformed?.Invoke(false);
        }
        
    }

    public void Disable()
    {
        colliderRef.enabled = false;
        interacting = false;
        targetInArea = false;
    }

    public void Enable()
    {
        colliderRef.enabled = true;
    }

    private void OnDestroy()
    {
        inputManager.OnInteract -= Input_OnInteract;
    }
}
