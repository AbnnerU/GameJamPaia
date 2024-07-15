
using UnityEngine;

public class BuyUpgradeFeedback : MonoBehaviour
{
    [SerializeField] private BuyUpgrade buyUpgrade;
    [SerializeField] private ParticleSystem particleToPlay;

    // Start is called before the first frame update
    void Awake()
    {
        buyUpgrade.OnBuyed += OnBuyedUpgrade;
    }

    private void OnBuyedUpgrade()
    {
        particleToPlay.Play();
    }

  
}
