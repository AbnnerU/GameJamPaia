using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class HealthUnityEvent : UnityEvent { }

#region Main
public abstract class HealthBasicsEvents : HealthEvents
{ 
}

public abstract class HealthBasics : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected bool isHittable;

    protected float currentHealth;

    protected bool isAlive=true;

    public void IsHittable(bool canHit)
    {
        this.isHittable = canHit;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}

public abstract class HealthEvents : HealthBasics
{
    public Action OnDeath;
    public Action OnHitted;
    public Action OnHeal;
    public Action OnRespawn;
}

#endregion


#region AdditionalClasses
public abstract class OnDeathAction : MonoBehaviour
{
    [SerializeField] protected bool executeAutomatically;
    [SerializeField] protected HealthEvents healthEventsRef;

    protected virtual void Awake()
    {
        if (healthEventsRef == null)
            TryGetComponent<HealthEvents>(out healthEventsRef);


        if (healthEventsRef)
        {
            healthEventsRef.OnDeath += HealthEvents_OnDeath;
        }
    }

    protected abstract void HealthEvents_OnDeath();

    public void ExecuteDeathAction()
    {
        executeAutomatically = true;
        HealthEvents_OnDeath();       
    }
}

public abstract class LifeCicleActions : OnDeathAction
{
    protected override void Awake()
    {
        if (healthEventsRef == null)
            TryGetComponent<HealthEvents>(out healthEventsRef);


        if (healthEventsRef)
        {
            healthEventsRef.OnDeath += HealthEvents_OnDeath;
            healthEventsRef.OnRespawn += HealthEvents_OnRespawn;
        }
    }
    protected abstract void HealthEvents_OnRespawn();

    public void ExecuteRespawnAction()
    {
        HealthEvents_OnRespawn();
    }
}
#endregion


#region Interface
public interface IHittable
{
    void ResetHealth();

    void OnHit(float damage);

    void WhenKilled();
}

public interface IHealable
{
    void OnHeal(int healValue);
}
#endregion