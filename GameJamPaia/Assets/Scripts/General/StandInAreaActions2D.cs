using Assets.Scripts.GameAction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StandInAreaActions2D :HoldTime
{
    //[SerializeField] private InputManager inputManager;
    [SerializeField] private Collider2D colliderRef;
    [SerializeField] private string targetTag;
    [SerializeField] private GameAction[] onInputAreaPerformed;
    [SerializeField] private GameAction[] onInputAreaCancel;

    [Header("Progress Bar")]
    [SerializeField] private Canvas progressCnavas;
    [SerializeField] private Image progressFill;
    //private bool targetInArea;

    public Action<bool> OnInputPerformed;
    public Action OnInteract;

    private void Awake()
    { 
        if (colliderRef == null)
            colliderRef = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            TargetInArea(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            TargetInArea(false);
        }
    }

    private void TargetInArea(bool value)
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

    public void Disable()
    {
        colliderRef.enabled = false;
        StopAllCoroutines();
        progressCnavas.enabled = false;
        progressFill.fillAmount = 0;
    }

    public void Enable()
    {
        colliderRef.enabled = true;
    }

}
