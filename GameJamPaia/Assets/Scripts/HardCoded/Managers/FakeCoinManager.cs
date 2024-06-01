using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCoinManager : MonoBehaviour
{
    [SerializeField]private GameManager gameManager;
    [SerializeField]private MapManager mapManager;
    [SerializeField]private LockRoomTrap trap;
    [SerializeField] private Transform trapTransform;

    [SerializeField] private float minDelayToSpawn;
    [SerializeField] private float maxDelayToSpawn;
    [SerializeField] private float timeToBeActive;

    Coroutine trapActiveTimer;


    private void Awake()
    {
        gameManager.OnEndTutorial += RunFakeCoinSystem;

        trap.OnTrapTriggered += OnTrapTriggered;
        trap.OnTrapDisabled += OnTrapDisabled;
    }
    
    private void RunFakeCoinSystem()
    {
        StartCoroutine(FakeCoinTimer());
    }

    private void OnTrapTriggered()
    {
        if (trapActiveTimer != null)
        {
            StopCoroutine(trapActiveTimer);
            trapActiveTimer = null;
        }

        StopAllCoroutines();
    }

    private void OnTrapDisabled()
    {
        if (trapActiveTimer != null)
        {
            StopCoroutine(trapActiveTimer);
            trapActiveTimer = null;
        }

        StopAllCoroutines();

        StartCoroutine(FakeCoinTimer());
    }

    IEnumerator FakeCoinTimer()
    {
        float delay = Random.Range(minDelayToSpawn, maxDelayToSpawn);

        yield return new WaitForSeconds(delay);

        Vector3 trapPosition = mapManager.GetRandomAvalibleRoomPosition();

        trapTransform.position = trapPosition;

        trap.TryEnableTrap();

        trap.UpdateBlockDoorsPositions();

        trapActiveTimer = StartCoroutine(ActiveTimer());
    }

    IEnumerator ActiveTimer()
    {
        yield return new WaitForSeconds(timeToBeActive);

        trap.TryDisableTrap();

        trapActiveTimer = null;

        StartCoroutine(FakeCoinTimer());
    }
}
