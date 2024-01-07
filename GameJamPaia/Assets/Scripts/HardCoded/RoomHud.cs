using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomHud : MonoBehaviour
{
    [SerializeField] private Image roomImage;
    [SerializeField] private Image alarmImage;
    [SerializeField] private Image coinImage;

    [SerializeField] private Image powerupImage;

    [SerializeField] private RoomDoorImage[] roomDoorsImages;


    [System.Serializable]
    private struct RoomDoorImage
    {
        public Image doorImage;
        public DoorDirection doorDirection;
    }

    public Image GetRoomImage()
    {
        return roomImage;
    }

    public Image GetAlarmImage() {  return alarmImage;}

    public Image GetCoinImage() {  return coinImage; }

    public Image GetPowerupImage() {  return powerupImage; }

    public Image GetDoorImageOnDirection(DoorDirection doorDirection)
    {
        for(int i=0; i <roomDoorsImages.Length; i++)
        {
            if (roomDoorsImages[i].doorDirection == doorDirection)
                return roomDoorsImages[i].doorImage;
        }

        return null;    
    }

}
