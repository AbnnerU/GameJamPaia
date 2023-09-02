using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputController inputActions;
    public Action<Vector2> OnMoveInput;
    public Action<bool> OnInteract;

    private void Awake()
    {
        inputActions = new InputController();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Gameplay.Move.performed += ctx => MovementInput(ctx.ReadValue<Vector2>());
        inputActions.Gameplay.Move.canceled += ctx => MovementInput(ctx.ReadValue<Vector2>());

        inputActions.Gameplay.Interact.performed += ctx => Interact_performed(ctx.ReadValue<float>());
        inputActions.Gameplay.Interact.canceled+=ctx=> Interact_performed(ctx.ReadValue<float>());
    }

    private void Interact_performed(float pressed)
    {
        if (pressed > 0)
            OnInteract?.Invoke(true);
        else
            OnInteract?.Invoke(false);
    }

    private void MovementInput(Vector2 inputValue)
    {
        OnMoveInput?.Invoke(inputValue);
    }
}
