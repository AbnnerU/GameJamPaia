using System;
using System.Collections;
using UnityEngine;

public class NegativeEffects : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    public Action<NegativeEffects> OnStuckEffectStart;
    public Action<NegativeEffects> OnStuckEffectEnd;
    public Action<int, int> StuckEffect_NewInput;

    public Action<NegativeEffects> OnCancelAll;
    private StuckData stuckData;

    public void Stuck(int clickAmountToRelease)
    {
        stuckData = new StuckData();

        stuckData.clickAmountToRelease = clickAmountToRelease;
        stuckData.currentValue = 0;
        stuckData.stucked = true;

        playerMovement.Disable();

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

                    OnStuckEffectEnd?.Invoke(this);
                }
            }
        }
    }

    public void Stun(float time)
    {
        StartCoroutine(StunTime(time));
    }

    public void CancelAll()
    {
        if (stuckData!=null)
        {
            stuckData.stucked = false;
        }

        StopAllCoroutines();

        OnCancelAll?.Invoke(this);
    }

    IEnumerator StunTime(float time)
    {
        playerMovement.Disable();
        yield return new WaitForSeconds(time);
        playerMovement.Enable();
    }


    private class StuckData
    {
        public bool stucked;
        public int clickAmountToRelease;
        public int currentValue;
    } 

}

