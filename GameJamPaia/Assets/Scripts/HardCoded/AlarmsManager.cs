using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlarmsManager : MonoBehaviour
{
    [SerializeField] private Alarm[] alarms;
    private List<int> alarmsIndexAvailable;

    private int alarmsOn = 0;

    public Action<int> OnUpdateAlarmsOnCount;

    public Action OnNewAlarmOn;
    public Action OnAlarmDisabled;
    public Action OnAlarmDisableCancel;
    public Action OnAlarmDisableStart;

    private void Awake()
    {
        alarmsIndexAvailable = new List<int>(alarms.Length);

        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i].AlarmInputCompleted += OnAlarmInputCompleted;
            alarms[i].AlarmInputStarted += OnAlarmInputStarted;
            alarms[i].AlarmInputCancel += OnAlarmInputCancel;

            alarmsIndexAvailable.Add(i);
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

    private void OnAlarmInputCompleted(Alarm alarmRef)
    {
        alarmRef.DisableAlarm();

        alarmsIndexAvailable.Add(GetAlarmIndex(alarmRef));

        alarmsOn--;

        OnUpdateAlarmsOnCount?.Invoke(alarmsOn);

        OnAlarmDisabled?.Invoke();
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
        if(alarmsIndexAvailable.Count > 0) 
            EnableAlarmAt(Random.Range(0, alarmsIndexAvailable.Count));
    }


    public void EnableAlarmAt(int index)
    {
        alarms[alarmsIndexAvailable[index]].EnableAlarm();

        alarmsIndexAvailable.RemoveAt(index);

        alarmsOn++;

        OnUpdateAlarmsOnCount?.Invoke(alarmsOn);

        OnNewAlarmOn?.Invoke();
    }

    public int AlarmsOnValue()
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
        int indexRef = GetAlarmIndex(alarmRef);

        if (indexRef >= 0)
        {
            for (int i = 0; i < alarmsIndexAvailable.Count; i++)
            {
                if (alarmsIndexAvailable[i] == indexRef)
                    return i;
            }
            return -1;
        }

        return -1;
    }


}
