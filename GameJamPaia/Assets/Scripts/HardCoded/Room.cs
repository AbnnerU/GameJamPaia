using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class Room : MonoBehaviour, IHasActiveState
{
    public bool roomActive;
    [SerializeField] private RoomHud roomHud;
    [Header("Room")]
    public Transform roomRefCenter;
    public RectTransform roomHudRectRef;
    public Image roomHudImageRef;

    [Header("Track")]
    public AudioConfig roomTrack;

    [Header("Alarm")]
    public Alarm roomAlarm;
    public Image alarmHudImageRef;


    [Header("Coin")]
    public Image coinHudImageRef;

    [Header("PowerUp")]
    public Image powerUpHudImageRef;

    [Header("Doors")]
    public RoomDoorsInfo[] roomDoors;


    [SerializeField] private Color roomEnabledColor;
    [SerializeField] private Color roomDisableColor;

    private void Start()
    {
        for (int i = 0; i < roomDoors.Length; i++)
        {
            roomDoors[i].doorRef.OnLockDoor += Doors_OnDoorLock;
            roomDoors[i].doorRef.OnUnlockDoor += Doors_OnDoorUnlock;
        }

        roomAlarm.OnAlarmEnabled += Alarms_OnAlarmEnabledUpdate;
    }

    private void Doors_OnDoorLock(Door2D doorRef)
    {
        for (int i = 0; i < roomDoors.Length; i++)
        {
            if (roomDoors[i].doorRef == doorRef)
            {
                roomDoors[i].doorHudImageRef.enabled = true;
                break;
            }
        }
    }

    private void Doors_OnDoorUnlock(Door2D doorRef)
    {
        for (int i = 0; i < roomDoors.Length; i++)
        {
            if (roomDoors[i].doorRef == doorRef)
            {
                roomDoors[i].doorHudImageRef.enabled = false;
                break;
            }
        }
    }


    private void Alarms_OnAlarmEnabledUpdate(Alarm alarm, bool enabled)
    {
        alarmHudImageRef.enabled = enabled;
    }

    public void Disable()
    {
        if (!roomActive) return;

        roomActive = false;

       roomAlarm.DisableAlarmWithoutAnimation();
       roomAlarm.Disable();

        for (int i = 0; i < roomDoors.Length; i++)
        {
           roomDoors[i].doorRef.Disable();
        }

        UpdateColor();


    }

    public void Enable()
    {
        if (roomActive) return;

        roomActive = true;

        UpdateColor();
    }


    public void SetUpRoomHud(RoomHud hudRef)
    {
        roomHud = hudRef;

        roomHudImageRef = roomHud.GetRoomImage();
        alarmHudImageRef = roomHud.GetAlarmImage();
        coinHudImageRef = roomHud.GetCoinImage();
        powerUpHudImageRef = roomHud.GetPowerupImage();
        roomHudRectRef = roomHudImageRef.GetComponent<RectTransform>();

        for (int d = 0; d < roomDoors.Length; d++)
        {
            roomDoors[d].doorHudImageRef = roomHud.GetDoorImageOnDirection(roomDoors[d].doorDirection);
        }
    }


    private void UpdateColor()
    {
        if (roomActive)
            roomHudImageRef.color = roomEnabledColor;
        else
            roomHudImageRef.color = roomDisableColor;
    }


    public Door2D GetDoorByDirection(DoorDirection direction)
    {
        for (int i = 0; i < roomDoors.Length; i++)
        {
            if (roomDoors[i].doorDirection == direction)
                return roomDoors[i].doorRef;
        }

        return null;
    }

    public AudioConfig GetRoomTrack()
    {
        return roomTrack;
    }


    public void RemoveDoor(DoorDirection direction)
    {
        RoomDoorsInfo[] temp = new RoomDoorsInfo[roomDoors.Length - 1];
        Door2D r = null;

        int id = 0;

        for (int i = 0; i < roomDoors.Length; i++)
        {
            if (roomDoors[i].doorDirection != direction)
            {
                temp[id] = roomDoors[i];
                id++;
            }
            else
                r = roomDoors[i].doorRef;
        }

        Destroy(r.gameObject);

        roomDoors = temp;
    }

    public void DisableAllHuds()
    {
        //roomHudImageRef.enabled = false;
        alarmHudImageRef.enabled = false;
        powerUpHudImageRef.enabled = false;
        coinHudImageRef.enabled = false;

        for (int d = 0; d < roomDoors.Length; d++)
        {
            roomDoors[d].doorHudImageRef.enabled = false;
        }
    }

    public void DisableDoor(DoorDirection doorDirection)
    {
        for(int i=0; i < roomDoors.Length; i++)
        {
            if(doorDirection == roomDoors[i].doorDirection)
            {
                roomDoors[i].doorRef.Disable();
                break;
            }
        }
    }
}

[Serializable]
public struct RoomDoorsInfo
{
    public Door2D doorRef;
    public Image doorHudImageRef;
    public DoorDirection doorDirection;
}

public enum DoorDirection { UP, DOWN, RIGHT, LEFT };