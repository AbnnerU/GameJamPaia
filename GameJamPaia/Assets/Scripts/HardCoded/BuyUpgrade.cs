
using UnityEngine;

public class BuyUpgrade : MonoBehaviour
{
    [SerializeField] private InputArea2D inputArea;
    [SerializeField] private CoinsManager coinsManager;
    [SerializeField]private UpgradeBase upgradeRef;
    [SerializeField] private int price;

    private void Awake()
    {
        if(coinsManager ==null)
            coinsManager = FindObjectOfType<CoinsManager>();

        inputArea.OnInteract += TryBuyUpgrade;
    }

    private void TryBuyUpgrade()
    {
        bool success = false;

        coinsManager.TryUseCoins(price, out success);

        if (success)
        {
            print("Buy success");
            upgradeRef.ApplyUpgrade();
        }
        else
        {
            print("Buy fail");
        }
    }
}
