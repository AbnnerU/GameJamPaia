using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : HoldTime, IHasActiveState
{
  
    [SerializeField] private bool canBeActive = true;

    [SerializeField] private OnTriggerSignal2D triggerArea;
    //[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator anim;
    [SerializeField] private MultiSoundRequest soundRequest;

    [Header("Progress Bar")]
    //[SerializeField] private float holdTime;
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
    [SerializeField] private int onEnableSoundId;

    [Header("On Disable")]
    //[SerializeField]private Color onDisableColor;
    [SerializeField] private string onDisableAnimation;
    [SerializeField] private int onDisableSoundId;

    public Action<Alarm> AlarmInputCompleted;

    public Action<Alarm> AlarmInputStarted;

    public Action<Alarm> AlarmInputCancel;

    public Action<Alarm,bool> OnAlarmEnabled;

    private bool holding = false;

    private bool alarmOn = false;

    protected override void Awake()
    {
        base.Awake();

        if (findScoreByTag)
        {
            scoreRef = GameObject.FindGameObjectWithTag(scoreTag).GetComponent<GameScore>();
        }

        triggerArea.OnTargetInArea += TargetInArea;

        progressCnavas.enabled = false;
        progressFill.fillAmount = 0;
    }

    private void TargetInArea(bool isTargetInsideArea)
    {
        if (!canBeActive) return;

        if (isTargetInsideArea && holding==false)
        {
            holding = true;

            AlarmInputStarted?.Invoke(this);

            StartCoroutine(HoldTime());
        }
        else if(!isTargetInsideArea && holding == true)
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
        if (!canBeActive) return;

        OnAlarmEnabled?.Invoke(this,true) ;
        //sprite.color = onEnableColor;
        triggerArea.Enable();
        soundRequest.ExecuteRequest(onEnableSoundId);
        holding = false;
       
        anim.Play(onEnableAnimation, 0, 0);

        alarmOn = true;
    }

    public void DisableAlarm()
    {
        if (!canBeActive) return;

        OnAlarmEnabled?.Invoke(this,false);
        //sprite.color = onDisableColor;   
        triggerArea.Disable();
        progressCnavas.enabled = false;
        soundRequest.ExecuteRequest(onDisableSoundId);
        soundRequest.StopRequest(onEnableSoundId);
        holding = false;


        anim.Play(onDisableAnimation, 0, 0);

        alarmOn = false;    
    }

    public void DisableAlarmSound()
    {
        soundRequest.StopRequest(onEnableSoundId);
    }


    public void DisableAlarmWithoutAnimation()
    {
        OnAlarmEnabled?.Invoke(this, false);
        //sprite.color = onDisableColor;   
        triggerArea.Disable();
        progressCnavas.enabled = false;
        holding = false;

        alarmOn = false;
    }

    public bool AlarmOn()
    {
        return alarmOn;
    }

    public bool CanBeActive()
    {
        return canBeActive;
    }

    public void Disable()
    {
        canBeActive= false;
    }

    public void Enable()
    {
        canBeActive = true;
    }
}
