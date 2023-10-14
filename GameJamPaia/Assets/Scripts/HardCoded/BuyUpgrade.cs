
using UnityEngine;

public class BuyUpgrade : MonoBehaviour
{
    [SerializeField] private InputArea2D inputArea;
    [SerializeField] private CoinsManager coinsManager;
    [SerializeField]private UpgradeBase upgradeRef;
    [SerializeField] private int price;
    [SerializeField] private int maximumPurchases=1;

    private int currentPurschase = 0;

    private void Awake()
    {
        if(coinsManager ==null)
            coinsManager = FindObjectOfType<CoinsManager>();

        inputArea.OnInteract += TryBuyUpgrade;
    }

    private void TryBuyUpgrade()
    {
        if (currentPurschase >= maximumPurchases) return;

        bool success = false;

        coinsManager.TryUseCoins(price, out success);

        if (success)
        {
            print("Buy success");
            upgradeRef.ApplyUpgrade();

            currentPurschase++;
        }
        else
        {
            print("Buy fail");
        }
    }
}
