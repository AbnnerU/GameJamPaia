
using UnityEngine;

public class NegativeEffects_SlowVisual : MonoBehaviour
{
    [SerializeField] private NegativeEffects negativeEffects;
    [SerializeField] private ParticleSystem[] particles;
    // Start is called before the first frame update
    void Awake()
    {
        if (negativeEffects == null)
            negativeEffects = GetComponent<NegativeEffects>();

        negativeEffects.OnSlowEffectAppliedUpdate += Effect;
    }

    private void Effect(bool slowActive)
    {
        if (slowActive) {
            for (int i = 0; i < particles.Length; i++)
                particles[i].Play();
        }
        else
        {
            for (int i = 0; i < particles.Length; i++)
                particles[i].Stop();
        }
    }

}
