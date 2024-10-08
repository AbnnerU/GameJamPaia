
using System;
using UnityEngine;
using TMPro;

public class BuyUpgrade : MonoBehaviour
{
    [SerializeField] private InputArea2D inputArea;
    [SerializeField] private CoinsManager coinsManager;
    [SerializeField]private UpgradeBase upgradeRef;
    [SerializeField] private int price;
    [SerializeField] private int maximumPurchases=1;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private Transform positionReference;

    [Header("UiAnimation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName;

    [Header("Upgrade Animator")]
    [SerializeField] private Animator upgradeAnimator;
    [SerializeField] private string onReachMaximumPurchaseAnimation;

    [Header("Canvas")]
    [SerializeField] private Canvas priceCanvas;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject interactCanvas;
    [SerializeField] private SpriteRenderer enterInAreaFeedback;

    public Action OnBuyed;

    private int currentPurschase = 0;

    private void Awake()
    {
        if(coinsManager ==null)
            coinsManager = FindObjectOfType<CoinsManager>();

        inputArea.OnInteract += TryBuyUpgrade;
        inputArea.OnTargetEnterInArea += ShowInteractCanvas;
        priceCanvas.enabled = true;
        interactCanvas.SetActive(false);
        enterInAreaFeedback.enabled = false;
        priceText.text = price.ToString();
    }

    private void ShowInteractCanvas(bool show)
    {
        interactCanvas.SetActive(show);
        enterInAreaFeedback.enabled = show;
    }

    public void TryBuyUpgrade()
    {
        if (currentPurschase >= maximumPurchases) return;

        bool success = false;

        coinsManager.TryUseCoins(price, out success);

        if (success)
        {
            print("Buy success");
            upgradeRef.ApplyUpgrade();

            currentPurschase++;

            PlaySound();
            PlayAnimation();

            if (currentPurschase >= maximumPurchases)
            {
                upgradeAnimator.Play(onReachMaximumPurchaseAnimation, 0, 0);
                priceCanvas.enabled = false;
                interactCanvas.SetActive(false);
                enterInAreaFeedback.enabled = false;
                inputArea.OnInteract -= TryBuyUpgrade;
                inputArea.OnTargetEnterInArea -= ShowInteractCanvas;
            }

            OnBuyed?.Invoke();
        }
        else
        {
            print("Buy fail");
        }
    }

    public void PlaySound()
    {
        if (positionReference)
            channel.AudioRequest(audioConfig, positionReference.position);
        else
            channel.AudioRequest(audioConfig, Vector3.zero);
    }

    public void PlayAnimation()
    {
        animator.Play(animationName, 0, 0);
    }
}
