
using UnityEngine;

public class TeleportAreaMusicChangeSignal : MonoBehaviour
{
    [SerializeField] private TeleportArea teleportArea;
    [SerializeField] private RoomsMusicManager roomsMusicManager;
    // Start is called before the first frame update
    void Start()
    {
        teleportArea.OnTeleport += OnTeleport;

        roomsMusicManager = FindObjectOfType<RoomsMusicManager>();  
    }

    private void OnTeleport()
    {
        roomsMusicManager.UpdateMusic();
    }
}
