using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour,IHasActiveState
{
    public bool roomActive;
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

    public void Disable()
    {
        if (!roomActive) return; 

        roomActive = false;

        UpdateColor();
    }

    public void Enable()
    {
        if(roomActive) return;

        roomActive = true;

        UpdateColor();
    }

    private void Start()
    {
        UpdateColor();
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
        for(int i=0;i<roomDoors.Length;i++)
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

}

[Serializable]
public struct RoomDoorsInfo
{
    public Door2D doorRef;
    public Image doorHudImageRef;
    public DoorDirection doorDirection;
}

public enum DoorDirection { UP, DOWN, RIGHT, LEFT };