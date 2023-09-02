using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool playOnStart;
    [SerializeField] private float minDelay;
    [SerializeField]private float maxDelay;
  
    [SerializeField] private Alarms[] alarms;
    [SerializeField]List<int> indexAvailable;

    [Header("Game Over")]
    [SerializeField] private int maxAlarmsOn=4;
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private bool disablePlayerMovement;

    private int alarmsOn=0;

    private void Awake()
    {
        indexAvailable = new List<int>(alarms.Length);

        if(playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();    

        for (int i = 0; i < alarms.Length; i++)
        {
            alarms[i].AlarmInput += OnAlarmInput;

            alarms[i].DisableAlarm();

            indexAvailable.Add(i);
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(GameMainLoop());
        }
    }

    private void OnAlarmInput(Alarms alarmRef)
    {
        alarmRef.DisableAlarm();

        indexAvailable.Add(GetAlarmIndex(alarmRef));
    }


    IEnumerator GameMainLoop()
    {
        int chooseIndex = 0;
      
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            if (indexAvailable.Count > 0)
            {
                chooseIndex = Random.Range(0, indexAvailable.Count);

                alarms[indexAvailable[chooseIndex]].EnableAlarm();

                indexAvailable.RemoveAt(chooseIndex);

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


    private int GetAlarmIndex(Alarms alarmRef)
    {
        for(int i = 0; i < alarms.Length; i++)
        {
            if (alarms[i] == alarmRef)
                return i;
        }

        return -1;
    }
}
