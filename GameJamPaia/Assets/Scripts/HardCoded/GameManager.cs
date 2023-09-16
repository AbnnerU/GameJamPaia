using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameScore gameScoreRef;
    [SerializeField] private HighScore highScore;
    [SerializeField]private AudioChannel audioChannel;
    [SerializeField] private bool active = true;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TMP_Text currentActiveClocksAmountText;
    [Header("Alarms")]
    [SerializeField] private AlarmsManager alarmsManager;
    [SerializeField] private Animator alarmIncreaseAnimator;
    [SerializeField] private string alarmIncreaseAnimationName;
    [SerializeField] private AudioConfig[] alarmIncreaseSound;

    [Header("Doors")]
    [SerializeField] private DoorsManager doorsManager;
    [Header("Game Over")]
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Canvas[] othersCanvas;
    [SerializeField] private EnemysOnMap enemyOnMap;

    [Header("Tutorial")]
    [SerializeField] private Door2D[] doorsStartLocked;
    [SerializeField] private Alarm alarmStartEnabled;

    [Header("Balance")]
    [SerializeField] private int maxAlarmsOn = 4;
    [SerializeField] private float minAlarmDelay;
    [SerializeField] private float maxAlarmDelay;
    [SerializeField] private float minLockNewDoorDelay;
    [SerializeField] private float maxLockNewDoorDelay;
    [SerializeField] private int startLockingDoorsOnReachScore;
    [SerializeField] private int maxLockedDoorsAmount;
    [SerializeField] private EnemySpawn[] enemySpawnConfig;
    [SerializeField] private MaxDoorsLockedConfig[] doorsLockedConfigs;
    [SerializeField] private AlarmsDelayConfig[] alarmsDelayProgression;

    private bool startLockingDoors = false;
    private bool pauseNextAlarmActivation = false;
    private bool tutorialEnded = false;

    private void Awake()
    {
        alarmsManager.OnNewAlarmOn += AlarmManager_OnNewAlarmOn;
        alarmsManager.OnAlarmDisabled += AlarmManager_OnAlarmDisableComplete;
        alarmsManager.OnAlarmDisableStart += AlarmManager_OnAlarmDisableStart;
        alarmsManager.OnAlarmDisableCancel += AlarmManager_OnAlarmDisableCancel;
        alarmsManager.OnUpdateAlarmsOnCount += AlarmManager_OnAlarmsOnCountUpdate;

        gameScoreRef.OnScoreChange += GameScore_OnScoreChange;
    }

   

    private void Start()
    {
        for(int i=0;i<doorsStartLocked.Length;i++)
            doorsManager.LockDoorAt(doorsManager.GetAvailableDoorIndex(doorsStartLocked[i]));

        alarmsManager.EnableAlarmAt(alarmsManager.GetAvailableAlarmIndex(alarmStartEnabled));

        alarmStartEnabled.AlarmInputCompleted += TutorialEnd;                 
    }

    private void AlarmManager_OnNewAlarmOn()
    {
        alarmIncreaseAnimator.Play(alarmIncreaseAnimationName, 0, 0);

        audioChannel?.AudioRequest(alarmIncreaseSound[Random.Range(0, alarmIncreaseSound.Length)], Vector3.zero);
    }

    private void AlarmManager_OnAlarmDisableStart()
    {
        pauseNextAlarmActivation = true;
    }

    private void AlarmManager_OnAlarmDisableCancel()
    {
        pauseNextAlarmActivation = false;
    }

    private void AlarmManager_OnAlarmDisableComplete()
    {
        pauseNextAlarmActivation = false;
    }

    private void AlarmManager_OnAlarmsOnCountUpdate(int value)
    {
        currentActiveClocksAmountText.text = value.ToString();

        if (value >= maxAlarmsOn)
        {
            active = false;
            GameOver();
        }
    }


    private void TutorialEnd(Alarm alarm)
    {
        if (!tutorialEnded)
        {
            StartCoroutine(AlarmsActivationGameplayLoop());
            tutorialEnded = true;
        }
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
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minLockNewDoorDelay, maxLockNewDoorDelay));

            if (doorsManager.DoorsLockedValue() < maxLockedDoorsAmount)
            {
                doorsManager.TryLockRandomDoor();
            }

        }
    }


    IEnumerator AlarmsActivationGameplayLoop()
    {
        float delayTime=0;
        float currentDelayTime=0;
      
        while (active)
        {
            delayTime = Random.Range(minAlarmDelay, maxAlarmDelay);
            currentDelayTime = 0;
            do
            {
                if (pauseNextAlarmActivation == false)
                    currentDelayTime += Time.deltaTime ;

                yield return null;
            } while (currentDelayTime < delayTime);

            alarmsManager.TryEnableRandomAlarm();
           
        }
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

        enemyOnMap.Disable();

        for(int i=0;i<othersCanvas.Length;i++)
            othersCanvas[i].enabled = false;

        playerMovement.gameObject.SetActive(false);
        playerMovement.Disable();

        active = false;

        highScore.TrySetNewHighScore();
        
    }

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
