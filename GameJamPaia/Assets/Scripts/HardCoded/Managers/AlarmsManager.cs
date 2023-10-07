using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlarmsManager : MonoBehaviour
{
    [SerializeField] private bool setDisabledAlarmsAsNotAvailableIndex = true;
    [SerializeField] private Alarm[] alarms;
    private List<Alarm> disabledAlarms;
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
        disabledAlarms = new List<Alarm>();

        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i].AlarmInputCompleted += OnAlarmInputCompleted;
            alarms[i].AlarmInputStarted += OnAlarmInputStarted;
            alarms[i].AlarmInputCancel += OnAlarmInputCancel;

            alarmsIndexAvailable.Add(i);
        }


        if (setDisabledAlarmsAsNotAvailableIndex)
        {
            List<int> temp = new List<int>();

            for (int i = 0; i < alarmsIndexAvailable.Count; i++)
            {
                if (alarms[alarmsIndexAvailable[i]].IsEnabled() == false)
                {
                    temp.Add(i);
                    disabledAlarms.Add(alarms[alarmsIndexAvailable[i]]);
                }
            }

            for (int i = 0; i < temp.Count; i++)
            {
                alarmsIndexAvailable.Remove(temp[i]);
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

    private void OnAlarmInputCompleted(Alarm alarmRef)
    {
        alarmRef.DisableAlarm();

        alarmsIndexAvailable.Add(GetAlarmIndex(alarmRef));

        alarmsOn--;

        OnUpdateAlarmsOnCount?.Invoke(alarmsOn);

        OnAlarmDisabled?.Invoke();
    }

    public void TryEnableAlarm(Alarm alarm)
    {
        int id = GetAvailableAlarmIndex(alarm);

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

    public void EnableAllAlarms()
    {
        int index = 0;
        for (int i = 0; i < disabledAlarms.Count; i++)
        {
            index = GetAlarmIndex(disabledAlarms[i]);

            disabledAlarms[i].Enable();
            //disabledAlarms[i].UnlockDoorWhitoutActions();

            alarmsIndexAvailable.Add(index);
        }

        disabledAlarms.Clear();
        disabledAlarms.Capacity = 0;
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
