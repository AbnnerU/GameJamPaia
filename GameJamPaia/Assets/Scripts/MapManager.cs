using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private bool active;
    [SerializeField] private bool tryAutoConfig;
    [SerializeField] private Vector2 areaSize;
    [SerializeField] private Room[] roomsInfo;
    [SerializeField] private AgentConfig[] enemyInfo;

    private RoomDoorsInfo[] roomsDoors;

    private void Awake()
    {
        int doorAmount=0;
        int id = 0;

        for (int i = 0; i < roomsInfo.Length; i++)
        {
            doorAmount += roomsInfo[i].roomDoors.Length;
        }

        roomsDoors = new RoomDoorsInfo[doorAmount];

        for (int i = 0; i < roomsInfo.Length; i++)
        {
            for (int y = 0; y < roomsInfo[i].roomDoors.Length; y++)
            {
                roomsDoors[id] = roomsInfo[i].roomDoors[y];

                id++;
            }
        }

        Alarm[] alarms = GameObject.FindObjectsOfType<Alarm>();

        foreach (var a in alarms)
            a.OnAlarmEnabled += Alarms_OnAlarmEnabledUpdate;

        foreach (var d in roomsDoors)
        {
            d.doorRef.OnLockDoor += Doors_OnDoorLock;
            d.doorRef.OnUnlockDoor += Doors_OnDoorUnluck;
        }

        DisableAllDoorsHud();
    }

    private void Doors_OnDoorUnluck(Door2D d)
    {
        for (int i = 0; i < roomsDoors.Length; i++)
        {
            if (roomsDoors[i].doorRef == d)
            {
                roomsDoors[i].doorHudImageRef.enabled = false;
                break;
            }
        }
    }

    private void Doors_OnDoorLock(Door2D d)
    {
        for (int i = 0; i < roomsDoors.Length; i++)
        {
            if (roomsDoors[i].doorRef == d)
            {
                roomsDoors[i].doorHudImageRef.enabled = true;
                break;
            }
        }
    }

    private void Alarms_OnAlarmEnabledUpdate(Alarm alarm, bool enabled)
    {
        for(int i=0; i< roomsInfo.Length; i++)
        {
            if (roomsInfo[i].roomAlarm == alarm)
            {
                roomsInfo[i].alarmHudImageRef.enabled = enabled;
                break;
            }
        }
    }

    private void DisableAllDoorsHud()
    {
        for(int i = 0; i < roomsInfo.Length; i++)
        {
            for (int y = 0; y < roomsInfo[i].roomDoors.Length; y++)
            {
                roomsInfo[i].roomDoors[y].doorHudImageRef.enabled = false;
            }     
        }
    }

    private void DisableAllDoorsAndAlarmsHud()
    {
        for (int i = 0; i < roomsInfo.Length; i++)
        {

            roomsInfo[i].alarmHudImageRef.enabled = false;

            for (int y = 0; y < roomsInfo[i].roomDoors.Length; y++)
            {
                roomsInfo[i].roomDoors[y].doorHudImageRef.enabled = false;
            }
        }
    }

    public void EnableAllRooms()
    {
        for(int i=0; i < roomsInfo.Length; i++)
        {
            roomsInfo[i].Enable();
        }
    }

    private void Update()
    {
        if (!active) return;

        int count = roomsInfo.Length;
        int agentsCount = enemyInfo.Length;

        NativeArray<Vector3> mapCenterArray = new NativeArray<Vector3>(count, Allocator.TempJob);
        NativeArray<Vector3> mapAreaSizeArray = new NativeArray<Vector3>(count, Allocator.TempJob);
        NativeArray<int> enemyPositionId = new NativeArray<int>(agentsCount, Allocator.TempJob);
        NativeArray<Vector3> agentsCurrentPositionArray = new NativeArray<Vector3>(agentsCount, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            mapCenterArray[i] = roomsInfo[i].roomRefCenter.position;
            mapAreaSizeArray[i] = areaSize / 2;
        }

        for (int i = 0; i < enemyInfo.Length; i++)
        {
            agentsCurrentPositionArray[i] = enemyInfo[i].agentRealWordTransform.position;
        }

        MarkerParallelJob markerParallelJob = new MarkerParallelJob
        {
            mapCenterRef = mapCenterArray,
            mapAreaSizeRef = mapAreaSizeArray,
            resultsIndex = enemyPositionId,
            currentAgentPosition = agentsCurrentPositionArray
        };

        JobHandle jobHandle = markerParallelJob.Schedule(agentsCount, 1);

        jobHandle.Complete();


        for (int i = 0; i < agentsCount; i++)
        {
            if (enemyInfo[i].agentRealWordTransform.gameObject.activeSelf == false)
            {
                enemyInfo[i].agentUIImage.enabled = false;
                continue;
            }
            else
                enemyInfo[i].agentUIImage.enabled = true;


            if (markerParallelJob.resultsIndex[i] >= 0)
            {
                enemyInfo[i].agentRectTransform.anchoredPosition = roomsInfo[markerParallelJob.resultsIndex[i]].roomHudRectRef.anchoredPosition;
            }
        }

        mapCenterArray.Dispose();
        mapAreaSizeArray.Dispose();
        enemyPositionId.Dispose();
        agentsCurrentPositionArray.Dispose();

    }

    private void OnValidate()
    {
        if (tryAutoConfig)
        {
            if (roomsInfo.Length > 0)
            {
                for (int i = 0; i < roomsInfo.Length; i++)
                {
                    Room roomRef = roomsInfo[i];

                    if (roomRef.roomAlarm.IsEnabled())
                    {
                        if (roomRef.roomActive)
                            roomRef.roomAlarm.Enable();
                        else
                            roomRef.roomAlarm.Disable();
                    }

                    GameObject roomHudRef = GameObject.Find("HUD " + roomRef.name);
                    if (roomHudRef != null)
                    {
                        roomRef.roomHudRectRef = roomHudRef.GetComponent<RectTransform>();
                        roomRef.roomHudImageRef = roomHudRef.GetComponent<Image>();

                        Image[] children = roomHudRef.GetComponentsInChildren<Image>();

                        for (int y = 0; y < roomRef.roomDoors.Length; y++)
                        {
                            if (roomRef.roomDoors[y].doorRef.IsEnabled())
                            {
                                if (roomRef.roomActive)
                                    roomRef.roomDoors[y].doorRef.Enable();
                                else
                                    roomRef.roomDoors[y].doorRef.Disable();
                            }

                            for (int c = 0; c < children.Length; c++)
                            {
                                if (children[c].name.Contains(roomRef.roomDoors[y].doorRef.name))
                                {
                                    roomRef.roomDoors[y].doorHudImageRef = children[c];
                                    break;
                                }
                            }
                        }

                        for (int y = 0; y < children.Length; y++)
                        {
                            if (children[y].name.Contains("Alarm"))
                                roomRef.alarmHudImageRef = children[y];
                        }
                    }


                }
            }
        }
    }

    [BurstCompile]
    public struct MarkerParallelJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> mapCenterRef;

        [ReadOnly]
        public NativeArray<Vector3> mapAreaSizeRef;


        public NativeArray<int> resultsIndex;

        [ReadOnly]
        public NativeArray<Vector3> currentAgentPosition;

        public void Execute(int index)
        {
            resultsIndex[index] = -1;

            for (int i = 0; i < mapAreaSizeRef.Length; i++)
            {
                //    //agent inside the area
                if (currentAgentPosition[index].x < mapCenterRef[i].x + mapAreaSizeRef[i].x &&
                    currentAgentPosition[index].x > mapCenterRef[i].x - mapAreaSizeRef[i].x &&
                    currentAgentPosition[index].y < mapCenterRef[i].y + mapAreaSizeRef[i].y &&
                    currentAgentPosition[index].y > mapCenterRef[i].y - mapAreaSizeRef[i].y)
                {
                    resultsIndex[index] = i;
                }
            }
        }

    }

    [Serializable]
    public struct MapPositionsConfig
    {
        public Transform center;
        public Vector2 areaSize;
        public RectTransform mapRef;
    }

    [Serializable]
    public struct AgentConfig
    {
        public Transform agentRealWordTransform;
        public Image agentUIImage;
        public RectTransform agentRectTransform;
    }

}


