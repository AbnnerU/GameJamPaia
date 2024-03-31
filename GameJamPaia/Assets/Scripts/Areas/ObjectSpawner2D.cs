using System.Collections;

using UnityEngine;
using UnityEngine.Rendering;

public class ObjectSpawner2D : MonoBehaviour
{
    [SerializeField] private bool active;
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Vector2 size;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnDelay;
    [SerializeField] private bool cacheStartPosition = true;
    [Header("MAX")]
    [SerializeField] private bool haveMaxObjects;
    [SerializeField] private int maxObjects;

    private GameObject[] objects;

    private Vector2 _startPosition;

    private void Awake()
    {
        if (haveMaxObjects)
            objects = new GameObject[maxObjects];

        if (cacheStartPosition)
            _startPosition = new Vector2(startPoint.position.x, startPoint.position.y);
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }


    IEnumerator SpawnLoop()
    {
        do
        {
            if (haveMaxObjects)
            {
                if (ActiveObjectsAmount() < maxObjects)
                {
                    Spawn();
                }
            }
            else
            {
                Spawn();
            }

            yield return new WaitForSeconds(spawnDelay);

        } while (active);
    }

    private void Spawn()
    {
        Vector2 s = size / 2;
        Vector2 position = Vector2.zero;
        position.x = Random.Range(-s.x, s.x);
        position.y = Random.Range(-s.y, s.y);

        if (cacheStartPosition)
            position += _startPosition;
        else
        {
            Vector3 start = startPoint.position;

            position += new Vector2(start.x, start.y);
        }

        if (haveMaxObjects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == null)
                {
                    GameObject obj = PoolManager.SpawnObject(prefab, position, Quaternion.identity);

                    objects[i] = obj;
                    return;
                }
            }

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].activeInHierarchy == false)
                {
                    objects[i].SetActive(true);
                    objects[i].transform.position = position;
                    return;
                }
            }
        }
        else
        {
            PoolManager.SpawnObject(prefab, position, Quaternion.identity);
        }
    }

    private int ActiveObjectsAmount()
    {
        int amount = 0;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                if (objects[i].activeInHierarchy == true)
                    amount++;
            }
        }
        print("MAX:" + amount);
        return amount;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.DrawWireCube(startPoint.position, new Vector3(size.x, size.y, 0));
        }
    }
}
