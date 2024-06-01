
using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class LockRoomTrap : MonoBehaviour
{
    [SerializeField] private bool startActive;
    [SerializeField] private bool drawGizmos;
    [SerializeField] private string targetTag;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Room roomRef;

    [Header("Trap")]
    [SerializeField] private OnTriggerEnter2DSignal[] trap;
    [SerializeField] private Renderer[] trapRenderer;
    [SerializeField] private Collider2D[] trapCollider;
    [SerializeField] private bool useRandomTrapPosition;
    [SerializeField] private Transform trapReandomAreaStartPoint;
    [SerializeField] private Vector2 trapRandomArea;

    [Header("DoorBlock")]
    [SerializeField] private GameObject doorBlockPrefab;

    [Header("Trap deactive")]
    [SerializeField] private GameObject trapDeactivatorPrefab;
    [SerializeField] private int deactivatorCount;
    [SerializeField] private Transform deactivatorSpawnStartPoint;
    [SerializeField] private Vector2 deactivatorSpawnArea;

    [Header("Active Trap Again")]
    [SerializeField] private bool autoResetTrap;
    [SerializeField] private bool useDelayToReset;
    [SerializeField] private float resetDelay;

    enum TrapState { DISABLED, ACTIVE, TRIGGERED, WAITINGDELAY };

    private TrapState currentTrapState;

    private Transform[] doorsBlocks;
    private Transform[] deactivators;

    private Vector2 deactivatorArea;
    private Vector2 trapsArea;

    private int currentDeactivatorsTriggeredNumber = 0;

    public Action OnTrapTriggered;
    public Action OnTrapDisabled;

    private void Awake()
    {
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        for (int i = 0; i < trap.Length; i++)
            trap[i].OnSendSignal += TriggeredTrap;

        deactivatorArea = deactivatorSpawnArea / 2;

        trapsArea = trapRandomArea / 2;
    }

    private void Start()
    {
        if (roomRef == null)
            roomRef = mapManager.GetRoomOfPosition(transform.position);


        RoomDoorsInfo[] doors = roomRef.roomDoors;

        doorsBlocks = new Transform[doors.Length];

        for (int i = 0; i < doors.Length; i++)
        {
            Vector2 pos = doors[i].doorRef.transform.position;

            doorsBlocks[i] = Instantiate(doorBlockPrefab, pos, Quaternion.identity).transform;

            doorsBlocks[i].gameObject.SetActive(false);
        }

        deactivators = new Transform[deactivatorCount];

        for (int i = 0; i < deactivatorCount; ++i)
        {
            Transform obj = Instantiate(trapDeactivatorPrefab, Vector2.zero, Quaternion.identity).transform;

            obj.GetComponent<OnTriggerEnter2DSignal>().OnSendSignal += DeactivatorSignal;

            deactivators[i] = obj;
        }

        if (startActive)
            TrapsActiveState(true);
        else
            TrapsActiveState(false);

    }

    public void TryEnableTrap()
    {
        if (currentTrapState == TrapState.DISABLED)
        {
            TrapsActiveState(true);
        }
    }

    public void TryDisableTrap()
    {
        if (currentTrapState == TrapState.ACTIVE)
        {
            TrapsActiveState(false);
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
                doorsBlocks[i].gameObject.SetActive(false);
            }

            currentTrapState = TrapState.DISABLED;

            OnTrapDisabled?.Invoke();

            if (autoResetTrap)
            {
                if (useDelayToReset)
                {
                    StartCoroutine(ResetDelay());
                }
                else
                {
                    TrapsActiveState(true);
                }
            }
        }
    }

    private void TriggeredTrap(OnTriggerEnter2DSignal signal)
    {
        for (int i = 0; i < doorsBlocks.Length; i++)
        {
            doorsBlocks[i].gameObject.SetActive(true);
        }

        TrapsActiveState(false);

        float areaX = deactivatorArea.x;
        float areaY = deactivatorArea.y;

        for (int i = 0; i < deactivatorCount; i++)
        {
            Vector3 pos = deactivatorSpawnStartPoint.position + (new Vector3(Random.Range(-areaX, areaX), Random.Range(-areaY, areaY), 0));

            deactivators[i].transform.position = pos;
            deactivators[i].gameObject.SetActive(true);
        }

        currentDeactivatorsTriggeredNumber = 0;

        currentTrapState = TrapState.TRIGGERED;

        OnTrapTriggered?.Invoke();

    }


    IEnumerator ResetDelay()
    {
        yield return new WaitForSeconds(resetDelay);
        TrapsActiveState(true);
    }

    private void TrapsActiveState(bool isActive)
    {
        for (int i = 0; i < trapRenderer.Length; i++)
            trapRenderer[i].enabled = isActive;


        for (int i = 0; i < trapCollider.Length; i++)
            trapCollider[i].enabled = isActive;

        if (isActive)
            currentTrapState = TrapState.ACTIVE;
        else
            currentTrapState = TrapState.DISABLED;

        if (isActive && useRandomTrapPosition)
            RandomizeTrapsPositions();
    }

    private void RandomizeTrapsPositions()
    {
        float areaX = trapsArea.x;
        float areaY = trapsArea.y;

        for (int i = 0; i < trap.Length; i++)
        {
            trap[i].transform.position = trapReandomAreaStartPoint.position + new Vector3(Random.Range(-areaX, areaX), Random.Range(-areaY, areaY), 0);
        }

    }


    public void UpdateBlockDoorsPositions()
    {
        roomRef = mapManager.GetRoomOfPosition(transform.position);
        RoomDoorsInfo[] doors = roomRef.roomDoors;

        for (int i = 0; i < doors.Length; i++)
        {
            Vector2 pos = doors[i].doorRef.transform.position;

            doorsBlocks[i].transform.position = pos;
        }

    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (deactivatorSpawnStartPoint)
            {
                Gizmos.DrawWireCube(deactivatorSpawnStartPoint.position, deactivatorSpawnArea);
            }

            if (useRandomTrapPosition)
            {
                if (trapReandomAreaStartPoint)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(trapReandomAreaStartPoint.position, trapRandomArea);
                }
            }
        }
    }


}
