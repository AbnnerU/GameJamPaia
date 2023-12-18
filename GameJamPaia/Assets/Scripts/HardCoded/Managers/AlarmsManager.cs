using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlarmsManager : MonoBehaviour
{
    [SerializeField] private bool setDisabledAlarmsAsNotAvailableIndex = true;
    [SerializeField] private Alarm[] alarms;
    private List<Alarm> disabledAlarms;
    private List<Alarm> alarmsAvailable;

    private int alarmsOn = 0;

    public Action<int> OnUpdateAlarmsOnCount;

    public Action OnNewAlarmOn;
    public Action OnAlarmDisabled;
    public Action OnAlarmDisableCancel;
    public Action OnAlarmDisableStart;

    private void Awake()
    {
        alarmsAvailable = new List<Alarm>(alarms.Length);
        disabledAlarms = new List<Alarm>();

        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i].AlarmInputCompleted += OnAlarmInputCompleted;
            alarms[i].AlarmInputStarted += OnAlarmInputStarted;
            alarms[i].AlarmInputCancel += OnAlarmInputCancel;

            alarmsAvailable.Add(alarms[i]);
        }


        if (setDisabledAlarmsAsNotAvailableIndex)
        {
            List<Alarm> temp = new List<Alarm>();

            for (int i = 0; i < alarmsAvailable.Count; i++)
            {
                if (alarmsAvailable[i].CanBeActive() == false)
                {
                    temp.Add(alarmsAvailable[i]);
                    disabledAlarms.Add(alarmsAvailable[i]);
                }
            }

            for (int i = 0; i < temp.Count; i++)
            {
                alarmsAvailable.Remove(temp[i]);
            }

        }

    }

    private void Start()
    {
        for (int i = 0; i < alarms.Length; i++)
            alarms[i].DisableAlarmWithoutAnimation();
    }

    public void DisableAllAlarms()
    {
        for (int i = 0; i < alarms.Length; i++)
            alarms[i].DisableAlarmWithoutAnimation();
    }

    public void DisableAllAlarmsSound()
    {
        for (int i = 0; i < alarms.Length; i++)
            alarms[i].DisableAlarmSound();
    }

    private void OnAlarmInputCompleted(Alarm alarmRef)
    {
        alarmRef.DisableAlarm();

        alarmsAvailable.Add(alarmRef);

        alarmsOn--;

        OnUpdateAlarmsOnCount?.Invoke(alarmsOn);

        OnAlarmDisabled?.Invoke();
    }

    public void TryEnableAlarm(Alarm alarm)
    {
        int id = alarmsAvailable.IndexOf(alarm);

        if (id >= 0)
        {
            EnableAlarmAt(id);
        }
    }

    private void OnAlarmInputCancel(Alarm alarm)
    {
        OnAlarmDisableCancel?.Invoke();
    }

    private void OnAlarmInputStarted(Alarm alarm)
    {
        OnAlarmDisableStart?.Invoke();
    }

    public void TryEnableRandomAlarm()
    {
        if (alarmsAvailable.Count > 0)
            EnableAlarmAt(Random.Range(0, alarmsAvailable.Count));
    }


    public void EnableAlarmAt(int index)
    {
        alarmsAvailable[index].EnableAlarm();

        alarmsAvailable.RemoveAt(index);

        alarmsOn++;

        OnUpdateAlarmsOnCount?.Invoke(alarmsOn);

        OnNewAlarmOn?.Invoke();
    }

    public void SetNewAlarmAvailable(Alarm alarm)
    {
        if (disabledAlarms.Contains(alarm))
            disabledAlarms.Remove(alarm);

        if (!alarmsAvailable.Contains(alarm))
            alarmsAvailable.Add(alarm);
        else
            Debug.LogWarning("For some reason, alarm ("+alarm+") is already available");


        //Enable edition
        alarm.Enable();
        alarm.DisableAlarmWithoutAnimation();
        
    }

    public int GetAlarmsActiveValue()
    {
        return alarmsOn;
    }


    private int GetAlarmIndex(Alarm alarmRef)
    {
        for (int i = 0; i < alarms.Length; i++)
        {
            if (alarms[i] == alarmRef)
                return i;
        }

        return -1;
    }

    public int GetAvailableAlarmIndex(Alarm alarmRef)
    {
        int indexRef = alarmsAvailable.IndexOf(alarmRef);


        if (indexRef >= 0)
            return indexRef;
        else
            return -1;
        
    }


}
