using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] private AlarmsOnPoints[] alarmsOnPoints;
    

    private void Awake()
    {
      
        for(int i=0; i<alarmsOnPoints.Length; i++)
        {
            alarmsOnPoints[i].alarmRef.OnAlarmEnabled += Alarms_OnChangeEnabledState;
            alarmsOnPoints[i].imageRef.enabled = false;
        }
    }

    private void Alarms_OnChangeEnabledState(Alarm alarms, bool enabled)
    {
        int id = GetMapAlarmId(alarms);

        if (id >= 0)
        {
            alarmsOnPoints[id].imageRef.enabled = enabled;

        }
    }

    private int GetMapAlarmId(Alarm alarms)
    {
        for(int i=0; i < alarmsOnPoints.Length; i++)
        {
            if (alarmsOnPoints[i].alarmRef == alarms)
                return i;
        }

        return -1;
    }


    [System.Serializable]
    public struct MapPositionConfig
    {
        public OnTriggerEnter2DSignal signal;
        public RectTransform canvasPosition;
    }

    [System.Serializable]
    public struct AlarmsOnPoints
    {
        public Alarm alarmRef;
        public Image imageRef;
    }
}
