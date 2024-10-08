
using UnityEngine;

public class InteractionSpeedUpgrade : UpgradeBase
{
    [SerializeField]private MapManager mapManager;
    [SerializeField] private HoldTime[] holdTimeRef;

    [Range(0,100)]
    [SerializeField] private float interactionSpeedUpgradePercentage;

    [SerializeField] private float currentUpgradeValue = 0;

    private void Awake()
    {
        mapManager.OnMapSetupCompleted += MapSetupCompleted;
    }

    private void MapSetupCompleted()
    {
        holdTimeRef = FindObjectsOfType<HoldTime>();
    }

    public override void ApplyUpgrade()
    {
        currentUpgradeValue += interactionSpeedUpgradePercentage;

        if (currentUpgradeValue > 100)
            currentUpgradeValue = 100;

        float originalValue = 0;
        float newValue;

        for(int i=0; i < holdTimeRef.Length; i++)
        {
            originalValue = holdTimeRef[i].GetOriginalHoldTimeValue();

            newValue = originalValue - (originalValue * (currentUpgradeValue/100));

            print(newValue);

            holdTimeRef[i].UpdateHoldTimeValue(newValue);
        }

    }
}
