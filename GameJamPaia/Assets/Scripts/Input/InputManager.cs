using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputController inputActions;
    public Action<Vector2> OnMoveInput;
    public Action<bool> OnInteract;
    public Action<bool> OnSprint;

    private void Awake()
    {
        inputActions = new InputController();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Gameplay.Move.performed += ctx => MovementInput(ctx.ReadValue<Vector2>());
        inputActions.Gameplay.Move.canceled += ctx => MovementInput(ctx.ReadValue<Vector2>());

        inputActions.Gameplay.Interact.performed += ctx => InteractInput(ctx.ReadValue<float>());
        inputActions.Gameplay.Interact.canceled+=ctx=> InteractInput(ctx.ReadValue<float>());

        inputActions.Gameplay.Sprint.performed += ctx => SprintInput(ctx.ReadValue<float>());
        inputActions.Gameplay.Sprint.canceled+= ctx => SprintInput(ctx.ReadValue<float>());
    }

    private void SprintInput(float pressed)
    {
        if (pressed > 0)
            OnSprint?.Invoke(true);
        else
            OnSprint?.Invoke(false);
    }

    private void InteractInput(float pressed)
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
