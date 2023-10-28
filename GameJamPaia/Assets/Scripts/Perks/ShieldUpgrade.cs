using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShieldUpgrade : UpgradeBase
{
    [SerializeField] private Shield shieldRef;
    [SerializeField] private GameObject shieldIcon;
    [SerializeField] private Image rechargeImage;

    private void Awake()
    {
        if (shieldRef == null)
            shieldRef = FindObjectOfType<Shield>();

        shieldRef.OnShieldEnterInRecharge += Shield_ShieldEnterInRecharge;

        shieldIcon.SetActive(false);
        rechargeImage.enabled = false;
    }

    private void Shield_ShieldEnterInRecharge(float rechargeTime)
    {
        StartCoroutine(RechargeTime(rechargeTime));
    }

    IEnumerator RechargeTime(float rechageTime)
    {
        //sprintImage.color = rechargingColor;

        float currentTime = 0;
        float percentage = 0;
        rechargeImage.fillAmount = 1;
        rechargeImage.enabled = true;

        do
        {
            currentTime += Time.deltaTime;

            percentage = ((currentTime * 100) / rechageTime) / 100;

            print("Current Time:" + currentTime);
            print("Percentage: " + percentage);

            rechargeImage.fillAmount = 1 - percentage;

            yield return null;
        } while (currentTime < rechageTime);

        rechargeImage.fillAmount = 0;
        rechargeImage.enabled = false;
        yield break;
    }

    public override void ApplyUpgrade()
    {
        shieldRef.EnableShield();
        shieldIcon.SetActive(true);
    }
}
