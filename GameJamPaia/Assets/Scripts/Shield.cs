using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active=false;
    [SerializeField] private SpriteRenderer shieldRender;
    [SerializeField] private float rechargeTime;
    [SerializeField] private ParticleSystem shieldDestroyedEffect;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private Transform positionReference;


    public Action<float> OnShieldEnterInRecharge;

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

        shieldDestroyedEffect.Play();

        ExecuteSound();
    }

    IEnumerator RechargeShield()
    {
        OnShieldEnterInRecharge?.Invoke(rechargeTime);

        yield return new WaitForSeconds(rechargeTime);

        EnableShield();
    }

    public void ExecuteSound()
    {
        if (positionReference)
            channel.AudioRequest(audioConfig, positionReference.position);
        else
            channel.AudioRequest(audioConfig, Vector3.zero);
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
