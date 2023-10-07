
using UnityEngine;

public abstract class HoldTime : MonoBehaviour
{
    [SerializeField] protected float holdTime;

    private float originalHoldTimeValue;

    protected virtual void Awake()
    {
        originalHoldTimeValue = holdTime;
    }

    public float GetOriginalHoldTimeValue()
    {
        return originalHoldTimeValue;
    }

    public float GetHoldTime()
    {
        return holdTime;
    }

    public void UpdateHoldTimeValue(float newValue)
    {
        holdTime = newValue;
    }
}
