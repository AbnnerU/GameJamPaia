using UnityEngine;

public class RoomsMusicManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private SoundInterpolation soundInterpolation;
    [SerializeField] private EventSignal eventSignal;
    [SerializeField] private Door2D[] allDoors;
    [SerializeField] private Room[] allRooms;

    private Vector2 areaSize;

    private void Awake()
    {
        areaSize = mapManager.GetAreaSize() / 2;

        eventSignal.OnSendSignal += UpdateMusic;

        for (int i = 0; i < allDoors.Length; i++)
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
        Room room = GetCurrentPlayerRoom();

        if (room != null)
        {
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
