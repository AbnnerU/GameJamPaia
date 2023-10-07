
using UnityEngine;

public class ShieldUpgrade : UpgradeBase
{
    [SerializeField] private Shield shieldRef;

    public override void ApplyUpgrade()
    {
        shieldRef.EnableShield();
    }
}
