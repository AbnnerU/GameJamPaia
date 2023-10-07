using Assets.Scripts.GameAction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputAreaAction2D : HoldTime { 
    [SerializeField] private InputArea2D inputArea;
    //[SerializeField] private float holdTime;
    [SerializeField] private GameAction[] onInputAreaPerformed;
    [SerializeField] private GameAction[] onInputAreaCancel;

    [Header("Progress Bar")]
    [SerializeField] private Canvas progressCnavas;
    [SerializeField] private Image progressFill;

    protected override void Awake()
    {
        base.Awake();
        inputArea.OnInputPerformed += InputArea_OnInputPerformed;
    }

    private void InputArea_OnInputPerformed(bool value)
    {
        if (value)
        {
            if (holdTime == 0)
            {
                for (int i = 0; i < onInputAreaPerformed.Length; i++)
                {
                    onInputAreaPerformed[i].DoAction();
                }
            }
            else           
                StartCoroutine(HoldTime());
            
        }
        else
        {
            StopAllCoroutines();

            progressCnavas.enabled = false;
            progressFill.fillAmount = 0;

            for (int i = 0; i < onInputAreaCancel.Length; i++)
            {
                onInputAreaCancel[i].DoAction();    
            }
        }

    }


    IEnumerator HoldTime()
    {
        float currentTime = 0;
        float percentage = 0;

        progressCnavas.enabled = true;
        progressFill.fillAmount = 0;

        do
        {
            currentTime += Time.deltaTime;
            percentage = ((currentTime * 100) / holdTime) / 100;

            progressFill.fillAmount = percentage;

            yield return null;

        } while (currentTime < holdTime);

        progressCnavas.enabled = false;

        for (int i = 0; i < onInputAreaPerformed.Length; i++)
        {
            onInputAreaPerformed[i].DoAction();
        }
    }

    private void OnDestroy()
    {
        inputArea.OnInputPerformed -= InputArea_OnInputPerformed;
    }
}
