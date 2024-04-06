
using System;
using System.Collections;
using UnityEngine;

public class StuckArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private int clicksToRelease;
    [SerializeField] private InputManager input;
    [SerializeField] private bool canStuck=true;
    [SerializeField] private float delayToStuckAgain;

    private NegativeEffects targetNegativeEffects;

    private void Awake()
    {
        if(input == null)
        {
            input = FindObjectOfType<InputManager>();
        }

        input.OnInteract += TryRelease;
    }

    IEnumerator DelayToStuckSameTargetAgain()
    {
        canStuck = false;
        yield return new WaitForSeconds(delayToStuckAgain);
        canStuck = true ;

    }

    private void TryRelease(bool clicked)
    {
        if (clicked)
        {
            if (targetNegativeEffects)
            {
                targetNegativeEffects.StuckEffect_TryRelease();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canStuck) return;

        if (collision.CompareTag(targetTag))
        {
            if (targetNegativeEffects != null) return;

            NegativeEffects ne = collision.GetComponent<NegativeEffects>();

            if (ne)
            {
                targetNegativeEffects = ne;

                targetNegativeEffects.Stuck(clicksToRelease);

                targetNegativeEffects.OnStuckEffectEnd += StuckEffectEnded;
            }
        }
    }

    private void StuckEffectEnded(NegativeEffects effects)
    {
        if(targetNegativeEffects == effects)
        {
            StartCoroutine(DelayToStuckSameTargetAgain());

            targetNegativeEffects.OnStuckEffectEnd -= StuckEffectEnded;

            targetNegativeEffects = null;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            NegativeEffects ne = collision.GetComponent<NegativeEffects>();

            if (targetNegativeEffects == ne)
            {
                targetNegativeEffects.OnStuckEffectEnd -= StuckEffectEnded;
                targetNegativeEffects = null;
            }
        }
    }

}
