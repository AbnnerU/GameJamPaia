using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : MonoBehaviour
{
    [SerializeField] private InputArea2D inputArea;
    //[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator anim;

    [Header("Progress Bar")]
    [SerializeField] private float holdTime;
    [SerializeField] private Canvas progressCnavas;
    [SerializeField] private Image progressFill;

    [Header("Score")]
    [SerializeField] private GameScore scoreRef;
    [SerializeField] private bool findScoreByTag;
    [SerializeField] private string scoreTag = "Score";
    [SerializeField] private int onCompleteScoreValue;

    [Header("On Enable")]
    //[SerializeField] private Color onEnableColor;
    [SerializeField] private string onEnableAnimation;

    [Header("On Disable")]
    //[SerializeField]private Color onDisableColor;
    [SerializeField] private string onDisableAnimation;

    public Action<Alarm> AlarmInputCompleted;

    public Action<Alarm> AlarmInputStarted;

    public Action<Alarm> AlarmInputCancel;

    public Action<Alarm,bool> OnAlarmEnabled;

    private bool holding = false;

    private void Awake()
    {
        if (findScoreByTag)
        {
            scoreRef = GameObject.FindGameObjectWithTag(scoreTag).GetComponent<GameScore>();
        }

        inputArea.OnInputPerformed += OnInput;

        progressCnavas.enabled = false;
        progressFill.fillAmount = 0;
    }

    private void OnInput(bool pressed)
    {
        if (pressed && holding==false)
        {
            holding = true;

            AlarmInputStarted?.Invoke(this);

            StartCoroutine(HoldTime());
        }
        else if(!pressed && holding == true)
        {
            holding = false;
            progressCnavas.enabled = false;
            AlarmInputCancel?.Invoke(this);
            StopAllCoroutines();
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

        } while (currentTime<holdTime);

        scoreRef.AddPoints(onCompleteScoreValue);

        progressCnavas.enabled = false;
        AlarmInputCompleted?.Invoke(this);
        holding = false;
    }

    public void EnableAlarm()
    {
        OnAlarmEnabled?.Invoke(this,true) ;
        //sprite.color = onEnableColor;
        inputArea.Enable();
        holding = false;

        anim.Play(onEnableAnimation, 0, 0);
    }

    public void DisableAlarm()
    {
        OnAlarmEnabled?.Invoke(this,false);
        //sprite.color = onDisableColor;   
        inputArea.Disable();
        progressCnavas.enabled = false;
        holding = false;

        anim.Play(onDisableAnimation, 0, 0);
    }

}
