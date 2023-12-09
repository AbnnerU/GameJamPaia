using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.UI;

using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class MapManager : MonoBehaviour
{
    [SerializeField] private bool active;
    [SerializeField] private bool tryAutoConfig;
    [Header("Enemy")]
    [SerializeField] private GameObject enemyHudIconPrefab;
    [SerializeField] private RectTransform enemyIconParent;
    [SerializeField] private AgentConfig[] enemyInfo;
    [Header("Rooms")]
    [SerializeField] private Vector2 areaSize;
    [SerializeField] private Room[] roomsInfo;
    [SerializeField] private List<Room> avaliableRooms;
    [SerializeField] private List<Room> disabledRooms;
    private RoomDoorsInfo[] roomsDoors;
    private DoorsManager doorsManager;
    private AlarmsManager alarmsManager;

    private void Awake()
    {
        doorsManager = FindObjectOfType<DoorsManager>();
        alarmsManager = FindObjectOfType<AlarmsManager>();

        avaliableRooms = new List<Room>();
        disabledRooms = new List<Room>();

        int doorAmount = 0;
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

        Alarm[] alarms = FindObjectsOfType<Alarm>();

        foreach (var a in alarms)
            a.OnAlarmEnabled += Alarms_OnAlarmEnabledUpdate;

        foreach (var d in roomsDoors)
        {
            d.doorRef.OnLockDoor += Doors_OnDoorLock;
            d.doorRef.OnUnlockDoor += Doors_OnDoorUnluck;
        }

        foreach (Room d in roomsInfo)
        {
            if (d.roomActive)
                avaliableRooms.Add(d);
            else
                disabledRooms.Add(d);
        }

        DisableAllDoorsHud();

        DisableAllCoinsHud();

        DisableAllPowerUpHud();
    }

    private void Update()
    {
        if (!active || enemyInfo.Length == 0) return;

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

    public void AddNewAgent(Transform agentTransform)
    {
        GameObject icon = Instantiate(enemyHudIconPrefab, enemyIconParent);

        RectTransform iconRect = icon.GetComponent<RectTransform>();
        Image iconImage = icon.GetComponent<Image>();

        AgentConfig agentConfig = new AgentConfig();
        agentConfig.agentRectTransform = iconRect;
        agentConfig.agentUIImage = iconImage;
        agentConfig.agentRealWordTransform = agentTransform;

        AgentConfig[] temp = enemyInfo;
        //print(enemyInfo.Length);
        //print(temp.Length);
        int originalSize = temp.Length;
        int newSize = originalSize + 1;

        //print(originalSize);
        //print(newSize);

        enemyInfo = new AgentConfig[newSize];

        if (originalSize > 0)
        {
            for (int i = 0; i < originalSize; i++)
            {
                enemyInfo[i] = temp[i];
            }
        }

        enemyInfo[newSize - 1] = agentConfig;

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
        for (int i = 0; i < roomsInfo.Length; i++)
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
        for (int i = 0; i < roomsInfo.Length; i++)
        {
            for (int y = 0; y < roomsInfo[i].roomDoors.Length; y++)
            {
                roomsInfo[i].roomDoors[y].doorHudImageRef.enabled = false;
            }
        }
    }

    private void DisableAllCoinsHud()
    {
        for (int i = 0; i < roomsInfo.Length; i++)
        {
            roomsInfo[i].coinHudImageRef.enabled = false;
        }
    }

    private void DisableAllPowerUpHud()
    {
        for (int i = 0; i < roomsInfo.Length; i++)
        {
            roomsInfo[i].powerUpHudImageRef.enabled = false;
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

    public void EnableRandomRoom()
    {
        if (disabledRooms.Count <= 0) return;
        int id;
        do
        {
            id = Random.Range(0, disabledRooms.Count);
            
        } while (UpdateDoors(disabledRooms[id]) == false);


        avaliableRooms.Add(disabledRooms[id]);

        alarmsManager.SetAlarmAvailable(disabledRooms[id].roomAlarm);

        disabledRooms[id].Enable();

        disabledRooms.RemoveAt(id);
    }

    private bool UpdateDoors(Room roomRef)
    {
        int roomId = GetRoomId(roomRef);

        print("-------------- RANDOM ROOM :" + roomId + " --------------");

        if (roomId < 0)
        {
            Debug.LogError("Room " + roomRef + " dont exist");
            return false;
        }

        int max = 5;

        int neighborTop = roomId + max;
        int neighborDown = roomId - max;
        int neighborLeft = roomId - 1;
        int neighborRight = roomId + 1;

        int roomLine = roomId / 5;

        if (neighborTop < 0 || neighborTop > roomsInfo.Length-1) neighborTop = -1;

        if (neighborDown < 0 || neighborDown > roomsInfo.Length-1) neighborDown = -1;

        if (neighborLeft < 0 || neighborLeft > roomsInfo.Length-1 || (int)(neighborLeft / 5) != roomLine) neighborLeft = -1;

        if (neighborRight < 0 || neighborRight > roomsInfo.Length-1 || (int)(neighborRight / 5) != roomLine) neighborRight = -1;


        if (neighborTop < 0 && neighborDown < 0 && neighborRight < 0 && neighborLeft < 0)
        {
            print("ENABLE ROOM FAIL.");
            return false;
        }

        bool haveSomeNeighborActive = false;

        if (neighborTop >= 0)
        {
            print("VIZINHO DO TOPO: "+ neighborTop);
            if (ValidateNeighborDoors(roomRef, neighborTop, DoorDirection.UP, DoorDirection.DOWN)) haveSomeNeighborActive = true;
        }

        if (neighborDown >= 0)
        {
            print("VIZINHO DE BAIXO: " + neighborDown);
            if (ValidateNeighborDoors(roomRef, neighborDown, DoorDirection.DOWN, DoorDirection.UP)) haveSomeNeighborActive = true;
        }

        if (neighborLeft >= 0)
        {
            print("VIZINHO DA ESQUERDA: " + neighborLeft);
            if (ValidateNeighborDoors(roomRef, neighborLeft, DoorDirection.LEFT, DoorDirection.RIGHT)) haveSomeNeighborActive = true;
        }

        if (neighborRight >= 0)
        {
            print("VIZINHO DA DIREITA: " + neighborRight);
            if (ValidateNeighborDoors(roomRef, neighborRight, DoorDirection.RIGHT, DoorDirection.LEFT)) haveSomeNeighborActive = true;
        }

        if (haveSomeNeighborActive)
            return true;
        else
        {
            print("None neighbor active");
            return false;
        }
    }

    private bool ValidateNeighborDoors(Room roomRef, int neighborIndex, DoorDirection roomRefDoorDirection, DoorDirection neighborDoorDirection)
    {
        if (roomsInfo[neighborIndex].roomActive == true)
        {
            Door2D door = roomRef.GetDoorByDirection(roomRefDoorDirection);

            if (door)
            {
                doorsManager.EnableDoor(door);

                Door2D neighborDoor = roomsInfo[neighborIndex].GetDoorByDirection(neighborDoorDirection);

                doorsManager.EnableDoor(neighborDoor);

                return true;

            }
            else
            {
                print("Doors not finded");
                return false;
            }
        }
        else
        {
            print(neighborIndex + "disabled");
            return false;
        }
    }

    private int GetRoomId(Room room)
    {
        for (int i = 0; i < roomsInfo.Length; i++)
        {
            if (roomsInfo[i] == room)
                return i;
        }

        return -1;
    }


    //public void EnableAllRooms()
    //{
    //    for(int i=0; i < roomsInfo.Length; i++)
    //    {
    //        roomsInfo[i].Enable();
    //    }

    //    foreach(Room d in disabledRooms)
    //        avaliableRooms.Add(d);

    //    disabledRooms.Clear();
    //    disabledRooms.Capacity = 0;
    //}

    public void LockDoorsOnRoom(int roomId)
    {
        Room r = avaliableRooms[roomId];

        if (r != null)
        {
            for (int i = 0; i < r.roomDoors.Length; i++)
            {
                doorsManager.TryLookDoor(r.roomDoors[i].doorRef);
            }
        }
    }

    public void EneableAlarmOnRoom(int roomId)
    {
        Room r = avaliableRooms[roomId];

        if (r != null)
        {
            alarmsManager.TryEnableAlarm(r.roomAlarm);
        }
    }

    public void ChangeCoinHudActiveStateOnPosition(Vector3 position, bool enabled)
    {
        Room r = GetRoomOfPosition(position);

        if (r != null)
        {
            r.coinHudImageRef.enabled = enabled;
        }
    }

    public void ChangePowerUpHudActiveStateOnPosition(Vector3 position, bool enabled)
    {
        Room r = GetRoomOfPosition(position);

        if (r != null)
        {
            r.powerUpHudImageRef.enabled = enabled;
        }
    }

    public void SetRoom(Transform reference, Vector3 offset, int id)
    {
        reference.position = avaliableRooms[id].roomRefCenter.position + offset;
        print(avaliableRooms[id].roomRefCenter.position);
    }

    public void SetRandomRoom(Transform reference, Vector3 offset)
    {
        int index = Random.Range(0, avaliableRooms.Count);
        reference.position = avaliableRooms[index].roomRefCenter.position + offset;
        print(avaliableRooms[index].roomRefCenter.position);
    }

    public void SetRandomRoom(Transform[] reference, Vector3[] offset)
    {
        int index = Random.Range(0, avaliableRooms.Count);
        for (int i = 0; i < reference.Length; i++)
        {
            reference[i].position = avaliableRooms[index].roomRefCenter.position + offset[i];
        }
    }

    public void RandomRoomNoRepeatAndAlarmOff(Transform reference, Vector3 offset)
    {
        Room currentRoom = GetRoomOfPosition(reference.position);

        List<Room> roomOptions = new List<Room>();

        foreach (Room r in avaliableRooms)
        {
            if (r.roomAlarm.IsEnabled() == false)
                roomOptions.Add(r);
        }


        if (currentRoom)
        {
            if (roomOptions.Contains(currentRoom))
                roomOptions.Remove(currentRoom);
        }

        int index = Random.Range(0, roomOptions.Count);

        reference.position = roomOptions[index].roomRefCenter.position + offset;

    }

    public void RandomRoomNoRepeatAndAlarmOff(Transform reference, Vector3 offset, out int id)
    {
        id = -1;

        Room currentRoom = GetRoomOfPosition(reference.position);

        print("Current Room: " + currentRoom);

        List<Room> roomOptions = new List<Room>();

        foreach (Room r in avaliableRooms)
        {
            if (r.roomAlarm.AlarmOn() == false)
                roomOptions.Add(r);
        }

        print("Room options: " + roomOptions.Count);

        if (currentRoom)
        {
            if (roomOptions.Contains(currentRoom))
            {
                print("Current Room removed");
                roomOptions.Remove(currentRoom);
            }

        }

        int index = Random.Range(0, roomOptions.Count);

        print("Options choose: " + index);

        reference.position = roomOptions[index].roomRefCenter.position + offset;

        id = avaliableRooms.IndexOf(roomOptions[index]);

        print("Avaliable room id:" + id);
    }

    public void SetRandomRoom(Transform[] reference, Vector3[] offset, out int roomId)
    {
        roomId = -1;
        int index = Random.Range(0, avaliableRooms.Count);
        roomId = index;
        for (int i = 0; i < reference.Length; i++)
        {
            reference[i].position = avaliableRooms[index].roomRefCenter.position + offset[i];
        }
    }

    public Room GetRoomOfPosition(Vector3 positionRef)
    {
        Vector3 size = areaSize / 2;
        Vector3 roomCenter;
        for (int i = 0; i < roomsInfo.Length; i++)
        {
            roomCenter = roomsInfo[i].roomRefCenter.position;

            if (positionRef.x < roomCenter.x + size.x &&
                positionRef.x > roomCenter.x - size.x &&
                positionRef.y < roomCenter.y + size.y &&
                positionRef.y > roomCenter.y - size.y)
            {
                return roomsInfo[i];
            }
        }
        return null;
    }

    public Vector3 GetRandomAvalibleRoomPosition()
    {
        int index = Random.Range(0, avaliableRooms.Count);

        return avaliableRooms[index].roomRefCenter.position;
    }


    public Vector2 GetAreaSize()
    {
        return areaSize;
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

                        for (int y = 0; y < children.Length; y++)
                        {
                            if (children[y].name.Contains("Coin"))
                                roomRef.coinHudImageRef = children[y];
                        }

                        for (int y = 0; y < children.Length; y++)
                        {
                            if (children[y].name.Contains("PowerUp"))
                                roomRef.powerUpHudImageRef = children[y];
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


