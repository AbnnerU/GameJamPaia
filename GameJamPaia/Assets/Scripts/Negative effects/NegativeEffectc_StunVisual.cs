
using UnityEngine;

public class NegativeEffectc_StunVisual : MonoBehaviour
{
    [SerializeField] private NegativeEffects negativeEffects;
    [SerializeField] private ParticleSystem particles;

    // Start is called before the first frame update
    private void Awake()
    {
        if(negativeEffects == null)
            negativeEffects = GetComponent<NegativeEffects>();

        negativeEffects.OnStunEffectStart += PlayStunParticle;
        negativeEffects.OnCancelAll += StopStunParticle;
    }

    private void StopStunParticle(NegativeEffects effects)
    {
        particles.Stop();
    }

    private void PlayStunParticle(NegativeEffects effects)
    {
        particles.Play();
    }
}
