using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_BalanceValues", menuName = "Assets/Balance/BalanceValues")]
public class BalanceValues : ScriptableObject
{
    public int maxAlarmsOn = 4;
    public float minAlarmDelay;
    public float maxAlarmDelay;
    public float minLockNewDoorDelay;
    public float maxLockNewDoorDelay;
    public int startLockingDoorsOnReachScore;
    //[SerializeField] private int maxLockedDoorsAmount;
    public int startSpawningPowerUpOnReachScore;
    public int minPowerUpDelay;
    public int maxPowerUpDelay;
}