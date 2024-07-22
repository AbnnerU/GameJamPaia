using System;
using UnityEngine;

public class RoomsMusicManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SoundInterpolation soundInterpolation;
    [SerializeField] private EventSignal eventSignal;
    [SerializeField] private Door2D[] allDoors;
    [SerializeField] private Room[] allRooms;

    private Vector2 areaSize;

    private void Awake()
    {
        gameManager.OnSetupCompleted += GameManager_OnMapSetupCompleted; 
    }

    private void GameManager_OnMapSetupCompleted()
    {
        allDoors = mapManager.GetAllDoors().ToArray();

        areaSize = mapManager.GetAreaSize() / 2;

        eventSignal.OnSendSignal += UpdateMusic;

        for (int i = 0; i < allDoors.Length; i++)
        {
            allDoors[i].OnUpdatePositions += UpdateMusic;
        }

        UpdateMusic();
    }

    private void UpdateMusic()
    {
        Room room = GetCurrentPlayerRoom();

        if (room != null)
        {
            print("OIIII");
            print(room.GetRoomTrack());
            soundInterpolation.PlaySound(room.GetRoomTrack());
        }
    }

    private Room GetCurrentPlayerRoom()
    {
        Vector3 playerPosition = player.position;
        return mapManager.GetRoomOfPosition(playerPosition);
    }


    public void TurnOffMusic()
    {
        soundInterpolation.StopSound();
    }



}
