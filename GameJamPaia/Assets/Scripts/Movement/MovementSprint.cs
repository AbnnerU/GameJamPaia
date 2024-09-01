
using System;
using System.Collections;

using UnityEngine;

public class MovementSprint : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active = true;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InputManager inputs;
    [SerializeField] private bool percentage;
    [SerializeField] private float newMaxSpeedValue;
    [SerializeField] private float newAccelerationValue;
    [SerializeField]private float newDecelerationValue;
    [SerializeField] private float sprintDuration;
    [SerializeField] private float sprintRechargeTime;

    public Action<float> WhenItGoesIntoRechargeTime;

    public Action<bool> OnSprintActive;

    private bool canUse = true;
    private void Awake()
    {
        inputs.OnInteract += Input_OnSprint;
    }

    private void Input_OnSprint(bool pressed)
    {
        if (!active) return;

        if (canUse && pressed)
        {
            StartCoroutine(SprintExecution());

        }
    }

    public void AddDuration(float addValue)
    {
        sprintDuration += addValue;
    }


    IEnumerator SprintExecution()
    {
        canUse = false;

        if (percentage)
        {
            float current = playerMovement.GetMaxSpeed();
            float percentage = newMaxSpeedValue / 100;
            current = current * percentage;

            playerMovement.SetNewMovementValues(current, newAccelerationValue, newDecelerationValue);
        }
        else
        {
            playerMovement.SetNewMovementValues(newMaxSpeedValue, newAccelerationValue, newDecelerationValue);
        }

        OnSprintActive?.Invoke(true);

        float currentTime = 0;

        do
        {
            currentTime += Time.deltaTime;

            yield return null;

        } while (currentTime < sprintDuration);

        //yield return new WaitForSeconds(sprintDuration);

        OnSprintActive?.Invoke(false);

        playerMovement.SetDefaultValues();

        WhenItGoesIntoRechargeTime?.Invoke(sprintRechargeTime);


        yield return new WaitForSeconds(sprintRechargeTime);

        canUse = true;
    }

    private void OnDestroy()
    {
        inputs.OnSprint -= Input_OnSprint;
    }

    public void Disable()
    {
       active = false;
    }

    public void Enable()
    {
        active = true;
    }
}
