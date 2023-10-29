using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameScore gameScoreRef;
    [SerializeField] private HighScore highScore;
    [SerializeField]private AudioChannel audioChannel;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private bool active = true;
    [SerializeField] private PlayerMovement playerMovement;
    [Header("Alarms")]
    [SerializeField] private AlarmsManager alarmsManager;
    [SerializeField] private Image alarmsActiveBar;
    [SerializeField] private Animator alarmIncreaseAnimator;
    [SerializeField] private string alarmIncreaseAnimationName;
    [SerializeField] private string alarmsOffAnimationName;
    [SerializeField] private AudioConfig[] alarmIncreaseSound;
    [SerializeField] private Color alarmAmountLowColor = Color.green;
    [SerializeField] private Color alarmAmountMediumColor = Color.yellow;
    [SerializeField] private Color alarmAmountHighColor= Color.red;
    [Header("Map")]
    [SerializeField]private MapManager mapManager;
    [Header("Doors")]
    [SerializeField] private DoorsManager doorsManager;
    [SerializeField] private int scoreToUnlockAllRooms;
    [Header("Game Over")]
    [SerializeField]private RoomsMusicManager roomMusicManager;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private GameObject gameOverOptionsPanel;
    [SerializeField] private Animator gameOverAnimator;
    [SerializeField]private string gameOverAnimationName;
    [SerializeField] private float gameOverAnimationTime;
    [SerializeField] private Canvas[] othersCanvas;

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
    //[SerializeField] private int maxLockedDoorsAmount;
    [SerializeField] private int startSpawningPowerUpOnReachScore;
    [SerializeField] private int minPowerUpDelay;
    [SerializeField] private int maxPowerUpDelay;
    [Header("Enemy Spawn")]
    [SerializeField] private SpawnConfig[] enemyBalanceOptions;
    [SerializeField] private MaxDoorsLockedConfig[] doorsLockedConfigs;
    [SerializeField] private AlarmsDelayConfig[] alarmsDelayProgression;

    private List<GameObject> powerUpList = new List<GameObject>();

    public Action OnEndTutorial;
    private EnemySpawn enemySpawnConfig;

    private bool startSpawningPowerUp = false;
    private bool startLockingDoors = false;
    private bool pauseNextAlarmActivation = false;
    private bool tutorialEnded = false;
    private bool roomsLocked = true;

    private void Awake()
    {
        alarmsManager.OnNewAlarmOn += AlarmManager_OnNewAlarmOn;
        alarmsManager.OnAlarmDisabled += AlarmManager_OnAlarmDisableComplete;
        alarmsManager.OnAlarmDisableStart += AlarmManager_OnAlarmDisableStart;
        alarmsManager.OnAlarmDisableCancel += AlarmManager_OnAlarmDisableCancel;
        alarmsManager.OnUpdateAlarmsOnCount += AlarmManager_OnAlarmsOnCountUpdate;

        gameScoreRef.OnScoreChange += GameScore_OnScoreChange;

        SetupEnemySpawn();

        alarmsActiveBar.fillAmount = 0;

        transitionPanel.SetActive(false);

        gameOverAnimator.enabled = false;
    }

  
    private void Start()
    {
        for(int i=0;i<doorsStartLocked.Length;i++)
            doorsManager.LockDoorAt(doorsManager.GetAvailableDoorIndex(doorsStartLocked[i]));

        alarmsManager.EnableAlarmAt(alarmsManager.GetAvailableAlarmIndex(alarmStartEnabled));

        alarmStartEnabled.AlarmInputCompleted += TutorialEnd;

        doorsManager.SendActiveStateEvent();
    }

    #region Alarms
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

        if(alarmsManager.GetAlarmsActiveValue() == 0)
            alarmIncreaseAnimator.Play(alarmsOffAnimationName, 0, 0);
    }

    private void AlarmManager_OnAlarmsOnCountUpdate(int value)
    {
        ChooseBarColor(value);

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

            OnEndTutorial?.Invoke();
        }
    }

    public void AddMaxAlarmOnValue(int addValue)
    {
        maxAlarmsOn += addValue;

        ChooseBarColor(alarmsManager.GetAlarmsActiveValue());
    }
    public void ChooseBarColor(int value)
    {
        float percentage = (value * 100) / maxAlarmsOn;

        alarmsActiveBar.fillAmount = percentage/100;

       // print("Percentage: " + percentage+" | Value: "+value+" | Max:"+maxAlarmsOn);

        if(percentage < 50)       
            alarmsActiveBar.color = alarmAmountLowColor;
        else if(value != maxAlarmsOn - 1)       
            alarmsActiveBar.color = alarmAmountMediumColor;
        else if(value == maxAlarmsOn - 1)      
            alarmsActiveBar.color = alarmAmountHighColor;
        
    }

    #endregion

    private void GameScore_OnScoreChange(int newValue)
    {
        TrySpawnEnemy(newValue);

        TryChangeDoorsConfig(newValue);

        TryApplyNewAlarmProgression(newValue);

        //-----Doors Lock-------
        if (!startLockingDoors && newValue >= startLockingDoorsOnReachScore)
        {
            StartCoroutine(DoorsLockedGameplayLoop());
            startLockingDoors = true;
        }

        //-----Power Up-------
        if(!startSpawningPowerUp && newValue >= startSpawningPowerUpOnReachScore)
        {
            StartCoroutine(PowerUpGameplayLoop());
            startSpawningPowerUp = true;
        }

        if(roomsLocked && newValue >= scoreToUnlockAllRooms)
        {
            roomsLocked = false;

            doorsManager.EnableAllDoors();

            mapManager.EnableAllRooms();

            alarmsManager.EnableAllAlarms();
        }
    }

    private void SetupEnemySpawn()
    {
        SpawnConfig current = enemyBalanceOptions[Random.Range(0, enemyBalanceOptions.Length)];

        enemySpawnConfig = new EnemySpawn();

        enemySpawnConfig.spawnConfig = current;

        enemySpawnConfig.enemySpawned = new ScoreReachEnemySpawned[current.spawnOnReachScore.Length];

        for (int i = 0; i < current.spawnOnReachScore.Length; i++)
        {

            ScoreReachEnemySpawned s = new ScoreReachEnemySpawned();
            s.onScoreRef = current.spawnOnReachScore[i]; ;
            s.spawned = false;

            enemySpawnConfig.enemySpawned[i] = s;
        }
    }

    #region GameLoop

    IEnumerator DoorsLockedGameplayLoop()
    {
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minLockNewDoorDelay, maxLockNewDoorDelay));

            //if (doorsManager.DoorsLockedValue() < maxLockedDoorsAmount)
            //{
                doorsManager.TryLockRandomDoor();
            //}

        }
    }

    IEnumerator PowerUpGameplayLoop()
    {
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minPowerUpDelay, maxPowerUpDelay));

            SpawnPowerUp();
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
        for(int i = 0; i < enemySpawnConfig.enemySpawned.Length; i++)
        {
            if (enemySpawnConfig.enemySpawned[i].spawned ==false && scoreValue >= enemySpawnConfig.enemySpawned[i].onScoreRef)
            {
                enemySpawnConfig.enemySpawned[i].spawned = true;
                SpawnEnemy();
            }
        }
    }

    private void SpawnPowerUp()
    {
        Vector3 pos = mapManager.GetRandomAvalibleRoomPosition();
        float x = Random.Range(0, 5);
        float y = Random.Range(0, 5);

        GameObject p = PoolManager.SpawnObject(powerUpPrefab, pos + new Vector3(x, y, 0), Quaternion.identity);

        if (!powerUpList.Contains(p))
        {
            powerUpList.Add(p);

            p.GetComponent<OnTriggerUnlockAllDoors2D>().WhenCollected += DisablePowerUpHud;
            p.GetComponent<DisableAfterTime>().OnTimeOut += DisablePowerUpHud;
        }

        mapManager.ChangePowerUpHudActiveStateOnPosition(pos, true);
    }

    private void DisablePowerUpHud(Vector3 position)
    {
        mapManager.ChangePowerUpHudActiveStateOnPosition(position, false);
    }

    #endregion

    private void SpawnEnemy()
    {
        float chance = Random.Range(0f, 100f);

        for(int i=0; i < enemySpawnConfig.spawnConfig.enemySpawnChance.Length; i++)
        {
            if (enemySpawnConfig.spawnConfig.enemySpawnChance[i].spawnChance >= chance)
            {
                GameObject g = PoolManager.SpawnObject(enemySpawnConfig.spawnConfig.enemySpawnChance[i].prefab);

                g.GetComponent<IHasBehaviourTree>().Setup();

                mapManager.SetRandomRoom(g.transform, Vector3.zero);

                g.GetComponent<IHasBehaviourTree>().StartBehaviourTree();
                break;
            } 
        }
    }

    public void PauseAlarmsLoop(bool pause)
    {
        pauseNextAlarmActivation = pause;
    }

    private void TryChangeDoorsConfig(int scoreValue)
    {
        for(int i = 0; i < doorsLockedConfigs.Length; i++)
        {
            if (doorsLockedConfigs[i].applied ==false && scoreValue >= doorsLockedConfigs[i].onReachScore)
            {
                doorsLockedConfigs[i].applied = true;
                //maxLockedDoorsAmount = doorsLockedConfigs[i].newMaxValue;

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
        alarmsManager.DisableAllAlarmsSound();

        roomMusicManager.TurnOffMusic();

        transitionPanel.SetActive(true);

        gameOverAnimator.enabled = true;

        gameOverAnimator.Play(gameOverAnimationName, 0, 0);

        for(int i=0;i<othersCanvas.Length;i++)
            othersCanvas[i].enabled = false;

        playerMovement.GetComponent<Collider2D>().enabled = false;
        playerMovement.Disable();

        active = false;

        highScore.TrySetNewHighScore();

        StopAllCoroutines();

        StartCoroutine(GameOverTransition());
    }


    IEnumerator GameOverTransition()
    {
        yield return new WaitForSeconds(gameOverAnimationTime);
        gameOverAnimator.enabled = false;
        gameOverOptionsPanel.SetActive(true);
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
        public SpawnConfig spawnConfig;
        public ScoreReachEnemySpawned[] enemySpawned;
    }

    [Serializable]
    private class ScoreReachEnemySpawned
    {
        public int onScoreRef;
        public bool spawned;
    }

    [Serializable]
    private class MaxDoorsLockedConfig
    {
        public int onReachScore;
        //public int newMaxValue;
        [Header("Delay")]
        public float minDelay;
        public float maxDelay;
        public bool applied;
    }
}
