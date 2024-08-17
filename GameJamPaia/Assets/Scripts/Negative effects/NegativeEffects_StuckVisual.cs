
using UnityEngine;

public class NegativeEffects_StuckVisual : MonoBehaviour
{
    [SerializeField] private NegativeEffects negativeEffects;
    [SerializeField] private ParticleSystem[] particles;
    // Start is called before the first frame update
    void Awake()
    {
        if (negativeEffects == null)
            negativeEffects = GetComponent<NegativeEffects>();

        negativeEffects.OnStuckEffectStart += StuckEffectStarted;
        negativeEffects.OnStuckEffectEnd += StuckEffectEnded;
        negativeEffects.OnCancelAll += StuckEffectEnded;
    }

    private void StuckEffectStarted(NegativeEffects effects)
    {
        for (int i = 0; i < particles.Length; i++)
            particles[i].Play();
    }

    private void StuckEffectEnded(NegativeEffects effects)
    {
        for (int i = 0; i < particles.Length; i++)
            particles[i].Stop();
    }
}
