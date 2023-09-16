using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public bool roomActive;
    [Header("Room")]
    public Transform roomRefCenter;
    public RectTransform roomHudRectRef;
    public Image roomHudImageRef;

    [Header("Alarm")]
    public Alarm roomAlarm;
    public Image alarmHudImageRef;

    [Header("Doors")]
    public RoomDoorsInfo[] roomDoors;


    [SerializeField] private Color roomEnabledColor;
    [SerializeField] private Color roomDisableColor;


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
    

}

[Serializable]
public struct RoomDoorsInfo
{
    public Door2D doorRef;
    public Image doorHudImageRef;
}
