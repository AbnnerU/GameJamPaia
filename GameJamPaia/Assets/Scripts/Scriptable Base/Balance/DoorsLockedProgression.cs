using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_DoorsLockedProgression" , menuName = "Assets/Balance/DoorsLockedProgression")]
public class DoorsLockedProgression : ScriptableObject
{
    public MaxDoorsLockedConfig[] doorsLockedConfigs;
}

[Serializable]
public class MaxDoorsLockedConfig
{
    public int onReachScore;
    //public int newMaxValue;
    [Header("Delay")]
    public float minDelay;
    public float maxDelay;
    public bool applied;
}