using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : HealthBasicsEvents, IHittable, IHealable
{
    [SerializeField] private bool releaseOnDeath;

    [SerializeField] private bool resetHealthOnEnable;

    private void Awake()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public void OnHit(float damage)
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

    void IHealable.OnHeal(int healValue)
    {
        if (isAlive)
        {
            currentHealth += healValue;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            OnHeal?.Invoke();
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
