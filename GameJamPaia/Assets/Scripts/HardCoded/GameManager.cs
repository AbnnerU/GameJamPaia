using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameScore gameScoreRef;
    [SerializeField] private bool active = true;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool playOnStart;
    [Header("Alarms")]
    [SerializeField] private float minAlarmDelay;
    [SerializeField]private float maxAlarmDelay;  
    [SerializeField] private Alarm[] alarms;
    List<int> alarmsIndexAvailable;
    [Header("Doors")]
    [SerializeField] private float startLockingDoorsOnReachScore;
    [SerializeField] private Door2D[] doors;
    [SerializeField] private float maxLockedDoors;
    [SerializeField] private float minLockedDoorDelay;
    [SerializeField] private float maxLockedDoorDelay;
    List<int> doorsIndexAvailable;
    [Header("Game Over")]
    [SerializeField] private int maxAlarmsOn=4;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private bool disablePlayerMovement;

    private int currentDoorsLockedValue = 0;
    private int alarmsOn=0;
    private bool startLockingDoors = false;

    private void Awake()
    {
        alarmsIndexAvailable = new List<int>(alarms.Length);
        doorsIndexAvailable = new List<int>(doors.Length);

        if(playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();    

        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i].AlarmInputCompleted += OnAlarmInput;

            alarms[i].DisableAlarmWithoutAnimation();

            alarmsIndexAvailable.Add(i);
        }

        for(int i = 0; i < doors.Length; i++)
        {
            doorsIndexAvailable.Add(i);

            doors[i].OnUnlockDoor += Door_OnUnlockDoor;
        }

        gameScoreRef.OnScoreChange += GameScore_OnScoreChange;
    }

    
    private void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(AlarmsActivationGameplayLoop());
        }
    }

    private void Door_OnUnlockDoor(Door2D doorRef)
    {
        currentDoorsLockedValue--;

    }

    private void OnAlarmInput(Alarm alarmRef)
    {
        alarmRef.DisableAlarm();

        alarmsIndexAvailable.Add(GetAlarmIndex(alarmRef));

        alarmsOn--;
    }

    private void GameScore_OnScoreChange(int newValue)
    {
        if (startLockingDoors) return;

        if (newValue >= startLockingDoorsOnReachScore)
        {
            StartCoroutine(DoorsLockedGameplayLoop());
            startLockingDoors = true;
        }
    }

    IEnumerator DoorsLockedGameplayLoop()
    {
        int chooseIndex = 0;

        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minLockedDoorDelay, maxLockedDoorDelay));

            if (currentDoorsLockedValue < maxLockedDoors)
            {
                if (doorsIndexAvailable.Count > 0)
                {
                    chooseIndex = Random.Range(0, doorsIndexAvailable.Count);

                    doors[doorsIndexAvailable[chooseIndex]].LockDoor();

                    doorsIndexAvailable.RemoveAt(chooseIndex);

                    currentDoorsLockedValue++;           
                }
            }

        }
    }


    IEnumerator AlarmsActivationGameplayLoop()
    {
        int chooseIndex = 0;
      
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minAlarmDelay, maxAlarmDelay));

            if (alarmsIndexAvailable.Count > 0)
            {
                chooseIndex = Random.Range(0, alarmsIndexAvailable.Count);

                alarms[alarmsIndexAvailable[chooseIndex]].EnableAlarm();

                alarmsIndexAvailable.RemoveAt(chooseIndex);

                alarmsOn++;

                if (alarmsOn >= maxAlarmsOn)
                {
                    active = false;
                    GameOver();
                }
            }
           
        }
    }

    private void GameOver()
    {
        gameOverCanvas.enabled = true;

        if (disablePlayerMovement)
            playerMovement.Disable();
        
    }


    private int GetAlarmIndex(Alarm alarmRef)
    {
        for(int i = 0; i < alarms.Length; i++)
        {
            if (alarms[i] == alarmRef)
                return i;
        }

        return -1;
    }

    private int GetDoorIndex(Door2D doorRef)
    {
        for(int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == doorRef)
                return i;
        }

        return -1;
    }

    //[Serializable]
    //private class DoorState
    //{
    //    public Door2D door;
    //    public bool locked;
    //}
}
