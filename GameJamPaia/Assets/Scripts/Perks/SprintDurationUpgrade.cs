
using UnityEngine;

public class SprintDurationUpgrade : UpgradeBase
{
    [SerializeField] private MovementSprint movementSprint;
    [SerializeField] private float addDurationValue;

    public override void ApplyUpgrade()
    {
        movementSprint.AddDuration(addDurationValue);
    }
}
