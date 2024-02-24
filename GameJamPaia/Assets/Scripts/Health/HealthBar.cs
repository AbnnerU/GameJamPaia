
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthBasicsEvents healthBasics;
    [SerializeField] private Slider slider;


    private void Awake()
    {
        if(healthBasics == null )
            healthBasics = GetComponent<HealthBasicsEvents>();

        healthBasics.OnHitted += HealthEvents_OnNewValue;
        healthBasics.OnHeal += HealthEvents_OnNewValue;
        //healthBasics.OnDeath += HealthEvents_OnDeath;

        slider.maxValue = healthBasics.GetMaxHealth();
        slider.value = healthBasics.GetMaxHealth();

    }

    //private void HealthEvents_OnDeath()
    //{
    //    throw new NotImplementedException();
    //}

    private void HealthEvents_OnNewValue()
    {
        slider.value = healthBasics.GetCurrentHealth();
    }

}
