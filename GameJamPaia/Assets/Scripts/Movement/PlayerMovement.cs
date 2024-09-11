
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active = true;
    //[SerializeField] private InputManager input;
    [SerializeField] private InputScript input;
    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private float speed;
    [SerializeField] private float slipFactor = 1f;

    [SerializeField] private float acceleration = 5f; // Aceleração do jogador
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float maxSpeed = 5f; // Velocidade máxima do jogador

    public Action<Vector2> OnInputValueChange;

    private Vector2 inputDirection;
    private float defaultMaxSpeed;
    private float defaultAcceleration;
    private float defaultDeceleration; 

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

       // input.OnMoveInput += Input_OnMoveInput;

        defaultMaxSpeed = maxSpeed;
        defaultAcceleration = acceleration;
        defaultDeceleration = deceleration;
    }


    //private void Input_OnMoveInput(Vector2 inputValue)
    //{
    //    inputDirection = inputValue;

    //    inputDirection.Normalize();
    //}

    private void FixedUpdate()
    {
        if (!active) return;

        Move();
    }

    private void Move()
    {
        // Aplica aceleração ao jogador
        inputDirection = input.GetInputDirection().normalized;

        OnInputValueChange?.Invoke(inputDirection);

        if (inputDirection != Vector2.zero)
        {
            Vector2 accelerationVector = inputDirection * (acceleration * Time.deltaTime);
            //rb.AddForce(accelerationVector, ForceMode2D.Force);
            rb.velocity = accelerationVector;
        }
        // Limita a velocidade do jogador para a velocidade máxima
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Aplica desaceleração se nenhuma tecla de movimento estiver sendo pressionada
        //print(rb.velocity.magnitude);
        if (inputDirection == Vector2.zero && rb.velocity.magnitude > 0.20)
        {
            Vector2 decelerationVector =rb.velocity - ( rb.velocity.normalized * ((deceleration/100) *Time.deltaTime));
            if (rb.velocity.magnitude < 0.20) decelerationVector = Vector2.zero;
            rb.velocity = decelerationVector;

            // Garante que a velocidade não se torne negativa devido à desaceleração
           
        }
        else if (inputDirection == Vector2.zero && rb.velocity.magnitude < 0.15)
        {
            rb.velocity = Vector2.zero;
        }


        // rb.velocity = inputDirection * (speed * slipFactor * Time.deltaTime);
    }

    public void SetDefaultValues()
    {
        maxSpeed = defaultMaxSpeed;
        acceleration = defaultAcceleration;
        deceleration = defaultDeceleration;
    }

    public float GetDefaultSpeed()
    {
        return defaultMaxSpeed;
    }

    public void SetNewMaxSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }

    public void SetNewMovementValues(float newMaxSpeedvalue, float newAcceleration, float newDeceleration)
    {
        maxSpeed = newMaxSpeedvalue;
        acceleration = newAcceleration;
        deceleration = newDeceleration;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
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
