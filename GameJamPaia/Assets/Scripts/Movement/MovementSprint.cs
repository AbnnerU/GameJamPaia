
using System;
using System.Collections;

using UnityEngine;

public class MovementSprint : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InputManager inputs;
    [SerializeField] private bool percentage;
    [SerializeField] private float newSpeedValue;
    [SerializeField] private float sprintDuration;
    [SerializeField] private float sprintRechargeTime;

    public Action<float> WhenItGoesIntoRechargeTime;

    public Action<bool> OnSprintActive;

    private bool canUse = true;
    private void Awake()
    {
        inputs.OnSprint += Input_OnSprint;
    }

    private void Input_OnSprint(bool pressed)
    {
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
            float current = playerMovement.GetSpeed();
            float percentage = newSpeedValue / 100;
            current = current * percentage;

            playerMovement.SetNewSpeed(current);
        }
        else
        {
            playerMovement.SetNewSpeed(newSpeedValue);
        }

        OnSprintActive?.Invoke(true);

        float currentTime = 0;

        if (percentage)
        {
            float current = playerMovement.GetSpeed();
            //float percentage = newSpeedValue / 100;
            current = current * (1f + (newSpeedValue / 100f));

            playerMovement.SetNewSpeed(current);
        }

        do
        {
            currentTime += Time.deltaTime;

            yield return null;

        } while (currentTime < sprintDuration);

        //yield return new WaitForSeconds(sprintDuration);

        OnSprintActive?.Invoke(false);

        playerMovement.SetDefaultSpeed();

        WhenItGoesIntoRechargeTime?.Invoke(sprintRechargeTime);


        yield return new WaitForSeconds(sprintRechargeTime);

        canUse = true;
    }

    private void OnDestroy()
    {
        inputs.OnSprint -= Input_OnSprint;
    }
}
