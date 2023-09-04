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
    [SerializeField] private bool playOnStart=true;
    [Header("Alarms")]
    [SerializeField] private Alarm[] alarms;
    List<int> alarmsIndexAvailable;
    [Header("Doors")]   
    [SerializeField] private Door2D[] doors;
    List<int> doorsIndexAvailable;
    [Header("Game Over")]
    [SerializeField] private int maxAlarmsOn=4;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private bool disablePlayerMovement;
    [Header("Tutorial")]
    [SerializeField] private Door2D[] doorsStartLocked;
    [SerializeField] private Alarm alarmStartEnabled;

    [Header("Balance")]
    [SerializeField] private float minAlarmDelay;
    [SerializeField] private float maxAlarmDelay;
    [SerializeField] private float minLockNewDoorDelay;
    [SerializeField] private float maxLockNewDoorDelay;
    [SerializeField] private int startLockingDoorsOnReachScore;
    [SerializeField] private int maxLockedDoorsAmount;
    [SerializeField] private EnemySpawn[] enemySpawnConfig;
    [SerializeField] private MaxDoorsLockedConfig[] doorsLockedConfigs;
    [SerializeField] private AlarmsDelayConfig[] alarmsDelayProgression;

    private int currentDoorsLockedValue = 0;
    private int alarmsOn=0;
    private bool startLockingDoors = false;

    private void Awake()
    {
        alarmsIndexAvailable = new List<int>(alarms.Length);
        doorsIndexAvailable = new List<int>(doors.Length);

        for(int i=0; i < enemySpawnConfig.Length; i++)
        {
            enemySpawnConfig[i].enemyRef.gameObject.SetActive(false);
        }


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
        for(int i=0;i<doorsStartLocked.Length;i++)
            LockDoorAt(GetAvailableDoorIndex(doorsStartLocked[i]));

        EnableAlarmAt(GetAvailableAlarmIndex(alarmStartEnabled));

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
        TrySpawnEnemy(newValue);

        TryChangeDoorsConfig(newValue);

        TryApplyNewAlarmProgression(newValue);

        if (!startLockingDoors && newValue >= startLockingDoorsOnReachScore)
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
            yield return new WaitForSeconds(Random.Range(minLockNewDoorDelay, maxLockNewDoorDelay));

            if (currentDoorsLockedValue < maxLockedDoorsAmount)
            {
                if (doorsIndexAvailable.Count > 0)
                {
                    chooseIndex = Random.Range(0, doorsIndexAvailable.Count);

                    LockDoorAt(chooseIndex);          
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

                EnableAlarmAt(chooseIndex);

                if (alarmsOn >= maxAlarmsOn)
                {
                    active = false;
                    GameOver();
                }
            }
           
        }
    }

    private void LockDoorAt(int index)
    {
        print("Index:" + doors[doorsIndexAvailable[index]]);
        doors[doorsIndexAvailable[index]].LockDoor();

        doorsIndexAvailable.RemoveAt(index);

        currentDoorsLockedValue++;
    }

    private void EnableAlarmAt(int index)
    {
        alarms[alarmsIndexAvailable[index]].EnableAlarm();

        alarmsIndexAvailable.RemoveAt(index);

        alarmsOn++;
    }

    private void TrySpawnEnemy(int scoreValue)
    {
        for(int i = 0; i < enemySpawnConfig.Length; i++)
        {
            if (enemySpawnConfig[i].applied==false && scoreValue>= enemySpawnConfig[i].spawnOnReachScore)
            {
                enemySpawnConfig[i].applied = true;
                enemySpawnConfig[i].enemyRef.gameObject.SetActive(true);
                enemySpawnConfig[i].enemyRef.StartBehaviourTree();
            }
        }
    }

    private void TryChangeDoorsConfig(int scoreValue)
    {
        for(int i = 0; i < doorsLockedConfigs.Length; i++)
        {
            if (doorsLockedConfigs[i].applied ==false && scoreValue >= doorsLockedConfigs[i].onReachScore)
            {
                doorsLockedConfigs[i].applied = true;
                maxLockedDoorsAmount = doorsLockedConfigs[i].newMaxValue;

                minLockNewDoorDelay = doorsLockedConfigs[i].minDelay;
                maxLockNewDoorDelay = doorsLockedConfigs[i].maxDelay;
            }
        }
    }

    private void TryApplyNewAlarmProgression(int scoreValue)
    {
        for(int i = 0; i < alarmsDelayProgression.Length; i++)
        {
            if (alarmsDelayProgression[i].applied==false && scoreValue >= alarmsDelayProgression[i].onReachScore)
            {
                alarmsDelayProgression[i].applied =true;

                minAlarmDelay = alarmsDelayProgression[i].minDelay;
                maxAlarmDelay = alarmsDelayProgression[i].maxDelay;
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
        for(int i=0; i < doors.Length; i++)
        {
            if (doorRef == doors[i])
                return i;
        }

        return -1;
    }

    private int GetAvailableDoorIndex(Door2D doorRef)
    {
        int indexRef = GetDoorIndex(doorRef);

        if (indexRef >= 0)
        {
            for(int i = 0; i < doorsIndexAvailable.Count; i++)
            {
                if (doorsIndexAvailable[i] == indexRef)
                    return i;
            }
            return -1;
        }

        return -1;
    }

    private int GetAvailableAlarmIndex(Alarm alarmRef)
    {
        int indexRef = GetAlarmId(alarmRef);

        if (indexRef >= 0)
        {
            for (int i = 0; i < alarmsIndexAvailable.Count; i++)
            {
                if (alarmsIndexAvailable[i] == indexRef)
                    return i;
            }
            return -1;
        }

        return -1;
    }

    private int GetAlarmId(Alarm alarmRef)
    {
        for(int i=0; i < alarms.Length; i++)
        {
            if (alarms[i] == alarmRef)
                return i;
        }

        return -1;
    }

    //private int GetDoorIndex(Door2D doorRef)
    //{
    //    for(int i = 0; i < doors.Length; i++)
    //    {
    //        if (doors[i] == doorRef)
    //            return i;
    //    }

    //    return -1;
    //}

    //[Serializable]
    //private class DoorState
    //{
    //    public Door2D door;
    //    public bool locked;
    //}

    [Serializable]
    private class AlarmsDelayConfig
    {
        public int onReachScore;
        [Header("Delay")]
        public float minDelay;
        public float maxDelay;
        public bool applied;
    }

    [Serializable]
    private class EnemySpawn
    {
        public FollowAI enemyRef;
        public int spawnOnReachScore;
        public bool applied=false;
    }

    [Serializable]
    private class MaxDoorsLockedConfig
    {
        public int onReachScore;
        public int newMaxValue;
        [Header("Delay")]
        public float minDelay;
        public float maxDelay;
        public bool applied;
    }
}
