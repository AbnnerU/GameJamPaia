using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TeleportArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameManager gameManager;
    //[SerializeField] private float teleportDelay;
    // [SerializeField] private string teleportPlayerAnimation;
    // [SerializeField] private bool disableCollider;


    // Start is called before the first frame update
    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (mapManager == null)
            mapManager = FindAnyObjectByType<MapManager>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
           // StartCoroutine(Teleport(collision.transform));
           Teleport(collision.transform);
        }
    }

    private void Teleport(Transform reference)
    {
        int id = 0;

        mapManager.RandomRoomNoRepeatAndAlarmOff(reference, Vector3.zero, out id);
        mapManager.SetObjectInRoom(cameraTransform, new Vector3(0, 0, -10), id);

    }

    //IEnumerator Teleport(Transform reference)
    //{
    //    int id = 0;

    //    mapManager.RandomRoomNoRepeatAndAlarmOff(reference, Vector3.zero, out id);
    //    mapManager.SetObjectInRoom(cameraTransform, new Vector3(0, 0, -10), id);

    //    yield return new WaitForSeconds(teleportDelay);

    //    gameManager.PauseAlarmsLoop(false);


    //}














}
