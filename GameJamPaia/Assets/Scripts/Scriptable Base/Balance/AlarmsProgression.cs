using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_AlarmProgression", menuName = "Assets/Balance/AlarmProgression")]
public class AlarmsProgression : ScriptableObject
{
    public AlarmsDelayConfig[] alarmsDelayProgression;
}

[Serializable]
public class AlarmsDelayConfig
{
    public int onReachScore;
    [Header("Delay")]
    public float minDelay;
    public float maxDelay;
    public bool applied;
}
