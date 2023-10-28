using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementSprintVisual : MonoBehaviour
{
    [SerializeField] private MovementSprint movementSprint;
    [SerializeField] private Image sprintImage;
    [SerializeField] private Image rechargeImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color rechargingColor = Color.gray;

    private void Awake()
    {
        movementSprint.WhenItGoesIntoRechargeTime += MovementSprint_RechargeTime;

        movementSprint.OnSprintActive += MovementSprint_OnSprintActive;

        rechargeImage.enabled = false;
        rechargeImage.fillAmount = 1;

        sprintImage.color = normalColor;    
    }

    private void MovementSprint_OnSprintActive(bool active)
    {
        sprintImage.color = activeColor;
    }

    private void MovementSprint_RechargeTime(float rechargeTime)
    {
        StartCoroutine(SprintRechargeTime(rechargeTime));
    }

    IEnumerator SprintRechargeTime(float rechageTime)
    {
        sprintImage.color = rechargingColor;

        float currentTime = 0;
        float percentage = 0;
        rechargeImage.enabled = true;
        rechargeImage.fillAmount = 1;
        do
        {
            currentTime += Time.deltaTime;
           
            percentage = ((currentTime * 100) / rechageTime)/100;

            print("Current Time:" + currentTime);
            print("Percentage: " + percentage);

            rechargeImage.fillAmount = 1 - percentage;

            yield return null;
        } while (currentTime < rechageTime);


        rechargeImage.enabled = false;

        sprintImage.color = normalColor;
        yield break;
    }
}
