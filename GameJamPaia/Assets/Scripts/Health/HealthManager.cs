using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : HealthBasicsEvents, IHittable
{
    [SerializeField] private bool releaseOnDeath;

    [SerializeField] private bool resetHealthOnEnable;

    private void Awake()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public void OnHit(int damage)
    {
        if (isHittable )
        {
            currentHealth -= damage;

            OnHitted?.Invoke();

            if (currentHealth <= 0 && isAlive)
            {
                currentHealth = 0;

                WhenKilled();
            }
        }
    }

    public void WhenKilled()
    {
        if (isAlive)
        {
            if (releaseOnDeath)
                PoolManager.ReleaseObject(gameObject);

            isAlive = false;

            OnDeath?.Invoke();
        }
    }

    public void ResetHealth()
    {
        isAlive = true;

        currentHealth = maxHealth;

        OnRespawn?.Invoke();
    }

    private void OnEnable()
    {
        if (resetHealthOnEnable && isAlive==false)
            ResetHealth();
    }
}
