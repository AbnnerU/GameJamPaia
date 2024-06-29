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
    [SerializeField]private FakeCoinManager fakeCoinManager;
    [SerializeField]private AudioChannel audioChannel;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private bool active = true;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField]private HealthManager healthManager;
    [Header("Alarms")]
    [SerializeField] private AlarmsManager alarmsManager;
    [SerializeField] private Image alarmsFill;
    [SerializeField] private Slider alarmsActiveBar;
    [SerializeField] private Image faceImage;
    [SerializeField] private Sprite[] faceProgression;
    //[SerializeField] private Animator alarmIncreaseAnimator;
    //[SerializeField] private string alarmIncreaseAnimationName;
    //[SerializeField] private string alarmsOffAnimationName;
    [SerializeField] private AudioConfig[] alarmIncreaseSound;
    [SerializeField] private Color alarmAmountLowColor = Color.green;
    [SerializeField] private Color alarmAmountMediumColor = Color.yellow;
    [SerializeField] private Color alarmAmountHighColor= Color.red;
    [Header("Map")]
    [SerializeField]private MapManager mapManager;
    [Header("Doors")]
    [SerializeField] private DoorsManager doorsManager;
    //[SerializeField] private int scoreToUnlockAllRooms;
    [Header("Game Over")]
    [SerializeField]private RoomsMusicManager roomMusicManager;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private GameObject gameOverOptionsPanel;
    [SerializeField] private Animator gameOverAnimator;
    [SerializeField]private string gameOverAnimationName;
    [SerializeField] private float gameOverAnimationTime;
    [SerializeField] private Canvas[] othersCanvas;
    [SerializeField] private MultiSoundRequest[] perksSound;

    [Header("Tutorial")]
    [SerializeField] private int roomToStartTutorial;

    [Header("Balance")]
    [SerializeField] private BalanceValues balanceValues;
    [SerializeField]private DoorsLockedProgression doorsLockedProgression;
    [SerializeField]private AlarmsProgression alarmsProgression;
    [SerializeField]private EnableRoomProgression enableRoomProgression;
   
    [Header("Enemy Spawn")]
    [SerializeField] private SpawnConfig[] enemyBalanceOptions;


    private List<GameObject> powerUpList = new List<GameObject>();

    public Action OnEndTutorial;
    public Action OnSetupCompleted;
    private EnemySpawn enemySpawnConfig;

    private bool startSpawningPowerUp = false;
    private bool startLockingDoors = false;
    private bool pauseNextAlarmActivation = false;
    private bool tutorialEnded = false;
    private bool roomsLocked = true;

    //--------------Balance-------------
    private int maxAlarmsOn = 4;
    private float minAlarmDelay;
    private float maxAlarmDelay;
    private float minLockNewDoorDelay;
    private float maxLockNewDoorDelay;
    private int startLockingDoorsOnReachScore;
    private int startSpawningPowerUpOnReachScore;
    private int minPowerUpDelay;
    private int maxPowerUpDelay;
    private MaxDoorsLockedConfig[] doorsLockedConfigs;
    private AlarmsDelayConfig[] alarmsDelayProgression;
    [SerializeField]private RoomProgression[] roomProgressions;
    //--------------------------------------

    private void Awake()
    {
        ApplyBalanceValues();

        alarmsManager.OnNewAlarmOn += AlarmManager_OnNewAlarmOn;
        alarmsManager.OnAlarmDisabled += AlarmManager_OnAlarmDisableComplete;
        alarmsManager.OnAlarmDisableStart += AlarmManager_OnAlarmDisableStart;
        alarmsManager.OnAlarmDisableCancel += AlarmManager_OnAlarmDisableCancel;
        alarmsManager.OnUpdateAlarmsOnCount += AlarmManager_OnAlarmsOnCountUpdate;
        healthManager.OnDeath += GameOver;
        gameScoreRef.OnScoreChange += GameScore_OnScoreChange;

        SetupEnemySpawn();

        alarmsActiveBar.value = 0;

        transitionPanel.SetActive(false);

        gameOverAnimator.enabled = false;
    }

  
    private void Start()
    {
        OnSetupCompleted?.Invoke();

        Door2D[] doorsForTutorial = mapManager.GetDoorsInRoom(roomToStartTutorial);
        Alarm alarmForTutorial = mapManager.GetAlarmInRoom(roomToStartTutorial);

        for (int i=0;i< doorsForTutorial.Length;i++)
            doorsManager.LockDoorAt(doorsManager.GetAvailableDoorIndex(doorsForTutorial[i]));

        alarmsManager.EnableAlarmAt(alarmsManager.GetAvailableAlarmIndex(alarmForTutorial));

        alarmForTutorial.AlarmInputCompleted += TutorialEnd;

        doorsManager.SendActiveStateEvent();
    }

    private void ApplyBalanceValues()
    {
        maxAlarmsOn = balanceValues.maxAlarmsOn;
        minAlarmDelay = balanceValues.minAlarmDelay;
        maxAlarmDelay = balanceValues.maxAlarmDelay;
        minLockNewDoorDelay = balanceValues.minLockNewDoorDelay;
        maxLockNewDoorDelay = balanceValues.maxLockNewDoorDelay;
        startLockingDoorsOnReachScore = balanceValues.startLockingDoorsOnReachScore;
        startSpawningPowerUpOnReachScore = balanceValues.startSpawningPowerUpOnReachScore;
        minPowerUpDelay = balanceValues.minPowerUpDelay;
        maxPowerUpDelay = balanceValues.maxPowerUpDelay;

        //doorsLockedConfigs = doorsLockedProgression.doorsLockedConfigs;
        //alarmsDelayProgression = alarmsProgression.alarmsDelayProgression;

        GetAndApplyBalanceValues();

    }

    private void GetAndApplyBalanceValues()
    {
        MaxDoorsLockedConfig[] reference = doorsLockedProgression.doorsLockedConfigs;

        doorsLockedConfigs = new MaxDoorsLockedConfig[reference.Length];

        for(int i=0; i<reference.Length; i++)
        {
            MaxDoorsLockedConfig current = new MaxDoorsLockedConfig();
            current.applied = false;
            current.onReachScore = reference[i].onReachScore;
            current.minDelay = reference[i].minDelay;
            current.maxDelay = reference[i].maxDelay;

            doorsLockedConfigs[i] = current;
        }

        AlarmsDelayConfig[] reference2 = alarmsProgression.alarmsDelayProgression;

        alarmsDelayProgression = new AlarmsDelayConfig[reference2.Length];

        for(int i=0; i < reference2.Length; i++)
        {
            AlarmsDelayConfig current = new AlarmsDelayConfig();
            current.applied = false;
            current.onReachScore = reference2[i].onReachScore;
            current.minDelay = reference2[i].minDelay;
            current.maxDelay = reference2[i].maxDelay;

            alarmsDelayProgression[i] = current;
        }

        RoomProgression[] reference3 = enableRoomProgression.roomProgressions;

        roomProgressions = new RoomProgression[reference3.Length];

        for (int i = 0; i < reference3.Length; i++)
        {
            RoomProgression current = new RoomProgression();
            current.applied = false;
            current.onReachScore = reference3[i].onReachScore;

            roomProgressions[i] = current;
        }
    }


    #region Alarms
    private void AlarmManager_OnNewAlarmOn()
    {
        //alarmIncreaseAnimator.Play(alarmIncreaseAnimationName, 0, 0);

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

        //if(alarmsManager.GetAlarmsActiveValue() == 0)
        //    alarmIncreaseAnimator.Play(alarmsOffAnimationName, 0, 0);
    }

    private void AlarmManager_OnAlarmsOnCountUpdate(int value)
    {
        ChooseBarColor(value);
        ChooseFaceImage(value);

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

        ChooseFaceImage(alarmsManager.GetAlarmsActiveValue());
    }
    public void ChooseBarColor(int value)
    {
        float percentage = (value * 100) / maxAlarmsOn;

        alarmsActiveBar.value = percentage/100;

       // print("Percentage: " + percentage+" | Value: "+value+" | Max:"+maxAlarmsOn);

        if(percentage < 50)
            alarmsFill.color = alarmAmountLowColor;
        else if(value != maxAlarmsOn - 1)
            alarmsFill.color = alarmAmountMediumColor;
        else if(value == maxAlarmsOn - 1)
            alarmsFill.color = alarmAmountHighColor;
        
    }

    public void ChooseFaceImage(int value)
    {
        float percentage = (value * 100) / maxAlarmsOn;

        percentage = percentage / 100;

        int imagesOptions = (int)((faceProgression.Length-1) * percentage);

        faceImage.sprite = faceProgression[imagesOptions];
    }

    #endregion

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
        float delayTime = 0;
        float currentDelayTime = 0;

        while (active)
        {
            delayTime = Random.Range(minAlarmDelay, maxAlarmDelay);
            currentDelayTime = 0;
            do
            {
                if (pauseNextAlarmActivation == false)
                    currentDelayTime += Time.deltaTime;

                yield return null;
            } while (currentDelayTime < delayTime);

            alarmsManager.TryEnableRandomAlarm();

        }
    }

    private void TrySpawnEnemy(int scoreValue)
    {
        for (int i = 0; i < enemySpawnConfig.enemySpawned.Length; i++)
        {
            if (enemySpawnConfig.enemySpawned[i].spawned == false && scoreValue >= enemySpawnConfig.enemySpawned[i].onScoreRef)
            {
                enemySpawnConfig.enemySpawned[i].spawned = true;
                SpawnEnemy();
            }
        }
    }

    private void SpawnPowerUp()
    {
        Vector3 pos = mapManager.GetRandomAvalibleRoomPosition();
        float x = Random.Range(2, 3.5f);
        float y = Random.Range(2, 3.5f);

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

    private void GameScore_OnScoreChange(int newValue)
    {
        TrySpawnEnemy(newValue);

        TryChangeDoorsConfig(newValue);

        TryApplyNewAlarmProgression(newValue);

        TryOpenNewRoom(newValue);

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

    private void SpawnEnemy()
    {
        float chance = Random.Range(0f, 100f);

        for(int i=0; i < enemySpawnConfig.spawnConfig.enemySpawnChance.Length; i++)
        {
            if (enemySpawnConfig.spawnConfig.enemySpawnChance[i].spawnChance >= chance)
            {
                GameObject g = PoolManager.SpawnObject(enemySpawnConfig.spawnConfig.enemySpawnChance[i].prefab);

                g.GetComponent<IHasBehaviourTree>().Setup();

                mapManager.SetObjectInRandomRoom(g.transform, Vector3.zero);

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

                return;
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

                return;
            }
        }
    }

    private void TryOpenNewRoom(int scoreValue)
    {
        for (int i = 0; i < roomProgressions.Length; i++)
        {
            if (roomProgressions[i].applied == false && scoreValue >= roomProgressions[i].onReachScore)
            {
                roomProgressions[i].applied = true;

                mapManager.EnableRandomRoom();

                return;

            }
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();

        alarmsManager.DisableAllAlarmsSound();

        roomMusicManager.TurnOffMusic();

        transitionPanel.SetActive(true);

        gameOverAnimator.enabled = true;

        gameOverAnimator.Play(gameOverAnimationName, 0, 0);

        for(int i=0;i<othersCanvas.Length;i++)
            othersCanvas[i].enabled = false;

        for (int i = 0; i < perksSound.Length; i++)
            perksSound[i].StopAll();

        playerMovement.GetComponent<Collider2D>().enabled = false;
        playerMovement.Disable();

        active = false;

        highScore.TrySetNewHighScore();

        StartCoroutine(GameOverTransition());
    }
  
    IEnumerator GameOverTransition()
    {
        yield return new WaitForSeconds(gameOverAnimationTime);
        gameOverAnimator.enabled = false;
        gameOverOptionsPanel.SetActive(true);
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

}
