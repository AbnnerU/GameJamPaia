using UnityEngine;

public abstract class InputScript : MonoBehaviour
{
    [SerializeField] protected InputType inputType;

    protected Vector2 inputDirection;

    public InputType GetInputType()
    {
        return inputType;
    }

    public Vector2 GetInputDirection()
    {
        return inputDirection;
    }
}


public enum InputType
{
    Vector,
    Quartenion
}
