using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeEffect_AutoRelease : MonoBehaviour
{
    [SerializeField] private NegativeEffects negativeEffects;
    [SerializeField] private float autoReleaseTickDelay;

    // Start is called before the first frame update
    void Awake()
    {
        if (negativeEffects == null)
            negativeEffects = GetComponent<NegativeEffects>();

        negativeEffects.OnStuckEffectStart += StuckEffectStarted;
        negativeEffects.OnStuckEffectEnd += StuckEffectEnded;
        negativeEffects.OnCancelAll += StuckEffectEnded;
    }

    IEnumerator ReleaseTick()
    {
        float currentTime = 0;
        while (true)
        {
            yield return null;
            currentTime += Time.deltaTime;

            if(currentTime >= autoReleaseTickDelay)
            {
                negativeEffects.StuckEffect_TryRelease();
                currentTime = 0;
            }
        }
    }

    private void StuckEffectStarted(NegativeEffects effects)
    {
       StartCoroutine(ReleaseTick());
    }

    private void StuckEffectEnded(NegativeEffects effects)
    {
        StopAllCoroutines();
    }
}
