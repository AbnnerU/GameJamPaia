using System;
using System.Collections;
using UnityEngine;

public class NegativeEffects : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MovementSprint movementSprint;
   
    public Action<NegativeEffects> OnStuckEffectStart;
    public Action<NegativeEffects> OnStuckEffectEnd;
    public Action<int, int> StuckEffect_NewInput;

    public Action<NegativeEffects> OnCancelAll;
    private StuckData stuckData;

    public Action<NegativeEffects> OnStunEffectStart;

    public Action<bool> OnSlowEffectAppliedUpdate;

    public Action<bool> OnSlipperyEffectAppliedUpdate;

    public void Stuck(int clickAmountToRelease)
    {
        stuckData = new StuckData();

        stuckData.clickAmountToRelease = clickAmountToRelease;
        stuckData.currentValue = 0;
        stuckData.stucked = true;

        playerMovement.Disable();

        movementSprint.Disable();

        OnStuckEffectStart?.Invoke(this);
    }

    public void StuckEffect_TryRelease()
    {
        if (stuckData != null)
        {
            if (stuckData.stucked)
            {
                stuckData.currentValue += 1;

                StuckEffect_NewInput?.Invoke(stuckData.clickAmountToRelease, stuckData.currentValue);

                if (stuckData.currentValue >= stuckData.clickAmountToRelease)
                {
                    playerMovement.Enable();
                    movementSprint.Enable();

                    OnStuckEffectEnd?.Invoke(this);
                }
            }
        }
    }

    public void Stun(float time)
    {
        OnStunEffectStart?.Invoke(this);
        StartCoroutine(StunTime(time));
    }

    public void CancelAll()
    {
        if (stuckData!=null)
        {
            stuckData.stucked = false;
        }

        CancelSlippery();
        CancelSlow();

        StopAllCoroutines();

        OnCancelAll?.Invoke(this);
    }

    public void ApllySlow(float slowPercentage)
    {
        float current = playerMovement.GetMaxSpeed();
        float slow = current * (1f - (slowPercentage / 100f));

        playerMovement.SetNewMaxSpeed(slow);

        OnSlowEffectAppliedUpdate?.Invoke(true);
    }

    public void CancelSlow()
    {
        playerMovement.SetDefaultValues();

        OnSlowEffectAppliedUpdate?.Invoke(false);
    }

    public void ApllySlippery(float maxSpeed, float accelerationSpeed, float decelerationSpeed)
    {
        playerMovement.SetNewMovementValues(maxSpeed, accelerationSpeed, decelerationSpeed);

        OnSlipperyEffectAppliedUpdate?.Invoke(true);
    }

    public void CancelSlippery()
    {
        playerMovement.SetDefaultValues();

        OnSlipperyEffectAppliedUpdate?.Invoke(false);
    }


    IEnumerator StunTime(float time)
    {
        playerMovement.Disable();
        movementSprint.Disable();
        yield return new WaitForSeconds(time);
        playerMovement.Enable();
        movementSprint.Enable();
    }


    private class StuckData
    {
        public bool stucked;
        public int clickAmountToRelease;
        public int currentValue;
    } 

}


