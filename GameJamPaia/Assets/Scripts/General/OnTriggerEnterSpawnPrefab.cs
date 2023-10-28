
using UnityEngine;

public class OnTriggerEnterSpawnPrefab : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        if(spawnPoint == null) spawnPoint = transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            PoolManager.SpawnObject(prefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
