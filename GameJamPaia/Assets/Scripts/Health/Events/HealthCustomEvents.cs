using System;
using UnityEngine;
using UnityEngine.Events;


public class HealthCustomEvents : MonoBehaviour
{
    [SerializeField] private HealthEvents healthEventsRef;

    [SerializeField] private HealthUnityEvent OnRespawned;

    [SerializeField] private HealthUnityEvent OnDeath;

    [SerializeField] private HealthUnityEvent OnRecibeDamage;

    [SerializeField] private HealthUnityEvent OnHeal;

    

    private void Awake()
    {
        if (healthEventsRef == null)
            TryGetComponent<HealthEvents>(out healthEventsRef);


        if (healthEventsRef)
        {
            healthEventsRef.OnDeath += HealthEvents_OnDeath;
            healthEventsRef.OnHitted += HealthEvents_OnHitted;
            healthEventsRef.OnHeal += HealthEvents_OnHeal;
            healthEventsRef.OnRespawn += HealthEvents_OnRespawn;

        }
    }

    private void HealthEvents_OnHeal()
    {
        OnHeal?.Invoke();
    }

    private void HealthEvents_OnHitted()
    {
        OnRecibeDamage?.Invoke();
    }

    private void HealthEvents_OnDeath()
    {
        OnDeath?.Invoke();
    }

    private void HealthEvents_OnRespawn()
    {
        OnRespawned?.Invoke();
    }

}
