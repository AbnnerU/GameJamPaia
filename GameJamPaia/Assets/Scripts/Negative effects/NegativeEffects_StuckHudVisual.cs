
using UnityEngine;
using UnityEngine.UI;

public class NegativeEffects_StuckHudVisual : MonoBehaviour
{
    [SerializeField] private NegativeEffects negativeEffects;
    [SerializeField] private GameObject hudGroup;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        negativeEffects.OnStuckEffectStart += StuckEffectStarted;
        negativeEffects.StuckEffect_NewInput += StuckEffect_Input;
        negativeEffects.OnStuckEffectEnd += StuckEffectEnded;

        negativeEffects.OnCancelAll += StuckEffectEnded;

        slider.maxValue = 1;
        slider.value = 0;

        hudGroup.SetActive(false);
    }
    private void StuckEffectStarted(NegativeEffects effects)
    {

        hudGroup.SetActive(true);

        slider.value = 0;
    }

    private void StuckEffectEnded(NegativeEffects effects)
    {
        hudGroup.SetActive(false);

        slider.value = 0;
    }

    private void StuckEffect_Input(int targetValue, int currentValue)
    {
        float percentage = ((float)(currentValue * 100) / targetValue) / 100;

        print(percentage);
        slider.value = percentage;


    }


}
