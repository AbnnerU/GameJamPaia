using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeEffects : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    public void Stun(float time)
    {
        StartCoroutine(StunTime(time));
    }

    public void CancelAll()
    {
        StopAllCoroutines();
    }

    IEnumerator StunTime(float time)
    {
        playerMovement.Disable();
        yield return new WaitForSeconds(time);
        playerMovement.Enable();
    }
}
