
using UnityEngine;

[CreateAssetMenu(fileName = "_EnemySpawnConfig", menuName = "Assets/Balance/EnemySpawnConfig")]
public class SpawnConfig : ScriptableObject
{
    public SpawnChance[] enemySpawnChance;

    public int[] spawnOnReachScore;

    [System.Serializable]
    public struct SpawnChance
    {
        public GameObject prefab;
        [Range(0,100)]
        public float spawnChance;
    }

}