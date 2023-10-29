using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsMusicManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private SoundInterpolation soundInterpolation;
    [SerializeField] private EventSignal eventSignal;
    [SerializeField] private Door2D[] allDoors;
    [SerializeField] private Room[] allRooms;
    [SerializeField] private AudioConfig[] allAudiosClips;

    private RoomClip[] roomClips;
    private Vector2 areaSize;

    private void Awake()
    {
        roomClips = new RoomClip[allRooms.Length];

        areaSize = mapManager.GetAreaSize()/2;

        eventSignal.OnSendSignal += UpdateMusic;

        for(int i = 0; i < roomClips.Length; i++)
        {
            RoomClip r = new RoomClip();

            r.room = allRooms[i];
            r.audioConfig = allAudiosClips[i];

            roomClips[i] = r;   
        }

        for(int i=0; i<allDoors.Length; i++)
        {
            allDoors[i].OnUpdatePositions += UpdateMusic;
        }
    }

    private void Start()
    {
        UpdateMusic();
    }

    private void UpdateMusic()
    {
        Room currentRoom = GetCurrentPlayerRoom();

        if(currentRoom != null)
        {
            for(int i=0;i < roomClips.Length;i++)
            {
                if (roomClips[i].room == currentRoom)
                {
                    soundInterpolation.PlaySound(roomClips[i].audioConfig);
                    return;
                }
            }
        }
    }

    public void TurnOffMusic()
    {
        soundInterpolation.StopSound();
    }

    private Room GetCurrentPlayerRoom()
    {
        Vector3 playerPosition = player.position;
        for(int i=0; i<allRooms.Length; i++)
        {
            if (playerPosition.x < allRooms[i].roomRefCenter.position.x + areaSize.x &&
                playerPosition.x > allRooms[i].roomRefCenter.position.x - areaSize.x &&
                playerPosition.y < allRooms[i].roomRefCenter.position.y + areaSize.y && 
                playerPosition.y > allRooms[i].roomRefCenter.position.y - areaSize.y)
            {
                return allRooms[i];
            }
        }

        return null;
    }

    private struct RoomClip
    {
        public Room room;
        public AudioConfig audioConfig;
    }
   
}
