using System;
using UnityEngine;


[CreateAssetMenu(fileName = "_EnableRoomProgression", menuName = "Assets/Balance/EnableRoomProgression")]
public class EnableRoomProgression : ScriptableObject
{
    public RoomProgression[] roomProgressions;
}

[Serializable]
public class RoomProgression
{
    public int onReachScore;
    public bool applied = false;
}