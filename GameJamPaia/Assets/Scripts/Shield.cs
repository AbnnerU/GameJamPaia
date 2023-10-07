using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active=false;
    [SerializeField] private SpriteRenderer shieldRender;
    [SerializeField] private float rechargeTime;

    private void Awake()
    {
        if (active)
            EnableShield();
        else
            DisableShield();
    }

    public void EnableShield()
    {
        active= true;
        shieldRender.enabled = true;
    }
    
    public void DisableShield()
    {
        active= false;
        shieldRender.enabled = false;
    }

    public void HitShield()
    {
        if (!active) return;

        DisableShield();

        StartCoroutine(RechargeShield());
    }

    IEnumerator RechargeShield()
    {
        yield return new WaitForSeconds(rechargeTime);

        EnableShield();
    }

    public bool IsShieldActive()
    {
        return active;
    }

    public void Disable()
    {
        DisableShield() ;
    }

    public void Enable()
    {
        EnableShield();
    }
}
