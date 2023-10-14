
using UnityEngine;

public class ShieldUpgrade : UpgradeBase
{
    [SerializeField] private Shield shieldRef;

    private void Awake()
    {
        if (shieldRef == null)
            shieldRef = FindObjectOfType<Shield>();
    }

    public override void ApplyUpgrade()
    {
        shieldRef.EnableShield();
    }
}
