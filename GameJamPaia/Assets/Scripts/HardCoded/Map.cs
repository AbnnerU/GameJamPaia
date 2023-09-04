using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] private AlarmsOnPoints[] alarmsOnPoints;

    [SerializeField] private MapDoorLocked[] mapDoorLocked;

    private void Awake()
    {
      
        for(int i=0; i<alarmsOnPoints.Length; i++)
        {
            alarmsOnPoints[i].alarmRef.OnAlarmEnabled += Alarms_OnChangeEnabledState;
            alarmsOnPoints[i].imageRef.enabled = false;
        }

        for(int i=0;i<mapDoorLocked.Length; i++)
        {
            mapDoorLocked[i].doorRef.OnLockDoor += Doors_OnLockDoor;
            mapDoorLocked[i].doorRef.OnUnlockDoor += Doors_OnUnlockDoor;

            mapDoorLocked[i].doorImage.enabled = false;
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

    private void Doors_OnUnlockDoor(Door2D d)
    {
        mapDoorLocked[GetDoorId(d)].doorImage.enabled = false;
    }

    private void Doors_OnLockDoor(Door2D d)
    {
        mapDoorLocked[GetDoorId(d)].doorImage.enabled = true;
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

    private int GetDoorId(Door2D doorRef)
    {
        for(int i = 0; i < mapDoorLocked.Length; i++)
        {
            if (mapDoorLocked[i].doorRef == doorRef) return i;
        }
        return -1;
    }


    //[System.Serializable]
    //public struct MapPositionConfig
    //{
    //    public OnTriggerEnter2DSignal signal;
    //    public RectTransform canvasPosition;
    //}

    [System.Serializable]
    public struct MapDoorLocked
    {
        public Image doorImage;
        public Door2D doorRef;
    }

    [System.Serializable]
    public struct AlarmsOnPoints
    {
        public Alarm alarmRef;
        public Image imageRef;
    }
}
