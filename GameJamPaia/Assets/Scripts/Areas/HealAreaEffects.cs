
using UnityEngine;

public class HealAreaEffects : MonoBehaviour
{
    [SerializeField] private HealArea healArea;
    [SerializeField] private ParticleSystem onHealParticle;
    [SerializeField] private SpriteRenderer canUseSprite;

    private void Awake()
    {
        if (healArea == null)
            healArea = GetComponent<HealArea>();

        canUseSprite.enabled = true;

        healArea.OnHeal += OnHeal;
        healArea.OnReadyToUse += OnReadyToUse;
    }
    private void OnHeal()
    {
        onHealParticle.Play();
        canUseSprite.enabled = false;
    }

    private void OnReadyToUse()
    {
        canUseSprite.enabled = true;
    }
}
