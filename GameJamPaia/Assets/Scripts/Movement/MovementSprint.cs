
using System.Collections;

using UnityEngine;

public class MovementSprint : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InputManager inputs;
    [SerializeField] private float newSpeedValue;
    [SerializeField] private float sprintDuration;
    [SerializeField] private float sprintRechargeTime;

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


    IEnumerator SprintExecution()
    {
        canUse = false;

        playerMovement.SetNewSpeed(newSpeedValue);

        yield return new WaitForSeconds(sprintDuration);

        playerMovement.SetDefaultSpeed();

        yield return new WaitForSeconds(sprintRechargeTime);  
        
        canUse = true;
    }
}