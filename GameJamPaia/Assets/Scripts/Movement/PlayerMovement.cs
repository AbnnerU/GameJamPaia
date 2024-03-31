
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active = true;
    [SerializeField] private InputManager input;
    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private float speed;
    [SerializeField] private float slipFactor = 1f;

    [SerializeField] private float acceleration = 5f; // Aceleração do jogador
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float maxSpeed = 5f; // Velocidade máxima do jogador

    private Vector2 inputDirection;
    private float defaultMaxSpeed;
    private float defaultAcceleration;
    private float defaultDeceleration; 

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        input.OnMoveInput += Input_OnMoveInput;

        defaultMaxSpeed = maxSpeed;
        defaultAcceleration = acceleration;
        defaultDeceleration = deceleration;
    }


    private void Input_OnMoveInput(Vector2 inputValue)
    {
        inputDirection = inputValue;

        inputDirection.Normalize();
    }

    private void FixedUpdate()
    {
        if (!active) return;

        Move();
    }

    private void Move()
    {
        // Aplica aceleração ao jogador
        Vector2 accelerationVector = inputDirection * (acceleration * Time.deltaTime);
        rb.AddForce(accelerationVector, ForceMode2D.Force);

        // Limita a velocidade do jogador para a velocidade máxima
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Aplica desaceleração se nenhuma tecla de movimento estiver sendo pressionada
        if (inputDirection == Vector2.zero && rb.velocity.magnitude > 0.15)
        {
            Vector2 decelerationVector = -rb.velocity.normalized * (deceleration*Time.deltaTime);
            rb.AddForce(decelerationVector, ForceMode2D.Force);

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
