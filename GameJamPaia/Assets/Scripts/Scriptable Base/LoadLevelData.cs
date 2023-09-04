using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Load Data", menuName = "Assets/Level Load Data")]
public class LoadLevelData : ScriptableObject
{
    public string levelToLoad;
}