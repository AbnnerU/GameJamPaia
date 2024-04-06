using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class LockRoomTrap : MonoBehaviour
{
    [SerializeField] private bool drawGizmos;
    [SerializeField] private string targetTag;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Room roomRef;

    [Header("Trap")]
    [SerializeField] private OnTriggerEnter2DSignal trap;
    [SerializeField] private Renderer trapRenderer;
    [SerializeField] private Collider2D trapCollider;

    [Header("DoorBlock")]
    [SerializeField] private GameObject doorBlockPrefab;

    [Header("Trap deactive")]
    [SerializeField] private GameObject trapDeactivatorPrefab;
    [SerializeField] private int deactivatorCount;
    [SerializeField] private Transform deactivatorSpawnStartPoint;
    [SerializeField] private Vector2 deactivatorSpawnArea;


    private GameObject[] doorsBlocks;
    private GameObject[] deactivators;

    private Vector2 area;

    private int currentDeactivatorsTriggeredNumber = 0;

    private void Awake()
    {
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        trap.OnSendSignal += TriggeredTrap;

        area = deactivatorSpawnArea / 2;
    }

    private void Start()
    {
        if (roomRef == null)
            roomRef = mapManager.GetRoomOfPosition(transform.position);


        RoomDoorsInfo[] doors = roomRef.roomDoors;

        doorsBlocks = new GameObject[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            Vector2 pos = doors[i].doorRef.transform.position;

            doorsBlocks[i] = Instantiate(doorBlockPrefab, pos, Quaternion.identity);

            doorsBlocks[i].SetActive(false);
        }

        deactivators = new GameObject[deactivatorCount];

        for (int i = 0; i < deactivatorCount; ++i)
        {
            GameObject obj = Instantiate(trapDeactivatorPrefab, Vector2.zero, Quaternion.identity);

            obj.GetComponent<OnTriggerEnter2DSignal>().OnSendSignal += DeactivatorSignal;

            deactivators[i] = obj;
        }

    }

    private void DeactivatorSignal(OnTriggerEnter2DSignal signal)
    {
        signal.gameObject.SetActive(false);
        currentDeactivatorsTriggeredNumber++;

        if (currentDeactivatorsTriggeredNumber >= deactivatorCount)
        {
            for (int i = 0; i < doorsBlocks.Length; i++)
            {
                doorsBlocks[i].SetActive(false);
            }

            trapRenderer.enabled = true;
            trapCollider.enabled = true;
        }
    }

    private void TriggeredTrap(OnTriggerEnter2DSignal signal)
    {
        for (int i = 0; i < doorsBlocks.Length; i++)
        {
            doorsBlocks[i].SetActive(true);
        }

        trapRenderer.enabled = false;
        trapCollider.enabled = false;


        float areaX = area.x;
        float areaY = area.y;

        for (int i = 0; i < deactivatorCount; i++)
        {
            Vector3 pos = deactivatorSpawnStartPoint.position + (new Vector3(Random.Range(-areaX, areaX), Random.Range(-areaY, areaY), 0));

            deactivators[i].transform.position = pos;
            deactivators[i].SetActive(true);
        }

        currentDeactivatorsTriggeredNumber = 0;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (deactivatorSpawnStartPoint)
            {
                Gizmos.DrawWireCube(deactivatorSpawnStartPoint.position, deactivatorSpawnArea);
            }
        }
    }


}
