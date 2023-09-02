using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarms : MonoBehaviour
{
    [SerializeField] private InputArea2D inputArea;
    [SerializeField] private SpriteRenderer sprite;

    [Header("On Enable")]
    [SerializeField] private Color onEnableColor;

    [Header("On Disable")]
    [SerializeField]private Color onDisableColor;

    public Action<Alarms> AlarmInput;

    private void Awake()
    {
        inputArea.OnInputPerformed += OnInput;
    }

    private void OnInput()
    {
        AlarmInput?.Invoke(this);
    }

    public void EnableAlarm()
    {
        sprite.color = onEnableColor;
        inputArea.Enable();
    }

    public void DisableAlarm()
    {
        sprite.color = onDisableColor;   
        inputArea.Disable();
    }

}
