
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinsManager : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active = true;
    [SerializeField] private int currentCoinsAmount;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameScore coinsScore;
    [SerializeField]private MapManager mapManager;
    [SerializeField]private GameManager gameManager;
    [Header("Spawn Coins")]
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    private List<GameObject> coins = new List<GameObject>();
    private void Awake()
    {
        if (mapManager == null)
            mapManager = FindObjectOfType<MapManager>();

        if(gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        gameManager.OnEndTutorial += OnEndTutorial;
        coinsScore.OnScoreChange += CoinsScore_OnScoreChange;
    }

    private void OnEndTutorial()
    {
        StartCoroutine(CoinsSpawnLoop());
    }

    IEnumerator CoinsSpawnLoop()
    {
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            Vector3 pos = mapManager.GetRandomAvalibleRoomPosition();
            float x = Random.Range(2, 3.5f);
            float y = Random.Range(2, 3.5f);
            GameObject c = PoolManager.SpawnObject(coinPrefab, pos + new Vector3(x,y,0), Quaternion.identity);

            if (!coins.Contains(c))
            {
                coins.Add(c);
                c.GetComponent<ColletableAddScore>().WhenCollected += DisableCoinHud;
                c.GetComponent<DisableAfterTime>().OnTimeOut += DisableCoinHud;
            }

            mapManager.ChangeCoinHudActiveStateOnPosition(pos, true);
        }
    }

    public void TryUseCoins(int value, out bool success)
    {
        success = false;
        if ((currentCoinsAmount - value) < 0) return;

        success= true;
        coinsScore.RemovePoints(value);
    }

    private void DisableCoinHud(Vector3 position)
    {
        mapManager.ChangeCoinHudActiveStateOnPosition(position, false);
    }

    private void CoinsScore_OnScoreChange(int newCoinsValue)
    {
       currentCoinsAmount = newCoinsValue;
    }

    private void OnDestroy()
    {
        coinsScore.OnScoreChange -= CoinsScore_OnScoreChange;
    }

    public void Disable()
    {
        active = false;
    }

    public void Enable()
    {
        active = true;
    }
}
