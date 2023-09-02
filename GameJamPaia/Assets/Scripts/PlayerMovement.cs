using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active=true;
    [SerializeField] private InputManager input;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;

    private Vector2 inputDirection;

    private void Awake()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();

        input.OnMoveInput += Input_OnMoveInput;
    }


    private void Input_OnMoveInput(Vector2 inputValue)
    {
        inputDirection = inputValue;

        inputDirection.Normalize();
    }

    private void FixedUpdate()
    {
        if(!active) return;

        Move();
    }

    private void Move()
    {
       rb.velocity = inputDirection * (speed * Time.deltaTime);
    }

    public void Disable()
    {
        active = false;
        rb.velocity = Vector2.zero;
    }

    public void Enable()
    {
        active = true;
    }
}
